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

        public ToggleBinding(
            Func<bool> get,
            Action<bool> set,
            Func<Color> getColor,
            Func<float> getHue,
            Action<float> setHue)
        {
            _get = get ?? throw new ArgumentNullException(nameof(get));
            _set = set ?? throw new ArgumentNullException(nameof(set));
            _getColor = getColor ?? throw new ArgumentNullException(nameof(getColor));
            _getHue = getHue ?? throw new ArgumentNullException(nameof(getHue));
            _setHue = setHue ?? throw new ArgumentNullException(nameof(setHue));
        }

        public bool Value
        {
            get => _get();
            set => _set(value);
        }

        public Color ColorValue => _getColor();

        public float HueValue
        {
            get => _getHue();
            set => _setHue(value);
        }
    }
}
