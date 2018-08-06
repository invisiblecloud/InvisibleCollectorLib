using System;
using System.Collections.Generic;
using System.Text;
using InvoiceCaptureLib.Model;
using Newtonsoft.Json;

namespace test.Model
{
    class ModelBuilder
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
        };

        private const string VatNumber = "510205933"; // is actually valid in pt
        private const string Id = "4567";

        private IDictionary<string, object> _fields = new Dictionary<string, object>();

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

        public T buildModel<T>()
            where T : InvoiceCaptureLib.Model.Model, new()
        {
            return new T() { Fields = new Dictionary<string, object>(_fields)};
        }

        public string buildJson()
        {
            return JsonConvert.SerializeObject(_fields, SerializerSettings);
        }

        public static ModelBuilder BuildReplyCompanyBuilder()
        {
            var builder = BuildRequestCompanyBuilder();
            builder[Company.IdName] = Id;

            return builder;
        }

        public static ModelBuilder BuildRequestCompanyBuilder()
        {
            var fields = new Dictionary<string, object>()
            {
                { Company.VatNumberName, VatNumber },
                { Company.NameName, "a name" },
                { Company.ZipCodeName, null },
            };

            return new ModelBuilder(fields);
        }

        public ModelBuilder clone()
        {
            return new ModelBuilder(_fields);
        }
    }
}
