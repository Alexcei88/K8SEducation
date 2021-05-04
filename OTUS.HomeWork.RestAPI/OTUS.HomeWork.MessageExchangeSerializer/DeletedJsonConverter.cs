using System;
using Newtonsoft.Json;

namespace OTUS.HomeWork.MessageExchangeSerializer
{
	/// <summary>
	/// Обрабатывает конверт строки в C# bool тип
	/// </summary>
	public class DeletedJsonConverter
		: JsonConverter<bool>
	{
		public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			switch (reader.Value.ToString().ToLower().Trim())
			{
				case "true":
				case "1":
					return true;
				case "false":
				case "0":
					return false;
			}

			// If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
			return new JsonSerializer().Deserialize<bool>(reader);
		}

		public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
		{
			writer.WriteValue((bool)value);
		}
	}
}
