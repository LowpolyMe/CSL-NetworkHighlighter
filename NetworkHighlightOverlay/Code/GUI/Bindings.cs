using System;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public sealed class ToggleBinding
    {
        private readonly Func<bool> _get;
        private readonly Action<bool> _set;
        private readonly Func<Color> _getColor;
        private readonly Func<float> _getHue;
        private readonly Action<float> _setHue;

        public ToggleBinding(Func<bool> get, Action<bool> set)
            : this(get, set, null, null, null)
        {
        }

        public ToggleBinding(
            Func<bool> get,
            Action<bool> set,
            Func<Color> getColor,
            Func<float> getHue,
            Action<float> setHue)
        {
            _get = get ?? throw new ArgumentNullException(nameof(get));
            _set = set ?? throw new ArgumentNullException(nameof(set));
            _getColor = getColor;
            _getHue = getHue;
            _setHue = setHue;

            if ((_getHue == null) != (_setHue == null))
            {
                throw new ArgumentException("Hue get/set delegates must both be provided or both be null.");
            }
        }

        public bool Value
        {
            get => _get();
            set => _set(value);
        }

        public Color ColorValue => _getColor != null ? _getColor() : Color.white;

        public bool CanAdjustHue => _getHue != null && _setHue != null;

        public float HueValue
        {
            get
            {
                if (!CanAdjustHue)
                    throw new InvalidOperationException("Hue binding is not configured.");

                return _getHue();
            }
            set
            {
                if (!CanAdjustHue)
                    throw new InvalidOperationException("Hue binding is not configured.");

                _setHue(value);
            }
        }
    }
}
