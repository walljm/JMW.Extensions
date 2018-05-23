﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using JMW.Types;

namespace JMW.Networking.Rules
{
    public class RulesEngine
    {
        private List<Rule> _rules;

        public RulesEngine(IEnumerable<Rule> rules)
        {
            _rules = rules.ToList();
        }

        public List<Rule> Compress(IEnumerable<Rule> rules)
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
            return Allowed(_rules, source, dest, port);
        }

        private static bool Allowed(IEnumerable<Rule> rules, Location source, Location dest, IntegerRange port)
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

    public class Rule
    {
        public Location Source { get; set; }
        public Location Dest { get; set; }
        public IntegerRange Port { get; set; }
        public Policy Policy { get; set; } = Policy.Deny;
    }

    public enum Policy
    {
        Permit,
        Deny
    }

    public class Location
    {
        public Location()
        {
            IsWildcard = true;
        }

        public Location(IPAddress address)
        {
            Address = address;
            IsWildcard = false;
        }

        public bool IsWildcard { get; set; }
        public IPAddress Address { get; }

        public bool IsMatch(Location loc)
        {
            if (IsWildcard || loc.IsWildcard)
            {
                return true;
            }

            if (!IsWildcard)
            {
                if (!loc.IsWildcard && Address.Equals(loc.Address))
                    return true;
            }

            return false;
        }
    }
}