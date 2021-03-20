using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataBuffer.BusClient.RabbitMq
{
	public class RabbitMQMessageSender
		: IMessageSender
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
			_logger = ServiceLocator.Instance.LogService.GetLogger(GetType());
			_logger.CustomProperties.Add(LogPropertiesConstants.NAME_QUEUE_LOG_ITEM, $"{exchangeName}:{routeKey}");

			_routeKey = routeKey;
			_serializer = serializer;
			_exchangeName = exchangeName;

			_rabbitMQChannelPool = rabbitMQChannelPool;
		}

		public void Dispose()
		{
			if (_isDisposing) return;
			_isDisposing = true;
		}


		public bool SendRequests(Request[] requests, string destination)
		{
			if (!requests.Any()) return true;

			RabbitMQChannel? channel = null;
			try
			{
				channel = GetChannel();
				destination = destination ?? _routeKey;
				foreach (var request in requests)
				{
					using (var stream = _serializer.SerializeRequest(request))
					{
						channel.Value.Channel.BasicPublish(_exchangeName, destination, GetMessageProperties(channel.Value.Channel), stream.ToArray());
					}
				}
				channel.Value.Channel.WaitForConfirmsOrDie();

				_logger.Debug($"Запросы({requests.Length}) типа {requests.First().Type} отправлен в RabbitMQ {_exchangeName}/{destination}");

				return requests.Length > 0;
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Ошибка отправки запросов типа {0} в RabbitMQ", requests.First().Type), ex);
				return false;
			}
			finally
			{
				if (channel != null) _rabbitMQChannelPool.Return(channel.Value);
			}
		}

		public Task<bool> SendRequestsAsync(Request[] requests, string destination = null)
		{
			return Task.FromResult(SendRequests(requests, destination));
		}

		private RabbitMQChannel GetChannel()
		{
			var channel = _rabbitMQChannelPool.Get();
			channel.Channel.ConfirmSelect();
			channel.Channel.ExchangeDeclarePassive(_exchangeName);
			return channel;
		}

		private IBasicProperties GetMessageProperties(IModel channel)
		{
			IBasicProperties props = channel.CreateBasicProperties();
			props.ContentType = _serializer.MimeTypeName;
			props.DeliveryMode = 2; // persistent mode
			return props;
		}
	}
}
