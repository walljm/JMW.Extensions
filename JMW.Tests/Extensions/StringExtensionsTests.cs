using System.Linq;
using NUnit.Framework;

namespace JMW.Extensions.String.Tests
{
    [TestFixture]
    public class TextExtensionsTests
    {
        [Test]
        public void SafeSubstringTest1()
        {
            var str = "This is a string";
            Assert.That("This is a" == str.SafeSubstring(0, 9));
            Assert.That("This is a string" == str.SafeSubstring(0, 30));
            Assert.That("is a" == str.SafeSubstring(5, 4));
            Assert.That(string.Empty == str.SafeSubstring(30));
            Assert.That(string.Empty == str.SafeSubstring(30, 10));
            Assert.That(string.Empty == str.SafeSubstring(0, -10));
            Assert.That(string.Empty == str.SafeSubstring(0, 0));
        }

        [Test]
        public void ParseToIndexOfTest1()
        {
            var str = "This is a string";
            Assert.That("This is a " == str.ParseToIndexOf("str"));
            Assert.That("This is a " == str.ParseToIndexOf(true, "Str"));
            Assert.That(str == str.ParseToIndexOf("blah"));
            Assert.That(str == str.ParseToIndexOf("Str"));
            Assert.That(string.Empty == string.Empty.ParseToIndexOf("Str"));
            Assert.That("that" == "that".ParseToIndexOf());
        }

        [Test]
        public void ParseToLastIndexOfTest1()
        {
            var str = "This is a string a string";
            Assert.That("This is a string a " == str.ParseToLastIndexOf("str"));
            Assert.That("This is a string a " == str.ParseToLastIndexOf(true, "Str"));
            Assert.That(str == str.ParseToLastIndexOf("blah"));
            Assert.That(str == str.ParseToLastIndexOf("Str"));
            Assert.That(string.Empty == string.Empty.ParseToLastIndexOf("Str"));
            Assert.That("that" == "that".ParseToLastIndexOf());
        }

        [Test]
        public void ParseToIndexOf_PlusLengthTest1()
        {
            var str = "This is a string";
            Assert.That("This is a str" == str.ParseToIndexOf_PlusLength("str"));
            Assert.That("This is a str" == str.ParseToIndexOf_PlusLength(true, "Str"));
            Assert.That(str == str.ParseToIndexOf_PlusLength("blah"));
            Assert.That(str == str.ParseToIndexOf_PlusLength("Str"));
            Assert.That(string.Empty == string.Empty.ParseToIndexOf_PlusLength("Str"));
            Assert.That("that" == "that".ParseToIndexOf_PlusLength());
        }

        [Test]
        public void ParseToLastIndexOf_PlusLengthTest1()
        {
            var str = "This is a string a string";
            Assert.That("This is a string a str" == str.ParseToLastIndexOf_PlusLength("str"));
            Assert.That("This is a string a str" == str.ParseToLastIndexOf_PlusLength(true, "Str"));
            Assert.That(str == str.ParseToLastIndexOf_PlusLength("blah"));
            Assert.That(str == str.ParseToLastIndexOf_PlusLength("Str"));
            Assert.That(string.Empty == string.Empty.ParseToLastIndexOf_PlusLength("Str"));
            Assert.That("that" == "that".ParseToLastIndexOf_PlusLength());
        }

        [Test]
        public void ParseAfterIndexOf_PlusLengthTest1()
        {
            var str = "This is a string";
            Assert.That(" is a string" == str.ParseAfterIndexOf_PlusLength("This"));
            Assert.That(" is a string" == str.ParseAfterIndexOf_PlusLength(true, "this"));
            Assert.That(str == str.ParseAfterIndexOf_PlusLength("blah"));
            Assert.That(str == str.ParseAfterIndexOf_PlusLength("Str"));
            Assert.That(string.Empty == string.Empty.ParseAfterIndexOf_PlusLength("Str"));
            Assert.That("that" == "that".ParseAfterIndexOf_PlusLength());
        }

        [Test]
        public void ParseAfterLastIndexOf_PlusLengthTest1()
        {
            var str = "This is another This is a string a string";
            Assert.That(" is a string a string" == str.ParseAfterLastIndexOf_PlusLength("This"));
            Assert.That(" is a string a string" == str.ParseAfterLastIndexOf_PlusLength(true, "this"));
            Assert.That(str == str.ParseAfterLastIndexOf_PlusLength("blah"));
            Assert.That(str == str.ParseAfterLastIndexOf_PlusLength("Str"));
            Assert.That(string.Empty == string.Empty.ParseAfterLastIndexOf_PlusLength("Str"));
            Assert.That("that" == "that".ParseAfterLastIndexOf_PlusLength());
        }

        [Test]
        public void ToLinesTest()
        {
            var str = "This is another\r\nThis is a string\r\n a string";
            Assert.AreEqual(3, str.ToLines().Count());
        }
    }
}