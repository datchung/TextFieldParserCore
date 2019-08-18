using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextFieldParserCore.Test
{
    [TestFixture]
    public class TextFieldParserCoreTest
    {
        [Test]
        public void ParseCsv()
        {
            var parsedRows = new List<string[]>();
            using (var streamReader = new StreamReader(StringToStream(
@"Col1,Col2,Col3
A,B,C
D,E,F")))
            {
                using (var parser = new TextFieldParserCore(streamReader))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    while (!parser.EndOfData)
                    {
                        parsedRows.Add(parser.ReadFields());
                    }
                }
            }

            Assert.AreEqual(3, parsedRows.Count);
            var i = 0;
            var j = 0;
            var row = parsedRows[i];
            Assert.AreEqual(3, row.Length, $"parsedRows[{i}] length");
            Assert.AreEqual("Col1", row[j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("Col2", row[++j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("Col3", row[++j], $"parsedRows[{i}] col[{j}]");

            j = 0;
            row = parsedRows[++i];
            Assert.AreEqual(3, row.Length, $"parsedRows[{i}] length");
            Assert.AreEqual("A", row[j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("B", row[++j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("C", row[++j], $"parsedRows[{i}] col[{j}]");

            j = 0;
            row = parsedRows[++i];
            Assert.AreEqual(3, row.Length, $"parsedRows[{i}] length");
            Assert.AreEqual("D", row[j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("E", row[++j], $"parsedRows[{i}] col[{j}]");
            Assert.AreEqual("F", row[++j], $"parsedRows[{i}] col[{j}]");
        }

        /// <summary>
        /// http://www.csharp411.com/c-convert-string-to-stream-and-stream-to-string/
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Stream StringToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }
    }
}