using System.Collections.Generic;
using System.Net;
using JMW.Networking.Rules;
using JMW.Types;
using NUnit.Framework;

namespace JMW.Extensions.Tests
{
    [TestFixture]
    public class NetworkingRulesTests
    {
        [Test]
        public void TestAllowed1()
        {
            var r0 = new Rule
            {
                Source = new Location { IsWildcard = true },
                Dest = new Location { IsWildcard = true },
                Port = new IntegerRange(0, 1000000000),
                Policy = Policy.Deny
            };
            var r1 = new Rule
            {
                Source = new Location(IPAddress.Parse("192.168.1.1")),
                Dest = new Location(),
                Port = new IntegerRange(22, 33),
                Policy = Policy.Permit
            };

            var engine = new RulesEngine(new List<Rule> { r0, r1 });

            Assert.IsTrue(engine.Allowed(new Location(IPAddress.Parse("192.168.1.1")), new Location(IPAddress.Parse("10.110.1.1")), new IntegerRange(22,22)));
            Assert.IsFalse(engine.Allowed(new Location(IPAddress.Parse("192.168.1.2")), new Location(IPAddress.Parse("10.110.1.1")), new IntegerRange(22, 22)));
        }


        [Test]
        public void TestCompress1()
        {
            var r0 = new Rule
            {
                Source = new Location { IsWildcard = true },
                Dest = new Location { IsWildcard = true },
                Port = new IntegerRange(0, 1000000000),
                Policy = Policy.Deny
            };

            var r1 = new Rule
            {
                Source = new Location(IPAddress.Parse("192.168.1.1")),
                Dest = new Location(),
                Port = new IntegerRange(22, 33),
                Policy = Policy.Permit
            };

            var r2 = new Rule
            {
                Source = new Location(IPAddress.Parse("192.168.1.1")),
                Dest = new Location(),
                Port = new IntegerRange(22, 23),
                Policy = Policy.Permit
            };

            var engine = new RulesEngine(new List<Rule> { r0, r1, r2 });

            var lst = RulesEngine.Compress(new List<Rule> {r0, r1, r2});
            
        }
    }
}