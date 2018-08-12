﻿using System;
using System.Collections.Generic;
using System.Text;
using InvisibleCollectorLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    class ItemTest
    {
        [Test]
        public void Clone_correctness()
        {
            const string name = "a name";
            var item1 = new Item()
            {
                Name = name
            };

            var item2 = item1.Clone();
            Assert.AreEqual(item1, item2);
        }
    }
}
