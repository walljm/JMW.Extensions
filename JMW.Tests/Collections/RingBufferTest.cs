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

            Assert.That(10, Is.EqualTo(buff.Count));
            Assert.That(14, Is.EqualTo(buff.Last()));

            var c = 5;
            foreach (var i in buff)
                Assert.That(c++, Is.EqualTo(i));

            Assert.That(15, Is.EqualTo(c));
            Assert.That(buff.Contains(11), Is.True);
            buff.Clear();
            Assert.That(0, Is.EqualTo(buff.Count));
            Assert.That(buff.IsReadOnly, Is.False);
        }

        [Test]
        public void RingBufferTest1()
        {
            var buff = new RingBuffer<int>(10, true);
            for (var i = 0; i < 15; i++)
                buff.Add(i);

            var c = 14;
            foreach (var i in buff)
                Assert.That(c--, Is.EqualTo(i));

            Assert.That(4, Is.EqualTo(c));
            Assert.That(10, Is.EqualTo(buff.Count));
        }

        [Test]
        public void RingBufferTest2()
        {
            var buff = new RingBuffer<int>([3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
            Assert.That(10, Is.EqualTo(buff.Count));
            Assert.That(buff.Contains(11), Is.True);
            buff.Clear();
            Assert.That(0, Is.EqualTo(buff.Count));
        }

        [Test]
        public void RingBufferTest3()
        {
            var buff = new RingBuffer<int>([3, 4, 5, 6, 7, 8, 9, 10, 11, 12], true);

            var c = 12;
            foreach (var i in buff)
                Assert.That(c--, Is.EqualTo(i));
            Assert.That(2, Is.EqualTo(c));
            Assert.That(10, Is.EqualTo(buff.Count));
        }

        [Test]
        public void RingBufferTest4()
        {
            var buff = new RingBuffer<int>([3, 4, 5]);
            Assert.That(5, Is.EqualTo(buff.Last()));
            buff = new RingBuffer<int>([3, 4, 5], true);
            Assert.That(3, Is.EqualTo(buff.Last()));

            buff = new RingBuffer<int>(10, [3, 4, 5]);
            Assert.That(5, Is.EqualTo(buff.Last()));
            Assert.That(3, Is.EqualTo(buff.Count));

            buff = new RingBuffer<int>(10, [3, 4, 5], true);
            Assert.That(3, Is.EqualTo(buff.Last()));
        }

        [Test]
        public void RingBufferTest5()
        {
            var buff = new RingBuffer<int>([3, 4, 5]);
            Assert.That(5, Is.EqualTo(buff[2]));
            Assert.That(4, Is.EqualTo(buff[1]));
            Assert.That(3, Is.EqualTo(buff[0]));
            Assert.That(3, Is.EqualTo(buff.First()));
            Assert.That(5, Is.EqualTo(buff.Last()));

            buff.Add(6);
            Assert.That(6, Is.EqualTo(buff[2]));
            Assert.That(5, Is.EqualTo(buff[1]));
            Assert.That(4, Is.EqualTo(buff[0]));
            Assert.That(4, Is.EqualTo(buff.First()));
            Assert.That(6, Is.EqualTo(buff.Last()));

            buff.Add(7);
            Assert.That(7, Is.EqualTo(buff[2]));
            Assert.That(6, Is.EqualTo(buff[1]));
            Assert.That(5, Is.EqualTo(buff[0]));
            Assert.That(5, Is.EqualTo(buff.First()));
            Assert.That(7, Is.EqualTo(buff.Last()));
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
            Assert.That(arr2.Length, Is.EqualTo(buff.Count));

            buff.ReverseIteration = true;
            arr2 = buff.ToArray();
            Assert.That(arr2.Length, Is.EqualTo(buff.Count));
        }
    }
}