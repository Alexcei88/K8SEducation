using Core;
using DataBuffer.BusClient.Abstraction;
using Infrastructure;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using DataBuffer.BusClient.RabbitMq.Pool;

namespace DataBuffer.BusClient.RabbitMq
{
	public class RabbitMQQueueRepeater
		: IQueueRepeater
	{
		/// <summary>
		/// Настройки обработчика очередей
		/// </summary>
		[MessagePackObject(keyAsPropertyName: true)]
		public class QueueRepeaterSetting
		{
			public string ConnectionString { get; set; }
			public string NameQueue { get; set; }
			public string TargetConnectionString { get; set; }
			public string TargetExchangeName { get; set; }
		}

		/// <summary>
		/// Настройки обработчика сообщения
		/// </summary>
		protected QueueRepeaterSetting Settings { get; } = new QueueRepeaterSetting();

		/// <summary>
		/// Имя прослушиваемой очереди
		/// </summary>
		public string NameQueue => Settings.NameQueue;

		/// <summary>
		/// Логгер.
		/// </summary>
		private readonly ILogger _logger = ServiceLocator.Instance.LogService.GetLogger(typeof(RabbitMQQueueRepeater));

		public bool IsStarted { get; protected set; }

		private Timer _timer;

		private readonly object _syncObj = new object();
		private CancellationTokenSource _cancellationTokenSource;
		private string _consumerTag;
		private IModel _consumerChannel;
		private readonly ManualResetEventSlim _handleMessageEvent;
		
		private readonly RabbitMqConnectionPool _rabbitMqConnectionPool;
		private readonly RabbitMQChannelPool _targetChannelPool;
		private RabbitMqConnection _rabbitMqConnection;

		public RabbitMQQueueRepeater(string queueName, RabbitMqConnectionPool connectionPool,
			string targetExchangeName, RabbitMQChannelPool targetChannelPool)
		{
			Settings.NameQueue = queueName;
			Settings.ConnectionString = connectionPool.ConnectionString;
			Settings.TargetConnectionString = targetChannelPool.ConnectionString;
			Settings.TargetExchangeName = targetExchangeName;

			_rabbitMqConnectionPool = connectionPool;
			_handleMessageEvent = new ManualResetEventSlim(true);
			_targetChannelPool = targetChannelPool;

			_timer = new Timer(Restart, null, Timeout.Infinite, Timeout.Infinite);
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public void Start()
		{
			lock (_syncObj)
			{
				if (IsStarted)
				{
					_logger.Warning($"Попытка повторного запуска ретранслятора очереди {NameQueue}");
					return;
				}

				_logger.Info($"Запуск ретранслятора очереди {NameQueue}...");

				StartListen();
				_logger.Info($"Ретранслятор очереди {NameQueue} успешно запущен!");

				_timer.Change(Timeout.Infinite, Timeout.Infinite);
				IsStarted = true;
			}
		}

		public void Stop()
		{
			lock (_syncObj)
			{
				if (!IsStarted)
				{
					_logger.Warning($"Попытка повторной остановки ретранслятора очереди {NameQueue}");
					return;
				}

				_logger.Info($"Остановка ретранслятора очереди {NameQueue}...");

				StopListen();
				_timer?.Dispose();
				_timer = null;

				IsStarted = false;
				_logger.Info($"Ретранслятор очереди {NameQueue} успешно остановлен!");
			}
		}

		private void StopListen()
		{
			try
			{
				StopListenQueue(ref _consumerTag, ref _consumerChannel);

				if (_rabbitMqConnection != null)
					_rabbitMqConnection.BrokerConnection.CallbackException -= OnConnectionCallbackException;
				_rabbitMqConnectionPool.Return(_rabbitMqConnection, true);
				_rabbitMqConnection = null;
			}
			catch (Exception ex)
			{
				_logger.Error($"Ошибка остановки ретранслятор очереди {NameQueue}", ex);
			}
		}

		public void Restart(object state)
		{
			lock (_syncObj)
			{
				_logger.Info($"Restarting RabbitMQ queue handler for queueName:{this.NameQueue}");
				try
				{
					if (IsStarted)
					{
						StopListen();
						IsStarted = false;
					}
					Start();
				}
				catch (Exception ex)
				{
					_logger.Warning($"Restarting RabbitMQ queue handler for queueName:{this.NameQueue} throw excepton: {ex.Message}");
				}
			}
		}

		private void StartListen()
		{
			_rabbitMqConnection = _rabbitMqConnectionPool.Get(true);
			_rabbitMqConnection.BrokerConnection.CallbackException += OnConnectionCallbackException;

			_cancellationTokenSource = new CancellationTokenSource();
			var token = _cancellationTokenSource.Token;

			_logger.Info($"Запуск прослушивания очереди {NameQueue}...");

			_consumerChannel = _rabbitMqConnection.CreateChannel();
			_consumerChannel.CallbackException += OnCallbackException;
			_consumerChannel.ModelShutdown += OnModelShutdown;
			_ = _consumerChannel.QueueDeclarePassive(NameQueue);
			var consumer = new EventingBasicConsumer(_consumerChannel);
			// exchange
			CheckExistExchange(Settings.TargetExchangeName, Constants.DEAD_EXCHANGE_NAME);
			CheckExistQueueAndBinding(Constants.UNKNOWN_MESSAGE_TYPE);

			consumer.Received += (ch, ea) =>
			{
				if (!IsStarted)
				{
					_logger.Warning("Обработка сообщения при остановленном обработчике сообщения. Сообщение будет обработано в следующий раз!");
					_consumerChannel?.BasicNack(ea.DeliveryTag, false, true);
					return;
				}
				_handleMessageEvent.Reset();

				var cts = new CancellationTokenSource();

				if (!TryGetActionTypeProperty(ea.BasicProperties, out string actionType))
				{
					_consumerChannel?.BasicNack(ea.DeliveryTag, false, false);
					return;
				}

				try
				{
					// 1. проверка биндинга и существования очереди
					CheckExistQueueAndBinding(actionType);
					// 2. перекладываем сообщение
					PublishMessage(actionType, ea);
					// 3. убиваем сообщение из очереди
					_consumerChannel.BasicAck(ea.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					_logger.Error("Ошибка ретрансляции сообщения!", ex);
					try
					{
						_consumerChannel.BasicReject(ea.DeliveryTag, !ea.Redelivered);
					}
					catch (Exception e)
					{
						_logger.Error("При отклонении сообщения возникла ошибка", e);
					}
				}
				finally
				{
					cts.Cancel();
					_handleMessageEvent.Set();
				}
			};

			_consumerTag = _consumerChannel.BasicConsume(NameQueue, false, consumer);
			_logger.Info($"Запуск прослушивания очереди {NameQueue} завершено!");
		}

		private void StopListenQueue(ref string consumerTag, ref IModel channel)
		{
			if (!string.IsNullOrEmpty(consumerTag))
			{
				channel.BasicCancel(consumerTag);
				consumerTag = null;
			}

			if (!_handleMessageEvent.IsSet)
			{
				TimeSpan waitTimeOut = new TimeSpan(0, 1, 30);
				_logger.Info($"В текущий момент времени идет обработка сообщения. Ожидание окончания обработки сообщения в течении {waitTimeOut.TotalSeconds} секунд");
				if (_handleMessageEvent.Wait(waitTimeOut) == true)
				{
					_logger.Info("Ожидание обработки сообщения завершилось успешно!");
				}
				else
				{
					_logger.Warning("Не удалось дождаться окончания обработки сообщения. Закрываем соединение с RabbitMQ сервером");
				}
			}

			_rabbitMqConnection.CloseChannel(channel);
			channel = null;
		}

		private bool TryGetActionTypeProperty(IBasicProperties properties, out string actionType)
		{
			if (!properties.IsTypePresent())
			{
				_logger.Warning($"В сообщении отсутствует обязательное свойство Type. Сообщение будет обработано в однопоточном режиме!");
				actionType = Constants.UNKNOWN_MESSAGE_TYPE;
				return true;
			}
			actionType = properties.Type;
			return true;
		}

		private void CheckExistQueueAndBinding(string actionType)
		{
			// 1. проверяем exchange
			RabbitMQChannel? channel = null;
			try
			{
				channel = _targetChannelPool.Get();

				string deadQueueName = $"{actionType}_dead";
				//queues
				channel.Value.Channel.QueueDeclare(actionType, true, false, false, new Dictionary<string, object>()
				{
					{"x-dead-letter-exchange", Constants.DEAD_EXCHANGE_NAME }
				});
				channel.Value.Channel.QueueDeclare(deadQueueName, true, false, false);
				// binding
				channel.Value.Channel.QueueBind(actionType, Settings.TargetExchangeName, actionType);
				channel.Value.Channel.QueueBind(deadQueueName, Constants.DEAD_EXCHANGE_NAME, actionType);
			}
			finally
			{
				if (channel != null)
					_targetChannelPool.Return(channel.Value);
			}
		}

		private void CheckExistExchange(string mainExchangeName, string deadLetterExchangeName)
		{
			// 1. проверяем exchange
			RabbitMQChannel? channel = null;
			try
			{
				channel = _targetChannelPool.Get();
				channel.Value.Channel.ExchangeDeclare(mainExchangeName, ExchangeType.Direct, true);
				channel.Value.Channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Direct, true);
			}
			finally
			{
				if (channel != null)
					_targetChannelPool.Return(channel.Value);
			}
		}

		private void PublishMessage(string actionType, BasicDeliverEventArgs message)
		{
			RabbitMQChannel? channel = null;
			try
			{
				channel = _targetChannelPool.Get();
				channel.Value.Channel.ConfirmSelect();
				channel.Value.Channel.BasicPublish(Settings.TargetExchangeName, actionType, message.BasicProperties, message.Body);
				channel.Value.Channel.WaitForConfirmsOrDie();
			}
			finally
			{
				if (channel != null)
					_targetChannelPool.Return(channel.Value);
			}
		}

		private void OnModelShutdown(object sender, ShutdownEventArgs args)
		{
			_logger.Warning($"The connection to RabbitMQ channel was broken by: {args.ReplyText} {args.Initiator}");
			_timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
		}

		private void OnConnectionCallbackException(object sender, CallbackExceptionEventArgs args)
		{
			_logger.Error("Возникло исключение на уровне соединения к брокеру очередей RabbitMQ", args.Exception);
		}

		private void OnCallbackException(object sender, CallbackExceptionEventArgs args)
		{
			_logger.Error("Возникло исключение на уровне канала", args.Exception);
		}

		public int CompareTo(IQueueRepeater other)
		{
			string s = HashHelper.GenerateSHA512Hash(HashHelper.MessagePackSerialize((other as RabbitMQQueueRepeater)?.Settings));
			string s1 = HashHelper.GenerateSHA512Hash(HashHelper.MessagePackSerialize(Settings));
			return string.Compare(s, s1, StringComparison.Ordinal);
		}

		public void Restart()
		{
			Restart(null);
		}
	}
}
