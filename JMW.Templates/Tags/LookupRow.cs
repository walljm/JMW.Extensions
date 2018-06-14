using System.Collections.Generic;

namespace JMW.Template.Tags
{
    public class LookupRow : TagHandlerBase
    {
        public const string TAG = "row";
        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_KEY };
        public const string ATTR_KEY = "key";

        private Lookup _tab;

        public LookupRow()
        {
        }

        public LookupRow(string name, Lookup data)
        {
            TagName = name;
            _tab = data;
        }

        public override void Handler(Tag tag, Interpreter interp)
        {
            TagHelpers.PrefixTags(tag.Children, _tab.Name);

            if (tag.Properties.ContainsKey(ATTR_KEY))
            {
                _tab.Key = tag.Properties[ATTR_KEY];

                // Handles all rows in sheet.
                for (_tab.CurrentRow = 0; _tab.CurrentRow < _tab.LookupTable[_tab.Key].Count; _tab.CurrentRow++)
                {
                    // we access the row using the CurrentRow value, but it need to be called for every row
                    interp.Eval(tag.Children);
                }
            }
            else
            {
                throw new System.Exception("The '" + ATTR_KEY + "' attribute is required.");
            }
        }
    }
}