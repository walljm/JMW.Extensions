using JMW.Extensions.Enumerable;
using JMW.Extensions.Numbers;
using JMW.Extensions.String;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Types
{
    public class IntegerRangeCollection : IList, IList<IntegerRange>
    {
        #region Constructors

        public IntegerRangeCollection()
        {
        }

        public IntegerRangeCollection(IntegerRange rng)
        {
            Ranges.Add(rng.Start, new IntegerRange(rng));
        }

        public IntegerRangeCollection(int start, int stop)
        {
            Ranges.Add(start, new IntegerRange(start, stop));
        }

        public IntegerRangeCollection(string rng)
        {
            rng.Split(',')
                .Select(o => o.Trim()) // trim whitespace
                .Where(o => o.Length > 0) // eliminate empty strings
                .Select(o => new IntegerRange(o)) // create ranges
                .SelectMany(o => o.GetInts()) // get integers so you can eliminate possible overlaps
                .Distinct() // remove duplicates
                .CollapseIntsToRanges() // collapse into ranges
                .Split(',') // split the string into ranges
                .Select(o => o.Trim()) // trim whitespace
                .Select(o => new IntegerRange(o)) // turn back into objects
                .Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        #endregion Constructors

        #region Properties

        public SortedList<int, IntegerRange> Ranges { get; set; } = new SortedList<int, IntegerRange>();

        #endregion Properties

        #region Public Methods

        public void AddUniqueIntegersFromRange(IEnumerable<string> lst)
        {
            var lookup = Ranges.Select(v => v.ToString()).ToHashSet();
            foreach (var i in lst)
            {
                if (i.Contains("--") || i.Contains("none")) continue;

                if (i.Contains('-'))
                {
                    // its a range, so process each one.
                    var blocks = i.Split('-');
                    if (!blocks[0].IsInt() || !blocks[1].IsInt()) continue;

                    var start = blocks[0].ToInt();
                    var end = blocks[1].ToInt();
                    for (var c = start; c <= end; c++)
                    {
                        if (!lookup.Contains(c.ToString()))
                            lookup.Add(c.ToString().Trim());
                    }
                }
                else if (!i.Contains("none") && !lookup.Contains(i) && !i.IsEmpty())
                {
                    lookup.Add(i.Trim());
                }
            }

            lookup.Select(v => new IntegerRange(v)).Each(i => Ranges.Add(i.Start, i));
        }

        public bool Contains(int num)
        {
            return IsInRange(num);
        }

        public bool Contains(string num)
        {
            if (!num.IsInt()) return false;
            return IsInRange(num.ToInt());
        }

        public bool IsInRange(int num)
        {
            var lo = 0;
            var hi = Ranges.Count - 1;
            while (lo <= hi)
            {
                var i = lo + ((hi - lo) >> 1);
                var order = Ranges.Values[i].IsInRange(num) ? 0 : Ranges.Values[i].Start.CompareTo(num);

                if (order == 0) return true;
                if (order < 0)
                    lo = i + 1;
                else
                    hi = i - 1;
            }

            return false;
        }

        public bool IsInRange(string num)
        {
            if (!num.IsInt()) return false;
            return IsInRange(num.ToInt());
        }

        public string RangeToString()
        {
            return Ranges.Values.Select(o => o.ToString()).ToDelimitedString(',');
        }

        public bool IntersectsWith(IntegerRange rng)
        {
            return Ranges.Any(o => o.Value.IntersectsWith(rng));
        }

        public List<int> GetInts()
        {
            return Ranges.Values.SelectMany(o => o.GetInts()).OrderBy(o => o).ToList();
        }

        public override string ToString()
        {
            return this.RangeToString();
        }

        #endregion Public Methods

        #region Public Static

        public static string CollapseIntegersToRanges(IEnumerable<int> integers)
        {
            return integers.CollapseIntsToRanges();
        }

        public static List<string> ExplodeRange(string rng)
        {
            var lst = new IntegerRangeCollection();
            lst.AddUniqueIntegersFromRange(rng.Split(','));
            return lst.SelectMany(v => v.GetInts().Select(i => i.ToString())).ToList();
        }

        #endregion Public Static

        #region IList

        IEnumerator<IntegerRange> IEnumerable<IntegerRange>.GetEnumerator()
        {
            return Ranges.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Ranges).GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)Ranges).CopyTo(array, index);
        }

        public bool Remove(IntegerRange item)
        {
            return Ranges.Remove(item.Start);
        }

        public int Count => Ranges.Count;

        public object SyncRoot => ((ICollection)Ranges).SyncRoot;

        public bool IsSynchronized => ((ICollection)Ranges).IsSynchronized;

        public int Add(object value)
        {
            return ((IList)Ranges).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)Ranges).Contains(value);
        }

        public void Add(IntegerRange item)
        {
            Ranges.Add(item.Start, item);
        }

        public void Clear()
        {
            Ranges.Clear();
        }

        public bool Contains(IntegerRange item)
        {
            return Ranges.Values.Any(i => i.Start == item.Start && i.Stop == item.Stop);
        }

        public void CopyTo(IntegerRange[] array, int arrayIndex)
        {
            Ranges.Values.CopyTo(array, arrayIndex);
        }

        public int IndexOf(object value)
        {
            return ((IList)Ranges).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)Ranges).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)Ranges).Remove(value);
        }

        public int IndexOf(IntegerRange item)
        {
            return Ranges.Values.IndexOf(item);
        }

        public void Insert(int index, IntegerRange item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Ranges.RemoveAt(index);
        }

        IntegerRange IList<IntegerRange>.this[int index]
        {
            get { return Ranges[index]; }
            set { Ranges[index] = value; }
        }

        public object this[int index]
        {
            get { return ((IList)Ranges)[index]; }
            set { ((IList)Ranges)[index] = value; }
        }

        public bool IsReadOnly => ((IList)Ranges).IsReadOnly;

        public bool IsFixedSize => ((IList)Ranges).IsFixedSize;

        #endregion IList
    }
}