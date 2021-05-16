using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.RabbitMq.Pool;
using RabbitMQ.Client;

namespace OTUS.HomeWork.RabbitMq
{
	public class RabbitMQMessageSender
	{	
		private readonly string _routeKey;

		private readonly string _exchangeName;

		private readonly IMessageExchangeSerializer _serializer;

		private readonly RabbitMQChannelPool _rabbitMQChannelPool;

		private bool _isDisposing;

		public RabbitMQMessageSender(string exchangeName
			, string routeKey
			, RabbitMQChannelPool rabbitMQChannelPool
			, IMessageExchangeSerializer serializer)
		{			
			_routeKey = routeKey;
			_serializer = serializer;
			_exchangeName = exchangeName;

			_rabbitMQChannelPool = rabbitMQChannelPool;

			CreateExchangeAndQueueIfNotExist();
		}

		private void CreateExchangeAndQueueIfNotExist()
        {
			Console.WriteLine("CreateExchangeAndQueueIfNotExist was called");
			// 1. проверяем exchange
			RabbitMQChannel? channel = null;
			try
			{
				channel = _rabbitMQChannelPool.Get();

				//queues
				try
				{
					channel.Value.Channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
				}
				catch(Exception ex) {
					Console.WriteLine($"Не удалось создать Exchange: {ex}");
					if (channel != null)
						_rabbitMQChannelPool.Return(channel.Value);
					channel = _rabbitMQChannelPool.Get();
				}

				try
				{
					channel.Value.Channel.QueueDeclare(_routeKey, true);
				}
				catch {
					if (channel != null)
						_rabbitMQChannelPool.Return(channel.Value);
					channel = _rabbitMQChannelPool.Get();
				}
				// binding
				try
				{
					channel.Value.Channel.QueueBind(_routeKey, _exchangeName, _routeKey);
				}
                catch(Exception ex) {
					Console.WriteLine($"Не удалось создать привязку: {ex}");
				}
			}
			finally
			{
				if (channel != null)
					_rabbitMQChannelPool.Return(channel.Value);
			}
		}

		public void Dispose()
		{
			if (_isDisposing) return;
			_isDisposing = true;
		}


		public bool SendMessage<T>(T request, string destination) where T : IBrokerMessage, new()
		{
			RabbitMQChannel? channel = null;
			try
			{
				channel = GetChannel();
				destination = destination ?? _routeKey;
				using (var stream = _serializer.SerializeRequest(request))
					{
						channel.Value.Channel.BasicPublish(_exchangeName, destination, GetMessageProperties(channel.Value.Channel, request.MessageType), stream.ToArray());
					}
				channel.Value.Channel.WaitForConfirmsOrDie();

				Console.WriteLine($"Запрос типа {typeof(T)} отправлен в RabbitMQ {_exchangeName}/{destination}");

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Ошибка отправки запросов типа {0} в RabbitMQ", typeof(T)), ex);
				return false;
			}
			finally
			{
				if (channel != null) _rabbitMQChannelPool.Return(channel.Value);
			}
		}

		public Task<bool> SendMessageAsync<T>(T requests, string destination = null) where T : IBrokerMessage, new()
		{
			return Task.FromResult(SendMessage(requests, destination));
		}

		private RabbitMQChannel GetChannel()
		{
			var channel = _rabbitMQChannelPool.Get();
			channel.Channel.ConfirmSelect();
			channel.Channel.ExchangeDeclarePassive(_exchangeName);
			return channel;
		}

		private IBasicProperties GetMessageProperties(IModel channel, string messageType)
		{
			IBasicProperties props = channel.CreateBasicProperties();
			props.ContentType = _serializer.MimeTypeName;
			props.DeliveryMode = 2; // persistent mode

			props.Headers = new Dictionary<string, object>();
			props.Headers.Add("messageType", messageType);
			return props;
		}
	}
}
