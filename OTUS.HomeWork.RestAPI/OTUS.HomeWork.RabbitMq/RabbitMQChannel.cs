using RabbitMQ.Client;

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
