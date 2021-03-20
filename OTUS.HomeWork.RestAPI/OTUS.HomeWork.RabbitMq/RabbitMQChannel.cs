using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBuffer.BusClient.RabbitMq
{
	public readonly struct RabbitMQChannel
	{
		public IModel Channel { get; }

		public RabbitMqConnection Connection { get; }

		public RabbitMQChannel(IModel channel, RabbitMqConnection connection)
		{
			Channel = channel;
			Connection = connection;
		}
	}
}
