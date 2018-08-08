using System.Collections.Generic;
using InvisibleCollectorLib.Model;
using Newtonsoft.Json;
using test.Utils;


namespace test.Model
{
    internal class ModelBuilder
    {
        private const string Id = "4567";

        private const string VatNumber = "510205933"; // is actually valid in pt

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
        };

        private readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

        public ModelBuilder()
        {
        }

        public ModelBuilder(IDictionary<string, object> fields)
        {
            _fields = new Dictionary<string, object>(fields);
        }

        public object this[string key]
        {
            set => _fields[key] = value;

            get
            {
                _fields.TryGetValue(key, out var value);
                return value;
            }
        }

        public string BuildJson()
        {
            return JsonConvert.SerializeObject(_fields, SerializerSettings);
        }

        public T BuildModel<T>(bool bStripNull = false)
            where T : InvisibleCollectorLib.Model.Model, new()
        {
            var fields = new Dictionary<string, object>(_fields);
            if (bStripNull)
                fields.StripNulls();

            return new T {Fields = fields };
        }

        // should only add the id
        public static ModelBuilder BuildReplyCompanyBuilder()
        {
            var builder = BuildRequestCompanyBuilder();
            builder[Company.IdName] = Id;

            return builder;
        }

        // should only add the id
        public static ModelBuilder BuildReplyCustomerBuilder()
        {
            var builder = BuildRequestCustomerBuilder();
            builder[Customer.IdName] = Id;

            return builder;
        }

        public static ModelBuilder BuildRequestCompanyBuilder()
        {
            var fields = new Dictionary<string, object>
            {
                {Company.VatNumberName, VatNumber},
                {Company.NameName, "a name"},
                {Company.ZipCodeName, null}
            };

            return new ModelBuilder(fields);
        }

        public static ModelBuilder BuildRequestCustomerBuilder()
        {
            var fields = new Dictionary<string, object>
            {
                {Customer.VatNumberName, VatNumber},
                {Customer.NameName, "a name"},
                {Customer.ZipCodeName, null},
                {Customer.CountryName, "PT"}
            };

            return new ModelBuilder(fields);
        }

        public ModelBuilder Clone()
        {
            return new ModelBuilder(_fields);
        }

        public static string DictToJson(IDictionary<string, string> dict)
        {
            return JsonConvert.SerializeObject(dict, SerializerSettings);
        }
    }
}