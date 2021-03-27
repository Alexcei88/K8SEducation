using OTUS.HomeWork.NotificationService.Contract.Messages;
using System.IO;

namespace DataBuffer.MessageExchangeSerializer
{
	/// <summary>
	/// Сериализатор сообщений обмена
	/// </summary>
	public interface IMessageExchangeSerializer
	{
		string Serialize<T>(T obj);

		MemoryStream SerializeRequest<TRequest>(TRequest obj) where TRequest : NotificationMessage, new();

		MemoryStream SerializeResponse<TResponse>(TResponse obj) where TResponse : NotificationMessage, new();

		TRequest DeserializeRequest<TRequest>(Stream stream) where TRequest : NotificationMessage, new();

		TResponse DeserializeResponse<TResponse>(Stream stream) where TResponse : NotificationMessage, new();

		string MimeTypeName { get; }
	}
}
