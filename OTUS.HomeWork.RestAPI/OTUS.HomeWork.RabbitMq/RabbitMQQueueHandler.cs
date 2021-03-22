using DataBuffer.MessageExchangeSerializer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Threading;
using DataBuffer.BusClient.RabbitMq.Pool;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DataBuffer.BusClient.RabbitMq
{
	/*
	public class RabbitMQQueueHandler
		: AbstractQueueHandler
	{
		private RabbitMqConnection _rabbitMqConnection;

		/// <summary>
		/// Канал для подключения к главной очереди
		/// </summary>
		private IModel _queueChannel;

		private string _consumerTag;

		private Timer _timer;

		private readonly RabbitMqConnectionPool _rabbitMqConnectionPool;

		private readonly object _syncObj = new object();

		private readonly ManualResetEventSlim _handleMessageEvent;

		private readonly bool _createQueueIfNotExist;

		public RabbitMQQueueHandler(string connectionString, string queueName, RabbitMqConnectionPool connectionPool,
			bool createQueueIfNotExist, IConfiguration configuration)
			: base(connectionString, queueName, configuration)
		{
			_rabbitMqConnectionPool = connectionPool;
			_messageExchangeDefaultSerializer = new JsonNetMessageExchangeSerializer();
			_handleMessageEvent = new ManualResetEventSlim(true);
			_timer = new Timer(Restart, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_createQueueIfNotExist = createQueueIfNotExist;
		}

		public override void Start()
		{
			if (IsStarted)
			{
				_logger.Warning($"Попытка повторного запуска обработчика очереди {NameQueue}");
				return;
			}

			_logger.Debug($"Запуск обработчика очереди {NameQueue}...");

			CheckExistQueue();
			StartListen();

			_logger.Debug($"Обработчик очереди {NameQueue} успешно запущен!");
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			IsStarted = true;
		}

		public override void Stop()
		{
			try
			{
				if (!IsStarted)
				{
					_logger.Warning("Попытка остановки незапущенного обработчика очереди");
					return;
				}
				lock (_syncObj)
				{
					_logger.Debug($"Остановка обработчика очереди {NameQueue}...");

					StopListen();
					IsStarted = false;
					_timer?.Dispose();
					_timer = null;
					_logger.Debug($"Обработчик очереди {NameQueue} успешно остановлен!");

				}
			}
			catch (Exception ex)
			{
				_logger.Error($"Ошибка остановки обработчика очереди {NameQueue}", ex);
			}
		}

		private void StopListen()
		{
			if (_rabbitMqConnection != null)
				_rabbitMqConnection.BrokerConnection.CallbackException -= OnConnectionCallbackException;

			if (_queueChannel != null)
			{
				_queueChannel.CallbackException -= OnCallbackException;
				_queueChannel.ModelShutdown -= OnModelShutdown;
			}

			StopListenQueue(ref _consumerTag, ref _queueChannel);
			_rabbitMqConnection = null;
		}

		private void Restart(object state)
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
				_logger.Warning(
					$"Restarting RabbitMQ queue handler for queueName:{this.NameQueue} throw excepton: {ex.Message}");
			}
		}

		private void StartListen()
		{
			_rabbitMqConnection = _rabbitMqConnectionPool.Get();
			_rabbitMqConnection.BrokerConnection.CallbackException += OnConnectionCallbackException;

			_queueChannel = _rabbitMqConnection.CreateChannel();
			_queueChannel.CallbackException += OnCallbackException;
			_queueChannel.ModelShutdown += OnModelShutdown;
			// возвращаем канал в пул
			_rabbitMqConnectionPool.Return(_rabbitMqConnection);

			var consumer = new EventingBasicConsumer(_queueChannel);
			consumer.Received += (ch, ea) =>
			{
				if (!IsStarted)
				{
					_logger.Warning(
						"Обработка сообщения при остановленном обраобтчике сообщения. Сообщение обработано будет в следующий раз");
					_queueChannel?.BasicNack(ea.DeliveryTag, false, true);
					return;
				}

				_handleMessageEvent.Reset();
				Stream body = null;
				var cts = new CancellationTokenSource();

				// ... process the message
				Response response = new Response();
				try
				{
					body = new MemoryStream(ea.Body.ToArray(), 0, ea.Body.Length);
					body = UnpackMessageStream(body, ea.BasicProperties.Headers);

					this.HandleMessage(body, ea.BasicProperties.ContentType, cts.Token, out response);
					ConfirmMessage(ea, response);
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
					try
					{
						AbandonMessage(ea, response, ex.Message);
					}
					catch (Exception e)
					{
						_logger.Error("При отклонении сообщения возникла ошибка", e);
					}
				}
				finally
				{
					cts.Cancel();
					body?.Dispose();
					_handleMessageEvent.Set();
				}
			};

			_consumerTag = _queueChannel.BasicConsume(NameQueue, false, consumer);
		}

		private void CheckExistQueue()
		{
			var rabbitMqConnection = _rabbitMqConnectionPool.Get();
			var queueChannel = rabbitMqConnection.CreateChannel();
			// возвращаем канал, могут другие использовать
			_rabbitMqConnectionPool.Return(rabbitMqConnection);
			if (_createQueueIfNotExist)
			{
				try
				{
					_ = queueChannel.QueueDeclarePassive(NameQueue);
				}
				catch (Exception)
				{
					// очереди нет, создаем очередь
					rabbitMqConnection.CloseChannel(queueChannel);
					rabbitMqConnection = _rabbitMqConnectionPool.Get();
					queueChannel = rabbitMqConnection.CreateChannel();
					// возвращаем канал, могут другие использовать
					_rabbitMqConnectionPool.Return(rabbitMqConnection);

					string deadQueueName = $"{NameQueue}_dead";
					//queues
					queueChannel.QueueDeclare(NameQueue, true, false, false, new Dictionary<string, object>()
					{
						{"x-dead-letter-exchange", Constants.DEAD_EXCHANGE_NAME }
					});
					queueChannel.QueueDeclare(deadQueueName, true, false, false);
					// binding
					queueChannel.QueueBind(deadQueueName, Constants.DEAD_EXCHANGE_NAME, NameQueue);
				}
			}
			else
			{
				_ = queueChannel.QueueDeclarePassive(NameQueue);
			}
			rabbitMqConnection.CloseChannel(queueChannel);
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
				_logger.Info(
					$"В текущий момент времени идет обработка сообщения. Ожидание окончания обработки сообщения в течении {waitTimeOut.TotalSeconds} секунд");
				if (_handleMessageEvent.Wait(waitTimeOut) == true)
				{
					_logger.Info("Ожидание обработки сообщения завершилось успешно!");
				}
				else
				{
					_logger.Warning(
						"Не удалось дождаться обработки сообщения. Закрываем соединение с RabbitMQ сервером");
				}
			}

			_rabbitMqConnection.CloseChannel(channel);
			channel = null;
		}

		private void ConfirmMessage(BasicDeliverEventArgs arg, Response response)
		{
			if (arg.BasicProperties.Headers != null
			    && arg.BasicProperties.Headers.TryGetValue(Constants.UNITTEST_MESSAGE_PROPERTY_NAME,
				    out object unitTest))
			{
				_requestRepository.RequestAccepted(response.Id, true);
			}

			_queueChannel.BasicAck(arg.DeliveryTag, false);
		}

		private void AbandonMessage(BasicDeliverEventArgs arg, Response response, string errorMessage)
		{
			if (arg.BasicProperties.Headers != null
			    && arg.BasicProperties.Headers.TryGetValue(Constants.UNITTEST_MESSAGE_PROPERTY_NAME, out _))
			{
				_requestRepository.RequestAccepted(response.Id, false);
			}

			_queueChannel.BasicReject(arg.DeliveryTag, !arg.Redelivered);
		}

		private void OnModelShutdown(object sender, ShutdownEventArgs args)
		{
			_logger.Warning($"The connection to RabbitMQ channel was broken by: {args.ReplyText} {args.Initiator}");
			if(_timer == null)
			{
				_timer = new Timer(Restart, null, Timeout.Infinite, Timeout.Infinite);
			}
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

		public override int CompareTo(IQueueHandler other)
		{
			string s = HashHelper.GenerateSHA512Hash(
				HashHelper.MessagePackSerialize((other as RabbitMQQueueHandler)?.Settings));
			string s1 = HashHelper.GenerateSHA512Hash(HashHelper.MessagePackSerialize(Settings));
			return string.Compare(s, s1, StringComparison.Ordinal);
		}
	}*/
}
