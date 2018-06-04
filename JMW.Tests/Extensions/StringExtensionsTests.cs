using System.Linq;
using NUnit.Framework;

namespace JMW.Extensions.String.Tests
{
    [TestFixture]
    public class StringExtensionsTests
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
            Assert.AreEqual("that this is a test", "that this is a testfoo blah foo blah foo ".ParseToIndexOf("blah", "foo"));
            Assert.AreEqual("that this is a test", "that this is a testfoo blah foo blah foo ".ParseToIndexOf("foo", "blah"));
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
            Assert.AreEqual("that this is a test foo ", "that this is a test foo blah jason".ParseToLastIndexOf("blah", "foo"));
            Assert.AreEqual("that this is a test foo ", "that this is a test foo blah jason".ParseToLastIndexOf("foo", "blah"));
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
            Assert.AreEqual("that this is a test foo", "that this is a test foo blah jason".ParseToIndexOf_PlusLength("blah", "foo"));
            Assert.AreEqual("that this is a test foo", "that this is a test foo blah jason".ParseToIndexOf_PlusLength("foo", "blah"));
        }

        [Test]
        public void ParseToDesignatedIndexOf_PlusLengthTest1()
        {
            var str = "This is a string a string a string";
            Assert.That("This is a string a str" == str.ParseToDesignatedIndexOf_PlusLength(2, false, "str"));
            Assert.That("This is a string a str" == str.ParseToDesignatedIndexOf_PlusLength(2, true, "Str"));
            Assert.That(str == str.ParseToDesignatedIndexOf_PlusLength(1, false, "blah"));
            Assert.That(str == str.ParseToDesignatedIndexOf_PlusLength(1, false, "Str"));
            Assert.That(string.Empty == string.Empty.ParseToDesignatedIndexOf_PlusLength(1, false, "Str"));
            Assert.That("that" == "that".ParseToDesignatedIndexOf_PlusLength(1, false));
            Assert.AreEqual("that this is a test foo", "that this is a test foo blah jason".ParseToDesignatedIndexOf_PlusLength(1, false, "blah", "foo"));
            Assert.AreEqual("that this is a test foo", "that this is a test foo blah jason".ParseToDesignatedIndexOf_PlusLength(1, false, "foo", "blah"));
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
            Assert.AreEqual("that this is a test foo blah", "that this is a test foo blah jason".ParseToLastIndexOf_PlusLength("blah", "foo"));
            Assert.AreEqual("that this is a test foo blah", "that this is a test foo blah jason".ParseToLastIndexOf_PlusLength("foo", "blah"));
        }

        [Test]
        public void ParseAfterIndexOfTest1()
        {
            var str = "This is is a string";
            Assert.That("is is is a string" == str.ParseAfterIndexOf("is"));
            Assert.That("is is is a string" == str.ParseAfterIndexOf(true, "IS"));
            Assert.That(str == str.ParseAfterIndexOf("blah"));
            Assert.That(str == str.ParseAfterIndexOf("Str"));
            Assert.That(string.Empty == string.Empty.ParseAfterIndexOf("Str"));
            Assert.That("that" == "that".ParseAfterIndexOf());
            Assert.AreEqual("blah foo that this is a test", "jason blah foo that this is a test".ParseAfterIndexOf("blah", "foo"));
            Assert.AreEqual("blah foo that this is a test", "jason blah foo that this is a test".ParseAfterIndexOf("foo", "blah"));
        }

        [Test]
        public void ParseAfterLastIndexOfTest1()
        {
            var str = "This is is a string";
            Assert.That("is a string" == str.ParseAfterLastIndexOf("is"));
            Assert.That("is a string" == str.ParseAfterLastIndexOf(true, "IS"));
            Assert.That(str == str.ParseAfterLastIndexOf("blah"));
            Assert.That(str == str.ParseAfterLastIndexOf("Str"));
            Assert.That(string.Empty == string.Empty.ParseAfterLastIndexOf("Str"));
            Assert.That("that" == "that".ParseAfterLastIndexOf());
            Assert.AreEqual("foo that this is a test", "jason blah foo that this is a test".ParseAfterLastIndexOf("foo", "blah"));
            Assert.AreEqual("foo that this is a test", "jason blah foo that this is a test".ParseAfterLastIndexOf("blah", "foo"));
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
            Assert.AreEqual(" blah that this is a test", "jason foo blah that this is a test".ParseAfterIndexOf_PlusLength("blah", "foo"));
            Assert.AreEqual(" blah that this is a test", "jason foo blah that this is a test".ParseAfterIndexOf_PlusLength("foo", "blah"));
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
            Assert.AreEqual(" that this is a test", "jason foo blah that this is a test".ParseAfterLastIndexOf_PlusLength("blah", "foo"));
            Assert.AreEqual(" that this is a test", "jason foo blah that this is a test".ParseAfterLastIndexOf_PlusLength("foo", "blah"));
        }

        [Test]
        public void ParseAfterDesignatedIndexOf_PlusLengthTest1()
        {
            var str = "This is a string a string a string";
            Assert.That("ing a string" == str.ParseAfterDesignatedIndexOf_PlusLength(2, false, "str"));
            Assert.That("ing a string" == str.ParseAfterDesignatedIndexOf_PlusLength(2, true, "Str"));
            Assert.That(str == str.ParseAfterDesignatedIndexOf_PlusLength(1, false, "blah"));
            Assert.That(str == str.ParseAfterDesignatedIndexOf_PlusLength(1, false, "Str"));
            Assert.That(string.Empty == string.Empty.ParseAfterDesignatedIndexOf_PlusLength(1, false, "Str"));
            Assert.That("that" == "that".ParseAfterDesignatedIndexOf_PlusLength(1, false));
            Assert.AreEqual(" blah jason", "that this is a test foo blah jason".ParseAfterDesignatedIndexOf_PlusLength(1, false, "blah", "foo"));
            Assert.AreEqual(" jason", "that this is a test foo blah jason".ParseAfterDesignatedIndexOf_PlusLength(2, false, "foo", "blah"));
        }


        [Test]
        public void IndexOfTest1()
        {
            //        01234567890123456798012345647980123
            var str = "This is a string a string a string";
            Assert.AreEqual(10, str.IndexOf("STR", true, 1));
            Assert.AreEqual(10, str.IndexOf("str", false, 1));

            Assert.AreEqual(19, str.IndexOf("STR", true, 2));
            Assert.AreEqual(19, str.IndexOf("str", false, 2));
        }


        [Test]
        public void ToLinesTest()
        {
            var str = "This is another\r\nThis is a string\r\n a string";
            Assert.AreEqual(3, str.ToLines().Count());
        }

        [Test]
        public void ContainsFastTest()
        {
            var str = "This is another\r\nThis is a string\r\n a string";
            Assert.AreEqual(true, str.ContainsFast('t'));
            Assert.AreEqual(false, str.ContainsFast('q'));
        }

        [Test]
        public void ToPascalCaseTest()
        {
            var str = "This is another";
            Assert.AreEqual("ThisIsAnother", str.ToPascalCase());
            Assert.AreEqual("This_IsAnother", "This _ is another".ToPascalCase());
        }
    }
}