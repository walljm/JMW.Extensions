﻿using JMW.Parsing.Compile;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMW.Parsing.Handlers
{
    public abstract class Base
    {
        protected List<IProperty> Props = [];

        public abstract IEnumerable<object[]> Parse(StreamReader reader);

        public IEnumerable<object[]> Parse(string text)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
            using var sr = new StreamReader(ms);
            foreach (var o in this.Parse(sr))
            {
                yield return o;
            }
        }

        public IEnumerable<Dictionary<string, object>> ParseNamed(string text)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
            using var sr = new StreamReader(ms);
            foreach (var o in this.Parse(sr))
            {
                var i = 0;
                var dict = new Dictionary<string, object>();
                foreach (var p in this.Props)
                {
                    dict.Add(p.Name, o[i]);
                    i++;
                }
                yield return dict;
            }
        }
    }
}