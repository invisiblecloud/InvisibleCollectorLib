using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InvisibleCollectorLib.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using test.Utils;

namespace test.Model
{
    internal class DebtBuilder : ModelBuilder
    {
        public DebtBuilder(Dictionary<string, object> fields) : base(fields)
        {
        }

        public override string BuildJson()
        {
            return BuildJson<Item>(Debt.ItemsName);
        }
    }

    internal class CustomerBuilder : ModelBuilder
    {
        public CustomerBuilder(IDictionary<string, object> fields) : base(fields)
        {
        }

        public CustomerBuilder WithoutContacts()
        {
            var copy = new CustomerBuilder(_fields);
            var contacts = copy[Customer.ContactsName];
            if (contacts != null)
            {
                copy._fields.Remove(Customer.ContactsName);
            }

            return copy;
        }

        public string BuildContactsJson()
        {
            var customerContacts = (IList<CustomerContact>) _fields[Customer.ContactsName];
            var contacts = customerContacts.Select(c => c.SendableDictionary).ToList();
            return JsonConvert.SerializeObject(contacts, SerializerSettings);
        }

        public override string BuildJson()
        {
            return BuildJson<CustomerContact>(Customer.ContactsName);
        }
    }

    internal class PaymentBuilder : ModelBuilder
    {
        public PaymentBuilder(Dictionary<string, object> fields) : base(fields)
        {
        }

        public override string BuildJson()
        {
            return BuildJson<PaymentLine>(Payment.LinesName);
        }
    }

    internal class ModelBuilder
    {
        internal const string Id = "4567";

        private const string VatNumber = "510205933"; // is actually valid in pt

        internal static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
        };

        internal readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

        public ModelBuilder()
        {
        }

        public ModelBuilder(IDictionary<string, object> fields)
        {
            _fields = new Dictionary<string, object>(fields);
        }

        public ModelBuilder(ModelBuilder other) : this(other._fields)
        {
        }

        protected string BuildJson<T>(string key)
        where T: InvisibleCollectorLib.Model.Model
        {
            var copy = new ModelBuilder(this);
            if (_fields.ContainsKey(key) && _fields[key] != null)
                copy._fields[key] = ((IList<T>) copy._fields[key]).Select(item => item.SendableDictionary)
                    .ToList();

            return copy.BuildJson();
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

        public static void AssertEquals<TModel>(TModel expectedModel, TModel result)
            where TModel : InvisibleCollectorLib.Model.Model, new()
        {
            Assert.AreEqual(expectedModel, result);
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

        public virtual string BuildJson()
        {
            return JsonConvert.SerializeObject(_fields, SerializerSettings);
        }

        // should be overriden by nested objects
        public ModelBuilder WithStrippedNulls()
        {
            _fields.StripNulls();

            return this;
        }

        public virtual T BuildModel<T>(bool bStripNull = false)
            where T : InvisibleCollectorLib.Model.Model, new()
        {
            var fields = new Dictionary<string, object>(_fields);
            if (bStripNull)
                fields.StripNulls();

            return new T {FieldsShallow = fields};
        }

        // should only add the id
        public static ModelBuilder BuildReplyCompanyBuilder()
        {
            var builder = BuildRequestCompanyBuilder();
            builder[Company.IdName] = Id;

            return builder;
        }

        // should only add the id
        public static ModelBuilder BuildReplyCustomerBuilder(string name = "johny")
        {
            var builder = BuildRequestCustomerBuilder(name);
            builder[Customer.IdName] = Id;

            return builder;
        }

        // should only add the id
        public static ModelBuilder BuildReplyDebtBuilder(string id = Id, string number = Id)
        {
            var builder = BuildRequestDebtBuilder(number);
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

        public static ModelBuilder BuildRequestCustomerBuilder(string name = "johny")
        {
            var fields = new Dictionary<string, object>
            {
                {Customer.VatNumberName, VatNumber},
                {Customer.NameName, name},
                {Customer.ZipCodeName, null},
                {Customer.CountryName, "PT"},
                {
                    Customer.AttributesName, new Dictionary<string, string>()
                    {
                        {"a2", "b2"},
                        {"1", "2"},
                    }
                },
            };

            return new ModelBuilder(fields);
        }

        public static CustomerBuilder BuildRequestCustomerWithContactsBuilder(string name = "johny")
        {
            var fields = BuildRequestCustomerBuilder(name)._fields;

            fields[Customer.ContactsName] = new List<CustomerContact>()
                {
                    new CustomerContact() {Name = "janis", Email = "janis@email.com"},
                    new CustomerContact() {Name = "dennis", Mobile = "920920920"},
                };

            return new CustomerBuilder(fields);
        }

        public static ModelBuilder BuildDebtNoteBuilder(string number = "12")
        {
            var fields = new Dictionary<string, object>
            {
                {Debit.NumberName, number},
                {Debit.DateName, new DateTime()},
            };

            return new ModelBuilder(fields);
        }

        public static ModelBuilder BuildGroupBuilder(string id = "12", string name = "boris")
        {
            var fields = new Dictionary<string, object>
            {
                {Group.IdName, id},
                {Group.NameName, name},
            };

            return new ModelBuilder(fields);
        }

        public static ModelBuilder BuildRequestCustomerContactBuilder(string name)
        {
            var fields = new Dictionary<string, object>
            {
                {CustomerContact.EmailName, name + "@b.com"},
                {CustomerContact.NameName, name}
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

        public static PaymentLine BuildPaymentLine(string number = "123")
        {
            return new PaymentLine
            {
                Number = number,
                Amount = 12.0
            };
        }

        public static ModelBuilder BuildReplyPaymentBuilder(string number = "1")
        {
            var date = new DateTime(2018, 10, 5);

            var fields = new Dictionary<string, object>
            {
                {Payment.NumberName, number},
                {Payment.ExternalIdName, "f733cece-7b69-4ae5-93cd-33bfa5f0d333"},
                {Payment.TypeName, "RG"},
                {Payment.CurrencyName, "EUR"},
                {Payment.DateName, date},
                {
                    Payment.LinesName, new List<PaymentLine>
                    {
                        BuildPaymentLine("1"),
                        BuildPaymentLine("2")
                    }
                }
            };

            return new PaymentBuilder(fields);
        }


        public static string DictToJson(IDictionary<string, string> dict)
        {
            return JsonConvert.SerializeObject(dict, SerializerSettings);
        }

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }
    }

    internal class ModelListBuilder
    {
        private List<ModelBuilder> _list = new List<ModelBuilder>();

        public ModelListBuilder Add(ModelBuilder builder)
        {
            _list.Add(builder);
            return this;
        }

        public IList<T> BuildModelList<T>()
            where T : InvisibleCollectorLib.Model.Model, new()
        {
            return _list.Select(e => e.BuildModel<T>()).ToList();
        }

        public string BuildJson()
        {
            var body = String.Join(",", _list.Select(e => e.BuildJson()));
            return $"[ {body} ]";
        }
    }
}