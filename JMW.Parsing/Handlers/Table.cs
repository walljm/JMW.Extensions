using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using JMW.Extensions.Numbers;
using JMW.Extensions.String;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;
using JMW.Parsing.Extractors;
using JMW.Parsing.IO;

namespace JMW.Parsing.Handlers
{
    public class Table : Base, IParser, IProperty
    {
        public const string NAME = "table";

        public const string A_INCLUDE = "include";
        public const string A_HEADER = "header";
        public const string A_ROW = "row";
        public const string A_PROPS = "props";
        public const string A_VALIDATE = "validate";
        //public const string A_SPLIT = "split";

        public string Name { get; } = NAME;

        private Include _include = new Include(null, null);
        private IExpression _headerExp = null;
        private IExpression _rowExp = null;
        private bool _validate = false;

        public Table(Tag token)
        {
            // optional.
            if (token.Properties.ContainsKey(A_INCLUDE))
                _include = new Include(token.Properties[A_INCLUDE]);

            // optional.
            if (token.Properties.ContainsKey(A_HEADER))
                _headerExp = Expressions.Base.ToExpression(token.Properties[A_HEADER]);

            //// optional
            //if (token.Properties.ContainsKey(A_SPLIT))
            //    _split = token.Properties[A_SPLIT].Value.ToString();

            // optional
            if (token.Properties.ContainsKey(A_VALIDATE))
            {
                var v = token.Properties[A_VALIDATE].Value.ToString();
                if (v.ToLower() != "true" && v.ToLower() != "false")
                    throw new ParseException("Property must be either 'true' or 'false'.");
                _validate = bool.Parse(v);
            }

            // find first line that matches.
            if (!token.Properties.ContainsKey(A_ROW))
                throw new ParseException("Missing property: " + A_ROW);
            _rowExp = Expressions.Base.ToExpression(token.Properties[A_ROW]);

            if (!token.Properties.ContainsKey(A_PROPS))
                throw new ParseException("Missing property: " + A_PROPS);

            var props = (Stack<Tag>)token.Properties[A_PROPS].Value;
            foreach (var p in props)
            {
                switch (p.Name)
                {
                    case Paragraph.NAME:
                        _props.Add(new Paragraph(p));
                        break;

                    case Property.NAME:
                        _props.Add(new Property(p));
                        break;

                    default:
                        throw new ArgumentException("Unsupported " + nameof(IProperty) + "Tag", p.Name);
                }
            }
        }

        public IEnumerable<string> GetNextRow(StreamReader reader, IExpression row)
        {
            var sr = new SectionReader(reader, _include);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (row != null && row.Test(line))
                {
                    yield return line;
                }
            }
        }

