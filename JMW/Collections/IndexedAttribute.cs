using System;

namespace JMW.Types.Collections
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Indexed : Attribute
    {
        private bool _IsUnique;

        public Indexed(bool is_unique = false)
        {
            _IsUnique = is_unique;
        }

        public bool IsUnique
        {
            get
            {
                return _IsUnique;
            }

            set
            {
                _IsUnique = value;
            }
        }
    }
}