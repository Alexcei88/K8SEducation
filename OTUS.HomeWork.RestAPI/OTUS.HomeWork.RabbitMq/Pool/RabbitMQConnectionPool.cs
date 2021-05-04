using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using RabbitMQ.Client;

namespace OTUS.HomeWork.RabbitMq.Pool
{
	public class RabbitMqConnectionPool
		: IDisposable
	{
		public string ConnectionString { get; }

		private readonly ConnectionFactory _connectionFactory;

		private bool _disposed = false;

		private readonly ConcurrentBag<RabbitMqConnection> _connections = new ConcurrentBag<RabbitMqConnection>();

		public RabbitMqConnectionPool(string connectionString)
		{
			ConnectionString = connectionString;
			_connectionFactory = new ConnectionFactory
			{
				Uri = new Uri(connectionString),
				RequestedHeartbeat = TimeSpan.FromSeconds(60), // 60 сек
				AutomaticRecoveryEnabled = false,
				TopologyRecoveryEnabled = false,
				RequestedChannelMax = 25, // 25 channels worked on simultaneously are optimal value for us I consider
			};
		}

		public RabbitMqConnection Get(bool withEmptyChannels = false)
		{
			List<RabbitMqConnection> toReturnToChannel = new List<RabbitMqConnection>();
			try
			{
				while (_connections.TryTake(out RabbitMqConnection connection))
				{
					if (!connection.BrokerConnection.IsOpen || connection.IsNoChannels)
					{
						CloseConnection(connection);
						continue;
					}
					if (connection.IsThresholdReached)
					{
						toReturnToChannel.Add(connection);
						continue;
					}

					if (withEmptyChannels && !connection.IsNoChannels)
					{
						// соединение уже содержит каналы, а нам нужно соединение без каналов
						toReturnToChannel.Add(connection);
						continue;
					}

					return connection;
				}
				return GetNewConnection();
			}
			finally
			{
				foreach (var con in toReturnToChannel)
					_connections.Add(con);
			}
		}

		public void Return(RabbitMqConnection connection, bool forceClose = false)
		{
			if (connection == null) return;

			if(forceClose || connection.IsNoChannels || !connection.BrokerConnection.IsOpen)
			{
				CloseConnection(connection);
				return;
			}

			if (connection.BrokerConnection.IsOpen)
				_connections.Add(connection);
		}

		private void CloseConnection(RabbitMqConnection connection)
		{
			try
			{
				connection.BrokerConnection.Close();
				connection.BrokerConnection.Dispose();
			}
			catch (IOException) { }
			catch (Exception ex)
			{
				Console.WriteLine("RabbitMQ connection closure failed", ex);
			}
		}

		private RabbitMqConnection GetNewConnection()
		{
			lock (_connectionFactory)
			{
				int i = 0;
				do
				{
					try
					{
						var connection = _connectionFactory.CreateConnection();

						Console.WriteLine("A connection to RabbitMQ broker was opened");
						// !!!!the connection variable not must use in the lambda function, else we will have a memory leak.
						// Otherwise(if we want to use the varialbe inside lambda), we must somewhere unsubscribe from the events.
						connection.ConnectionShutdown += (sender, args) =>
							Console.WriteLine($"The connection to RabbitMQ broker was closed by reason: {args.ReplyText}");

						connection.ConnectionBlocked += (sender, args) =>
							Console.WriteLine($"The connection to RabbitMQ was blocked by reason: {args.Reason}");

						connection.ConnectionUnblocked += (sender, args) =>
							Console.WriteLine($"The connection to RabbitMQ was unblocked");

						return new RabbitMqConnection(connection);
					}
					catch (Exception ex)
					{
						if (++i > 3)
							throw ex;
					}
				}
				while (true);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				while (_connections.TryTake(out RabbitMqConnection connection))
				{
					CloseConnection(connection);
				}
			}

			// dispose unmanaged resources
			_disposed = true;
		}
	}
}
