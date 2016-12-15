using NUnit.Framework;
using System;

namespace JMW.Extensions.Conversions.Tests
{
    [TestFixture]
    public class ConversionsExtensionsTests
    {
        [Test]
        public void ToInt32Test()
        {
            "12".ToInt32().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToInt32().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt32().Do(val => val, null);
            var v2 = "blah".ToInt32().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToInt16Test()
        {
            "12".ToInt16().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToInt16().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt16().Do(val => val, null);
            var v2 = "blah".ToInt16().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToInt64Test()
        {
            "12".ToInt64().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToInt64().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt64().Do(val => val, null);
            var v2 = "blah".ToInt64().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToUInt32Test()
        {
            "12".ToUInt32().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt32().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt32().Do(val => val, null);
            var v2 = "blah".ToUInt32().Do(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToUInt16Test()
        {
            "12".ToUInt16().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt16().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt16().Do(val => val, null);
            var v2 = "blah".ToUInt16().Do(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToUInt64Test()
        {
            "12".ToUInt64().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt64().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt64().Do(val => val, null);
            var v2 = "blah".ToUInt64().Do(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToDecimalTest()
        {
            "12".ToDecimal().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToDecimal().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToDecimal().Do(val => val, null);
            var v2 = "blah".ToDecimal().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }
        
        [Test]
        public void ToFloatTest()
        {
            "12".ToFloat().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToFloat().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToFloat().Do(val => val, null);
            var v2 = "blah".ToFloat().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToDoubleTest()
        {
            "12".ToDouble().Do(v => Assert.AreEqual(12, v), null);
            "blah".ToDouble().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToDouble().Do(val => val, null);
            var v2 = "blah".ToDouble().Do(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToBooleanTest()
        {
            "1".ToBoolean().Do(v => Assert.AreEqual(true, v), null);
            "blah".ToBoolean().Do(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "1".ToBoolean().Do(val => val, null);
            var v2 = "yes".ToBoolean().Do(val => val, null);
            var v3 = "YES".ToBoolean().Do(val => val, null);
            var v4 = "Yes".ToBoolean().Do(val => val, null);
            var v5 = "on".ToBoolean().Do(val => val, null);
            var v6 = "ON".ToBoolean().Do(val => val, null);
            var v7 = "On".ToBoolean().Do(val => val, null);
            var v8 = "true".ToBoolean().Do(val => val, null);
            var v9 = "TRUE".ToBoolean().Do(val => val, null);
            var v10 = "True".ToBoolean().Do(val => val, null);
            var v11 = "T".ToBoolean().Do(val => val, null);
            var v12 = "t".ToBoolean().Do(val => val, null);

            var v13 = "blah".ToBoolean().Do(null, val => false);
            var v14 = "0".ToBoolean().Do(val => val, null);
            var v15 = "no".ToBoolean().Do(val => val, null);
            var v16 = "NO".ToBoolean().Do(val => val, null);
            var v17 = "No".ToBoolean().Do(val => val, null);
            var v18 = "False".ToBoolean().Do(val => val, null);
            var v19 = "false".ToBoolean().Do(val => val, null);
            var v20 = "FALSE".ToBoolean().Do(val => val, null);
            var v21 = "off".ToBoolean().Do(val => val, null);
            var v22 = "Off".ToBoolean().Do(val => val, null);
            var v23 = "OFF".ToBoolean().Do(val => val, null);
            var v24 = "f".ToBoolean().Do(val => val, null);
            var v25 = "F".ToBoolean().Do(val => val, null);

            Assert.AreEqual(true, v1);
            Assert.AreEqual(true, v2);
            Assert.AreEqual(true, v3);
            Assert.AreEqual(true, v4);
            Assert.AreEqual(true, v5);
            Assert.AreEqual(true, v6);
            Assert.AreEqual(true, v7);
            Assert.AreEqual(true, v8);
            Assert.AreEqual(true, v9);
            Assert.AreEqual(true, v10);
            Assert.AreEqual(true, v11);
            Assert.AreEqual(true, v12);

            Assert.AreEqual(false, v13);
            Assert.AreEqual(false, v14);
            Assert.AreEqual(false, v15);
            Assert.AreEqual(false, v16);
            Assert.AreEqual(false, v17);
            Assert.AreEqual(false, v18);
            Assert.AreEqual(false, v19);
            Assert.AreEqual(false, v20);
            Assert.AreEqual(false, v21);
            Assert.AreEqual(false, v22);
            Assert.AreEqual(false, v23);
            Assert.AreEqual(false, v24);
            Assert.AreEqual(false, v25);
        }

    }
}