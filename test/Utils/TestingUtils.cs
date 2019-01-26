using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace test.Utils
{
    internal static class TestingUtils
    {
        public static NumberFormatInfo DotFormatter = new NumberFormatInfo {NumberDecimalSeparator = "."};

        public static void AssertDictionaryContainsItems<T>(IDictionary<string, T> dictionary,
            params (string, T)[] pairs)
        {
            foreach (var pair in pairs)
            {
                Assert.True(dictionary.ContainsKey(pair.Item1));
                Assert.AreEqual(dictionary[pair.Item1], pair.Item2);
            }
        }

        public static void AssertStringContainsValues(string containingString, params string[] values)
        {
            foreach (var value in values)
                StringAssert.Contains(value, containingString);
        }

        public static string BuildJson(params (string, object)[] pairs)
        {
            var builder = new StringBuilder();
            builder.Append("{ ");
            var stringifiedPairs = pairs.Select(pair =>
            {
                var value = pair.Item2;
                string jsonValue;
                switch (value)
                {
                    case null:
                        jsonValue = "null";
                        break;
                    case string _:
                        jsonValue = $"\"{value}\"";
                        break;
                    case double _:
                    case float _:
                        jsonValue = value is double
                            ? ((double) value).ToString(DotFormatter)
                            : ((float) value).ToString(DotFormatter);
                        break;
                    default:
                        jsonValue = value.ToString();
                        break;
                }

                return $"\"{pair.Item1}\":{jsonValue}";
            }).ToArray();
            builder.AppendJoin(", ", stringifiedPairs);
            builder.Append(" }");

            return builder.ToString();
        }

        public static InvisibleCollectorLib.Model.Model BuildModelMock(IDictionary<string, object> fields)
        {
            var mock = new Mock<InvisibleCollectorLib.Model.Model>();
            mock.Setup(m => m.SendableDictionary).Returns(fields);
            return mock.Object;
        }

        public static Stream StringToStream(string message)
        {
            Encoding uniEncoding = new UTF8Encoding();
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, uniEncoding);

            streamWriter.Write(message);
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public static void StripNulls<T>(this IDictionary<string, T> dictionary)
        {
            foreach (var nullEntry in dictionary.Where(entry => entry.Value == null).ToList())
                dictionary.Remove(nullEntry.Key);
        }
    }
}