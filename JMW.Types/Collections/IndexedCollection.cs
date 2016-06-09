using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JMW.Types.Collections
{
    public class IndexedCollection<T> : IEnumerable<T>, ICollection<T>, IBindingList, IList<T> where T : IndexedClass
    {
        private Dictionary<string, Dictionary<string, List<T>>> _IndexCollection = new Dictionary<string, Dictionary<string, List<T>>>();
        private Dictionary<string, Dictionary<string, T>> _UniqueIndexCollection = new Dictionary<string, Dictionary<string, T>>();
        private BindingList<T> _Collection = new BindingList<T>();
        private Dictionary<string, PropInfo> _IndexedProps = new Dictionary<string, PropInfo>();

        public IndexedCollection()
        {
            _IndexedProps = typeof(T).GetProperties<Indexed>().ToDictionary(k => k.Name, v => new PropInfo(v, isUnique(v)));

            _Collection.AllowEdit = true;
            _Collection.AllowNew = true;
            _Collection.AllowRemove = true;

            foreach (var prop in _IndexedProps)
            {
                if (prop.Value.IsUnique) _UniqueIndexCollection.Add(prop.Key, new Dictionary<string, T>());
                else _IndexCollection.Add(prop.Key, new Dictionary<string, List<T>>());
            }
        }

        public event ListChangedEventHandler ListChanged;

        #region Private Methods

        private void onIndexedPropertyChanged(object sender, IndexedPropertyEventArgs e)
        {
            if (!_IndexedProps.ContainsKey(e.PropertyName)) return; // don't update the index if the property isn't indexed
            var prop = _IndexedProps[e.PropertyName];
            bool is_collection = prop.Info.PropertyType.HasInterface<ICollection>();

            if (prop.IsUnique)
            {
                var alter = (Dictionary<string, T>)_UniqueIndexCollection[e.PropertyName];

                if (is_collection)
                {
                    var before = ((IEnumerable)e.Before).GetEnumerator();
                    var after = ((IEnumerable)e.After).GetEnumerator();

                    while (before.MoveNext())
                    {
                        alter.Remove(before.Current.ToString());
                    }

                    while (after.MoveNext())
                    {
                        alter.Add(after.Current.ToString(), (T)sender);
                    }
                }
                else
                {
                    if (e.Before != null) alter.Remove(e.Before.ToString());
                    alter.Add(e.After.ToString(), (T)sender);
                }
            }
            else
            {
                var alter = (Dictionary<string, List<T>>)_IndexCollection[e.PropertyName];

                if (is_collection)
                {
                    var before = ((IEnumerable)e.Before).GetEnumerator();
                    var after = ((IEnumerable)e.After).GetEnumerator();

                    while (before.MoveNext())
                    {
                        removeFromIndex(sender, before.Current, alter);
                    }

                    while (after.MoveNext())
                    {
                        addToIndex(sender, after.Current, alter);
                    }
                }
                else
                {
                    removeFromIndex(sender, e.Before, alter);
                    addToIndex(sender, e.After, alter);
                }
            }
        }

        private void removeFromIndex(object sender, object prev, Dictionary<string, List<T>> alter)
        {
            if (alter.ContainsKey(prev.ToString()))
            {
                if (alter[prev.ToString()].Count == 1) alter.Remove(prev.ToString());
                else alter[prev.ToString()].Remove((T)sender);
            }
        }

        private void addToIndex(object sender, object curr, Dictionary<string, List<T>> alter)
        {
            if (alter.ContainsKey(curr.ToString()))
            {
                alter[curr.ToString()].Add((T)sender);
            }
            else
            {
                alter.Add(curr.ToString(), new List<T>() { (T)sender });
            }
        }

        private bool addToDictionaries(T val)
        {
            foreach (var prop in _IndexedProps.Values)
            {
                var is_collection = prop.Info.PropertyType.HasInterface<IEnumerable<T>>();
                var is_dict = prop.Info.PropertyType.HasInterface<IDictionary>();
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (is_collection)
                    {
                        // setup events on the list.
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexable, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = o.ToString();
                                if (!_UniqueIndexCollection[p.Name].AddIfNotPresent<string, T>(key, val))
                                    return false;
                            }
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val, null).ToString();
                        if (!_UniqueIndexCollection[p.Name].AddIfNotPresent<string, T>(key, val))
                            return false;
                    }
                }
                else
                {
                    if (is_collection)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexed, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = (is_dict) ? o.GetType().GetProperty("Value").GetValue(o).ToString() : o.ToString();
                                _IndexCollection[p.Name].SafeAdd<string, T>(key, val);
                            }
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val).ToString();
                        _IndexCollection[p.Name].SafeAdd<string, T>(key, val);
                    }
                }
            }

            return true;
        }

        private bool removeFromDictionaries(T val)
        {
            foreach (var prop in _IndexedProps.Values)
            {
                var is_collection = prop.Info.PropertyType.HasInterface<IEnumerable<T>>();
                var is_dict = prop.Info.PropertyType.HasInterface<IDictionary>();
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (is_collection)
                    {
                        // setup events on the list.
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexable, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = o.ToString();
                                if (!_UniqueIndexCollection[p.Name].Remove(key))
                                    return false;
                            }
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val, null).ToString();
                        if (!_UniqueIndexCollection[p.Name].Remove(key))
                            return false;
                    }
                }
                else
                {
                    if (is_collection)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexed, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = (is_dict) ? o.GetType().GetProperty("Value").GetValue(o).ToString() : o.ToString();
                                _IndexCollection[p.Name].Remove(key);
                            }
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val).ToString();
                        _IndexCollection[p.Name].Remove(key);
                    }
                }
            }

            return true;
        }

        #endregion Private Methods

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_Collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_Collection).GetEnumerator();
        }

        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new OperationCanceledException("Operation was cancelled due to an index violation.");
        }

        public bool TryAdd(T obj)
        {
            if (!addToDictionaries(obj)) return false;

            _Collection.Add(obj);
            obj.IndexedPropertyChanged += new IndexedClass.IndexedPropertyChangedHandler(onIndexedPropertyChanged);
            return true;
        }

        public void Clear()
        {
            foreach (T obj in _Collection) obj.IndexedPropertyChanged -= onIndexedPropertyChanged;

            _Collection.Clear();

            foreach (PropInfo p in _IndexedProps.Values)
            {
                if (p.IsUnique)
                    _UniqueIndexCollection[p.Name].Clear();
                else
                    _IndexCollection[p.Name].Clear();
            }
        }

        public bool Contains(T item)
        {
            return _Collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            removeFromDictionaries(item);
            _Collection.Remove(item);
            item.IndexedPropertyChanged -= onIndexedPropertyChanged;

            return true;
        }

        public bool Remove(Func<T, bool> predicate)
        {
            foreach (T o in _Collection.Where(predicate))
                Remove(o);
            return true;
        }

        public object AddNew()
        {
            var n = Activator.CreateInstance<T>();
            this.Add(n);
            return n;
        }

        public void AddIndex(PropertyDescriptor property)
        {
            ((IBindingList)_Collection).AddIndex(property);
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            ((IBindingList)_Collection).ApplySort(property, direction);
        }

        public int Find(PropertyDescriptor property, object key)
        {
            return ((IBindingList)_Collection).Find(property, key);
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            ((IBindingList)_Collection).RemoveIndex(property);
        }

        public void RemoveSort()
        {
            ((IBindingList)_Collection).RemoveSort();
        }

        public int Add(object value)
        {
            T obj = (T)value;
            addToDictionaries(obj);

            var r = ((IBindingList)_Collection).Add(value);
            obj.IndexedPropertyChanged += new IndexedClass.IndexedPropertyChangedHandler(onIndexedPropertyChanged);
            return r;
        }

        public bool Contains(object value)
        {
            return ((IBindingList)_Collection).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IBindingList)_Collection).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IBindingList)_Collection).Insert(index, value);
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            var item = _Collection[index];
            Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((IBindingList)_Collection).CopyTo(array, index);
        }

        #endregion IEnumerable<T>

        #region IList

        public int IndexOf(T item)
        {
            return ((IList<T>)_Collection).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            addToDictionaries(item);

            ((IList<T>)_Collection).Insert(index, item);
            item.IndexedPropertyChanged += new IndexedClass.IndexedPropertyChangedHandler(onIndexedPropertyChanged);
        }

        #endregion IList

        #region ICollection<T>

        public int Count
        {
            get
            {
                return ((ICollection<T>)_Collection).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)_Collection).IsReadOnly;
            }
        }

        #endregion ICollection<T>

        #region IBindingList

        public bool AllowNew
        {
            get
            {
                return ((IBindingList)_Collection).AllowNew;
            }
        }

        public bool AllowEdit
        {
            get
            {
                return ((IBindingList)_Collection).AllowEdit;
            }
        }

        public bool AllowRemove
        {
            get
            {
                return ((IBindingList)_Collection).AllowRemove;
            }
        }

        public bool SupportsChangeNotification
        {
            get
            {
                return ((IBindingList)_Collection).SupportsChangeNotification;
            }
        }

        public bool SupportsSearching
        {
            get
            {
                return ((IBindingList)_Collection).SupportsSearching;
            }
        }

        public bool SupportsSorting
        {
            get
            {
                return ((IBindingList)_Collection).SupportsSorting;
            }
        }

        public bool IsSorted
        {
            get
            {
                return ((IBindingList)_Collection).IsSorted;
            }
        }

        public PropertyDescriptor SortProperty
        {
            get
            {
                return ((IBindingList)_Collection).SortProperty;
            }
        }

        public ListSortDirection SortDirection
        {
            get
            {
                return ((IBindingList)_Collection).SortDirection;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IBindingList)_Collection).IsFixedSize;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((IBindingList)_Collection).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((IBindingList)_Collection).IsSynchronized;
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                return ((IList<T>)_Collection)[index];
            }

            set
            {
                ((IList<T>)_Collection)[index] = value;
            }
        }

        public object this[int index]
        {
            get
            {
                return ((IBindingList)_Collection)[index];
            }

            set
            {
                ((IBindingList)_Collection)[index] = value;
            }
        }

        #endregion IBindingList

        #region Linq
        
        public T First()
        {
            return _Collection.First();
        }

        public T First(Func<T, bool> predicate)
        {
            return _Collection.First(predicate);
        }

        public void ForEach(Action<T> action)
        {
            foreach (var item in _Collection) action(item);
        }

        #endregion Linq

        private static bool isUnique(System.Reflection.PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(typeof(Indexed), false);
            if (attrs.Length > 0)
            {
                return (attrs[0] as Indexed).IsUnique;
            }
            return false;
        }

        private class PropInfo
        {
            private PropertyInfo _Info;
            private bool _IsUnique;

            public PropInfo(PropertyInfo info, bool is_unique)
            {
                _Info = info;
                _IsUnique = is_unique;
            }

            public PropertyInfo Info
            {
                get
                {
                    return _Info;
                }
            }

            public bool IsUnique
            {
                get
                {
                    return _IsUnique;
                }
            }

            public string Name
            {
                get
                {
                    return _Info.Name;
                }
            }
        }
    }
}