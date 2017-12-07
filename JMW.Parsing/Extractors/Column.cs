using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.Enumerable;
using JMW.Parsing.Compile;
using JMW.Parsing.Handlers;

namespace JMW.Parsing.Extractors
{
    public class Column : Base
    {
        public const string NAME = "col";

        public List<ColumnPosition> Positions { get; set; }

        public override string Parse(string s)
        {
            if (Positions == null)
                throw new ArgumentException("Positions property must be set.");

            if (Positions.Count < Index)
                throw new ArgumentException("Provided index (" + Index + ") exceeded the number of columns (" + Positions.Count + "):" + Positions.Select(c => c.Name).ToDelimitedString(","));

            return Positions[Index].GetColumnValue(s);
        }

        public Column(Tag t) : base(t)
        {
        }

        public Column(int idx) : base(null, "", -1, idx)
        {
        }
    }
}