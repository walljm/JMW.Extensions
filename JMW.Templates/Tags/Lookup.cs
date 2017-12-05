using System;
using System.Collections.Generic;

namespace JMW.Template.Tags
{
    public class Lookup : TagHandlerBase
    {
        private bool _hasBeenIndexed = false;
        private string _indexDetails = string.Empty;

        public const string TAG = "lookup";
        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_KEY_EXP, ATTR_KEY_COL, ATTR_TABLE };
        public const string ATTR_KEY_EXP = "key_exp";
        public const string ATTR_KEY_COL = "key_col";
        public const string ATTR_TABLE = "table";

        public TableData TableData { get; }
        public Dictionary<string, List<List<string>>> LookupTable { get; private set; } = new Dictionary<string, List<List<string>>>();
        public readonly string Name;

        public int CurrentRow { get; set; }
        public string Key { get; set; }

        public Lookup()
        {
        }

        public Lookup(TableData table_data, Interpreter interp)
        {
            Name = "lookup_" + table_data.Name.ToLower();
            TableData = table_data;

            interp.Retrievers.Add(Name, RetrieveColumnValue);

            foreach (var column in table_data.Columns)
                interp.AddHandler(makeColumnHandler(column));

            interp.AddHandler(new LookupRow(Name + ":"+LookupRow.TAG, this));
        }

        public override void Handler(Tag tag, Interpreter interp)
        {
            TagHelpers.PrefixTags(tag.Children, Name);

            var index_details = (tag.Properties.ContainsKey(ATTR_KEY_EXP) ? tag.Properties[ATTR_KEY_EXP] : string.Empty) +
                               (tag.Properties.ContainsKey(ATTR_KEY_COL) ? tag.Properties[ATTR_KEY_COL] : string.Empty);

            if (!_hasBeenIndexed || index_details != _indexDetails)
            {
                foreach (var lr in TableData.Data)
                {
                    var key = string.Empty;

                    if (tag.Properties.ContainsKey(ATTR_KEY_EXP))
                        key = TagHelpers.EvaluateExpression(tag.Properties[ATTR_KEY_EXP], lr);
                    else if (tag.Properties.ContainsKey(ATTR_KEY_COL)) // use the value of the key column
                        key = lr[TableData.ColumnIndexes[tag.Properties[ATTR_KEY_COL].ToLower()]];
                    else
                        throw new Exception("Require attribute is missing. A " + ATTR_KEY_EXP + " or " + ATTR_KEY_COL + "attribute is required.");

                    if (LookupTable.ContainsKey(key))
                        LookupTable[key].Add(lr);
                    else
                        LookupTable.Add(key, new List<List<string>> { lr });
                }
            }
            interp.HandleChildren(tag);
        }

        public string RetrieveColumnValue(string column)
        {
            return LookupTable[Key][CurrentRow][TableData.ColumnIndexes[column.ToLower()]];
        }

        private ITagHandler makeColumnHandler(string column_name)
        {
            return new LookupColumn(Name + ":" + column_name.ToLower().Trim(), this);
        }
    }
}