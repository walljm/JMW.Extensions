using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.Enumerable;
using JMW.Extensions.Numbers;
using JMW.Extensions.String;
using Lambda.Generic.Arithmetic;

namespace JMW.Types
{
    public class LongRangeCollection : RangeCollection<long, LongMath>
    {
        public LongRangeCollection()
        {
        }

        public LongRangeCollection(IEnumerable<Range<long, LongMath>> rng) : base(rng)
        {
        }

        public LongRangeCollection(Range<long, LongMath> rng) : base(rng)
        {
        }

        public LongRangeCollection(Signed<long, LongMath> start, Signed<long, LongMath> stop) : base(start, stop)
        {
        }

        public LongRangeCollection(string rng) : base(rng)
        {
        }

        public LongRangeCollection(IEnumerable<string> rng) : base(rng)
        {
        }

        public LongRangeCollection(IEnumerable<Signed<long, LongMath>> rng) : base(rng)
        {
        }
    }

    public class IntegerRangeCollection : RangeCollection<int, IntMath>
    {
        public IntegerRangeCollection()
        {
        }

        public IntegerRangeCollection(IEnumerable<Range<int, IntMath>> rng) : base(rng)
        {
        }

        public IntegerRangeCollection(Range<int, IntMath> rng) : base(rng)
        {
        }

        public IntegerRangeCollection(Signed<int, IntMath> start, Signed<int, IntMath> stop) : base(start, stop)
        {
        }

        public IntegerRangeCollection(string rng) : base(rng)
        {
        }

        public IntegerRangeCollection(IEnumerable<string> rng) : base(rng)
        {
        }

        public IntegerRangeCollection(IEnumerable<Signed<int, IntMath>> rng) : base(rng)
        {
        }
    }

    public class RangeCollection<T, C> : IList<Range<T, C>>
        where T :  IComparable<T>, IComparable, IFormattable, IConvertible, IEquatable<T>, new()
        where C : ISignedMath<T>, new()
    {
        #region Constructors

        public RangeCollection()
        {
        }

        public RangeCollection(IEnumerable<Range<T, C>> rng)
        {
            var rngs = rng.OrderBy(r => r.Start).ToList();
            collapseRanges(rngs);
            rngs.Each(i => Ranges.Add(i.Start, i)); // add them to the range.;
        }

        public RangeCollection(Range<T, C> rng)
        {
            Ranges.Add(rng.Start, new Range<T, C>(rng));
        }

        public RangeCollection(Signed<T, C> start, Signed<T, C> stop)
        {
            Ranges.Add(start, new Range<T, C>(start, stop));
        }

        public RangeCollection(string rng)
        {
            getCleanInts(rng.Split(',')).Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        public RangeCollection(IEnumerable<string> rng)
        {
            getCleanInts(rng).Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        public RangeCollection(IEnumerable<Signed<T, C>> rng)
        {
            rng.CollapseNumbersToRanges() // turn back into objects
               .Each(i => Ranges.Add(i.Start, i)); // add them to the range.
        }

        private static IEnumerable<Range<T, C>> getCleanInts(IEnumerable<string> rng)
        {
            var rngs = rng.Select(o => o.Trim()) // trim whitespace
                .Where(o => o.Length > 0 && (o.Contains('-') || o.IsIntFast())) // eliminate empty strings
                .Select(o => new Range<T, C>(o))// create ranges
                .OrderBy(r => r.Start)
                .ToList();
            collapseRanges(rngs);
            return rngs;
        }

        #endregion Constructors

        #region Properties

        public SortedList<Signed<T, C>, Range<T, C>> Ranges { get; set; } = new SortedList<Signed<T, C>, Range<T, C>>();

        #endregion Properties

        #region Public Methods

        public void AddUniqueLongsFromRange(IEnumerable<string> lst)
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

            AddRange(lookup.Select(v => new Range<T, C>(v)));
        }

        public bool Contains(Signed<T, C> num)
        {
            return IsInRange(num);
        }

        public bool Contains(string num)
        {
            if (!num.IsInt()) return false;
            return IsInRange(Signed<T, C>.Parse(num));
        }

        /// <summary>
        /// Returns true if the provided int is included in the ranges.
        /// </summary>
        /// <param name="num">number to check</param>
        public bool IsInRange(Signed<T, C> num)
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
            return IsInRange(Signed<T, C>.Parse(num));
        }

        public string RangeToString()
        {
            return Ranges.Values.Select(o => o.ToString()).ToDelimitedString(',');
        }

        public bool IntersectsWith(Range<T, C> rng)
        {
            return Ranges.Any(o => o.Value.IntersectsWith(rng));
        }

        public List<Signed<T, C>> GetInts()
        {
            return Ranges.Values.SelectMany(o => o.GetNumbers()).OrderBy(o => o).ToList();
        }

        public override string ToString()
        {
            return RangeToString();
        }

        #endregion Public Methods

        #region Public Static

        public static List<string> ExplodeRange(string rng)
        {
            var lst = new RangeCollection<T, C>();
            lst.AddUniqueLongsFromRange(rng.Split(','));
            return lst.SelectMany(v => v.GetNumbers().Select(i => i.ToString())).ToList();
        }

        #endregion Public Static

        #region IList

        IEnumerator<Range<T, C>> IEnumerable<Range<T, C>>.GetEnumerator()
        {
            return Ranges.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Ranges.Values).GetEnumerator();
        }

