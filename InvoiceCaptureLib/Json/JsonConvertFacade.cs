using System.Collections.Generic;
using System.IO;
using InvoiceCaptureLib.Exception;
using Newtonsoft.Json;

namespace InvoiceCaptureLib.Json
{
    internal class JsonConvertFacade
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
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

        internal T JsonToModel<T>(string json)
            where T : Model.Model, new()
        {
            var fields = JsonToDictionary(json);
            return new T {Fields = fields};
        }

        internal string ModelToSendableJson(Model.Model model)
        {
            try
            {
                return JsonConvert.SerializeObject(model.SendableDictionary, SerializerSettings);
            }
            catch (JsonException e)
            {
                throw new IcException($"Failed to create json: {e.Message}", e);
            }
        }

        internal IDictionary<string, object> JsonToDictionary(string json)
        {
            try
            { 
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json, SerializerSettings);
            }
            catch (JsonException e)
            {
                throw new IcException($"Failed to parse json: {e.Message}", e);
            }
        }
    }
}