        public override IEnumerable<object[]> Parse(StreamReader reader)
        {
            if (reader == null)
                yield break;

            var pos = reader.BaseStream.Position;

            // get header row, and handle it
            var cols = _props.FindAll(p => p is Property && ((Property)p).Extractors.Any(ex => ex is Column));
            if (_headerExp != null && cols.Count > 0)
            {
                var exp = _validate ? new Or(new List<IExpression> { _headerExp, _rowExp }) : _headerExp;

                var header = GetNextRow(reader, exp);

                var column_positions = DeriveColumnPositions(header.ToList(), validate_with_data: _validate).Positions.Values.ToList();
                foreach (var p in cols)
                {
                    foreach (var c in ((Property)p).Extractors)
                        ((Column)c).Positions = column_positions;
                }
            }
            reader.BaseStream.Position = pos;

            // parse the rows
            foreach (var line in GetNextRow(reader, _rowExp))
            {
                var record = new List<object>();

                foreach (var p in _props)
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    using (var sr = new StreamReader(ms))
                    {
                        record.Add(p.Parse(sr));
                    }
                }
                yield return record.ToArray();
            }
        }

        object IProperty.Parse(StreamReader reader)
        {
            return Parse(reader);
        }

        #region Static Methods

        /// <summary>
        ///   Assumes the first row of the table is the column row.  Derives the column positions.  Handles whitespace and paging breaks in the table.
        /// </summary>
        /// <param name="table"><see>
        ///         <cref>List{string}</cref>
        ///     </see>
        ///     holding the rows of the table.</param>
        /// <param name="validate_with_data">If true, uses the table data to validate the column positions</param>
        /// <returns>Dictionary object indexed by Column name, that has the starting position and length for each column.</returns>
        public static ColumnPositionCollection DeriveColumnPositions(IList<string> table, bool validate_with_data = true)
        {
            var whitespace = new HashSet<string> { " ", "\r", "\n", "+" };
            var original_column_string = "";

            #region Get Columns

            // find the first non empty line.
            var c = 0;
            var columnnames = table[c];
            while (columnnames.Trim().Length == 0)
            {
                columnnames = table[++c];
            }
            original_column_string = columnnames;

            // initialize the field positions dictionary object.
            var field_positions = new Dictionary<string, ColumnPosition>();
            var positions = new List<ColumnPosition>();

            // get the field names.  we assume the line has them.
            var pre_columns = columnnames.Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // handle columnnames
            columnnames = HandleDuplicateColumnNames(columnnames, pre_columns);
            table[c] = columnnames;
            var columns = columnnames.Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            for (var i = 0; i < columns.Length; i++)
            {
                var col = columns[i];

                var o = new ColumnPosition();

                if (i + 1 == columns.Length) // we're on the last one
                {
                    o.Start = columnnames.LastIndexOf(" " + col, StringComparison.Ordinal).ToString().ToInt() + 1;
                    o.Length = -1;
                }
                else if (i + 2 == columns.Length) // we're on the second to last, but that means the next column is the last.
                {
                    o.Start = columnnames.IndexOf(" " + col + " ", StringComparison.Ordinal).ToString().ToInt() + 1;
                    o.Length = columnnames.LastIndexOf(" " + columns[i + 1], StringComparison.Ordinal) - o.Start.ToString().ToInt() + 1;
                }
                else if (i == 0) // we're on the first, treat the start special.
                {
                    o.Start = columnnames.IndexOf(col + " ", StringComparison.Ordinal).ToString().ToInt();
                    o.Length = columnnames.IndexOf(" " + columns[i + 1] + " ", StringComparison.Ordinal) - o.Start.ToString().ToInt() + 1;
                }
                else
                {
                    o.Start = columnnames.IndexOf(" " + col + " ", StringComparison.Ordinal).ToString().ToInt() + 1;
                    o.Length = columnnames.IndexOf(" " + columns[i + 1] + " ", StringComparison.Ordinal) - o.Start.ToString().ToInt() + 1;
                }
                o.Name = columns[i];
                o.Key = columns[i];
                field_positions.Add(col, o);
                positions.Add(o);
            }

            #endregion Get Columns

            #region Validate

            if (validate_with_data)
            {
                // we need to access the keys line a normal array/list and the key collection in the dict object wont let us.
                var keys = positions.Select(o => o.Key).ToList();

                //  we increase the start position at least one to hedge our bets, as we can rely on there being
                //   at least 1 char in the column for the row, and if there isn't, it doesn't matter anyway.
                //   but this does allow for situations where the header is off by one, because that happens.
                for (var i = 0; i < positions.Count; i++) positions[i].Start++;

                // now test to make sure your column boundaries are correct by looping through the rest of the table
                for (var i = c; i < table.Count; i++)
                {
                    var row = table[i];

                    if (row.Trim().GetCharHistogram().Count > 1) // don't bother with empty lines.
                    {
                        //check to see if the first column starts at the beginning of the line.
                        while (field_positions[keys[0]].Start != 0 &&
                            row.Substring(field_positions[keys[0]].Start - 1, 1) != " " &&
                            row.Substring(field_positions[keys[0]].Start - 1, 1) != "+")
                        {
                            field_positions[keys[0]].Start--;
                            field_positions[keys[0]].Length++;
                        }

                        // Adjust the rows if necessary.
                        for (var j = 0; j < keys.Count - 1; j++) // we ignore the last column, because its starting index will get moved, but its length is -1.
                        {
                            var key = keys[j];

                            var start = field_positions[key].Start + field_positions[key].Length - 1;

                            while (start >= 0 && row.Length > start && !whitespace.Contains(row.Substring(start, 1)))
                            {
                                field_positions[key].Length--; // only decrement the length if its not the last column
                                field_positions[keys[j + 1]].Start--; // only decrement the starting index of the next column if its not the last column

                                // now deal with the increased col length.
                                if (j + 2 < keys.Count) // its not the second to last
                                {
                                    field_positions[keys[j + 1]].Length++; // only increment the length of the next column if the next colum isn't the last column
                                }
                                start = field_positions[key].Start + field_positions[key].Length - 1; // reset the start pos.
                            }
                        }
                    }
                }
            }

            field_positions = CombineMultiwordColumnNames(positions);

            #endregion Validate

            // fix the column names.
            foreach (var kvp in field_positions.ToList())
            {
                field_positions.Remove(kvp.Key);

                kvp.Value.Name = kvp.Value.GetColumnValue(original_column_string).Trim();
                var d = 1;
                while (field_positions.ContainsKey(kvp.Value.Name))
                {
                    kvp.Value.Name += d;
                }
                field_positions.Add(kvp.Value.Name, kvp.Value);
            }

            return new ColumnPositionCollection(field_positions);
        }

        /// <summary>
        ///   This handles the case where column names had multiple words in them and were
        ///     initially treated as individual columns.  When a column has a length of 0
        ///     then you can combine the name with the column next to it.
        /// </summary>
        /// <param name="positions">A list of the Positions for the columns in order they were found.</param>
        /// <returns></returns>
        public static Dictionary<string, ColumnPosition> CombineMultiwordColumnNames(List<ColumnPosition> positions)
        {
            // look for columns with a length of 1, as that column is basically a zero length column and should be combined with the column next to it.
            var field_positions = new Dictionary<string, ColumnPosition>();
            for (var i = 0; i < positions.Count; i++)
            {
                var p = positions[i]; // get the position
                if (p.Length == 0)
                {
                    // the length of the position is 0, so it needs to be added to the column next to it.
                    positions[i + 1].Name = positions[i].Name + positions[i + 1].Name; // merge this position with the next position.

                    while (positions.Count > i + 1 && positions[i + 1].Length == 0)
                    {
                        i++; // go to the next line.
                        positions[i + 1].Name = positions[i].Name + positions[i + 1].Name; // merge this position with the next position.
                    }

                    field_positions.Add(positions[i + 1].Name, positions[i + 1]); // add the next position to the fields.

                    if (positions.Count > i + 1) i++;
                    else break;
                }
                else
                {
                    var id = p.Name;
                    var increment = 0;
                    while (field_positions.ContainsKey(id))
                    {
                        // deal with the old, so the unit test doesn't break
                        var len = id.Length;
                        var old = field_positions[id];
                        field_positions.Remove(id);
                        id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                        while (field_positions.ContainsKey(id))
                        {
                            increment++;
                            id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                        }
                        field_positions.Add(id, old);

                        increment++;
                        len = p.Name.Length;
                        id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                        increment++;
                    }
                    field_positions.Add(id, p);
                }
            }
            return field_positions;
        }

        /// <summary>
        ///   This handles the case where duplicate column names were detected in the column header.
        ///     when this happens, you have to change the column names to be unique, preserving the
        ///     length of the column.
        /// </summary>
        /// <param name="columnnames">a string containing the column header</param>
        /// <param name="columns">the array of unique column names</param>
        /// <returns></returns>
        public static string HandleDuplicateColumnNames(string columnnames, string[] columns)
        {
            // now check for uniqueness.
            var cols = columns.Distinct().ToArray();

            foreach (var col in cols)
            {
                var occurrences = columnnames.CountInstances(col, true);
                if (occurrences > 1)
                {
                    var n = 0;
                    while (columnnames.Contains(col))
                    {
                        var nc = columnnames.ParseToIndexOf(col) + col.Substring(0, col.Length - (n.ToString().Length)) + n + columnnames.ParseAfterIndexOf_PlusLength(col);
                        columnnames = nc;
                        n++;
                    }
                }
            }

            return columnnames;
        }

        #endregion Static Methods
    }

    public class ColumnPositionCollection
    {
        public ColumnPositionCollection(Dictionary<string, ColumnPosition> positions)
        {
            Positions = positions;
            var start = -1;
            foreach (var pos in positions)
            {
                if (pos.Value.Start > start) start = pos.Value.Start;
            }
        }

        public Dictionary<string, ColumnPosition> Positions { get; set; }
    }

    public class ColumnPosition
    {
        public string Name { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        [DefaultValue(-1)]
        public int Start { get; set; } = -1;

        public int Length { get; set; } = -1;

        public override string ToString()
        {
            switch (Length)
            {
                case -1:
                    return Name + ": " + Start;

                case 0:
                    return Name + ": " + Start + "-" + Start;
            }

            return Name + ": " + Start + "-" + (Start + Length - 1);
        }

        /// <summary>
        /// Uses a set of Column Positions to get a value from a string.
        /// </summary>
        /// <param name="line"></param>
        public string GetColumnValue(string line)
        {
            if (line.Length >= Start + Length && Length > 0)
            {
                return line.Substring(Start, Length);
            }

            if (line.Length > Start)
            {
                return line.Substring(Start);
            }

            return string.Empty;
        }
    }
}