using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace OTUS.HomeWork.RabbitMq.Pool
{
	// TODO надо механизм, который будет сжимать количество каналов, которые образовались при пиках
	public class RabbitMQChannelPool
		: IDisposable
	{
		private readonly RabbitMqConnectionPool _connectionPool;

		private bool _disposed = false;

		private ConcurrentBag<RabbitMQChannel> _channels = new ConcurrentBag<RabbitMQChannel>();

		public string ConnectionString => _connectionPool.ConnectionString;

		public RabbitMQChannelPool(RabbitMqConnectionPool connectionPool)
		{
			_connectionPool = connectionPool;
		}

		public RabbitMQChannel Get()
		{
			while (_channels.TryTake(out RabbitMQChannel channel))
			{
				if (!channel.Channel.IsOpen)
				{
					CloseChannel(channel);
					continue;
				}
				return channel;
			}
			var newChannel = GetNewChannel();
			return newChannel;
		}

		private RabbitMQChannel GetNewChannel()
		{
			RabbitMqConnection connection = null;
			IModel channel = null;
			try
			{
				while(channel == null)
				{
					connection = _connectionPool.Get();
					channel = connection.CreateChannel();
					if(channel == null) // достигли максимального количества каналов на соединение
						_connectionPool.Return(connection);
				}
			}
			finally
			{
				_connectionPool.Return(connection);
			}
			channel.ModelShutdown += (sender, args) =>
			{
				Console.WriteLine("The connection to rabbitMQ broker was closed");
				//_logger.Info("The connection to rabbitMQ broker was closed");
			};
			return new RabbitMQChannel(channel, connection);
		}

		public void Return(RabbitMQChannel channel)
		{
			if (channel.Channel.IsOpen)
			{
				_channels.Add(channel);
			}
			else
			{
				CloseChannel(channel);
			}
		}

		private void CloseChannel(RabbitMQChannel channel)
		{
			try
			{
				channel.Connection.CloseChannel(channel.Channel);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Channel closure failed", ex);
				//_logger.Error("Channel closure failed", ex);
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				// dispose managed resources
				while (_channels.TryTake(out var channel))
				{
					CloseChannel(channel);
				}
			}

			// dispose unmanaged resources
			_disposed = true;
		}
	}
}
