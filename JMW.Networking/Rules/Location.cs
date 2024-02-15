using System.Net;

namespace JMW.Networking.Rules;

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
    public IPAddress? Address { get; }

    public bool IsMatch(Location loc)
    {
        if (IsWildcard || loc.IsWildcard)
        {
            return true;
        }

        if (Address is not null)
        {
            if (!loc.IsWildcard && Address.Equals(loc.Address))
                return true;
        }

        return false;
    }
}