using RabbitMQ.Client;

namespace OTUS.HomeWork.RabbitMq
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
