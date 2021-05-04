using System;
using System.Collections.Concurrent;

namespace OTUS.HomeWork.RabbitMq.Pool
{
	public class RabbitMQPoolStorage
		: IDisposable
	{
		private ConcurrentDictionary<string, (RabbitMqConnectionPool, RabbitMQChannelPool)> pools
			= new ConcurrentDictionary<string, (RabbitMqConnectionPool, RabbitMQChannelPool)>();

		public RabbitMqConnectionPool GetConnectionPool(string connectionString)
		{
			(RabbitMqConnectionPool, RabbitMQChannelPool) pool;
			if (!pools.TryGetValue(connectionString, out pool))
			{
				var connectionPool = new RabbitMqConnectionPool(connectionString);
				pool = (connectionPool, new RabbitMQChannelPool(connectionPool));
				pools.AddOrUpdate(connectionString, pool, (k, v) => pool);
			}
			return pool.Item1;
		}

		public RabbitMQChannelPool GetChannelPool(string connectionString)
		{
			(RabbitMqConnectionPool, RabbitMQChannelPool) pool;
			if (!pools.TryGetValue(connectionString, out pool))
			{
				var connectionPool = new RabbitMqConnectionPool(connectionString);
				pool = (connectionPool, new RabbitMQChannelPool(connectionPool));
				pools.AddOrUpdate(connectionString, pool, (k, v) => pool);
			}
			return pool.Item2;
		}

		public void Dispose()
		{
			foreach(var pool in pools)
			{
				pool.Value.Item1.Dispose();
				pool.Value.Item2.Dispose();
			}
		}
	}
}
