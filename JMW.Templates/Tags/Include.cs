﻿using System;
using System.Collections.Generic;
using System.IO;

namespace JMW.Template.Tags
{
    public static class Include
    {
        public static string TAG { get; } = "include";
        public const string ATTR_PATH = "path";
        public const string ATTR_NAME = "name";
        public const string ATTR_DEFINE = "define";

        public static HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_PATH, ATTR_NAME, ATTR_DEFINE };

        public static void ReplaceIncludes(List<Tag> tags, Dictionary<string, Tag> includes)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];

                // ignore the include definitions.
                if (tag.Properties.ContainsKey(ATTR_DEFINE) && tag.Name.ToLower() == TAG)
                    continue;

                if (tag.TagType == TagTypes.Tag && tag.Name.ToLower().Equals(TAG))
                {
                    if (tag.Properties.ContainsKey(ATTR_PATH))
                    {
                        var file_name = "";
                        try
                        {
                            file_name = tag.Properties[ATTR_PATH];

                            if (!Path.IsPathRooted(file_name))
                                file_name = Path.Combine(Directory.GetCurrentDirectory(), file_name);

                            var parser = new Parser();
                            var include_tags = parser.Parse(File.ReadAllText(file_name));
                            var index = tags.IndexOf(tag);
                            tags.Remove(tag);
                            tags.InsertRange(index, include_tags);

                            i--;
                        }
                        catch (KeyNotFoundException e)
                        {
                            throw new Exception("<" + TAG + "> tag refers to a template which does not exist: " + file_name + ".", e);
                        }
                    }
                    else if (tag.Properties.ContainsKey(ATTR_NAME))
                    {
                        try
                        {
                            var name = tag.Properties[ATTR_NAME];

                            if (!includes.ContainsKey(name))
                                throw new Exception(TAG + " referenced a named " + TAG + ":\"" + name + "\" that does not exist in the template.");

                            var children = includes[name].Children;

                            var index = tags.IndexOf(tag);
                            tags.Remove(tag);
                            tags.InsertRange(index, children);

                            i--;
                        }
                        catch (KeyNotFoundException e)
                        {
                            throw new Exception("<" + TAG + "> tag refers to a template which does not exist: " + tag.Properties[ATTR_PATH] + ".", e);
                        }
                    }
                    else
                        throw new Exception("The <" + TAG + "> tag requires either a " + ATTR_NAME + " attribute or a " + ATTR_PATH + " attribute.");
                }
                else if (tag.TagType == TagTypes.Tag && tag.HasChildren)
                {
                    ReplaceIncludes(tag.Children, includes);
                }
            }
        }
    }
}