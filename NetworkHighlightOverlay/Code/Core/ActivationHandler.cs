using System;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class ActivationHandler : MonoBehaviour
    {
        private Manager _manager;
        private ModSettings _settings;
        private Action _highlightRulesChangedHandler;
        private bool _isActive;

        public event Action<bool> ActivationChanged;

        public bool IsActive => _isActive;

        public void Initialize(Manager manager, ModSettings settings)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
            _settings = settings ?? throw new ArgumentNullException("settings");
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
            {
                return;
            }

            _isActive = isActive;
            if (isActive) _manager.OnActivated();
            else _manager.OnDeactivated();

            RaiseActivationChanged(isActive);
        }

        private void Start()
        {
            EnsureInitialized();
            _highlightRulesChangedHandler = OnHighlightRulesChanged;
            _settings.HighlightRulesChanged += _highlightRulesChangedHandler;
        }

        private void Update()
        {
            if (!_settings.ToggleOverlayHotkey.IsKeyUp()) return;

            SetActive(!IsActive);
        }

        private void OnDestroy()
        {
            SetActive(false);
            UnsubscribeFromSettingsEvents();
        }

        private void OnHighlightRulesChanged()
        {
            _manager.OnHighlightRulesChanged();
        }

        private void UnsubscribeFromSettingsEvents()
        {
            if (_settings != null && _highlightRulesChangedHandler != null)
            {
                _settings.HighlightRulesChanged -= _highlightRulesChangedHandler;
                _highlightRulesChangedHandler = null;
            }
        }

        private void RaiseActivationChanged(bool isActive)
        {
            Action<bool> activationChanged = ActivationChanged;
            if (activationChanged != null)
            {
                activationChanged(isActive);
            }
        }

        private void EnsureInitialized()
        {
            if (_manager == null || _settings == null)
                throw new InvalidOperationException("ActivationHandler.Initialize must be called before Start.");
        }
    }
}
