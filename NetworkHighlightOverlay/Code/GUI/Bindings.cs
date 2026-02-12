using System;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public sealed class ToggleBinding
    {
        private readonly Observable<HighlightCategorySetting> _categoryState;
        private readonly Observable<float> _strengthState;

        public ToggleBinding(
            Observable<HighlightCategorySetting> categoryState,
            Observable<float> strengthState)
        {
            _categoryState = categoryState ?? throw new ArgumentNullException(nameof(categoryState));
            _strengthState = strengthState ?? throw new ArgumentNullException(nameof(strengthState));
        }

        public bool Value
        {
            get => _categoryState.Value.IsEnabled;
            set
            {
                HighlightCategorySetting currentValue = _categoryState.Value;
                if (currentValue.IsEnabled == value)
                    return;

                _categoryState.Value = currentValue.WithEnabled(value);
            }
        }

        public float HueValue
        {
            get => _categoryState.Value.Hue;
            set
            {
                HighlightCategorySetting currentValue = _categoryState.Value;
                if (Mathf.Approximately(currentValue.Hue, value))
                    return;

                _categoryState.Value = currentValue.WithHue(value);
            }
        }

        public Color ColorValue => ColorConversion.FromHue(_categoryState.Value.Hue, _strengthState.Value);

        public IDisposable Subscribe(Action callback)
        {
            return Subscribe(callback, false);
        }

        public IDisposable Subscribe(Action callback, bool notifyImmediately)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Action<HighlightCategorySetting, HighlightCategorySetting> onCategoryChanged =
                (previousValue, currentValue) => callback();
            Action<float, float> onStrengthChanged = (previousValue, currentValue) => callback();

            IDisposable categorySubscription = _categoryState.Subscribe(onCategoryChanged);
            IDisposable strengthSubscription = _strengthState.Subscribe(onStrengthChanged);

            if (notifyImmediately)
            {
                callback();
            }

            return new ToggleBindingSubscription(categorySubscription, strengthSubscription);
        }

        private sealed class ToggleBindingSubscription : IDisposable
        {
            private IDisposable _categorySubscription;
            private IDisposable _strengthSubscription;

            public ToggleBindingSubscription(
                IDisposable categorySubscription,
                IDisposable strengthSubscription)
            {
                _categorySubscription = categorySubscription;
                _strengthSubscription = strengthSubscription;
            }

            public void Dispose()
            {
                if (_categorySubscription != null)
                {
                    _categorySubscription.Dispose();
                    _categorySubscription = null;
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
