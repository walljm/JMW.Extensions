﻿using JMW.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Template.Tags
{
    public class Table : TagHandlerBase
    {
        private string _name;

        public const string TAG = "table";
        public const string TAB_TAG = "tab";
        public const string ANON_TABLE_NAME = "anon_table";

        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_NAME, ATTR_DISTINCT, ATTR_WHERE, ATTR_ORDERBY, ATTR_ORDER };
        public const string ATTR_DISTINCT = "distinct";
        public const string ATTR_WHERE = "where";
        public const string ATTR_ORDERBY = "orderby";
        public const string ATTR_ORDER = "order";
        public const string ATTR_NAME = "name";

        public const string ATTR_ORDER_VALUE_DESC = "desc";

        public TableData TableData { get; }
        public int CurrentRow { get; set; }

        public Table()
        {
        }

        public Table(TableData table_data, Interpreter interp)
        {
            _name = table_data.Name.ToLower();
            TableData = table_data;

            interp.Retrievers.Add(table_data.Name.ToLower(), RetrieveColumnValue);

            foreach (var column in table_data.Columns)
                interp.AddHandler(makeColumnHandler(column));
        }

        public override void Handler(Tag tag, Interpreter interp)
        {
            // prefix the names of the columns
            TagHelpers.PrefixTags(tag.Children, _name);

            var old = TableData.Data;

            // filter the distinct if present
            FilterDistinct(tag, TableData);

            // filter data by where if present.
            FilterWhere(tag, TableData);

            // filter data by where if present.
            FilterOrderBy(tag, TableData);

            // evaluate the children of the table tag.
            for (CurrentRow = 0; CurrentRow < TableData.Data.Count; CurrentRow++)
            {
                // we access the row using the CurrentRow value, but it need to be called for every row
                interp.Eval(tag.Children);
            }

            // just in case you modified TableData.Data in the distinct or where clause, put it back.
            TableData.Data = old;
        }

        public static void FilterOrderBy(Tag tag, TableData table_data)
        {
            if (tag.Properties.ContainsKey(ATTR_ORDERBY))
            {
                if (tag.Properties.ContainsKey(ATTR_ORDER) && tag.Properties[ATTR_ORDER] == ATTR_ORDER_VALUE_DESC)
                {
                    table_data.Data = table_data.Data.OrderByDescending(r => r[table_data.ColumnIndexes[tag.Properties[ATTR_ORDERBY]]]).ToList();
                }
                else
                    table_data.Data = table_data.Data.OrderBy(r => r[table_data.ColumnIndexes[tag.Properties[ATTR_ORDERBY]]]).ToList();
            }
        }

        public static void FilterWhere(Tag tag, TableData table_data)
        {
            if (tag.Properties.ContainsKey(ATTR_WHERE))
            {
                table_data.Data = table_data.Data.Where(r => TagHelpers.EvaluateBooleanExpression(tag.Properties[ATTR_WHERE], r)).ToList();
            }
        }

        public static void FilterDistinct(Tag tag, TableData table_data)
        {
            if (tag.Properties.ContainsKey(ATTR_DISTINCT))
            {
                var cols = tag.Properties[ATTR_DISTINCT].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var dict = new Dictionary<string, List<string>>();
                foreach (var row in table_data.Data)
                {
                    var key = cols.Select(f => row[table_data.ColumnIndexes[f.Trim().ToLower()]]).ToDelimitedString("|");
                    dict.AddIfNotPresent(key, row);
                }

                table_data.Data = dict.Values.ToList();
            }
        }

        public string RetrieveColumnValue(string column)
        {
            return TableData.Data[CurrentRow][TableData.ColumnIndexes[column.ToLower()]];
        }

        private ITagHandler makeColumnHandler(string column_name)
        {
            return new TableColumn(_name + ":" + column_name.ToLower().Trim(), this);
        }

        public static bool IsTable(Tag tag)
        {
            return tag.TagType == TagTypes.Tag && (tag.Name == TAG || tag.Name == TAB_TAG);
        }
    }
}