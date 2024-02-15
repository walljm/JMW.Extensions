using JMW.Types;

namespace JMW.Networking.Rules;

public class FirewallRule
{
    public Location? Source { get; set; }
    public Location? Dest { get; set; }
    public IntegerRange? Port { get; set; }
    public Policy Policy { get; set; } = Policy.Deny;
}
