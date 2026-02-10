using System;

namespace NetworkHighlightOverlay.Code.GUI
{
        public sealed class ToggleBinding
        {
            private readonly Func<bool> _get;
            private readonly Action<bool> _set;
            
            public ToggleBinding(Func<bool> get, Action<bool> set)
            {
                _get = get ?? throw new ArgumentNullException(nameof(get));
                _set = set ?? throw new ArgumentNullException(nameof(set));
            }

            public bool Value
            {
                get => _get();
                set => _set(value);
            }
        }
}