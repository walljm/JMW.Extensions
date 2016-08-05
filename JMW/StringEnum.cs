/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace JMW.Types
{
    /// <summary>
    /// The StringEnum class is designed to allow you to use a class like an Enum.
    /// </summary>
    public abstract class StringEnum<T> : IEquatable<StringEnum<T>>, IComparable, IComparable<StringEnum<T>> where T : StringEnum<T>
    {
        protected static HashSet<StringEnum<T>> _values = new HashSet<StringEnum<T>>();

        #region Constructors

        public StringEnum(string value, long ordinal = -1)
        {
            _Value = value;
            _Ordinal = (ordinal > 0) ? ordinal : _values.Count;

            AddValue(this);
        }

        #endregion Constructors

        #region Properties

        private string _Value = "";
        /// <summary>
        /// The Value of the enum.
        /// </summary>
        public string Value
        {
            get { return _Value; }
        }

        private long _Ordinal = -1;
        /// <summary>
        /// This provides an optional ordinal value for backwards compatibility.
        /// </summary>
        public long Ordinal
        {
            get { return _Ordinal; }
        }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Compares two StringEnum instances
        /// </summary>
        /// <param name="enum1">the first StringEnum</param>
        /// <param name="enum2">the second StringEnum</param>
        /// <returns>true if both have the same value (case sensitive)</returns>
        public static bool operator ==(StringEnum<T> enum1, StringEnum<T> enum2)
        {
            if (Object.ReferenceEquals(enum1, null) && Object.ReferenceEquals(enum2, null))
            {
                return true;
            }
            else if (Object.ReferenceEquals(enum1, null))
            {
                return false; // the other reference isn't null because of the first if check
            }
            else
            {
                return enum1.Equals(enum2);
            }
        }

        /// <summary>
        /// Returns the opposite of the == operator
        /// </summary>
        /// <param name="enum1">the first StringEnum</param>
        /// <param name="enum2">the second StringEnum</param>
        /// <returns>true if both have different values (case sensitive)</returns>
        public static bool operator !=(StringEnum<T> enum1, StringEnum<T> enum2)
        {
            return !(enum1 == enum2);
        }

        /// <summary>
        /// Checks to see if the name of the StringEnum is the same as the string
        /// </summary>
        /// <param name="enum1">Any StringEnum</param>
        /// <param name="value"></param>
        /// <returns>true if enum1 and value match</returns>
        public static bool operator ==(StringEnum<T> enum1, string value)
        {
            if (Object.ReferenceEquals(enum1, null) && Object.ReferenceEquals(value, null))
            {
                return true;
            }
            else if (Object.ReferenceEquals(enum1, null))
            {
                return false;
            }
            else
            {
                return enum1.Equals(value);
            }
        }

        /// <summary>
        /// Checks to see if the name of the StringEnum is the same as the string
        /// </summary>
        /// <param name="enum1">Any StringEnum</param>
        /// <param name="value"></param>
        /// <returns>true if enum1 and value match</returns>
        public static bool operator ==(StringEnum<T> enum1, long value)
        {
            if (Object.ReferenceEquals(enum1, null))
            {
                return false;
            }
            else
            {
                return enum1.Equals(value);
            }
        }

        /// <summary>
        /// Check to make sure the name of the StringEnum is different from the string
        /// </summary>
        /// <param name="enum1">any StringEnum</param>
        /// <param name="value">any string</param>
        /// <returns>true if value is null or if the enum value and the string value don't match</returns>
        public static bool operator !=(StringEnum<T> enum1, long value)
        {
            return !(enum1 == value);
        }

        /// <summary>
        /// Check to make sure the name of the StringEnum is different from the string
        /// </summary>
        /// <param name="enum1">any StringEnum</param>
        /// <param name="value">any string</param>
        /// <returns>true if value is null or if the enum value and the string value don't match</returns>
        public static bool operator !=(StringEnum<T> enum1, string value)
        {
            return !(enum1 == value);
        }

        ///// <summary>
        ///// Implicitly converts string to StringEnum.
        ///// </summary>
        ///// <param name="value">the string value to convert to a StringEnum</param>
        ///// <returns>the StringEnum with the matching valuel (case sensitive)</returns>
        //public static implicit operator StringEnum<T>(string value)
        //{
        //    return (T)GetValue(value);
        //}

        ///// <summary>
        ///// Implicitly converts long to StringEnum.
        ///// </summary>
        ///// <param name="value">the string value to convert to a StringEnum</param>
        ///// <returns>the StringEnum with the matching valuel (case sensitive)</returns>
        //public static implicit operator StringEnum<T>(long ordinal)
        //{
        //    return (T)GetValue(ordinal);
        //}

        #endregion Operators

        #region Functions

        public static void AddValue(StringEnum<T> value)
        {
            _values.Add(value);
        }

        /// <summary>
        /// Searches all the static OSs for a desired instance
        /// </summary>
        /// <param name="value">A StringEnum with the desired value</param>
        /// <returns>the instance of the StringEnum with the same value as value (case sensitive),
        /// null if nothing is found</returns>
        public static StringEnum<T> GetValue(StringEnum<T> value)
        {
            foreach (StringEnum<T> val in _values)
            {
                if (val == value)
                {
                    return val;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches all the static values for a desired instance
        /// </summary>
        /// <param name="value">The name of the desired enum value</param>
        /// <returns>the instance of the StringEnum with a matching value (case sensitive),
        /// null if nothing is found</returns>
		public static StringEnum<T> GetValue(string value)
        {
            if (value == null) return null;
            foreach (StringEnum<T> val in _values)
            {
                if (val.Value.ToUpper() == value.ToUpper())
                {
                    return val;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches all the static OSs for a desired instance
        /// </summary>
        /// <param name="ordinal">The ordinal (numeric) value of the desired enum value</param>
        /// <returns>the instance of the StringEnum with a matching value (case sensitive),
        /// null if nothing is found</returns>
        public static StringEnum<T> GetValue(long ordinal)
        {
            var type_name = typeof(T).FullName;
            foreach (StringEnum<T> val in _values)
            {
                if (val.Ordinal == ordinal)
                {
                    return val;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all the strings in the enum.
        /// </summary>
        public static IEnumerable<string> GetValues()
        {
            foreach (var i in _values) yield return i.Value;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as StringEnum<T>);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(StringEnum<T> other)
        {
            if (Object.ReferenceEquals(other, null)) return 1;
            return string.Compare(this.Value, other.Value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(String other)
        {
            if (other == null) return 1;
            return string.Compare(this.Value, other);
        }

        #endregion Functions

        #region Overrides

        /// <summary>
        /// Overrides Object.ToString()
        /// </summary>
        /// <returns>The name of the OS</returns>
        public override string ToString()
        {
            return _Value;
        }

        /// <summary>
        /// Checks for equality with another StringEnum
        /// </summary>
        /// <param name="os">another StringEnum</param>
        /// <returns>false if os is null, true if the OS names are the same (case sensitive)</returns>
        public bool Equals(StringEnum<T> os)
        {
            if (Object.ReferenceEquals(os, null))
            {
                return false; // we know we aren't null
            }

            return _Value == ((StringEnum<T>)os)._Value;
        }

        /// <summary>
        /// Checks for equality with string based on OS name
        /// </summary>
        /// <param name="name">the string to check against</param>
        /// <returns>false if the string is null, true if the StringEnum.Name == name (case sensitive)</returns>
        public bool Equals(string name)
        {
            if (Object.ReferenceEquals(name, null))
            {
                return false; // we know we aren't null
            }

            return _Value == name;
        }

        /// <summary>
        /// Checks for equality with string based on OS name
        /// </summary>
        /// <param name="ordinal">the string to check against</param>
        /// <returns>false if the string is null, true if the StringEnum.Name == name (case sensitive)</returns>
        public bool Equals(long ordinal)
        {
            return _Ordinal == ordinal;
        }

        /// <summary>
        /// Overrides Object.Equals(object)
        /// </summary>
        /// <param name="o">The other object to compare to</param>
        /// <returns>true if the name of the other OS is the same as this one (case sensitive),
        /// false if otherwise or if the other object isn't a StringEnum</returns>
        public override bool Equals(object o)
        {
            if (Object.ReferenceEquals(o, null))
            {
                return false; // we know we aren't null
            }
            else if (o.GetType().IsInstanceOfType(this))
            {
                return this.Equals((StringEnum<T>)o);
            }
            else if (o.GetType() == typeof(string)) // don't want to create an instance of string
            {
                return this.Equals((string)o);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///   Returns the hashcode of the name, as that is what is important here.
        /// </summary>
        /// <returns>the Names HashCode</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion Overrides
    }
}