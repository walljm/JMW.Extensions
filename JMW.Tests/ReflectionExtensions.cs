using System.Collections.Generic;
using JMW.Extensions.Reflection;
using JMW.Types.Collections;
using NUnit.Framework;
using System.Linq;

namespace JMW.Extensions.Text.Tests
{
    [TestFixture]
    public class ReflectionExtensions
    {
        [Test]
        public void GetPropertiesByAttributeTest1()
        {
            var obj = new Test1() { Prop1 = "Jason" };
            var props = obj.GetType().GetPropertiesByAttribute<Indexed>();
            Assert.That(props.Count() == 1);
            Assert.That(props.First().Name == "Prop1");
            Assert.That(obj.Prop1 == "Jason");
        }

        [Test]
        public void HasInterfaceTest1()
        {
            var obj = new Test1() { Prop1 = "Jason" };
            var t = obj.GetType().HasInterface<ITest>();
            Assert.That(t);
        }

        [Test]
        public void SafeAddTest1()
        {
            var dict = new Dictionary<string, List<string>>();
            dict.SafeAdd("key1", "v1");
            dict.SafeAdd("key2", "v2");
            dict.SafeAdd("key1", "v2");

            Assert.That(dict.Count == 2);
            Assert.That(dict["key1"].Count == 2);
            Assert.That(dict["key2"].Count == 1);
        }

        [Test]
        public void AddIfNotPresentTest1()
        {
            var dict = new Dictionary<string, string>();
            dict.AddIfNotPresent("key1", "v1");
            dict.AddIfNotPresent("key2", "v2");
            dict.AddIfNotPresent("key1", "v2");

            Assert.That(dict.Count == 2);
            Assert.That(dict["key1"] == "v1");
            Assert.That(dict["key2"] == "v2");
        }

        [Test]
        public void GetValueTest1()
        {
            var obj = new Test1() { Prop1 = "Jason" };
            var t = obj.GetType().GetProperty("Prop1");
            var v = t.GetValue(obj);
            Assert.That(v.ToString() == "Jason");
            Assert.That(t.GetValue<string>(obj) == "Jason");
        }

        [Test]
        public void CastDelegateTest1()
        {
            Jason d1 =
                s =>
            {
                return "";
            };

            var d2 = d1.CastDelegate<Jason>();
            
            Assert.That(d1("") == d2(""));
        }

        private delegate string Jason(string one);

        private interface ITest
        {
            string Prop1 { get; set; }
        }

        private class Test1 : ITest
        {
            private string _Prop1 = "";
            [Indexed]
            public string Prop1
            {
                get
                {
                    return _Prop1;
                }

                set
                {
                    _Prop1 = value;
                }
            }
        }
    }
}