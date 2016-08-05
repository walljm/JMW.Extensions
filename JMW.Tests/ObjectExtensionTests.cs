using NUnit.Framework;

namespace JMW.Extensions.String.Tests
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        [Test]
        public void IsEmpty_IsNotEmpty()
        {
            Assert.That("This is a".IsNotEmpty());
            Assert.That("".IsEmpty());
            Assert.That(((string)null).IsEmpty());
        }
    }
}