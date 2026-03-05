using System;
using UnityEngine;
using UnifiedUI.Helpers;
using NetworkHighlightOverlay.Code.Utility;

namespace NetworkHighlightOverlay.Code.UI
{
    public sealed class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";
        private const string ToggleTooltip = "Toggle Network Highlight Overlay";

        private UUICustomButton _button;
        private Action<bool> _toggleRequested;

        public void RegisterUui(Action<bool> toggleRequested)
        {
            _toggleRequested = toggleRequested;

            if (_button != null && _button.Button != null) return;

            Texture2D iconTexture = ModResources.LoadTexture("UUIIcon.png");
            _button = UUIHelpers.RegisterCustomButton(
                name: ButtonName,
                groupName: null,
                tooltip: ToggleTooltip,
                icon: iconTexture,
                onToggle: OnButtonToggled,
                onToolChanged: null);
        }

        public void UnregisterUui()
        {
            if (_button != null && _button.Button != null)
            {
                UUIHelpers.Destroy(_button.Button);
            }

            _button = null;
            _toggleRequested = null;
        }

        private void OnButtonToggled(bool isPressed)
        {
            Action<bool> toggleRequested = _toggleRequested;
            if (toggleRequested == null) return;

            toggleRequested(isPressed);
        }

        public void SetPressed(bool isPressed)
        {
            if (_button == null || _button.IsPressed == isPressed) return;

            _button.IsPressed = isPressed;
        }
    }
}
