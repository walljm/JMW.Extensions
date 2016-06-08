using NUnit.Framework;
using System;

namespace JMW.Extensions.Conversions.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void ToInt32Test()
        {
            "12".ToInt32().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToInt32().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt32().Use(val => val, null);
            var v2 = "blah".ToInt32().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToInt16Test()
        {
            "12".ToInt16().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToInt16().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt16().Use(val => val, null);
            var v2 = "blah".ToInt16().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToInt64Test()
        {
            "12".ToInt64().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToInt64().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToInt64().Use(val => val, null);
            var v2 = "blah".ToInt64().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToUInt32Test()
        {
            "12".ToUInt32().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt32().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt32().Use(val => val, null);
            var v2 = "blah".ToUInt32().Use(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToUInt16Test()
        {
            "12".ToUInt16().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt16().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt16().Use(val => val, null);
            var v2 = "blah".ToUInt16().Use(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToUInt64Test()
        {
            "12".ToUInt64().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToUInt64().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToUInt64().Use(val => val, null);
            var v2 = "blah".ToUInt64().Use(null, val => 0);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(0, v2);
        }

        [Test]
        public void ToDecimalTest()
        {
            "12".ToDecimal().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToDecimal().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToDecimal().Use(val => val, null);
            var v2 = "blah".ToDecimal().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToSingleTest()
        {
            "12".ToSingle().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToSingle().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToSingle().Use(val => val, null);
            var v2 = "blah".ToSingle().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToFloatTest()
        {
            "12".ToFloat().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToFloat().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToFloat().Use(val => val, null);
            var v2 = "blah".ToFloat().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToDoubleTest()
        {
            "12".ToDouble().Use(v => Assert.AreEqual(12, v), null);
            "blah".ToDouble().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "12".ToDouble().Use(val => val, null);
            var v2 = "blah".ToDouble().Use(null, val => -1);

            Assert.AreEqual(12, v1);
            Assert.AreEqual(-1, v2);
        }

        [Test]
        public void ToBooleanTest()
        {
            "1".ToBoolean().Use(v => Assert.AreEqual(true, v), null);
            "blah".ToBoolean().Use(null,
                v =>
                {
                    Assert.AreEqual(typeof(ArgumentException), v.GetType());
                });

            var v1 = "1".ToBoolean().Use(val => val, null);
            var v2 = "yes".ToBoolean().Use(val => val, null);
            var v3 = "YES".ToBoolean().Use(val => val, null);
            var v4 = "Yes".ToBoolean().Use(val => val, null);
            var v5 = "on".ToBoolean().Use(val => val, null);
            var v6 = "ON".ToBoolean().Use(val => val, null);
            var v7 = "On".ToBoolean().Use(val => val, null);
            var v8 = "true".ToBoolean().Use(val => val, null);
            var v9 = "TRUE".ToBoolean().Use(val => val, null);
            var v10 = "True".ToBoolean().Use(val => val, null);
            var v11 = "T".ToBoolean().Use(val => val, null);
            var v12 = "t".ToBoolean().Use(val => val, null);

            var v13 = "blah".ToBoolean().Use(null, val => false);
            var v14 = "0".ToBoolean().Use(val => val, null);
            var v15 = "no".ToBoolean().Use(val => val, null);
            var v16 = "NO".ToBoolean().Use(val => val, null);
            var v17 = "No".ToBoolean().Use(val => val, null);
            var v18 = "False".ToBoolean().Use(val => val, null);
            var v19 = "false".ToBoolean().Use(val => val, null);
            var v20 = "FALSE".ToBoolean().Use(val => val, null);
            var v21 = "off".ToBoolean().Use(val => val, null);
            var v22 = "Off".ToBoolean().Use(val => val, null);
            var v23 = "OFF".ToBoolean().Use(val => val, null);
            var v24 = "f".ToBoolean().Use(val => val, null);
            var v25 = "F".ToBoolean().Use(val => val, null);

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

        [Test]
        public void ConvertedConstructorTest1()
        {
            string v = null;
            var c = new Converted<string>(v);
            Assert.AreEqual(false, c.Success);
        }
    }
}