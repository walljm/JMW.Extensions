using JMW.Extensions.Numbers;
using System;
using System.Collections.Generic;

namespace JMW.Types
{
    public class IntegerRange
    {
        #region Constructors

        public IntegerRange()
        {
        }

        public IntegerRange(IntegerRange rng)
        {
            Start = rng.Start;
            Stop = rng.Stop;
        }

        public IntegerRange(int start, int stop)
        {
            Start = start;
            Stop = stop;
        }

        public IntegerRange(string rng)
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
            }
        }

        #endregion Constructors

        #region Properties

        public int Start { get; set; } = -1;

        public int Stop { get; set; } = -1;

        #endregion Properties

        #region Public Methods

        public bool Contains(int num)
        {
            return IsInRange(num);
        }

        public bool Contains(string num)
        {
            return IsInRange(num);
        }

        public bool IsInRange(int num)
        {
            return num >= Start && num <= Stop;
        }

        public bool IsInRange(string num)
        {
            if (num.IsInt()) return IsInRange(num.ToInt());
            throw new ArgumentException(num + " isn't valid");
        }

        public string RangeToString()
        {
            string r;

            if (Start == Stop) r = Start.ToString();
            else r = Start + "-" + Stop;

            return r;
        }

        public bool IntersectsWith(IntegerRange rng)
        {
            if (rng.Start >= Start && rng.Start <= Stop) return true;
            if (rng.Stop >= Start && rng.Stop <= Stop) return true;
            return false;
        }

        public List<int> GetInts()
        {
            var lst = new List<int>();
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