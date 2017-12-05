using System.Collections.Generic;

namespace JMW.Parsing.Compile
{
    public class Tag
    {
        public TagTypes TagType { get; set; } = TagTypes.Object;

        private string _Name = string.Empty;
        public string Name { get { return _Name; } set { _Name = value.Trim().ToLower(); } }

        public Dictionary<string, Tag> Properties { get; set; } = new Dictionary<string, Tag>();

        public object Value { get; set; } = string.Empty;
        
        public override string ToString()
        {
            return Name + ":" + TagType;
        }
    }
}
