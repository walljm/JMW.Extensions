using System.Collections.Generic;

namespace JMW.Networking.Parsers.JunosConfig;

public class Tag
{
    public TagTypes TagType { get; set; } = TagTypes.Object;

    private string _Name = string.Empty;
    public string Name { get { return _Name; } set { _Name = value.Trim(); } }

    public Dictionary<string, Tag> Properties { get; set; } = new Dictionary<string, Tag>();

    public string Value { get; set; } = string.Empty;

    public Stack<Tag> Children { get; set; } = new Stack<Tag>();

    public override string ToString()
    {
        return Name + ":" + TagType;
    }
}
