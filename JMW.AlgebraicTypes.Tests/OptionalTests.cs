using NUnit.Framework;

namespace JMW.AlgebraicTypes.Tests
{
    [TestFixture]
    public class OptionalTests
    {
        [Test]
        public void OptionalConstructorTest1()
        {
            string v = null;
            var c = new Maybe<string>(v);
            Assert.AreEqual(false, c.Success);
        }
    }
}