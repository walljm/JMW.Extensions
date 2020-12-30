using System;
using System.Collections.Generic;
using JMW.Template.Tags;
using NUnit.Framework;

namespace JMW.Template.Tests
{
    [TestFixture]
    public class TagHelpersTest
    {
        [Test]
        public void EvaluateBooleanExpressionErrorTest()
        {
            try
            {
                TagHelpers.EvaluateBooleanExpression("x1.substr(0,2)", new List<string> { "jason" });
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The expression:\nx1.substr(0,2)\ndid not evaluate to a boolean value.", ex.Message);
            }
        }

        [Test]
        public void EvaluateArithmeticExpressionErrorTest()
        {
            try
            {
                TagHelpers.EvaluateArithmeticExpression("x1.substr(0,2)", new List<string> { "jason" });
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The expression:\nx1.substr(0,2)\ndid not evaluate to a numeric value.", ex.Message);
            }
        }

        [Test]
        public void RetrieveOctetErrorTest()
        {
            try
            {
                TagHelpers.RetrieveOctet("1.1.1.1", "0");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The value provided for octet argument was invalid. Must be a number between 1 and 4.", ex.Message);
            }

            try
            {
                TagHelpers.RetrieveOctet("1.1.1.1", "5");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The value provided for octet argument was invalid. Must be a number between 1 and 4.", ex.Message);
            }

            try
            {
                TagHelpers.RetrieveOctet("1.1.1.1", "x");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The value provided for octet argument was invalid. Must be a number between 1 and 4.", ex.Message);
            }

            var data = "1.1.1";
            try
            {
                TagHelpers.RetrieveOctet(data, "1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("There was a problem parsing octets from the IP: '" + data + "'. Please check formatting.", ex.Message);
            }

            data = "1.1.1.1.1";
            try
            {
                TagHelpers.RetrieveOctet(data, "1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("There was a problem parsing octets from the IP: '" + data + "'. Please check formatting.", ex.Message);
            }

            data = "11111";
            try
            {
                TagHelpers.RetrieveOctet(data, "1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The provided column value: '" + data + "' is not an IP.", ex.Message);
            }
        }

        [Test]
        public void ReplaceOctetTest()
        {
            Assert.AreEqual("1.1.1.255", TagHelpers.ReplaceOctet("1.1.1.1", "4", 2600));
            Assert.AreEqual("1.1.1.0", TagHelpers.ReplaceOctet("1.1.1.1", "4", -1));
        }

        [Test]
        public void SetupEngineErrorTest()
        {
            try
            {
                TagHelpers.EvaluateBooleanExpression("x1.substr(0,2); x2", new List<string> { "jason" });
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The list of arguments provided in:\njason\n is greater that the number of variables listed in the expression: x1.substr(0,2); x2\n" +
                                "Variables must be specified in the format: x1, x2, x3 etc. and must be equal or less than the argumenst provided.", ex.Message);
            }
        }

        [Test]
        public void CheckAllowedAttributesErrorTest()
        {
            try
            {
                var t = new Tag
                {
                    Name = "foo",
                    TagType = TagTypes.Tag
                };
                t.Properties.Add("foo", "bar");

                TagHelpers.CheckAllowedAttributes(t, new HashSet<string> { "blah" }, new Token());
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid attribute found on tag. Allowed attributes: blah Line: 1 Column: 1", ex.Message);
            }
        }
    }
}