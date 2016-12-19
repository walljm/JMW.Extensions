using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JMW.Types
{
    // Shamelessly taken from: https://github.com/thomaslevesque/WeakEvent
    public class WeakEventSource<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly List<WeakDelegate> _handlers;
        public int HandlerCount => _handlers.Count;

        public WeakEventSource()
        {

            _handlers = new List<WeakDelegate>();
        }

        public void Clear()
        {
            lock (_handlers)
            {
                _handlers.Clear();
            }
        }

        public void Raise(object sender, TEventArgs e)
        {
            lock (_handlers)
            {
                _handlers.RemoveAll(h => !h.Invoke(sender, e));
            }
        }

        public void Subscribe(EventHandler<TEventArgs> handler)
        {
            var weakHandlers = handler
                .GetInvocationList()
                .Select(d => new WeakDelegate(d))
                .ToList();

            lock (_handlers)
            {
                _handlers.AddRange(weakHandlers);
            }
        }

        public void Unsubscribe(EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
            {
                int index = _handlers.FindIndex(h => h.IsMatch(handler));

                if (index >= 0)
                    _handlers.RemoveAt(index);
            }
        }

        private class WeakDelegate
        {
            #region Open handler generation and cache

            private delegate void OpenEventHandler(object target, object sender, TEventArgs e);

            // ReSharper disable once StaticMemberInGenericType (by design)
            private static readonly ConcurrentDictionary<MethodInfo, OpenEventHandler> _openHandlerCache =
                new ConcurrentDictionary<MethodInfo, OpenEventHandler>();

            private static OpenEventHandler CreateOpenHandler(MethodInfo method)
            {
                var target = Expression.Parameter(typeof(object), "target");
                var sender = Expression.Parameter(typeof(object), "sender");
                var e = Expression.Parameter(typeof(TEventArgs), "e");

                if (method.IsStatic)
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
                else
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            Expression.Convert(target, method.DeclaringType),
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
            }

            #endregion Open handler generation and cache

            private readonly WeakReference _weakTarget;
            private readonly MethodInfo _method;
            private readonly OpenEventHandler _openHandler;

            public WeakDelegate(Delegate handler)
            {
                _weakTarget = handler.Target != null ? new WeakReference(handler.Target) : null;
                _method = handler.Method;
                _openHandler = _openHandlerCache.GetOrAdd(_method, CreateOpenHandler);
            }

            public bool Invoke(object sender, TEventArgs e)
            {
                object target = null;
                if (_weakTarget != null)
                {
                    target = _weakTarget.Target;
                    if (target == null)
                        return false;
                }
                _openHandler(target, sender, e);
                return true;
            }

            public bool IsMatch(EventHandler<TEventArgs> handler)
            {
                if (_weakTarget == null && !_method.IsStatic) return false;
                if (_method.IsStatic)
                {
                    return handler.Method.Equals(_method);
                }
                return ReferenceEquals(handler.Target, _weakTarget.Target)
                    && handler.Method.Equals(_method);
            }
        }
    }
}