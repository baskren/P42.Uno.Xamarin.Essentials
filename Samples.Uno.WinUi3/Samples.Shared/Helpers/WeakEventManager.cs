using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xamarin.Essentials
{
    class WeakEventManager
    {
        struct Subscription
        {
            public readonly WeakReference Subscriber;

            public readonly MethodInfo Handler;

            public Subscription(WeakReference subscriber, MethodInfo handler)
            {
                Subscriber = subscriber;
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }
        }

        private readonly Dictionary<string, List<Subscription>> _eventHandlers = new Dictionary<string, List<Subscription>>();

        public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null)
            where TEventArgs : EventArgs
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        public void HandleEvent(object sender, object args, string eventName)
        {
            var list = new List<(object, MethodInfo)>();
            var list2 = new List<Subscription>();
            if (_eventHandlers.TryGetValue(eventName, out var value))
            {
                for (var i = 0; i < value.Count; i++)
                {
                    var item = value[i];
                    if (item.Subscriber == null)
                    {
                        list.Add((null, item.Handler));
                        continue;
                    }

                    var target = item.Subscriber.Target;
                    if (target == null)
                    {
                        list2.Add(item);
                    }
                    else
                    {
                        list.Add((target, item.Handler));
                    }
                }

                for (var j = 0; j < list2.Count; j++)
                {
                    var item2 = list2[j];
                    value.Remove(item2);
                }
            }

            for (var k = 0; k < list.Count; k++)
            {
                var tuple = list[k];
                var item3 = tuple.Item1;
                tuple.Item2.Invoke(item3, new object[2]
                {
                    sender,
                    args
                });
            }
        }

        public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null)
            where TEventArgs : EventArgs
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        public void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        private void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var value))
            {
                value = new List<Subscription>();
                _eventHandlers.Add(eventName, value);
            }

            if (handlerTarget == null)
            {
                value.Add(new Subscription(null, methodInfo));
            }
            else
            {
                value.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
            }
        }

        private void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var value))
            {
                return;
            }

            for (var num = value.Count; num > 0; num--)
            {
                var item = value[num - 1];
                if (item.Subscriber?.Target == handlerTarget && !(item.Handler.Name != methodInfo.Name))
                {
                    value.Remove(item);
                    break;
                }
            }
        }
    }
}
