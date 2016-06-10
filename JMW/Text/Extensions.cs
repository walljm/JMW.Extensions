/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace JMW.Extensions.Text
{
    public static class Extensions
    {
        #region ParseToIndexOf

        public static string ParseToIndexOf(this string s, params string[] parms)
        {
            return s.ParseToIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the index of one of the provided string parameters
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        /// <returns>Subsection of the string from begining to the index of the first matching parameter</returns>
        public static string ParseToIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                int idx = IndexOf(str, p, case_insensitive);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(0, idx);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf

        #region ParseToLastIndexOf

        public static string ParseToLastIndexOf(this string s, params string[] parms)
        {
            return s.ParseToLastIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the last index of one of the provided string parameters
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseToLastIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                int idx = IndexOf(str, p, case_insensitive, last: true);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(0, idx);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToLastIndexOf

        #region ParseToIndexOf_PlusLength

        public static string ParseToIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseToIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the index of one of the provided string parameters, plus the length of the found parameter
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        /// <returns>Subsection of the string from begining to the index of the first matching parameter</returns>
        public static string ParseToIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                int idx = IndexOf(str, p, case_insensitive);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(0, idx + p.Length);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf_PlusLength

        #region ParseToLastIndexOf_PlusLength

        public static string ParseToLastIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseToLastIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the last index of one of the provided string parameters, plus the length of the found parameter
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseToLastIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                int idx = IndexOf(str, p, case_insensitive, last: true);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(0, idx + p.Length);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToLastIndexOf_PlusLength

        #region ParseToIndexOf_PlusLength

        public static string ParseAfterIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseAfterIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                int idx = IndexOf(str, p, case_insensitive);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(idx + p.Length);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf_PlusLength

        #region ParseAfterLastIndexOf_PlusLength

        public static string ParseAfterLastIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseAfterLastIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterLastIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                int idx = IndexOf(str, p, case_insensitive, last: true);

                // if its greater than 0, return it.
                if (idx > -1)
                {
                    return s.SafeSubstring(idx + p.Length);
                }
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseAfterLastIndexOf_PlusLength

        /// <summary>
        /// Finds the index of a search string.  Optionally allows case insensitivity or option to search for last index.
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="search_string">String to search for</param>
        /// <param name="case_insensitive">Ignore case</param>
        /// <param name="last">Search for last instance</param>
        public static int IndexOf(this string str, string search_string, bool case_insensitive, bool last = false)
        {
            // get the index of our search parameter
            var idx = -1;
            if (case_insensitive)
                idx = last ? str.LastIndexOf(search_string.ToLower()) : str.IndexOf(search_string.ToLower());
            else
                idx = last ? str.LastIndexOf(search_string) : str.IndexOf(search_string);
            return idx;
        }

        /// <summary>
        ///   Safely handles the cases where your start exceeds the length of the string, or your start+length exceeds the length of the string.
        /// </summary>
        /// <param name="s">The string to do the substring on</param>
        /// <param name="start">The index of the string to start parsing from</param>
        /// <param name="length">The number of characters to return</param>
        /// <returns>A subsection of the provided string</returns>
        public static string SafeSubstring(this string s, int start, int? length = null)
        {
            // return empty if the requested start index is past the length of the provided string
            if (start > s.Length) return string.Empty;

            // if length isn't provided, return without it.
            if (length == null) return s.Substring(start);

            // if the length is 0 or less, the string should be empty.
            if (length < 1) return string.Empty;

            int len = (int)length;
            if (start + length > s.Length)
            {
                len = s.Length - start;
            }

            return s.Substring(start, len);
        }
    }
}