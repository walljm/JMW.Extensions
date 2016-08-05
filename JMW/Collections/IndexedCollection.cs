/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JMW.Extensions.Reflection;
using JMW.Reflection;
using JMW.Types.Functional;

namespace JMW.Types.Collections
{
    /// <summary>
    /// A collection that allows you to index more than one property.  The indices are maintained, so when a value
    /// is modified, the indices are updated.
    /// </summary>
    /// <typeparam name="T">The type of object being stored in the collection.</typeparam>
    public class IndexedCollection<T> : IEnumerable<T>, ICollection<T>, INotifyCollectionChanged, IList<T>, IEnumerable where T : IndexedClass
    {
        private Dictionary<string, Dictionary<string, List<T>>> _IndexCollection = new Dictionary<string, Dictionary<string, List<T>>>();
        private Dictionary<string, Dictionary<string, T>> _UniqueIndexCollection = new Dictionary<string, Dictionary<string, T>>();
        private ObservableCollection<T> _Collection = new ObservableCollection<T>();
        private Dictionary<string, PropInfo> _IndexedProps = new Dictionary<string, PropInfo>();

        public IndexedCollection()
        {
            _IndexedProps = typeof(T).GetPropertiesByAttribute<Indexed>().ToDictionary(k => k.Name, v => new PropInfo(v));

            foreach (var prop in _IndexedProps)
            {
                if (prop.Value.IsUnique) _UniqueIndexCollection.Add(prop.Key, new Dictionary<string, T>());
                else _IndexCollection.Add(prop.Key, new Dictionary<string, List<T>>());
            }
        }

        /// <summary>
        /// This event gets fired whenever the collection is modified.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _Collection.CollectionChanged += value; }
            remove { _Collection.CollectionChanged -= value; }
        }

