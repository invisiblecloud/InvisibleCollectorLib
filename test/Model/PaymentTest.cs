using System;
using System.Collections.Generic;
using InvisibleCollectorLib.Model;
using NUnit.Framework;
using InvisibleCollectorLib.Utils;

namespace test.Model
{
    [TestFixture]

    public class PaymentTest
    {
        [Test]
        public void AddLine_Correctness()
        {
            var item = new PaymentLine
            {
                ReferenceNumber = "1"
            };

            var debt = new Payment();
            Assert.IsNull(debt.Lines);

            debt.AddLine(item);
            Assert.AreEqual(debt.Lines.Count, 1);
            Assert.AreEqual(debt.Lines[0], item);
        }
        
        [Test]
        public void AssertLinesHaveMandatoryFields_correctness()
        {
            var debt = new Payment();
            debt.AssertLinesHaveMandatoryFields(Item.NameName);

            debt.Lines = new List<PaymentLine>();
            debt.AssertLinesHaveMandatoryFields(Item.NameName);

            var list = new List<PaymentLine>
            {
                new PaymentLine()
                {
                    Number = "123"
                }
            };
            debt.Lines = list;
            debt.AssertLinesHaveMandatoryFields(PaymentLine.NumberName);

            list.Add(new PaymentLine());
            debt.Lines = list;
            Assert.Throws<ArgumentException>(() => debt.AssertLinesHaveMandatoryFields(Item.NameName));
        }
        
        [Test]
        public void EqualityOperator_NoNestedCollection()
        {
            var number = "1234";
            var date = new DateTime(2015, 2, 3);

            var pay1 = new Payment {Number = number, Date = date};
            var pay2 = new Payment {Number = number, Date = date};
            Assert.True(pay1 == pay2);
        }

        [Test]
        public void EqualityOperator_Nulls()
        {
            var payment = new Payment();
            Assert.False(payment == null);
        }
        
        [Test]
        public void EqualityOperator_LinesCorrectness()
        {
            var number = "1234";
            var date = new DateTime(2015, 2, 3);

            var pay1 = new Payment {Number = number, Date = date};
            var pay2 = new Payment {Number = number, Date = date};

            var lines = new List<PaymentLine>();

            pay1.Lines = lines;
            pay2.Lines = lines;
            Assert.True(pay1 == pay2);

            lines.Add(new PaymentLine {Number = "123"});
            pay1.Lines = lines;
            pay2.Lines = lines;
            Assert.True(pay1 == pay2);


            lines[0] = new PaymentLine {Number = "678"};
            pay2.Lines = lines;
            Assert.False(pay1 == pay2);

            lines.Clear();
            pay2.Lines = lines;
            Assert.False(pay1 == pay2);
        }
        
        [Test]
        public void Items_Immutability()
        {
            var expected = BuildList();

            var actual = BuildList();
            var payment = new Payment {Lines = actual};
            actual.Add(new PaymentLine());
            Assert.True(payment.Lines.EqualsCollection(expected));

            actual[0].Number = "5555";
            Assert.True(payment.Lines.EqualsCollection(expected));

            List<PaymentLine> BuildList()
            {
                return new List<PaymentLine>
                {
                    new PaymentLine
                    {
                        Number = "1"
                    },
                    new PaymentLine
                    {
                        Number = "91"
                    }
                };
            }
        }
    }
}