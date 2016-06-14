/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using JMW.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JMW.Types.Collections
{
    /// <summary>
    /// <para>
    /// This class serves as a base class for classes that can be indexed by <see cref="IndexedCollection{T}"/>.
    /// </para>
    ///
    /// <para>
    /// The purpose of the class is to implement the <see cref="INotifyPropertyChanged"/> and
    /// <see cref="INotifyPropertyChanging"/> interfaces, to implement an <see cref="IndexedPropertyChanged"/>
    /// event and create helper methods for setting the value so that all the proper events are fired.
    /// </para>
    /// 
    /// <para>This class is used in conjunction with the <see cref="IndexedCollection{T}"/> class to allow for an in
    /// memory indexed lookup table.</para>
    /// 
    /// <para>The only caveat here is that you must add the <see cref="Indexed"/> attribute to the appropriate
    /// property and use the Set method to handle setting props to trigger the event.</para>
    /// </summary>
    public abstract class IndexedClass : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// The signature of the <see cref="IndexedPropertyChangedHandler"/> used by the <see cref="IndexedPropertyChanged"/>
        /// event.
        /// </summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="IndexedPropertyChangedEventArgs"/></param>
        public delegate void IndexedPropertyChangedHandler(object sender, IndexedPropertyChangedEventArgs e);

        #region Properties

        /// <summary>
        /// The number of registered handlers for the event.
        /// </summary>
        public int PropertyChangingEventCount { get { return _PropertyChanging.HandlerCount; } }

        /// <summary>
        /// The number of registered handlers for the event.
        /// </summary>
        public int PropertyChangedEventCount { get { return _PropertyChanged.HandlerCount; } }

        /// <summary>
        /// The number of registered handlers for the event.
        /// </summary>
        public int IndexedPropertyChangedEventCount { get { return _IndexedPropertyChanged.HandlerCount; } }

        #endregion Properties

        #region Public Methods

        /// <summary>
        ///   Clears all the events attached to the object.
        ///
        /// <para>Specifically IndexedPropertyChanged, PropertyChanged, and PropertyChanging</para>
        /// </summary>
        public void ClearEvents()
        {
            _IndexedPropertyChanged.Clear();
            _PropertyChanged.Clear();
            _PropertyChanging.Clear();
        }

        /// <summary>
        /// Clears the IndexedPropertyChanged events.
        /// </summary>
        public void ClearIndexedPropertyChanged()
        {
            _IndexedPropertyChanged.Clear();
        }

        /// <summary>
        /// Clears the PropertyChanged events.
        /// </summary>
        public void ClearPropertyChanged()
        {
            _PropertyChanged.Clear();
        }

        /// <summary>
        /// Clears the PropertyChanging events.
        /// </summary>
        public void ClearPropertyChanging()
        {
            _PropertyChanging.Clear();
        }

        #endregion Public Methods

        #region Events

        private WeakEventSource<IndexedPropertyChangedEventArgs> _IndexedPropertyChanged = new WeakEventSource<IndexedPropertyChangedEventArgs>();
        /// <summary>
        /// Fires when a property has changed.  This event will give you the before and after value of the
        /// property, as well as the property name.
        ///
        /// <para>Event references are weak.  Events only fire if the connected
        /// objects have not been collected by the GC.  <see cref="WeakReference"/>'s don't prevent
        /// the GC from collecting the objects if there are no more references to them.</para>
        /// </summary>
        public event IndexedPropertyChangedHandler IndexedPropertyChanged
        {
            add { _IndexedPropertyChanged.Subscribe(value.CastDelegate<EventHandler<IndexedPropertyChangedEventArgs>>()); }
            remove { _IndexedPropertyChanged.Unsubscribe(value.CastDelegate<EventHandler<IndexedPropertyChangedEventArgs>>()); }
        }

        private WeakEventSource<PropertyChangingEventArgs> _PropertyChanging = new WeakEventSource<PropertyChangingEventArgs>();
        /// <summary>
        /// The <see cref="INotifyPropertyChanging"/> event.  Gives you the property name before
        /// the value is changed.
        ///
        /// <para>Event references are weak.  Events only fire if the connected
        /// objects have not been collected by the GC.  <see cref="WeakReference"/>'s don't prevent
        /// the GC from collecting the objects if there are no more references to them.</para>
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { _PropertyChanging.Subscribe(value.CastDelegate<EventHandler<PropertyChangingEventArgs>>()); }
            remove { _PropertyChanging.Unsubscribe(value.CastDelegate<EventHandler<PropertyChangingEventArgs>>()); }
        }

        private WeakEventSource<PropertyChangedEventArgs> _PropertyChanged = new WeakEventSource<PropertyChangedEventArgs>();
        /// <summary>
        /// The <see cref="INotifyPropertyChanged"/> event.  Gives you the property name after
        /// the value has changed.
        /// <para>Event references are weak.  Events only fire if the connected
        /// objects have not been collected by the GC.  <see cref="WeakReference"/>'s don't prevent
        /// the GC from collecting the objects if there are no more references to them.</para>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _PropertyChanged.Subscribe(value.CastDelegate<EventHandler<PropertyChangedEventArgs>>()); }
            remove { _PropertyChanged.Unsubscribe(value.CastDelegate<EventHandler<PropertyChangedEventArgs>>()); }
        }

        #endregion Events

        /// <summary>
        /// Sets a value and fires the appropriate events.
        /// </summary>
        /// <typeparam name="T">The type of the Property</typeparam>
        /// <param name="field">A reference to the field being set.  This is usually the private backer of the property.</param>
        /// <param name="value">The value to set the field to.</param>
        /// <param name="property_name">The name of the Property being set.  This should be provided automatically
        /// using the <see cref="CallerMemberNameAttribute"/> via the magic of C# 6.
        /// </param>
        /// <returns>true/false indicating the success of the operation.  false if the field is null or the value is the same</returns>
        protected bool Set<T>(ref T field, T value, [CallerMemberName]string property_name = "")
        {
            raisePropertyChanging(property_name);
            if (field == null || EqualityComparer<T>.Default.Equals(field, value)) { return false; }
            raiseIndexedPropertyChanging(property_name, field, value);
            field = value;
            raisePropertyChanged(property_name);
            return true;
        }

        #region Private Methods

        private void raiseIndexedPropertyChanging(string prop_name, object before, object after)
        {
            _IndexedPropertyChanged.Raise(this, new IndexedPropertyChangedEventArgs(prop_name, before, after));
        }

        private void raisePropertyChanged(string prop)
        {
            _PropertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }

        private void raisePropertyChanging(string prop)
        {
            _PropertyChanging.Raise(this, new PropertyChangingEventArgs(prop));
        }

        #endregion Private Methods
    }
}