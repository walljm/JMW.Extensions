using System;
using System.Collections.Generic;
using JMW.Extensions.Numbers;
using Lambda.Generic.Arithmetic;

namespace JMW.Types
{
    public class LongRange : Range<long, LongMath>
    {
        public LongRange()
        {
        }

        public LongRange(Range<long, LongMath> rng) : base(rng)
        {
        }
        
        public LongRange(LongRange rng) : base(rng)
        {
        }
        public LongRange(long start, long stop) : base(start, stop)
        {
        }

        public LongRange(string rng) : base(rng)
        {
        }
    }

    public class IntegerRange : Range<int, IntMath>
    {
        public IntegerRange()
        {
        }

        public IntegerRange(Range<int, IntMath> rng) : base(rng)
        {
        }
        
        public IntegerRange(IntegerRange rng) : base(rng)
        {
        }
        public IntegerRange(int start, int stop) : base(start, stop)
        {
        }

        public IntegerRange(string rng) : base(rng)
        {
        }
    }

    public class Range<T, C>
        where T :  IComparable<T>, IComparable, IFormattable, IConvertible, IEquatable<T>, new()
        where C : ISignedMath<T>, new()
    {
        /// <summary>
        /// An instance of the calculator. We will use this to perform the calculations.
        /// </summary>
        private static C calc = new C();

        #region Constructors

        public Range()
        {
        }

        public Range(Range<T, C> rng)
        {
            Start = rng.Start;
            Stop = rng.Stop;
        }

        public Range(T start, T stop)
        {
            Start = start;
            Stop = stop;
        }

        public Range(string rng)
        {
            try
            {
                if (rng.Contains("-"))
                {
                    Start = Signed<T, C>.Parse(rng.Split('-')[0]);
                    Stop = Signed<T, C>.Parse(rng.Split('-')[1]);
                }
                else
                {
                    Start = Signed<T, C>.Parse(rng.Split('-')[0]);
                    Stop = Start;
                }
            }
            catch
            {
                //ignore
            }
        }

        #endregion Constructors

        #region Properties

        public Signed<T, C> Start { get; set; } = calc.Negate(calc.One);

        public Signed<T, C> Stop { get; set; } = calc.Negate(calc.One);

        #endregion Properties

        #region Public Methods

        public bool Contains(Signed<T, C> num)
        {
            return IsInRange(num);
        }

        public bool Contains(string num)
        {
            return IsInRange(num);
        }

        public bool IsInRange(Signed<T, C> num)
        {
            if (num >= Start && num <= Stop) return true;
            return false;
        }

        public bool IsInRange(string num)
        {
            if (num.IsInt()) return IsInRange(Signed<T, C>.Parse(num));
            throw new ArgumentException(num + " isn't valid");
        }

        public string RangeToString()
        {
            var r = string.Empty;

            if (Start == Stop) r = Start.ToString();
            else r = Start + "-" + Stop;

            return r;
        }

        public bool IntersectsWith(Range<T, C> rng)
        {
            if (rng.Start >= Start && rng.Start <= Stop) return true;
            if (rng.Stop >= Start && rng.Stop <= Stop) return true;
            return false;
        }

        public List<Signed<T, C>> GetNumbers()
        {
            var lst = new List<Signed<T, C>>();
            for (var i = Start; i <= Stop; i = i + Signed<T, C>.One)
            {
                lst.Add(i);
            }
            return lst;
        }

        public override string ToString()
        {
            return RangeToString();
        }

        #endregion Public Methods
    }
}