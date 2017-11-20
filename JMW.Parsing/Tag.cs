using System.Collections.Generic;

namespace JMW.Parsing
{
    public class Tag
    {
        public TagTypes TagType { get; set; } = TagTypes.Object;

        private string _Name = "";
        public string Name { get { return _Name; } set { _Name = value.Trim().ToLower(); } }

        public Dictionary<string, Tag> Properties { get; set; } = new Dictionary<string, Tag>();

        public object Value { get; set; } = "";
        
        public override string ToString()
        {
            return Name + ":" + TagType;
        }
    }
}
