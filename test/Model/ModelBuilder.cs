using System;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Model;
using Newtonsoft.Json;
using test.Utils;

namespace test.Model
{
    internal class DebtBuilder : ModelBuilder
    {
        public DebtBuilder(Dictionary<string, object> fields): base(fields)
        { }

        public override string BuildJson()
        {
            var key = Debt.ItemsName;
            if (_fields.ContainsKey(key) && _fields[key] != null)
            {
                _fields[key] = ((IList<Item>) _fields[key]).Select(item => item.SendableDictionary)
                    .ToList();
            }

            return base.BuildJson();
        }
    } 

    internal class ModelBuilder
    {
        internal const string Id = "4567";

        private const string VatNumber = "510205933"; // is actually valid in pt

        protected readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

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

        public virtual string BuildJson()
        {
            return JsonConvert.SerializeObject(_fields, SerializerSettings);
        }

        public virtual T BuildModel<T>(bool bStripNull = false)
            where T : InvisibleCollectorLib.Model.Model, new()
        {
            var fields = new Dictionary<string, object>(_fields);
            if (bStripNull)
                fields.StripNulls();

            return new T {Fields = fields};
        }

        public static Item BuildItem(string name = "item-name")
        {
            return new Item
            {
                Name = name,
                Description = "a description",
                Price = 12.0,
                Quantity = 2,
                Vat = 24
            };
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

        // should only add the id
        public static ModelBuilder BuildReplyDebtBuilder(string id = Id)
        {
            var builder = BuildRequestDebtBuilder();
            builder[Debt.IdName] = id;

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

        public static ModelBuilder BuildRequestDebtBuilder(string number = "1")
        {
            var date = new DateTime(2018, 10, 5);

            var fields = new Dictionary<string, object>
            {
                {Debt.NumberName, number},
                {Debt.CustomerIdName, "f733cece-7b69-4ae5-93cd-33bfa5f0d333"},
                {Debt.TypeName, "FT"},
                {Debt.DateName, date},
                {Debt.DueDateName, date.AddYears(1)},
                {
                    Debt.ItemsName, new List<Item>
                    {
                        BuildItem("name-1"),
                        BuildItem("name-2")
                    }
                },
                {
                    Debt.AttributesName, new Dictionary<string, string>
                    {
                        {"attr-1", "val-1"},
                        {"attr-2", "val-2"}
                    }
                }
            };

            return new DebtBuilder(fields);
        }

        internal static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
        };

        public static string DictToJson(IDictionary<string, string> dict)
        {
            return JsonConvert.SerializeObject(dict, SerializerSettings);
        }
    }
}