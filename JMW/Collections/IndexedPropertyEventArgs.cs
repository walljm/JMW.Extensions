using System;

namespace JMW.Types.Collections
{
    public class IndexedPropertyEventArgs : EventArgs
    {
        private object _Before;
        public object Before
        {
            get
            {
                return _Before;
            }

            set
            {
                _Before = value;
            }
        }

        private object _After;
        public object After
        {
            get
            {
                return _After;
            }

            set
            {
                _After = value;
            }
        }

        private string _PropertyName;
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
        }


        public IndexedPropertyEventArgs(string prop_name, object before, object after)
        {
            _Before = before;
            _After = after;
            _PropertyName = prop_name;
        }
    }
}