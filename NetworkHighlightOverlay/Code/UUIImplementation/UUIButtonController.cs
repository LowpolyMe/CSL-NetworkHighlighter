using UnityEngine;
using UnifiedUI.Helpers;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;

namespace NetworkHighlightOverlay.Code.UI
{
    public static class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";

        private static UUICustomButton _button;
        private static bool _syncing;
        private static bool _isSubscribedToManager;

        public static void RegisterUui()
        {
            SubscribeToManagerIsEnabledChanged();

            if (_button != null && _button.Button != null)
            {
                ApplyPressedState(Manager.Instance.IsEnabled);
                return;
            }

            Texture2D iconTexture = ModResources.LoadTexture("UUIIcon.png");

            _button = UUIHelpers.RegisterCustomButton(
                name: ButtonName,
                groupName: null,
                tooltip: "Toggle Network Highlight Overlay",
                icon: iconTexture,
                onToggle: OnButtonToggled,
                onToolChanged: null,
                hotkeys: null
            );

            ApplyPressedState(Manager.Instance.IsEnabled);
        }

        public static void UnregisterUui()
        {
            UnsubscribeFromManagerIsEnabledChanged();
        }

        private static void OnButtonToggled(bool isPressed)
        {
            if (_syncing)
            {
                return;
            }

            Manager.Instance.IsEnabled = isPressed;
        }

        private static void OnManagerIsEnabledChanged(bool isEnabled)
        {
            ApplyPressedState(isEnabled);
        }

        private static void ApplyPressedState(bool isEnabled)
        {
            if (_button == null)
            {
                return;
            }

            if (_button.IsPressed == isEnabled)
            {
                return;
            }

            _syncing = true;
            _button.IsPressed = isEnabled;
            _syncing = false;
        }

        private static void SubscribeToManagerIsEnabledChanged()
        {
            if (_isSubscribedToManager)
                return;

            Manager.Instance.IsEnabledChanged += OnManagerIsEnabledChanged;
            _isSubscribedToManager = true;
        }

        private static void UnsubscribeFromManagerIsEnabledChanged()
        {
            if (!_isSubscribedToManager)
                return;

            Manager.Instance.IsEnabledChanged -= OnManagerIsEnabledChanged;
            _isSubscribedToManager = false;
        }
    }
}
