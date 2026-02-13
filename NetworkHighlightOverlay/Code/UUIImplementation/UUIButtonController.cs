using System;
using UnityEngine;
using UnifiedUI.Helpers;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;

namespace NetworkHighlightOverlay.Code.UI
{
    public static class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";
        private const string ToggleTooltip = "Toggle Network Highlight Overlay";

        private static UUICustomButton _button;
        private static IDisposable _enabledStateSubscription;

        public static void RegisterUui()
        {
            SubscribeToEnabledState();

            if (_button != null && _button.Button != null)
            {
                ApplyPressedState(Manager.Instance.IsEnabled);
                return;
            }

            Texture2D iconTexture = ModResources.LoadTexture("UUIIcon.png");
            _button = UUIHelpers.RegisterCustomButton(
                name: ButtonName,
                groupName: null,
                tooltip: ToggleTooltip,
                icon: iconTexture,
                onToggle: OnButtonToggled,
                onToolChanged: null,
                hotkeys: null);

            ApplyPressedState(Manager.Instance.IsEnabled);
        }

        public static void UnregisterUui()
        {
            DisposeEnabledStateSubscription();
            if (_button != null && _button.Button != null)
            {
                UUIHelpers.Destroy(_button.Button);
            }
            _button = null;
        }

        private static void OnButtonToggled(bool isPressed)
        {
            if (Manager.Instance.IsEnabled == isPressed)
            {
                return;
            }

            Manager.Instance.IsEnabled = isPressed;
        }

        private static void OnEnabledStateChanged(bool previousState, bool isEnabled)
        {
            ApplyPressedState(isEnabled);
        }

        private static void ApplyPressedState(bool isEnabled)
        {
            if (_button == null || _button.IsPressed == isEnabled)
            {
                return;
            }

            _button.IsPressed = isEnabled;
        }

        private static void SubscribeToEnabledState()
        {
            if (_enabledStateSubscription != null)
                return;

            _enabledStateSubscription = Manager.Instance.EnabledState.Subscribe(OnEnabledStateChanged, true);
        }

        private static void DisposeEnabledStateSubscription()
        {
            if (_enabledStateSubscription == null)
                return;

            _enabledStateSubscription.Dispose();
            _enabledStateSubscription = null;
        }
    }
}
