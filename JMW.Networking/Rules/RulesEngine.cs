using System.Collections.Generic;
using System.Linq;
using JMW.Types;

namespace JMW.Networking.Rules;

public class RulesEngine
{
    private readonly List<FirewallRule> rules;

    public RulesEngine(IEnumerable<FirewallRule> rules)
    {
        this.rules = rules.ToList();
    }

    public static List<FirewallRule> Compress(IEnumerable<FirewallRule> rules)
    {
        // loop through each rule, starting at the bottom, removing any rules above that are obviated.
        var lst = rules.ToList();

        for (var i = 0; i < lst.Count; i++)
        {
            var r = lst[i];

            var a = Allowed(lst.Skip(i), r.Source, r.Dest, r.Port);
            switch (r.Policy)
            {
                case Policy.Deny when !a:
                    lst.RemoveAt(i);
                    break;

                case Policy.Permit when a:
                    lst.RemoveAt(i);
                    break;
            }
        }
        return lst;
    }

    public bool Allowed(Location source, Location dest, IntegerRange port)
    {
        return Allowed(rules, source, dest, port);
    }

    private static bool Allowed(IEnumerable<FirewallRule> rules, Location source, Location dest, IntegerRange port)
    {
        var r = false;
        foreach (var rule in rules)
        {
            if (rule.Source.IsMatch(source)
                &&
                rule.Dest.IsMatch(dest)
                &&
                rule.Port.IntersectsWith(port))
            {
                r = rule.Policy != Policy.Deny;
            }
        }

        return r;
    }
}
