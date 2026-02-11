using System;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public sealed class ToggleBinding
    {
        private readonly Observable<bool> _enabledState;
        private readonly Observable<float> _hueState;
        private readonly Observable<float> _strengthState;

        public ToggleBinding(
            Observable<bool> enabledState,
            Observable<float> hueState,
            Observable<float> strengthState)
        {
            _enabledState = enabledState ?? throw new ArgumentNullException(nameof(enabledState));
            _hueState = hueState ?? throw new ArgumentNullException(nameof(hueState));
            _strengthState = strengthState ?? throw new ArgumentNullException(nameof(strengthState));
        }

        public Observable<bool> EnabledState => _enabledState;
        public Observable<float> HueState => _hueState;
        public Observable<float> StrengthState => _strengthState;

        public bool Value
        {
            get => _enabledState.Value;
            set => _enabledState.Value = value;
        }

        public float HueValue
        {
            get => _hueState.Value;
            set => _hueState.Value = value;
        }

        public Color ColorValue => ColorConversion.FromHue(_hueState.Value, _strengthState.Value);

        public IDisposable Subscribe(Action callback)
        {
            return Subscribe(callback, false);
        }

        public IDisposable Subscribe(Action callback, bool notifyImmediately)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Action<bool, bool> onEnabledChanged = (previousValue, currentValue) => callback();
            Action<float, float> onHueChanged = (previousValue, currentValue) => callback();
            Action<float, float> onStrengthChanged = (previousValue, currentValue) => callback();

            IDisposable enabledSubscription = _enabledState.Subscribe(onEnabledChanged);
            IDisposable hueSubscription = _hueState.Subscribe(onHueChanged);
            IDisposable strengthSubscription = _strengthState.Subscribe(onStrengthChanged);

            if (notifyImmediately)
            {
                callback();
            }

            return new ToggleBindingSubscription(enabledSubscription, hueSubscription, strengthSubscription);
        }

        private sealed class ToggleBindingSubscription : IDisposable
        {
            private IDisposable _enabledSubscription;
            private IDisposable _hueSubscription;
            private IDisposable _strengthSubscription;

            public ToggleBindingSubscription(
                IDisposable enabledSubscription,
                IDisposable hueSubscription,
                IDisposable strengthSubscription)
            {
                _enabledSubscription = enabledSubscription;
                _hueSubscription = hueSubscription;
                _strengthSubscription = strengthSubscription;
            }

            public void Dispose()
            {
                if (_enabledSubscription != null)
                {
                    _enabledSubscription.Dispose();
                    _enabledSubscription = null;
                }

                if (_hueSubscription != null)
                {
                    _hueSubscription.Dispose();
                    _hueSubscription = null;
                }

                if (_strengthSubscription != null)
                {
                    _strengthSubscription.Dispose();
                    _strengthSubscription = null;
                }
            }
        }
    }
}