        public void CopyTo(Array array, Signed<T, C> index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Range<T, C> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return Ranges.Remove(item.Start);
        }

        public int Count => Ranges.Count;

        public object SyncRoot => ((ICollection)Ranges.Values).SyncRoot;

        public bool IsSynchronized => ((ICollection)Ranges.Values).IsSynchronized;

        public void Clear()
        {
            Ranges.Clear();
        }

        public bool Contains(Range<T, C> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return Ranges.Values.Any(i => i.Start == item.Start && i.Stop == item.Stop);
        }

        public void CopyTo(Range<T, C>[] array, int arrayIndex)
        {
            Ranges.Values.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Range<T, C> item)
        {
            return Ranges.Values.IndexOf(item);
        }

        public void Insert(int index, Range<T, C> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Ranges.RemoveAt(index);
        }

        Range<T, C> IList<Range<T, C>>.this[int index]
        {
            get { return Ranges.Values[index]; }
            set { Ranges.RemoveAt(index); Add(value); }
        }

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        #endregion IList

        public void Add(Range<T, C> item)
        {
            AddRange(item.ToListOfItem());
        }

        public void AddRange(IEnumerable<Range<T, C>> items)
        {
            var lst = mergeVlanRanges(Ranges.Values.ToList(), items.ToList());
            Ranges.Clear();
            lst.Each(o => Ranges.Add(o.Start, o));
        }

        public void AddRange(IEnumerable<string> items)
        {
            var lst = mergeVlanRanges(Ranges.Values.ToList(), new RangeCollection<T, C>(items).ToList());
            Ranges.Clear();
            lst.Each(o => Ranges.Add(o.Start, o));
        }

        private static List<Range<T, C>> mergeVlanRanges(IList<Range<T, C>> a, IList<Range<T, C>> b)
        {
            var rngs = new List<Range<T, C>>();

            while (b.Count > 0 && a.Count > 0)
            {
                Range<T, C> r;

                if (b[0].Start <= a[0].Start)
                {
                    r = b[0];

                    if (b[0].Stop >= a[0].Start)
                    {
                        if (b[0].Stop > a[0].Stop)
                            r = new Range<T, C>(r.Start, b[0].Stop);
                        else
                            r = new Range<T, C>(r.Start, a[0].Stop);

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
                            r = new Range<T, C>(r.Start, a[0].Stop);
                        else
                            r = new Range<T, C>(r.Start, b[0].Stop);

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

        private static void collapseRanges(IList<Range<T, C>> rngs)
        {
            // collapse contiguous ranges together
            for (var i = 1; i < rngs.Count; i++)
            {
                var prev = rngs[i - 1];
                var next = rngs[i];

                if (prev.Stop + Signed<T, C>.One >= next.Start)
                {
                    if (prev.Stop <= next.Stop)
                        rngs[i - 1] = new Range<T, C>(prev.Start, next.Stop);

                    rngs.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}