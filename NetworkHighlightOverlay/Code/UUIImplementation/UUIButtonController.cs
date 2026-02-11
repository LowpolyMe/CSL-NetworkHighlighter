using UnityEngine;
using UnifiedUI.Helpers;
using NetworkHighlightOverlay.Code.Core;
using System;
using NetworkHighlightOverlay.Code.Utility;

namespace NetworkHighlightOverlay.Code.UI
{
    public static class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";

        private static UUICustomButton _button;
        private static bool _syncing;
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
            DisposeEnabledStateSubscription();
        }

        private static void OnButtonToggled(bool isPressed)
        {
            if (_syncing)
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