        #region Public Methods

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <param name="value">The object identified by the <paramref name="key"/>.</param>
        /// <returns>True if the operation succeeded.</returns>
        public bool TryGetByIndex(Expression<Func<T, object>> property_lambda, object key, out List<T> value)
        {
            var name = Linq.GetPropertyName<T>(property_lambda);

            if (!_IndexCollection.ContainsKey(name))
            {
                value = null;
                return false;
            }

            value = _IndexCollection[name][key.ToString()];
            return true;
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{List{T}}"/> that holds the object.</returns>
        public Maybe<List<T>> GetByIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName<T>(property_lambda);

            if (!_IndexCollection.ContainsKey(name))
                return new Maybe<List<T>>(new ArgumentException("Property does not have an index."));

            return new Maybe<List<T>>(_IndexCollection[name][key.ToString()]);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <param name="value">The object identified by the <paramref name="key"/>.</param>
        /// <returns>True if the operation succeeded.</returns>
        public bool TryGetByUniqueIndex(Expression<Func<T, object>> property_lambda, object key, out T value)
        {
            var name = Linq.GetPropertyName<T>(property_lambda);

            if (!_UniqueIndexCollection.ContainsKey(name))
            {
                value = null;
                return false;
            }

            value = _UniqueIndexCollection[name][key.ToString()];
            return true;
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{T}"/> that holds the object.</returns>
        public Maybe<T> GetByUniqueIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName<T>(property_lambda);

            if (!_UniqueIndexCollection.ContainsKey(name))
                return new Maybe<T>(new ArgumentException("Property does not have an index."));

            return new Maybe<T>(_UniqueIndexCollection[name][key.ToString()]);
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property.
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="property_lambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, T>> GetUniqueIndex(Expression<Func<T, object>> property_lambda)
        {
            var name = Linq.GetPropertyName<T>(property_lambda);
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
        public Maybe<Dictionary<string, List<T>>> GetIndex(Expression<Func<T, object>> propertyLambda)
        {
            var name = Linq.GetPropertyName<T>(propertyLambda);

            if (_IndexCollection.ContainsKey(name))
                return new Maybe<Dictionary<string, List<T>>>(_IndexCollection[name].ToDictionary(k => k.Key, v => v.Value));
            else
                return new Maybe<Dictionary<string, List<T>>>(new ArgumentException("Property \"" + name + "\" does not exist."));
        }

        /// <summary>
        /// Clears then rebuilds the indices.  This is sometimes necessary if an exception was thrown during an operation.
        /// </summary>
        public void RefreshIndices()
        {
            foreach (var idx in _IndexCollection) idx.Value.Clear();
            foreach (var idx in _UniqueIndexCollection) idx.Value.Clear();
            foreach (var item in _Collection)
            {
                item.IndexedPropertyChanged -= onIndexedPropertyChanged;
                addToDictionaries(item);
                item.IndexedPropertyChanged += onIndexedPropertyChanged;
            }
        }

        /// <summary>
        /// Adds an item to the collection.  Throws an exception of type <see cref="OperationCanceledException"/> if the item is unable to be added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new OperationCanceledException("Operation was cancelled due to an index violation. The Index may have been violated.");
        }

        /// <summary>
        /// Adds an item to the collection.  Throws an exception of type <see cref="OperationCanceledException"/> if the item is unable to be added.
        /// </summary>
        /// <param name="obj">The obejct to add</param>
        /// <returns>True if the item was added successfully.</returns>
        public bool TryAdd(T obj)
        {
            if (!addToDictionaries(obj))
                return false;

            _Collection.Add(obj);
            obj.IndexedPropertyChanged += onIndexedPropertyChanged;
            return true;
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>Returns true if the item was successfully removed.</returns>
        public bool Remove(T item)
        {
            var r = removeFromDictionaries(item);
            _Collection.Remove(item);
            item.IndexedPropertyChanged -= onIndexedPropertyChanged;

            return r;
        }

        /// <summary>
        /// Removes items that match the supplied predicate.
        /// </summary>
        /// <param name="predicate">A function that evaluates if the item is something we want to remove.</param>
        /// <returns>True if the operation succeeds.</returns>
        public bool Remove(Func<T, bool> predicate)
        {
            var items = _Collection.Where(predicate).ToList();
            foreach (T o in items)
                Remove(o);
            return true;
        }

        /// <summary>
        /// Removes an item at the supplied location.
        /// </summary>
        /// <param name="index">The index to remove the item from.</param>
        public void RemoveAt(int index)
        {
            var item = _Collection[index];
            Remove(item);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// This event fires whenever an Indexed Property has been changed.
        /// </summary>
        /// <param name="sender">The object being changed</param>
        /// <param name="e">Arguments that give you the name of the property, the value of the property before and after the change.</param>
        private void onIndexedPropertyChanged(object sender, IndexedPropertyChangedEventArgs e)
        {
            if (!_IndexedProps.ContainsKey(e.PropertyName)) return; // don't update the index if the property isn't indexed
            var prop = _IndexedProps[e.PropertyName];

            if (prop.IsUnique)
            {
                var alter = (Dictionary<string, T>)_UniqueIndexCollection[e.PropertyName];

                if (prop.IsCollection)
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

                if (prop.IsCollection)
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

        /// <summary>
        /// Removes a value from the dictionaries (indexes).  This is called by the onIndexedPropertyChanged function.
        /// </summary>
        /// <param name="sender">The value/object being added.</param>
        /// <param name="curr">The current value of the property</param>
        /// <param name="alter">The altered value.</param>
        private void removeFromIndex(object sender, object prev, Dictionary<string, List<T>> alter)
        {
            if (alter.ContainsKey(prev.ToString()))
            {
                if (alter[prev.ToString()].Count == 1)
                    alter.Remove(prev.ToString());
                else
                    alter[prev.ToString()].Remove((T)sender);
            }
        }

        /// <summary>
        /// Adds a value to the dictionaries (indexes).  This is called by the onIndexedPropertyChanged function.
        /// </summary>
        /// <param name="sender">The value/object being added.</param>
        /// <param name="curr">The current value of the property</param>
        /// <param name="alter">The altered value.</param>
        private void addToIndex(object sender, object curr, Dictionary<string, List<T>> alter)
        {
            if (alter.ContainsKey(curr.ToString()))
                alter[curr.ToString()].Add((T)sender);
            else
                alter.Add(curr.ToString(), new List<T>() { (T)sender });
        }

        /// <summary>
        /// Adds the value to all the relevant dictionaries (indexes)
        /// </summary>
        /// <param name="val">The value to add.</param>
        /// <returns>True if the operation succeeded.</returns>
        private bool addToDictionaries(T val)
        {
            foreach (var prop in _IndexedProps.Values)
            {
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (prop.IsCollection || prop.IsDict)
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
                    if (prop.IsCollection || prop.IsDict)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexed, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = (prop.IsDict) ? o.GetType().GetProperty("Value").GetValue(o).ToString() : o.ToString();
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

        /// <summary>
        /// Removes the value from all the relevant dictionaries (indexes)
        /// </summary>
        /// <param name="val">The value to remove</param>
        /// <returns>True if the operation succeeded.</returns>
        private bool removeFromDictionaries(T val)
        {
            foreach (var prop in _IndexedProps.Values)
            {
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (prop.IsCollection || prop.IsDict)
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
                    if (prop.IsCollection || prop.IsDict)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        if (objs != null) // its possible for a property to be specified as indexed, but be null.
                        {
                            foreach (object o in objs)
                            {
                                string key = (prop.IsDict) ? o.GetType().GetProperty("Value").GetValue(o).ToString() : o.ToString();
                                var idx = _IndexCollection[p.Name];
                                if (idx[key].Count > 1)
                                    idx[key].Remove(val);
                                else
                                    idx.Remove(key);
                            }
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val).ToString();
                        var idx = _IndexCollection[p.Name];
                        if (idx[key].Count > 1)
                            idx[key].Remove(val);
                        else
                            idx.Remove(key);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a property is uniquely indexed or not.
        /// </summary>
        /// <param name="prop">A <see cref="PropertyInfo"/> to check for the IsUnique attribute.</param>
        /// <returns>True if the property is unigue.</returns>
        private static bool isUnique(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(typeof(Indexed), false);
            if (attrs.Length > 0 && (attrs[0] as Indexed).IsUnique)
            {
                return true;
            }
            return false;
        }

        #endregion Private Methods

        #region Interface Implementations

        public IEnumerator<T> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Collection.GetEnumerator(); ;
        }

        public void Clear()
        {
            foreach (T obj in _Collection) obj.IndexedPropertyChanged -= onIndexedPropertyChanged;

            _Collection.Clear();

            foreach (var p in _IndexedProps.Values)
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

        public int IndexOf(T item)
        {
            return ((IList<T>)_Collection).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            item.IndexedPropertyChanged -= onIndexedPropertyChanged;
            addToDictionaries(item);

            ((IList<T>)_Collection).Insert(index, item);
            item.IndexedPropertyChanged += onIndexedPropertyChanged;
        }

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

        #endregion Interface Implementations

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

        #region Private classes

        private class PropInfo
        {
            private PropertyInfo _Info;
            private bool _IsUnique;
            private bool _IsCollection;
            private bool _IsDict;
            private Type _PropertyType;

            public PropInfo(PropertyInfo info)
            {
                _Info = info;
                _IsUnique = isUnique(info);
                _IsCollection = info.PropertyType.HasInterface<ICollection>();
                _IsDict = info.PropertyType.HasInterface<IDictionary>();

                _PropertyType = info.PropertyType;
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

            public Type PropertyType
            {
                get
                {
                    return _PropertyType;
                }
            }

            public bool IsDict
            {
                get
                {
                    return _IsDict;
                }

                set
                {
                    _IsDict = value;
                }
            }

            public bool IsCollection
            {
                get
                {
                    return _IsCollection;
                }

                set
                {
                    _IsCollection = value;
                }
            }
        }

        #endregion Private classes
    }
}