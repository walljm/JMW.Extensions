/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using JMW.Extensions.Reflection;
using JMW.Reflection;
using JMW.Types.Functional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JMW.Types.Collections
{
    public class IndexedCollection<T> : IEnumerable<T>, ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IList<T> where T : IndexedClass
    {
        private Dictionary<string, Dictionary<string, List<T>>> _IndexCollection = new Dictionary<string, Dictionary<string, List<T>>>();
        private Dictionary<string, Dictionary<string, T>> _UniqueIndexCollection = new Dictionary<string, Dictionary<string, T>>();
        private ObservableCollection<T> _Collection = new ObservableCollection<T>();
        private Dictionary<string, PropInfo> _IndexedProps = new Dictionary<string, PropInfo>();

        public IndexedCollection()
        {
            _IndexedProps = typeof(T).GetPropertiesByAttribute<Indexed>().ToDictionary(k => k.Name, v => new PropInfo(v, isUnique(v)));

            foreach (var prop in _IndexedProps)
            {
                if (prop.Value.IsUnique) _UniqueIndexCollection.Add(prop.Key, new Dictionary<string, T>());
                else _IndexCollection.Add(prop.Key, new Dictionary<string, List<T>>());
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _Collection.CollectionChanged += value; }
            remove { _Collection.CollectionChanged -= value; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { ((INotifyPropertyChanged)_Collection).PropertyChanged += value; }
            remove { ((INotifyPropertyChanged)_Collection).PropertyChanged -= value; }
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property. 
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="propertyLambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, T>> GetUniqueIndexCollection(Expression<Func<T, object>> propertyLambda)
        {
            var name = Linq.GetPropertyName<T>(propertyLambda);
            if (_UniqueIndexCollection.ContainsKey(name))
                return new Maybe<Dictionary<string, T>>(_UniqueIndexCollection[name].ToDictionary(k => k.Key, v => v.Value));
            else
                return new Maybe<Dictionary<string, T>>(new ArgumentException("Property \"" + name + "\" does not exist."));
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given property.  The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="propertyLambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, List<T>>> GetIndexCollection(Expression<Func<T, object>> propertyLambda)
        {
            var name = Linq.GetPropertyName<T>(propertyLambda);

            if (_IndexCollection.ContainsKey(name))
                return new Maybe<Dictionary<string, List<T>>>(_IndexCollection[name].ToDictionary(k => k.Key, v => v.Value));
            else
                return new Maybe<Dictionary<string, List<T>>>(new ArgumentException("Property \"" + name + "\" does not exist."));
        }

        #region Private Methods

        private void onIndexedPropertyChanged(object sender, IndexedPropertyChangedEventArgs e)
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
                var is_collection = prop.Info.PropertyType.HasInterface<IEnumerable<T>>() && prop.Info.PropertyType != typeof(string);
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
                var is_collection = prop.Info.PropertyType.HasInterface<IEnumerable<T>>() && prop.Info.PropertyType != typeof(string);
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

        public void RemoveAt(int index)
        {
            var item = _Collection[index];
            Remove(item);
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

        public T this[int index]
        {
            get
            {
                return _Collection[index];
            }

            set
            {
                var item = _Collection[index];

                removeFromDictionaries(item);
                item.IndexedPropertyChanged -= onIndexedPropertyChanged;

                _Collection[index] = value;

                addToDictionaries(value);
                value.IndexedPropertyChanged += new IndexedClass.IndexedPropertyChangedHandler(onIndexedPropertyChanged);
            }
        }

        #endregion ICollection<T>

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