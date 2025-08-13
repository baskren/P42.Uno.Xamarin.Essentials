using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Samples.Helpers;

public interface IMessagingCenter
{
    void Send<TSender, TArgs>(TSender sender, string message, TArgs args)
        where TSender : class;

    void Send<TSender>(TSender sender, string message)
        where TSender : class;

    void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender? source = null)
        where TSender : class;

    void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender? source = null)
        where TSender : class;

    void Unsubscribe<TSender, TArgs>(object subscriber, string message)
        where TSender : class;

    void Unsubscribe<TSender>(object subscriber, string message)
        where TSender : class;
}

public class MessagingCenter
    : IMessagingCenter
{
    public static IMessagingCenter Instance { get; } = new MessagingCenter();

    class Sender : Tuple<string, Type, Type?>
    {
        public Sender(string message, Type senderType, Type? argType)
            : base(message, senderType, argType)
        {
        }
    }

    delegate bool Filter(object sender);

    class MaybeWeakReference
    {
        WeakReference? DelegateWeakReference { get; }

        object? DelegateStrongReference { get; }

        readonly bool isStrongReference;

        public MaybeWeakReference(object subscriber, object? delegateSource)
        {
            if (subscriber.Equals(delegateSource))
            {
                // The target is the subscriber; we can use a weakreference
                DelegateWeakReference = new WeakReference(delegateSource);
                isStrongReference = false;
            }
            else
            {
                DelegateStrongReference = delegateSource;
                isStrongReference = true;
            }
        }

        public object? Target => isStrongReference ? DelegateStrongReference : DelegateWeakReference?.Target;

        public bool IsAlive => isStrongReference || (DelegateWeakReference?.IsAlive ?? false);
    }

    class Subscription : Tuple<WeakReference, MaybeWeakReference, MethodInfo, Filter>
    {
        public Subscription(object subscriber, object? delegateSource, MethodInfo methodInfo, Filter filter)
            : base(new WeakReference(subscriber), new MaybeWeakReference(subscriber, delegateSource), methodInfo, filter)
        {
        }

        public WeakReference Subscriber => Item1;

        MaybeWeakReference DelegateSource => Item2;

        MethodInfo MethodInfo => Item3;

        Filter Filter => Item4;

        public void InvokeCallback(object sender, object? args)
        {
            if (!Filter(sender))
            {
                return;
            }

            if (MethodInfo.IsStatic)
            {
                MethodInfo.Invoke(null, MethodInfo.GetParameters().Length == 1 ? [sender] : [sender, args]);
                return;
            }

            var target = DelegateSource.Target;

            if (target == null)
            {
                return; // Collected
            }

            MethodInfo.Invoke(target, MethodInfo.GetParameters().Length == 1 ? [sender] : [sender, args]);
        }

        public bool CanBeRemoved()
        {
            return !Subscriber.IsAlive || !DelegateSource.IsAlive;
        }
    }

    readonly Dictionary<Sender, List<Subscription>> subscriptions = [];

    public static void Send<TSender, TArgs>(TSender sender, string message, TArgs args)
        where TSender : class
    {
        Instance.Send(sender, message, args);
    }

    public static void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender? source = null)
        where TSender : class
    {
        Instance.Subscribe(subscriber, message, callback, source);
    }

    public static void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender? source = null)
        where TSender : class
    {
        Instance.Subscribe(subscriber, message, callback, source);
    }

    public static void Unsubscribe<TSender, TArgs>(object subscriber, string message)
        where TSender : class
    {
        Instance.Unsubscribe<TSender, TArgs>(subscriber, message);
    }

    public static void Unsubscribe<TSender>(object subscriber, string message)
        where TSender : class
    {
        Instance.Unsubscribe<TSender>(subscriber, message);
    }

    public static void Send<TSender>(TSender sender, string message)
        where TSender : class
    {
        Instance.Send(sender, message);
    }

    void IMessagingCenter.Send<TSender, TArgs>(TSender sender, string message, TArgs args)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        InnerSend(message, typeof(TSender), typeof(TArgs), sender, args);
    }

    void IMessagingCenter.Send<TSender>(TSender sender, string message)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        InnerSend(message, typeof(TSender), null, sender, null);
    }

    void IMessagingCenter.Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        var target = callback.Target;

        bool filter(object sender)
        {
            var send = (TSender)sender;
            return source == null || send == source;
        }

        InnerSubscribe(subscriber, message, typeof(TSender), typeof(TArgs), target, callback.GetMethodInfo(), filter);
    }

    void IMessagingCenter.Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        var target = callback.Target;

        bool filter(object sender)
        {
            var send = (TSender)sender;
            return source == null || send == source;
        }

        InnerSubscribe(subscriber, message, typeof(TSender), null, target, callback.GetMethodInfo(), filter);
    }

    void IMessagingCenter.Unsubscribe<TSender, TArgs>(object subscriber, string message)
    {
        InnerUnsubscribe(message, typeof(TSender), typeof(TArgs), subscriber);
    }

    void IMessagingCenter.Unsubscribe<TSender>(object subscriber, string message)
    {
        InnerUnsubscribe(message, typeof(TSender), null, subscriber);
    }

    void InnerSend(string message, Type senderType, Type? argType, object sender, object? args)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var key = new Sender(message, senderType, argType);
        if (!subscriptions.TryGetValue(key, out List<Subscription>? value))
            return;
        var subcriptions = value;
        if (subcriptions == null || !subcriptions.Any())
            return; // should not be reachable

        // ok so this code looks a bit funky but here is the gist of the problem. It is possible that in the course
        // of executing the callbacks for this message someone will subscribe/unsubscribe from the same message in
        // the callback. This would invalidate the enumerator. To work around this we make a copy. However if you unsubscribe
        // from a message you can fairly reasonably expect that you will therefor not receive a call. To fix this we then
        // check that the item we are about to send the message to actually exists in the live list.
        var subscriptionsCopy = subcriptions.ToList();
        foreach (var subscription in subscriptionsCopy)
        {
            if (subscription.Subscriber.Target != null && subcriptions.Contains(subscription))
            {
                subscription.InvokeCallback(sender, args);
            }
        }
    }

    void InnerSubscribe(object subscriber, string message, Type senderType, Type? argType, object? target, MethodInfo methodInfo, Filter filter)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var key = new Sender(message, senderType, argType);
        var value = new Subscription(subscriber, target, methodInfo, filter);
        if (subscriptions.TryGetValue(key, out List<Subscription>? sublist))
        {
            sublist.Add(value);
        }
        else
        {
            var list = new List<Subscription> { value };
            subscriptions[key] = list;
        }
    }

    void InnerUnsubscribe(string message, Type senderType, Type? argType, object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var key = new Sender(message, senderType, argType);
        if (!subscriptions.TryGetValue(key, out List<Subscription>? value))
            return;
        value.RemoveAll(sub => sub.CanBeRemoved() || sub.Subscriber.Target == subscriber);
        if (value.Count == 0)
            subscriptions.Remove(key);
    }

    // This is a bit gross; it only exists to support the unit tests in PageTests
    // because the implementations of ActionSheet, Alert, and IsBusy are all very
    // tightly coupled to the MessagingCenter singleton
    internal static void ClearSubscribers()
    {
        (Instance as MessagingCenter)?.subscriptions.Clear();
    }
}
