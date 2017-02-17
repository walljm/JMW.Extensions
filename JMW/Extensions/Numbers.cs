﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Extensions.Numbers
{
    public static class Numbers
    {
        /// <summary>
        /// Indicates if the string is an <see cref="int"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInt(this string s)
        {
            int t;
            return int.TryParse(s, out t);
        }

        /// <summary>
        /// Indicates if the string is an <see cref="long"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsLong(this string s)
        {
            long t;
            return long.TryParse(s, out t);
        }

        /// <summary>
        /// Indicates if the string is a <see cref="double"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDouble(this string s)
        {
            double t;
            return double.TryParse(s, out t);
        }

        /// <summary>
        /// Indicates if the string is not null.
        /// </summary>
        public static bool IsNotNull(this object o)
        {
            return o != null;
        }

        /// <summary>
        /// Indicates if the string is null
        /// </summary>
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        /// <summary>
        /// Converts a Decimal to an Int32
        /// </summary>
        public static int ToInt(this decimal d)
        {
            return Convert.ToInt32(d);
        }

        /// <summary>
        /// Converts a Double to an Int32
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int ToInt(this double d)
        {
            return Convert.ToInt32(d);
        }

        /// <summary>
        /// Converts a string to an integer using int.Parse
        /// </summary>
        public static int ToInt(this string i)
        {
            return int.Parse(i);
        }

        /// <summary>
        /// Converts a Decimal to an Int32
        /// </summary>
        public static long ToLong(this decimal d)
        {
            return Convert.ToInt64(d);
        }

        /// <summary>
        /// Converts a Double to an Int32
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long ToLong(this double d)
        {
            return Convert.ToInt64(d);
        }

        /// <summary>
        /// Converts a string to an integer using int.Parse
        /// </summary>
        public static long ToLong(this string i)
        {
            return long.Parse(i);
        }

        /// <summary>
        /// Converts a string to an integer using int.Parse
        /// </summary>
        public static double ToDouble(this string i)
        {
            return double.Parse(i);
        }

        /// <summary>
        /// Converts a string to an integer using int.Parse
        /// </summary>
        public static double ToDouble(this int i)
        {
            return Convert.ToDouble(i);
        }

        /// <summary>
        /// Converts a string to an integer using int.Parse
        /// </summary>
        public static double ToDouble(this long i)
        {
            return Convert.ToDouble(i);
        }
        
        /// <summary>
        /// Safely converts a string to a integer. If the string is not an integer, returns -1 as a default.
        /// </summary>
        /// <param name="s">A string to convert to an Integer.</param>
        /// <returns>An Integer.</returns>
        public static int ToIntOrNeg1(this string s)
        {
            //good for timers in defunct state
            int i;
            var r = int.TryParse(s, out i);

            if (r)
            {
                return i;
            }
            return -1;
        }

        /// <summary>
        /// Safely converts a string to a 64bit integer. If the string is not a 64bit integer, returns -1 as a default.
        /// </summary>
        /// <param name="s">A string to convert to a 64bit Integer.</param>
        /// <returns>An Integer.</returns>
        public static long ToInt64OrNeg1(this string s)
        {
            //good for timers in defunct state
            long i;
            var r = long.TryParse(s, out i);

            if (r)
            {
                return i;
            }
            return -1;
        }

        private static readonly Dictionary<string, long> _numAbbreviations = new Dictionary<string, long> { { "k", 1000 }, { "m", 1000000 }, { "g", 1000000000 }, { "t", 1000000000000 } };

        /// <summary>
        /// Handles converting text to an Int64. Will properly deal with common abbreviations (1k, 2m)
        /// </summary>
        /// <param name="i">The text to be converted</param>
        /// <returns></returns>
        public static long ToInt64(this string i)
        {
            var last = i.ToLower().ToCharArray().Last().ToString();
            var prefix = i.ToLower().Substring(0, i.Length - 1);

            // first, check if its abbreviated.
            if (!_numAbbreviations.ContainsKey(last) || !prefix.IsDouble()) return long.Parse(i);

            var n = prefix.ToDouble() * _numAbbreviations[last];
            return long.Parse(n.ToString());
        }

        /// <summary>
        /// Handles converting text to an Int64. Will properly deal with common abbreviations (1k, 2m)
        /// </summary>
        /// <param name="i">The text to be converted</param>
        /// <returns></returns>
        public static long ToInt64orNeg1(this string i)
        {
            long val;
            if (i.Length == 0) return -1;

            var last = i.ToLower().ToCharArray().Last().ToString();
            var prefex = i.ToLower().Substring(0, i.Length - 1);
            // first, check if its abbreviated.
            if (_numAbbreviations.ContainsKey(last) && prefex.IsDouble())
            {
                var n = prefex.ToDouble() * _numAbbreviations[last];

                if (long.TryParse(n.ToString(), out val)) return val;
                return -1;
            }

            if (long.TryParse(i, out val)) return val;
            return -1;
        }

        public static float ToFloatOrNeg1(this string s)
        {
            float i;
            var r = float.TryParse(s, out i);

            if (r)
            {
                return i;
            }
            return -1;
        }

        public static double ToDoubleOrNeg1(this string s)
        {
            double i;
            var r = double.TryParse(s, out i);

            if (r)
            {
                return i;
            }
            return -1;
        }

        public static string CollapseIntsToRanges(this IEnumerable<string> ints)
        {
            return ints.Where(s => s.IsInt()).Select(s => s.ToInt()).CollapseIntsToRanges();
        }

        public static string CollapseIntsToRanges(this IEnumerable<int> ints)
        {
            var r = "";
            var prev = -1;
            var current = -1;

            var sorted = ints.Distinct().OrderBy(i => i).ToList();

            if (sorted.Count > 1)
            {
                for (var i = 1; i < sorted.Count; i++)
                {
                    prev = sorted[i - 1];
                    current = sorted[i];

                    if (prev + 1 == current) // you're at the start of a range.
                    {
                        r += prev + "-";

                        while (prev + 1 == current)
                        {
                            prev = current;
                            if (i < sorted.Count - 1)
                                current = sorted[++i];
                            else
                                break;
                        }
                        r += prev + ", ";
                    }
                    else // its a single.
                    {
                        r += prev + ", ";
                    }
                }
            }
            else if (sorted.Count == 1)
            {
                r = sorted[0].ToString();
            }

            if (prev != current)
                r += sorted.Last();

            r = r.Trim().Trim(',');

            return r;
        }

        public static string CollapseLongsToRanges(this IEnumerable<string> ints)
        {
            return ints.Where(s => s.IsLong()).Select(s => s.ToLong()).CollapseLongsToRanges();
        }

        public static string CollapseLongsToRanges(this IEnumerable<long> ints)
        {
            var r = "";
            long prev = -1;
            long current = -1;

            var sorted = ints.Distinct().OrderBy(i => i).ToList();

            if (sorted.Count > 1)
            {
                for (var i = 1; i < sorted.Count; i++)
                {
                    prev = sorted[i - 1];
                    current = sorted[i];

                    if (prev + 1 == current) // you're at the start of a range.
                    {
                        r += prev + "-";

                        while (prev + 1 == current)
                        {
                            prev = current;
                            if (i < sorted.Count - 1)
                                current = sorted[++i];
                            else
                                break;
                        }
                        r += prev + ", ";
                    }
                    else // its a single.
                    {
                        r += prev + ", ";
                    }
                }
            }
            else if (sorted.Count == 1)
            {
                r = sorted[0].ToString();
            }

            if (prev != current)
                r += sorted.Last();

            r = r.Trim().Trim(',');

            return r;
        }

        public static bool NearlyEqual(this double a, double b, double epsilon)
        {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == 0 || b == 0 || diff < double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }

            // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}