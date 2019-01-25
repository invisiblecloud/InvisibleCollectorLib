using System.Collections.Generic;
using System.IO;
using InvisibleCollectorLib.Exception;
using InvisibleCollectorLib.Utils;
using Newtonsoft.Json;

namespace InvisibleCollectorLib.Json
{
    internal class JsonConvertFacade
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = IcConstants.DateTimeFormat
        };

        //should nulls be ignored?
        private static readonly JsonSerializerSettings DeserializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = IcConstants.DateTimeFormat
        };


        internal IDictionary<string, string> JsonStreamToStringDictionary(Stream stream)
        {
            try
            {
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var jsonSerializer = JsonSerializer.Create(SerializerSettings);
                    return jsonSerializer.Deserialize<Dictionary<string, string>>(jsonReader);
                }
            }
            catch (JsonException e)
            {
                throw new IcException($"Failed to parse json stream: {e.Message}", e);
            }
        }

        internal T JsonToObject<T>(string json)
            where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, DeserializerSettings);
            }
            catch (JsonException e)
            {
                throw new IcException($"Failed to parse json: {e.Message}", e);
            }
        }
        
        internal string DictionaryToJson<TValue>(IDictionary<string, TValue> dict)
        {
            try
            {
                return JsonConvert.SerializeObject(dict, SerializerSettings);
            }
            catch (JsonException e)
            {
                throw new IcException($"Failed to create json: {e.Message}", e);
            }
        }

      
    }
}