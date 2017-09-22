using JMW.Extensions.Enumerable;
using JMW.Extensions.Numbers;
using JMW.Extensions.String;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Types
{
    public class IntegerRangeCollection : IList<IntegerRange>
    {
        #region Constructors

        public IntegerRangeCollection()
        {
        }

        public IntegerRangeCollection(IEnumerable<IntegerRange> rng)
        {
            var rngs = rng.OrderBy(r => r.Start).ToList();
            collapseRanges(rngs);
            rngs.Each(i => Ranges.Add(i.Start, i)); // add them to the range.;
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
            getCleanInts(rng.Split(',')).Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        public IntegerRangeCollection(IEnumerable<string> rng)
        {
            getCleanInts(rng).Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        public IntegerRangeCollection(IEnumerable<int> rng)
        {
            rng.CollapseIntsToIntegerRanges() // turn back into objects
               .Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        private static IEnumerable<IntegerRange> getCleanInts(IEnumerable<string> rng)
        {
            var rngs = rng.Select(o => o.Trim()) // trim whitespace
                .Where(o => o.Length > 0 && (o.Contains('-') || o.IsIntFast())) // eliminate empty strings
                .Select(o => new IntegerRange(o))// create ranges
                .OrderBy(r => r.Start)
                .ToList();
            collapseRanges(rngs);
            return rngs;
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
                    var start = blocks[0].ToInt();
                    var end = blocks[1].ToInt();
                    for (var c = start; c <= end; c++)
                    {
                        if (!lookup.Contains(c.ToString()))
                        {
                            lookup.Add(c.ToString().Trim());
                        }
                    }
                }
                else if (!i.Contains("none") && !lookup.Contains(i) && !i.IsEmpty())
                {
                    lookup.Add(i.Trim());
                }
            }

            AddRange(lookup.Select(v => new IntegerRange(v)));
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

        /// <summary>
        /// Returns true if the provided int is included in the ranges.
        /// </summary>
        /// <param name="num">number to check</param>
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

        /// <summary>
        /// Returns true if the provided int is included in the ranges.
        /// </summary>
        /// <param name="num">number to check</param>
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
            return RangeToString();
        }

        #endregion Public Methods

        #region Public Static

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
            return ((IEnumerable)Ranges.Values).GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IntegerRange item)
        {
            return Ranges.Remove(item.Start);
        }

        public int Count => Ranges.Count;

        public object SyncRoot => ((ICollection)Ranges.Values).SyncRoot;

        public bool IsSynchronized => ((ICollection)Ranges.Values).IsSynchronized;

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
            get { return Ranges.Values[index]; }
            set { Ranges.RemoveAt(index); Add(value); }
        }

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        #endregion IList

        public void Add(IntegerRange item)
        {
            AddRange(item.ToListOfItem());
        }

        public void AddRange(IEnumerable<IntegerRange> items)
        {
            var lst = mergeVlanRanges(Ranges.Values.ToList(), items.ToList());
            Ranges.Clear();
            lst.Each(o => Ranges.Add(o.Start, o));
        }

        public void AddRange(IEnumerable<string> items)
        {
            var lst = mergeVlanRanges(Ranges.Values.ToList(), new IntegerRangeCollection(items).ToList());
            Ranges.Clear();
            lst.Each(o => Ranges.Add(o.Start, o));
        }

        private static List<IntegerRange> mergeVlanRanges(IList<IntegerRange> a, IList<IntegerRange> b)
        {
            var rngs = new List<IntegerRange>();

            while (b.Count > 0 && a.Count > 0)
            {
                IntegerRange r;

                if (b[0].Start <= a[0].Start)
                {
                    r = b[0];

                    if (b[0].Stop >= a[0].Start)
                    {
                        if (b[0].Stop > a[0].Stop)
                            r = new IntegerRange(r.Start, b[0].Stop);
                        else
                            r = new IntegerRange(r.Start, a[0].Stop);

                        a.RemoveAt(0); // in this case, b includes a, so a should be removed also.
                    }

                    b.RemoveAt(0); // b is what we started with, so it always gets removed.
                    rngs.Add(r);
                }
                else if (a[0].Start <= b[0].Start)
                {
                    r = a[0];

                    if (a[0].Stop >= b[0].Start)
                    {
                        if (a[0].Stop > b[0].Stop)
                            r = new IntegerRange(r.Start, a[0].Stop);
                        else
                            r = new IntegerRange(r.Start, b[0].Stop);

                        b.RemoveAt(0); // in this case, a includes b, so b should be removed also.
                    }

                    a.RemoveAt(0); // a is what we started with, so it always gets removed.
                    rngs.Add(r);
                }
            }
            if (b.Count > 0) rngs.AddRange(b);
            if (a.Count > 0) rngs.AddRange(a);

            collapseRanges(rngs);

            return rngs;
        }

        private static void collapseRanges(List<IntegerRange> rngs)
        {
            // collapse contiguous ranges together
            for (var i = 1; i < rngs.Count; i++)
            {
                var prev = rngs[i - 1];
                var next = rngs[i];

                if (prev.Stop + 1 >= next.Start)
                {
                    if (prev.Stop <= next.Stop)
                        rngs[i - 1] = new IntegerRange(prev.Start, next.Stop);

                    rngs.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}