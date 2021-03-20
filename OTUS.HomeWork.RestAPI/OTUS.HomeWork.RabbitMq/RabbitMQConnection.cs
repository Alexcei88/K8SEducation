using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.IO;
using System.Threading;

namespace DataBuffer.BusClient.RabbitMq
{
	public class RabbitMqConnection
	{
		private int _channelCount;

        public IConnection BrokerConnection { get; }

		public bool IsNoChannels => _channelCount <= 0;

		public bool IsThresholdReached => _channelCount >= BrokerConnection.ChannelMax;

        public RabbitMqConnection(IConnection connection)
		{
			_channelCount = 0;
			BrokerConnection = connection;
		}

		/// <summary>
		/// Создать новый канал. Если мы достигли максимального количества каналов в соединении, то вернуть null
		/// </summary>
		/// <returns></returns>
		public IModel CreateChannel()
		{
			if (IsThresholdReached)
				return null;

			try
			{
				var model = BrokerConnection.CreateModel();
				model.BasicQos(0, 1, false);
				Interlocked.Increment(ref _channelCount);
				return model;
			}
			catch (ChannelAllocationException) // The max possible connections were created from specified connection
			{
				return null;
			}
		}

		public void CloseChannel(IModel channel)
		{
			try
			{
				Interlocked.Decrement(ref _channelCount);
				channel.Close();
				channel.Dispose();
			}
			catch (IOException) { }
		}
	}
}
