using System.Collections.Generic;
using System.IO;
using System.Text;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Handlers
{
    public abstract class Base
    {
        protected List<IProperty> _props = new List<IProperty>();

        public abstract IEnumerable<object[]> Parse(StreamReader reader);

        public IEnumerable<object[]> Parse(string text)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var sr = new StreamReader(ms))
            {
                foreach (var o in Parse(sr))
                    yield return o;
            }
        }

        public IEnumerable<Dictionary<string, object>> ParseNamed(string text)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var sr = new StreamReader(ms))
            {
                foreach (var o in Parse(sr))
                {
                    var i = 0;
                    var dict = new Dictionary<string, object>();
                    foreach (var p in _props)
                    {
                        dict.Add(p.Name, o[i]);
                        i++;
                    }
                    yield return dict;
                }
            }
        }
    }
}