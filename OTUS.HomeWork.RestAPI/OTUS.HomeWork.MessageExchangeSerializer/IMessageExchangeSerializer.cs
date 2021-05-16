using System.IO;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.NotificationService.Contract.Messages;

namespace OTUS.HomeWork.MessageExchangeSerializer
{
	/// <summary>
	/// Сериализатор сообщений обмена
	/// </summary>
	public interface IMessageExchangeSerializer
	{
		string Serialize<T>(T obj);

		MemoryStream SerializeRequest<TRequest>(TRequest obj) where TRequest : IBrokerMessage, new();

		MemoryStream SerializeResponse<TResponse>(TResponse obj) where TResponse : IBrokerMessage, new();

		TRequest DeserializeRequest<TRequest>(Stream stream) where TRequest : IBrokerMessage, new();

		TResponse DeserializeResponse<TResponse>(Stream stream) where TResponse : IBrokerMessage, new();

		string MimeTypeName { get; }
	}
}
