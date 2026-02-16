using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NetworkHighlightOverlay.Utility
{
    public sealed class Observable<T>
    {
        private readonly object _sync = new object();
        private static readonly Func<T, T, bool> _areEqual = CreateEquality();
        private T _value;
        private Action<T, T> _valueChanged;

        public Observable(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get
            {
                lock (_sync)
                {
                    return _value;
                }
            }
            set => SetValue(value);
        }

        public bool SetValue(T value)
        {
            T oldValue;
            Action<T, T> callbacks;
            lock (_sync)
            {
                if (_areEqual(_value, value))
                    return false;

                oldValue = _value;
                _value = value;
                callbacks = _valueChanged;
            }

            InvokeCallbacks(callbacks, oldValue, value);
            return true;
        }

        public bool Update(Func<T, T> updater)
        {
            if (updater == null)
                throw new ArgumentNullException(nameof(updater));

            // Keep updater execution outside the lock; contention can cause retries.
            while (true)
            {
                T oldValue;
                lock (_sync)
                {
                    oldValue = _value;
                }

                T newValue = updater(oldValue);
                Action<T, T> callbacks;
                lock (_sync)
                {
                    if (!_areEqual(_value, oldValue))
                        continue;

                    if (_areEqual(oldValue, newValue))
                        return false;

                    _value = newValue;
                    callbacks = _valueChanged;
                }

                InvokeCallbacks(callbacks, oldValue, newValue);
                return true;
            }
        }

        public IDisposable Subscribe(Action<T, T> callback) => Subscribe(callback, false);

        public IDisposable Subscribe(Action<T, T> callback, bool notifyImmediately)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            T currentValue = default(T);
            bool shouldNotifyImmediately = false;
            lock (_sync)
            {
                _valueChanged += callback;
                if (notifyImmediately)
                {
                    currentValue = _value;
                    shouldNotifyImmediately = true;
                }
            }

            if (shouldNotifyImmediately)
            {
                callback(currentValue, currentValue);
            }

            return new Subscription(this, callback);
        }

        private static void InvokeCallbacks(Action<T, T> callbacks, T oldValue, T newValue)
        {
            if (callbacks != null)
            {
                callbacks(oldValue, newValue);
            }
        }

        private static bool AreEqualByDefault(T left, T right) => EqualityComparer<T>.Default.Equals(left, right);

        private static Func<T, T, bool> CreateEquality()
        {
            if (typeof(T) == typeof(float)) return AreEqualAsFloat;

            return AreEqualByDefault;
        }

        private static bool AreEqualAsFloat(T left, T right)
        {
            float leftValue = (float)(object)left;
            float rightValue = (float)(object)right;
            return Mathf.Approximately(leftValue, rightValue);
        }

        private void Unsubscribe(Action<T, T> callback)
        {
            lock (_sync)
            {
                _valueChanged -= callback;
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Observable<T> _owner;
            private Action<T, T> _callback;

            public Subscription(Observable<T> owner, Action<T, T> callback)
            {
                _owner = owner;
                _callback = callback;
            }

            public void Dispose()
            {
                Observable<T> owner = Interlocked.Exchange(ref _owner, null);
                Action<T, T> callback = Interlocked.Exchange(ref _callback, null);
                if (owner == null || callback == null)
                    return;

                owner.Unsubscribe(callback);
            }
        }
    }
}
