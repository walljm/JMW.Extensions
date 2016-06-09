using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JMW.Types.Collections
{
    public abstract class IndexedClass : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public delegate void IndexedPropertyChangedHandler(object sender, IndexedPropertyEventArgs e);

        private WeakEventSource<IndexedPropertyEventArgs> _IndexedPropertyChanged = new WeakEventSource<IndexedPropertyEventArgs>();
        public event IndexedPropertyChangedHandler IndexedPropertyChanged
        {
            add { _IndexedPropertyChanged.Subscribe(value.CastDelegate<EventHandler<IndexedPropertyEventArgs>>()); }
            remove { _IndexedPropertyChanged.Unsubscribe(value.CastDelegate<EventHandler<IndexedPropertyEventArgs>>()); }
        }

        private WeakEventSource<PropertyChangingEventArgs> _PropertyChanging = new WeakEventSource<PropertyChangingEventArgs>();
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { _PropertyChanging.Subscribe(value.CastDelegate<EventHandler<PropertyChangingEventArgs>>()); }
            remove { _PropertyChanging.Unsubscribe(value.CastDelegate<EventHandler<PropertyChangingEventArgs>>()); }
        }

        private WeakEventSource<PropertyChangedEventArgs> _PropertyChanged = new WeakEventSource<PropertyChangedEventArgs>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _PropertyChanged.Subscribe(value.CastDelegate<EventHandler<PropertyChangedEventArgs>>()); }
            remove { _PropertyChanged.Unsubscribe(value.CastDelegate<EventHandler<PropertyChangedEventArgs>>()); }
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName]string property_name = "")
        {
            RaisePropertyChanging(property_name);
            if (field == null || EqualityComparer<T>.Default.Equals(field, value)) { return false; }
            RaiseIndexedPropertyChanging(property_name, field, value);
            field = value;
            RaisePropertyChanged(property_name);
            return true;
        }

        private void RaiseIndexedPropertyChanging(string prop_name, object before, object after)
        {
            _IndexedPropertyChanged.Raise(this, new IndexedPropertyEventArgs(prop_name, before, after));
        }

        private void RaisePropertyChanged(string prop)
        {
            _PropertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }

        private void RaisePropertyChanging(string prop)
        {
            _PropertyChanging.Raise(this, new PropertyChangingEventArgs(prop));
        }
    }
}