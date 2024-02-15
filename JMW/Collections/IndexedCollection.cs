﻿/*
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
using JMW.Extensions.Enumerable;
using JMW.Extensions.Reflection;
using JMW.Functional;
using JMW.Reflection;

namespace JMW.Collections
{
    /// <summary>
    /// A collection that allows you to index more than one property.  The indices are maintained, so when a value
    /// is modified, the indices are updated.
    /// </summary>
    /// <typeparam name="T">The type of object being stored in the collection.</typeparam>
    public class IndexedCollection<T> : INotifyCollectionChanged, IList<T> where T : IndexedClass
    {
        private readonly Dictionary<string, Dictionary<string, List<T>>> _indexCollection = new Dictionary<string, Dictionary<string, List<T>>>();
        private readonly Dictionary<string, Dictionary<string, T>> _uniqueIndexCollection = new Dictionary<string, Dictionary<string, T>>();
        private readonly ObservableCollection<T> _collection = new ObservableCollection<T>();
        private readonly Dictionary<string, PropInfo> _indexedProps;

        public IndexedCollection()
        {
            _indexedProps = typeof(T).GetPropertiesByAttribute<Indexed>().ToDictionary(k => k.Name, v => new PropInfo(v));

            foreach (var prop in _indexedProps)
            {
                if (prop.Value.IsUnique) _uniqueIndexCollection.Add(prop.Key, new Dictionary<string, T>());
                else _indexCollection.Add(prop.Key, new Dictionary<string, List<T>>());
            }
        }

        /// <summary>
        /// This event gets fired whenever the collection is modified.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collection.CollectionChanged += value; }
            remove { _collection.CollectionChanged -= value; }
        }

        public event EventHandler<IndexViolationEventArgs<T>> IndexViolated;

        #region Public Methods

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <param name="value">The object identified by the <paramref name="key"/>.</param>
        /// <returns>True if the operation succeeded.</returns>
        public bool TryGetByIndex(string name, object key, out List<T> value)
        {
            if (!_indexCollection.ContainsKey(name))
            {
                value = null;
                return false;
            }

            value = _indexCollection[name][key.ToString()];
            return true;
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <param name="value">The object identified by the <paramref name="key"/>.</param>
        /// <returns>True if the operation succeeded.</returns>
        public bool TryGetByIndex(Expression<Func<T, object>> property_lambda, object key, out List<T> value)
        {
            var name = Linq.GetPropertyName(property_lambda);

            return TryGetByIndex(name, key, out value);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{T}"/> that holds the object.</returns>
        public Maybe<List<T>> MaybeGetByIndex(string name, object key)
        {
            if (!_indexCollection.ContainsKey(name))
                return new Maybe<List<T>>(new ArgumentException("Property does not have an index."));

            return new Maybe<List<T>>(_indexCollection[name][key.ToString()]);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{T}"/> that holds the object.</returns>
        public Maybe<List<T>> MaybeGetByIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName(property_lambda);

            return MaybeGetByIndex(name, key);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="List{T}"/> that holds the object.</returns>
        public List<T> GetByIndex(string name, object key)
        {
            if (!_indexCollection.ContainsKey(name))
                throw new ArgumentException("Property does not have an index.");

            return new List<T>(_indexCollection[name][key.ToString()]);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="List{T}"/> that holds the object.</returns>
        public List<T> GetByIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName(property_lambda);

            return GetByIndex(name, key);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <param name="value">The object identified by the <paramref name="key"/>.</param>
        /// <returns>True if the operation succeeded.</returns>
        public bool TryGetByUniqueIndex(string name, object key, out T value)
        {
            if (!_uniqueIndexCollection.ContainsKey(name))
            {
                value = null;
                return false;
            }

            value = _uniqueIndexCollection[name][key.ToString()];
            return true;
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
            var name = Linq.GetPropertyName(property_lambda);

            return TryGetByUniqueIndex(name, key, out value);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{T}"/> that holds the object.</returns>
        public Maybe<T> MaybeGetByUniqueIndex(string name, object key)
        {
            if (!_uniqueIndexCollection.ContainsKey(name))
                return new Maybe<T>(new ArgumentException("Property does not have an index."));

            return new Maybe<T>(_uniqueIndexCollection[name][key.ToString()]);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="Maybe{T}"/> that holds the object.</returns>
        public Maybe<T> MaybeGetByUniqueIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName(property_lambda);

            return MaybeGetByUniqueIndex(name, key);
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">A string that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="T"/> that holds the object.</returns>
        public T GetByUniqueIndex(string name, object key)
        {
            if (!_uniqueIndexCollection.ContainsKey(name))
                throw new ArgumentException("Property does not have an index.");

            return _uniqueIndexCollection[name][key.ToString()];
        }

        /// <summary>
        /// Gets the object keyed by the <paramref name="key"/> for the index indicated by the <paramref name="property_lambda"/>.
        /// </summary>
        /// <param name="property_lambda">A property lambda that identifies a property that has been indexed.</param>
        /// <param name="key">The value in the index that the object is keyed to.</param>
        /// <returns>A <see cref="T"/> that holds the object.</returns>
        public T GetByUniqueIndex(Expression<Func<T, object>> property_lambda, object key)
        {
            var name = Linq.GetPropertyName(property_lambda);

            return GetByUniqueIndex(name, key);
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property.
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="name">a string that indicates an indexed property name</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, T>> MaybeGetUniqueIndex(string name)
        {
            if (_uniqueIndexCollection.ContainsKey(name))
                return new Maybe<Dictionary<string, T>>(_uniqueIndexCollection[name].ToDictionary(k => k.Key, v => v.Value));

            return new Maybe<Dictionary<string, T>>(new ArgumentException("Property \"" + name + "\" does not exist."));
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property.
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="property_lambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, T>> MaybeGetUniqueIndex(Expression<Func<T, object>> property_lambda)
        {
            var name = Linq.GetPropertyName(property_lambda);
            return MaybeGetUniqueIndex(name);
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property.
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="name">a string that indicates an indexed property name</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/>  with a copy of the dictionary.</returns>
        public Dictionary<string, T> GetUniqueIndex(string name)
        {
            if (_uniqueIndexCollection.ContainsKey(name))
                return _uniqueIndexCollection[name].ToDictionary(k => k.Key, v => v.Value);

            throw new ArgumentException("Property \"" + name + "\" does not exist.");
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given uniquely indexed property.
        /// The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="property_lambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with a copy of the dictionary.</returns>
        public Dictionary<string, T> GetUniqueIndex(Expression<Func<T, object>> property_lambda)
        {
            var name = Linq.GetPropertyName(property_lambda);
            return GetUniqueIndex(name);
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given property.  The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="name">a string that indicates an indexed property name</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, List<T>>> MaybeGetIndex(string name)
        {
            if (_indexCollection.ContainsKey(name))
                return new Maybe<Dictionary<string, List<T>>>(_indexCollection[name].ToDictionary(k => k.Key, v => v.Value.ToList()));

            return new Maybe<Dictionary<string, List<T>>>(new ArgumentException("Property \"" + name + "\" does not exist."));
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given property.  The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="propertyLambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Maybe{T}"/> with a copy of the dictionary.</returns>
        public Maybe<Dictionary<string, List<T>>> MaybeGetIndex(Expression<Func<T, object>> propertyLambda)
        {
            var name = Linq.GetPropertyName(propertyLambda);

            return MaybeGetIndex(name);
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given property.  The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="name">a string that indicates an indexed property name</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with a copy of the dictionary.</returns>
        public Dictionary<string, List<T>> GetIndex(string name)
        {
            if (_indexCollection.ContainsKey(name))
                return _indexCollection[name].ToDictionary(k => k.Key, v => v.Value.ToList());

            throw new ArgumentException("Property \"" + name + "\" does not exist.");
        }

        /// <summary>
        /// This function returns a copy of the dictionary index for a given property.  The values themselves are not
        /// copied, but the Dictionary is, and can be modified without damaging the IndexedCollection.
        /// </summary>
        /// <param name="propertyLambda">an expression in the form of p=>p.PropertyName</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with a copy of the dictionary.</returns>
        public Dictionary<string, List<T>> GetIndex(Expression<Func<T, object>> propertyLambda)
        {
            var name = Linq.GetPropertyName(propertyLambda);

            return GetIndex(name);
        }

        /// <summary>
        /// Clears then rebuilds the indices.  This is sometimes necessary if an exception was thrown during an operation.
        /// </summary>
        public void RefreshIndices()
        {
            foreach (var idx in _indexCollection) idx.Value.Clear();
            foreach (var idx in _uniqueIndexCollection) idx.Value.Clear();
            foreach (var item in _collection)
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
        /// <exception cref="OperationCanceledException">Thrown if the method is unable to add the item to one of the indices</exception>
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
        /// <exception cref="OperationCanceledException">Thrown if the method is unable to add the item to one of the indices</exception>
        public bool TryAdd(T obj)
        {
            if (!addToDictionaries(obj))
                return false;

            _collection.Add(obj);
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
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var r = removeFromDictionaries(item);
            _collection.Remove(item);
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
            var items = _collection.Where(predicate).ToList();
            foreach (var o in items)
                Remove(o);
            return true;
        }

        /// <summary>
        /// Removes an item at the supplied location.
        /// </summary>
        /// <param name="index">The index to remove the item from.</param>
        public void RemoveAt(int index)
        {
            var item = _collection[index];
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
            if (!_indexedProps.ContainsKey(e.PropertyName)) return; // don't update the index if the property isn't indexed
            var prop = _indexedProps[e.PropertyName];

            if (prop.IsUnique)
            {
                var unique_index = _uniqueIndexCollection[e.PropertyName];

                if (prop.IsCollection)
                {
                    var before = ((IEnumerable)e.Before).GetEnumerator();
                    var after = ((IEnumerable)e.After).GetEnumerator();

                    while (before.MoveNext())
                        // ReSharper disable once PossibleNullReferenceException
                        unique_index.Remove(before.Current.ToString());

                    while (after.MoveNext())
                        // ReSharper disable once PossibleNullReferenceException
                        unique_index.Add(after.Current.ToString(), (T)sender);
                }
                else
                {
                    if (e.Before != null) unique_index.Remove(e.Before.ToString());
                    unique_index.Add(e.After.ToString(), (T)sender);
                }
            }
            else
            {
                var index = _indexCollection[e.PropertyName];

                if (prop.IsCollection)
                {
                    var before = ((IEnumerable)e.Before).GetEnumerator();
                    var after = ((IEnumerable)e.After).GetEnumerator();

                    while (before.MoveNext())
                        removeFromIndex(sender, before.Current, index);

                    while (after.MoveNext())
                        addToIndex(sender, after.Current, index);
                }
                else
                {
                    removeFromIndex(sender, e.Before, index);
                    addToIndex(sender, e.After, index);
                }
            }
        }

        /// <summary>
        /// Removes a value from the dictionaries (indexes).  This is called by the onIndexedPropertyChanged function.
        /// </summary>
        /// <param name="sender">The value/object being added.</param>
        /// <param name="item">The current value of the property</param>
        /// <param name="index">The altered value.</param>
        private static void removeFromIndex(object sender, object item, IDictionary<string, List<T>> index)
        {
            if (!index.ContainsKey(item.ToString())) return;

            if (index[item.ToString()].Count == 1)
                index.Remove(item.ToString());
            else
                index[item.ToString()].Remove((T)sender);
        }

        /// <summary>
        /// Adds a value to the dictionaries (indexes).  This is called by the onIndexedPropertyChanged function.
        /// </summary>
        /// <param name="sender">The value/object being added.</param>
        /// <param name="item">The current value of the property</param>
        /// <param name="index">The altered value.</param>
        private static void addToIndex(object sender, object item, IDictionary<string, List<T>> index)
        {
            if (index.ContainsKey(item.ToString()))
                index[item.ToString()].Add((T)sender);
            else
                index.Add(item.ToString(), new List<T> { (T)sender });
        }

        /// <summary>
        /// Adds the value to all the relevant dictionaries (indexes)
        /// </summary>
        /// <param name="val">The value to add.</param>
        /// <returns>True if the operation succeeded.</returns>
        private bool addToDictionaries(T val)
        {
            foreach (var prop in _indexedProps.Values)
            {
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (prop.IsCollection || prop.IsDict)
                    {
                        // setup events on the list.
                        var objs = (IEnumerable)p.GetValue(val);
                        // its possible for a property to be specified as indexable, but be null.
                        if (objs == null) continue;

                        foreach (var o in objs)
                        {
                            var key = o.ToString();
                            if (_uniqueIndexCollection[p.Name].AddIfNotPresent(key, val)) continue;

                            IndexViolated?.Invoke(this, new IndexViolationEventArgs<T>(val, "Value violated index '" + p.Name + "' using key '" + key + "'"));
                            return false;
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val, null).ToString();
                        if (_uniqueIndexCollection[p.Name].AddIfNotPresent(key, val)) continue;

                        IndexViolated?.Invoke(this, new IndexViolationEventArgs<T>(val, "Value violated index '" + p.Name + "' using key '" + key + "'"));
                        return false;
                    }
                }
                else
                {
                    if (prop.IsCollection || prop.IsDict)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        // its possible for a property to be specified as indexed, but be null.
                        if (objs == null) continue;

                        foreach (var o in objs)
                        {
                            var key = prop.IsDict ? o.GetType().GetProperty(nameof(KeyValuePair<string, T>.Value))?.GetValue(o).ToString() : o.ToString();
                            _indexCollection[p.Name].SafeAdd(key, val);
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val).ToString();
                        _indexCollection[p.Name].SafeAdd(key, val);
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
            foreach (var prop in _indexedProps.Values)
            {
                var p = prop.Info;

                if (prop.IsUnique)
                {
                    if (prop.IsCollection || prop.IsDict)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        // its possible for a property to be specified as indexable, but be null.
                        if (objs == null) continue;

                        foreach (var o in objs)
                        {
                            var key = o.ToString();
                            if (_uniqueIndexCollection[p.Name].Remove(key)) continue;

                            IndexViolated?.Invoke(this, new IndexViolationEventArgs<T>(val, "Value was not found in index '" + p.Name + "' using key '" + key + "'"));
                            return false;
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val, null).ToString();
                        if (_uniqueIndexCollection[p.Name].Remove(key)) continue;

                        IndexViolated?.Invoke(this, new IndexViolationEventArgs<T>(val, "Value was not found in index '" + p.Name + "' using key '" + key + "'"));
                        return false;
                    }
                }
                else
                {
                    if (prop.IsCollection || prop.IsDict)
                    {
                        var objs = (IEnumerable)p.GetValue(val);
                        // its possible for a property to be specified as indexed, but be null.
                        if (objs == null) continue;

                        foreach (var o in objs)
                        {
                            var key = prop.IsDict ? o.GetType().GetProperty(nameof(KeyValuePair<string, T>.Value))?.GetValue(o).ToString() : o.ToString();
                            var idx = _indexCollection[p.Name];
                            if (key == null)
                                throw new ArgumentNullException(nameof(key), "Unable to retrieve the value of the KeyValuePair.");
                            if (idx[key].Count > 1)
                                idx[key].Remove(val);
                            else
                                idx.Remove(key);
                        }
                    }
                    else
                    {
                        var key = p.GetValue(val).ToString();
                        var idx = _indexCollection[p.Name];
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
            var indexed = attrs[0] as Indexed;
            return indexed != null && attrs.Length > 0 && indexed.IsUnique;
        }

        #endregion Private Methods

        #region Interface Implementations

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public void Clear()
        {
            foreach (var obj in _collection) obj.IndexedPropertyChanged -= onIndexedPropertyChanged;

            _collection.Clear();

            foreach (var p in _indexedProps.Values)
            {
                if (p.IsUnique)
                    _uniqueIndexCollection[p.Name].Clear();
                else
                    _indexCollection[p.Name].Clear();
            }
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.IndexedPropertyChanged -= onIndexedPropertyChanged;
            addToDictionaries(item);

            _collection.Insert(index, item);
            item.IndexedPropertyChanged += onIndexedPropertyChanged;
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => ((ICollection<T>)_collection).IsReadOnly;

        public T this[int index]
        {
            get
            {
                return _collection[index];
            }

            set
            {
                var item = _collection[index];

                removeFromDictionaries(item);
                item.IndexedPropertyChanged -= onIndexedPropertyChanged;

                _collection[index] = value;

                addToDictionaries(value);
                value.IndexedPropertyChanged += onIndexedPropertyChanged;
            }
        }

        #endregion Interface Implementations

        #region Linq

        public T First()
        {
            return _collection.First();
        }

        public T First(Func<T, bool> predicate)
        {
            return _collection.First(predicate);
        }

        public void Each(Action<T> action)
        {
            foreach (var item in _collection)
                action(item);
        }

        #endregion Linq

        #region Private classes

        private class PropInfo
        {
            public PropInfo(PropertyInfo info)
            {
                Info = info;
                IsUnique = isUnique(info);
                IsCollection = info.PropertyType.HasInterface<ICollection>();
                IsDict = info.PropertyType.HasInterface<IDictionary>();
            }

            public PropertyInfo Info { get; }

            public bool IsUnique { get; }

            public string Name => Info.Name;

            public bool IsDict { get; }

            public bool IsCollection { get; }
        }

        #endregion Private classes
    }
}