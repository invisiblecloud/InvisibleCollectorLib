﻿using System;
using System.Collections.Generic;
using System.Text;
using InvisibleCollectorLib.Model;
using InvisibleCollectorLib.Utils;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    class DebtTest
    {
        [Test]
        public void Items_Immutability()
        {
            var items = BuildList();

            var listModified = BuildList();
            var debt = new Debt() { Items = listModified };
            listModified.Add(new Item());
            Assert.True(debt.Items.EqualsCollection(items));

            listModified[0].Name = "a different name";
            Assert.True(debt.Items.EqualsCollection(items));

            List<Item> BuildList()
            {
                return new List<Item>()
                {
                    new Item()
                    {
                        Name = "a name",
                    },
                    new Item()
                    {
                        Name = "new name",
                    },
                };
            }
        }

        [Test]
        public void Attributes_Immutability()
        {
            var attributes = new Dictionary<string, string>()
            {
                { "a", "b" },
                { "key", "value" }
            };

            var attributesModified = new Dictionary<string, string>(attributes);
            var debt = new Debt() { Attributes = attributesModified };
            attributesModified["4"] = "5";
            Assert.True(debt.Attributes.EqualsCollection(attributes));

            attributesModified["a"] = "aaaa";
            Assert.True(debt.Attributes.EqualsCollection(attributes));

        }

        [Test]
        public void AddItem_Correctness()
        {
            var item = new Item()
            {
                Name = "a name",
            };

            var debt = new Debt();
            Assert.IsNull(debt.Items);

            debt.AddItem(item);
            Assert.AreEqual(debt.Items.Count, 1);
            Assert.AreEqual(debt.Items[0], item);
        }

        [Test]
        public void SetAttribute_Correctness()
        {
            const string key = "key";
            const string value = "value";

            var debt = new Debt();
            Assert.IsNull(debt.Attributes);

            debt.SetAttribute(key, value);
            Assert.AreEqual(debt.Attributes.Count, 1);
            Assert.AreEqual(debt.Attributes[key], value);
        }

        [Test]
        public void AssertItemsHaveMandatoryFields_correctness()
        {
            var debt = new Debt();
            debt.AssertItemsHaveMandatoryFields(Item.NameName);

            debt.Items = new List<Item>();
            debt.AssertItemsHaveMandatoryFields(Item.NameName);

            var list = new List<Item>()
            {
                new Item()
                {
                    Name = "new name",
                },
            };
            debt.Items = list;
            debt.AssertItemsHaveMandatoryFields(Item.NameName);

            list.Add(new Item());
            debt.Items = list;
            Assert.Throws<ArgumentException>(() => debt.AssertItemsHaveMandatoryFields(Item.NameName));
        }

        [Test]
        public void SendableDictionary_correctness()
        {
            var debt = new Debt();
            Assert.AreEqual(debt.Fields.Count, 0);

            debt.Items = new List<Item>();
            var items = (IList<IDictionary<string, object>>) debt.SendableDictionary[Debt.ItemsName];
            Assert.AreEqual(items.Count, 0);

            const string name = "a name";
            debt.Items = new List<Item>() {
                new Item()
                {
                    Name = name
                }
            };
            var items2 = (IList<IDictionary<string, object>>)debt.SendableDictionary[Debt.ItemsName];
            Assert.AreEqual(items2.Count, 1);
            Assert.AreEqual(name, items2[0]["name"]);
        }

        [Test]
        public void EqualityOperator_Nulls()
        {
            var debt = new Debt();
            Assert.False(debt == null);
        }

        [Test]
        public void EqualityOperator_NoNestedCollection()
        {
            var id = "1234";
            var date = new DateTime(2015,2,3);

            var debt1 = new Debt() { Id = id, Date = date };
            var debt2 = new Debt() { Id = id, Date = date };
            Assert.True(debt1 == debt2);
        }

        [Test]
        public void EqualityOperator_ItemsCorrectness()
        {
            var id = "1234";

            var debt1 = new Debt() { Id = id };
            var debt2 = new Debt() { Id = id };



            Assert.True(debt1 == debt2);
        }

    }
}
