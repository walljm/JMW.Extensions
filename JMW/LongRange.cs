using JMW.Extensions.Numbers;
using System;
using System.Collections.Generic;

namespace JMW.Types
{
    public class LongRange
    {
        #region Constructors

        public LongRange()
        {
        }

        public LongRange(LongRange rng)
        {
            Start = rng.Start;
            Stop = rng.Stop;
        }

        public LongRange(long start, long stop)
        {
            Start = start;
            Stop = stop;
        }

        public LongRange(string rng)
        {
            try
            {
                if (rng.Contains("-"))
                {
                    Start = rng.Split('-')[0].ToInt();
                    Stop = rng.Split('-')[1].ToInt();
                }
                else
                {
                    Start = rng.ToInt();
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

        public long Start { get; set; } = -1;

        public long Stop { get; set; } = -1;

        #endregion Properties

        #region Public Methods

        public bool Contains(long num)
        {
            return IsInRange(num);
        }

        public bool Contains(string num)
        {
            return IsInRange(num);
        }

        public bool IsInRange(long num)
        {
            if (num >= Start && num <= Stop) return true;
            return false;
        }

        public bool IsInRange(string num)
        {
            if (num.IsInt()) return IsInRange(num.ToInt());
            throw new ArgumentException(num + " isn't valid");
        }

        public string RangeToString()
        {
            var r = "";

            if (Start == Stop) r = Start.ToString();
            else r = Start + "-" + Stop;

            return r;
        }

        public bool IntersectsWith(LongRange rng)
        {
            if (rng.Start >= Start && rng.Start <= Stop) return true;
            if (rng.Stop >= Start && rng.Stop <= Stop) return true;
            return false;
        }

        public List<long> GetInts()
        {
            var lst = new List<long>();
            for (var i = Start; i <= Stop; i++)
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