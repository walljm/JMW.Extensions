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
            Assert.AreEqual(14, buff.Last());

            var c = 5;
            foreach (var i in buff)
                Assert.AreEqual(c++, i);

            Assert.AreEqual(15, c);
            Assert.IsTrue(buff.Contains(11));
            buff.Clear();
            Assert.AreEqual(0, buff.Count);
            Assert.IsFalse(buff.IsReadOnly);
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

        [Test]
        public void RingBufferTest4()
        {
            var buff = new RingBuffer<int>(new[] { 3, 4, 5 });
            Assert.AreEqual(5, buff.Last());
            buff = new RingBuffer<int>(new[] { 3, 4, 5 }, true);
            Assert.AreEqual(3, buff.Last());

            buff = new RingBuffer<int>(10, new[] { 3, 4, 5 });
            Assert.AreEqual(5, buff.Last());
            Assert.AreEqual(3, buff.Count);

            buff = new RingBuffer<int>(10, new[] { 3, 4, 5 }, true);
            Assert.AreEqual(3, buff.Last());
        }

        [Test]
        public void RingBufferTest5()
        {
            var buff = new RingBuffer<int>(new[] { 3, 4, 5 });
            Assert.AreEqual(5, buff[2]);
            Assert.AreEqual(4, buff[1]);
            Assert.AreEqual(3, buff[0]);
            Assert.AreEqual(3, buff.First());
            Assert.AreEqual(5, buff.Last());

            buff.Add(6);
            Assert.AreEqual(6, buff[2]);
            Assert.AreEqual(5, buff[1]);
            Assert.AreEqual(4, buff[0]);
            Assert.AreEqual(4, buff.First());
            Assert.AreEqual(6, buff.Last());

            buff.Add(7);
            Assert.AreEqual(7, buff[2]);
            Assert.AreEqual(6, buff[1]);
            Assert.AreEqual(5, buff[0]);
            Assert.AreEqual(5, buff.First());
            Assert.AreEqual(7, buff.Last());
        }

        [Test]
        public void RingBufferTest6()
        {
            var buff = new RingBuffer<int>(100)
            {
                3,
                4,
                5
            };
            var arr2 = buff.ToArray();
            Assert.AreEqual(arr2.Length, buff.Count);

            buff.ReverseIteration = true;
            arr2 = buff.ToArray();
            Assert.AreEqual(arr2.Length, buff.Count);
        }
    }
}