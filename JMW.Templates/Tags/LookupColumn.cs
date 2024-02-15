using JMW.Extensions.String;

namespace JMW.Template.Tags;

public class LookupColumn : ColumnBase
{
    private Lookup _parent;
    public override string TagName { get; }

    public LookupColumn(string name, Lookup data)
    {
        TagName = name;
        _parent = data;
    }

    public override void Handler(Tag token, Interpreter interp)
    {
        var column_name = TagName.ParseAfterLastIndexOf_PlusLength(":");
        var data = _parent.RetrieveColumnValue(column_name);

        HandleColumn(token, interp, data);
    }
}