using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OTUS.HomeWork.NotificationService.Contract.Messages;

namespace OTUS.HomeWork.MessageExchangeSerializer
{
	public class JsonNetMessageExchangeSerializer
		: IMessageExchangeSerializer
	{
		private readonly JsonSerializer _jsonSerializer;

		private static string _mimeType = "application/json";

		private static readonly DefaultContractResolver _contractResolver = new DefaultContractResolver
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		};

		private static readonly JsonSerializerSettings _serializerSetting; 

		public string MimeTypeName => _mimeType;

		static JsonNetMessageExchangeSerializer()
		{
			_serializerSetting = new JsonSerializerSettings()
			{
				ContractResolver = _contractResolver,
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore,
				Converters = new List<JsonConverter>()
				{
					new DeletedJsonConverter()
				}
			};
		}
		public JsonNetMessageExchangeSerializer()
		{
			_jsonSerializer = JsonSerializer.Create(_serializerSetting);
		}

		public TRequest DeserializeRequest<TRequest>(Stream stream)
			where TRequest : NotificationMessage, new()
		{
			var sr = new StreamReader(stream);
			var jsonTextReader = new JsonTextReader(sr);
			var rootDeser = _jsonSerializer.Deserialize<TRequest>(jsonTextReader);
			if (rootDeser == null)
			{
				stream.Position = 0;
				throw new Exception("Не удалось десериализовать сообщение в Response тип");
			}
			stream.Position = 0;
			return rootDeser;
		}

		public TResponse DeserializeResponse<TResponse>(Stream stream)
			where TResponse : NotificationMessage, new()
		{
			var sr = new StreamReader(stream);
			var jsonTextReader = new JsonTextReader(sr);
			var rootDeser = _jsonSerializer.Deserialize<TResponse>(jsonTextReader);
			if(rootDeser == null)
			{
				stream.Position = 0;
				throw new Exception("Не удалось десериализовать сообщение в Response тип");
			}
			stream.Position = 0;
			return rootDeser;
		}

		public string Serialize<T>(T obj)
		{
			throw new NotImplementedException();
		}

		public MemoryStream SerializeRequest<TRequest>(TRequest obj)
			where TRequest : NotificationMessage, new()
		{
			var stream = new MemoryStream();
			var wr = new StreamWriter(stream);
			var jwr = new JsonTextWriter(wr);
			_jsonSerializer.Serialize(jwr, obj);
			jwr.Flush();
			stream.Position = 0;
			return stream;
		}

		public MemoryStream SerializeResponse<TResponse>(TResponse obj)
			where TResponse : NotificationMessage, new()
		{
			var stream = new MemoryStream();
			var wr = new StreamWriter(stream);
			var jwr = new JsonTextWriter(wr);
			_jsonSerializer.Serialize(jwr, obj);
			jwr.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
