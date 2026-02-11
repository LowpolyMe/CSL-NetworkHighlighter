using System;
using System.Collections.Generic;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class Observable<T>
    {
        private T _value;
        private event Action<T, T> _valueChanged;

        public Observable(T initialValue)
        {
            _value = initialValue;
        }

        public event Action<T, T> ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        public T Value
        {
            get { return _value; }
            set { SetValue(value); }
        }

        public bool SetValue(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return false;

            T oldValue = _value;
            _value = value;
            Notify(oldValue, _value);
            return true;
        }

        public bool Update(Func<T, T> updater)
        {
            if (updater == null)
                throw new ArgumentNullException("updater");

            return SetValue(updater(_value));
        }

        public IDisposable Subscribe(Action<T, T> callback)
        {
            return Subscribe(callback, false);
        }

        public IDisposable Subscribe(Action<T, T> callback, bool notifyImmediately)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _valueChanged += callback;
            if (notifyImmediately)
            {
                callback(_value, _value);
            }

            return new Subscription(this, callback);
        }

        public void ForceNotify()
        {
            Notify(_value, _value);
        }

        private void Notify(T oldValue, T newValue)
        {
            Action<T, T> callbacks = _valueChanged;
            if (callbacks != null)
            {
                callbacks(oldValue, newValue);
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
                if (_owner == null || _callback == null)
                    return;

                _owner._valueChanged -= _callback;
                _owner = null;
                _callback = null;
            }
        }
    }
}
