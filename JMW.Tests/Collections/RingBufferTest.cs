using NUnit.Framework;

namespace JMW.Collections.Tests
{
    [TestFixture]
    public class RingBufferTests
    {
        [Test]
        public void RingBufferTest()
        {
            var buff = new RingBuffer<int>(10);
            for (var i = 0; i < 15; i++)
                buff.Add(i);

            Assert.AreEqual(10, buff.Count);

            var c = 5;
            foreach (var i in buff)
                Assert.AreEqual(c++, i);

            Assert.AreEqual(15, c);
            Assert.IsTrue(buff.Contains(11));
            buff.Clear();
            Assert.AreEqual(0, buff.Count);
        }

        [Test]
        public void RingBufferTest1()
        {
            var buff = new RingBuffer<int>(10, true);
            for (var i = 0; i < 15; i++)
                buff.Add(i);

            var c = 14;
            foreach (var i in buff)
                Assert.AreEqual(c--, i);

            Assert.AreEqual(4, c);
            Assert.AreEqual(10, buff.Count);
        }

        [Test]
        public void RingBufferTest2()
        {
            var buff = new RingBuffer<int>(new[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
            Assert.AreEqual(10, buff.Count);
            Assert.IsTrue(buff.Contains(11));
            buff.Clear();
            Assert.AreEqual(0, buff.Count);
        }

        [Test]
        public void RingBufferTest3()
        {
            var buff = new RingBuffer<int>(new[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, true);

            var c = 12;
            foreach (var i in buff)
                Assert.AreEqual(c--, i);
            Assert.AreEqual(2, c);
            Assert.AreEqual(10, buff.Count);
        }
    }
}