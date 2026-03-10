using System;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.ModOptions;
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
        private ModSettings _settings;
        private ActivationHandler _activationHandler;
        private Action _settingsChangedHandler;
        private Action<bool> _activationChangedHandler;
        private bool _lastUseUuiButton;

        public void Initialize(ModSettings settings, ActivationHandler activationHandler)
        {
            _settings = settings ?? throw new ArgumentNullException("settings");
            _activationHandler = activationHandler ?? throw new ArgumentNullException("activationHandler");

            _settingsChangedHandler = SyncRegistration;
            _settings.SettingsChanged += _settingsChangedHandler;

            _activationChangedHandler = SetPressed;
            _activationHandler.ActivationChanged += _activationChangedHandler;

            SyncRegistration();
        }

        public void Dispose()
        {
            if (_activationHandler != null && _activationChangedHandler != null)
            {
                _activationHandler.ActivationChanged -= _activationChangedHandler;
                _activationChangedHandler = null;
            }

            if (_settings != null && _settingsChangedHandler != null)
            {
                _settings.SettingsChanged -= _settingsChangedHandler;
                _settingsChangedHandler = null;
            }

            UnregisterUui();
            _activationHandler = null;
            _settings = null;
        }

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

        private void SyncRegistration()
        {
            bool useUuiButton = _settings.UseUuiButton;
            if (useUuiButton == _lastUseUuiButton)
                return;

            _lastUseUuiButton = useUuiButton;
            if (useUuiButton)
            {
                RegisterUui(_activationHandler.SetActive);
                SetPressed(_activationHandler.IsActive);
                return;
            }

            UnregisterUui();
        }
    }
}
