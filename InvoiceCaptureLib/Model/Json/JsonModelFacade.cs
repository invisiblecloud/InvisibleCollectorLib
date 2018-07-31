using System.Collections.Generic;
using Newtonsoft.Json;

namespace InvoiceCaptureLib.Model.Json
{
    internal class JsonModelConverterFacade
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd", 
        }; 

        internal string ModelToSendableJson(Model model)
        {
            return JsonConvert.SerializeObject(model.SendableDictionary, SerializerSettings);
        }

        private IDictionary<string, object> JsonToDictionary(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json, SerializerSettings);
        }

        internal T JsonToModel<T>(string json) 
            where T : Model, new () 
        {
            var fields = JsonToDictionary(json);
            return new T() { Fields = fields };
        }
    }
}
