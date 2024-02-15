﻿using JMW.Extensions.String;

namespace JMW.Template.Tags;

public class JoinColumn : ColumnBase
{
    private Join _parent;
    public override string TagName { get; }
    public const string LEFT_PREFIX = "left_";
    public const string RIGHT_PREFIX = "right_";

    public JoinColumn(string name, Join data)
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