using System.IO;

namespace DataBuffer.MessageExchangeSerializer
{
	/// <summary>
	/// Сериализатор сообщений обмена
	/// </summary>
	public interface IMessageExchangeSerializer
	{
		string Serialize<T>(T obj);

		MemoryStream SerializeRequest<TRequest>(TRequest obj) where TRequest : new();

		MemoryStream SerializeResponse<TResponse>(TResponse obj) where TResponse : new();

		TRequest DeserializeRequest<TRequest>(Stream stream) where TRequest : new();

		TResponse DeserializeResponse<TResponse>(Stream stream) where TResponse : new();

		string MimeTypeName { get; }
	}
}
