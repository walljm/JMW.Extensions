using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JMW.Extensions.String;
using JMW.Functional;
using JMW.Template.Tags;
using Version = JMW.Template.Tags.Version;

namespace JMW.Template
{
    public delegate void Handler(Tag token, Interpreter interpreter);

    public delegate string ValueRetriever(string column);

    public class Interpreter
    {
        #region Handlers

        private Variable _varHandler;
        private Text _textHandler;

        #endregion Handlers

        #region Properties

        public readonly Dictionary<string, ValueRetriever> Retrievers = new Dictionary<string, ValueRetriever>();
        public Dictionary<string, ITagHandler> Handlers { get; } = new Dictionary<string, ITagHandler>();
        public Optional<Handler> DefaultHandler { get; set; } = Optional<Handler>.None();
        public TextWriter OutputStream { get; set; }
        public Parser Parser { get; }

        #endregion Properties

        public Interpreter(TextWriter output_stream) : this(new Parser(), output_stream, new List<ITagHandler>())
        {
        }

        public Interpreter(Parser parser, TextWriter output_stream, IEnumerable<ITagHandler> handlers)
        {
            Parser = parser;
            OutputStream = output_stream;

            _varHandler = new Variable();
            _textHandler = new Text();

            AddHandler(new Conditional(Retrievers));
            AddHandler(_varHandler);
            AddHandler(new Timestamp());
            AddHandler(new Version());
            AddHandler(new Output());

            foreach (var h in handlers)
                AddHandler(h);
        }

        #region Public Functions

        /// <summary>
        /// Parse the template in the input, then interpret the tags.
        /// </summary>
        /// <param name="input">A template string</param>
        public void Eval(string input)
        {
            Eval(Parser.Parse(input));
        }

        /// <summary>
        /// Interpret the tags with a given list of handlers.
        /// </summary>
        /// <param name="tags">The template tags to interpret</param>
        public void Eval(List<Tag> tags)
        {
            // find the named includes.
            var includes = tags.Where(o => o.TagType == TagTypes.Tag && o.Name == Include.TAG && o.Properties.ContainsKey(Include.ATTR_DEFINE))
                .ToDictionary(k => k.Properties[Include.ATTR_DEFINE], v => v);

            // includes should be handled first.
            Include.ReplaceIncludes(tags, includes);

            // now populated the variables declared in the template using the <var name="varname" value="varvalue" /> syntax.
            _varHandler.PopulateVariables(tags, this);

            foreach (var tag in tags)
            {
                //  handle variable replacements in tag properties.
                _varHandler.ReplaceVariables(tag);

                switch (tag.TagType)
                {
                    case TagTypes.Tag:
                        var tag_name = tag.Name;

                        // tables are registered by their name attribute.  so if its a table tag,
                        //  then you need to get the name attribute.
                        if (tag.Name == Table.TAG || tag.Name == Table.TAB_TAG)
                        {
                            if (tag.Properties.ContainsKey(Table.ATTR_NAME))
                                tag_name = tag.Properties[Table.ATTR_NAME].ToLower();
                            else
                                tag_name = Table.ANON_TABLE_NAME;
                        }

                        // Join tag handlers are built when they are found using table data
                        // from tables that have already been added. No need to make the user
                        // prep the tag before hand.  Besides, you won't know what the data
                        // source is until you've seen the tag in the template.
                        if (tag.Name == Join.TAG)
                        {
                            // get the tables.
                            var left_tag = tag.Properties[Join.ATTR_LEFT_TABLE].ToLower();
                            var right_tag = tag.Properties[Join.ATTR_RIGHT_TABLE].ToLower();

                            // this is how we build Join names.
                            tag_name = left_tag + "_" + right_tag;

                            // if we haven't built this join before, build it now.
                            if (!Handlers.ContainsKey(tag_name))
                            {
                                // we need to create the handler here using the data from the tables specified.
                                if (!Handlers.ContainsKey(left_tag))
                                    throw new Exception("Specified table: '" + left_tag + "' in '" + Join.ATTR_LEFT_TABLE + "' attribute does not exist.");
                                if (!Handlers.ContainsKey(right_tag))
                                    throw new Exception("Specified table: '" + right_tag + "' in '" + Join.ATTR_RIGHT_TABLE + "' attribute does not exist.");

                                var left = (Table)Handlers[left_tag];
                                var right = (Table)Handlers[right_tag];
                                AddHandler(new Join(left.TableData, right.TableData, this));
                            }
                        }

                        // Lookup tag handlers are also built when they are found using table data
                        // from tables that have already been added. No need to make the user
                        // prep the tag before hand.  Besides, you won't know what the data
                        // source is until you've seen the tag in the template.
                        if (tag.Name == Lookup.TAG)
                        {
                            // get the tables.
                            var table_tag = tag.Properties[Lookup.ATTR_TABLE].ToLower();

                            // this is how we build Join names.
                            tag_name = "lookup_" + table_tag;

                            // if we haven't built this join before, build it now.
                            if (!Handlers.ContainsKey(tag_name))
                            {
                                // we need to create the handler here using the data from the tables specified.
                                if (!Handlers.ContainsKey(table_tag))
                                    throw new Exception("Specified table: '" + table_tag + "' in '" + Lookup.ATTR_TABLE + "' attribute does not exist.");

                                var table = (Table)Handlers[table_tag];
                                AddHandler(new Lookup(table.TableData, this));
                            }
                        }

                        if (Handlers.ContainsKey(tag_name))
                            Handlers[tag_name].Handler(tag, this);
                        else if (tag_name.Contains(":") && Handlers.ContainsKey(tag_name.ParseToIndexOf(":"))) // handle variables
                            Handlers[tag_name.ParseToIndexOf(":")].Handler(tag, this);
                        else
                            DefaultHandler.Do(h => h(tag, this));
                        break;

                    case TagTypes.Text:
                        _textHandler.Handler(tag, this);
                        break;
                }
            }
        }

        public void HandleChildren(Tag token)
        {
            if (token.HasChildren)
                Eval(token.Children);
        }

        public void AddHandler(ITagHandler handler)
        {
            var name = handler.TagName;
            var is_table = handler.TagName == Table.TAG || handler.TagName == Table.TAB_TAG;

            // tables get registered as the table name.
            if (is_table)
            {
                var h = (Table)handler;
                if (h.TableData.Name.IsEmpty())
                    name = Table.ANON_TABLE_NAME;
                else
                    name = h.TableData.Name;
            }

            // join tables get registered as the table name.
            else if (handler.TagName == Join.TAG)
                name = ((Join)handler).Name;

            // lookup tables get registered as the table name.
            else if (handler.TagName == Lookup.TAG)
                name = ((Lookup)handler).Name;

            // add the handler
            if (!Handlers.ContainsKey(handler.TagName))
            {
                Handlers.Add(name, handler);

                // add the column tags to the parser
                if (is_table)
                {
                    var h = (Table)handler;

                    // add the table_name tag.
                    Parser.AddTag(h.TableData.Name, h);

                    // add the column_names.
                    foreach (var c in h.TableData.Columns)
                        Parser.AddColumnTag(c);
                }
            }
        }

        public void AddVariable(string name, string value)
        {
            _varHandler.AddVariable(name.ToLower(), value);
        }

        #endregion Public Functions
    }
}