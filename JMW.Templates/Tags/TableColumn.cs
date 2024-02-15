using JMW.Extensions.String;

namespace JMW.Template.Tags;

public class TableColumn : ColumnBase
{
    private Table _parent;
    public override string TagName { get; }

    public TableColumn(string name, Table data)
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