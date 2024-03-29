﻿using System.Collections.Generic;

namespace JMW.Parsing;

public class Search
{
    public const string MODS = "m";
    public const string SEARCH = "s";

    public List<string> Query { get; set; } = [];
    public string Mods { get; set; } = string.Empty;
}