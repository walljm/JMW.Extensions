using System.Collections.Generic;

namespace JMW.Template
{
    public class Tag
    {
        public TagTypes TagType { get; set; } = TagTypes.Text;

        private string _Name = "";
        public string Name { get { return _Name; } set { _Name = value.Trim().ToLower(); } }

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string TokenText { get; set; } = "";

        public List<Tag> Children { get; set; } = new List<Tag>();

        public bool HasChildren
        {
            get
            {
                return Children.Count > 0;
            }
        }

        public override string ToString()
        {
            return Name + ":" + TagType;
        }
    }
}
