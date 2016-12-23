using JMW.Types;
using NUnit.Framework;

namespace JMW.Extensions.Tests
{
    [TestFixture]
    public class IntegerRangeCollectionTests
    {
        [Test]
        public void Test1()
        {
            // arrange/act
            var foo = new IntegerRangeCollection("1-4,5,7,10,8, 12-15, 8-10, 19");

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19"));
            Assert.That(foo.IsInRange(9), Is.EqualTo(true));
            Assert.That(foo.IsInRange(3), Is.EqualTo(true));
            Assert.That(foo.IsInRange(13), Is.EqualTo(true));
            Assert.That(foo.IsInRange(0), Is.EqualTo(false));
            Assert.That(foo.IsInRange(20), Is.EqualTo(false));
        }
    }
}