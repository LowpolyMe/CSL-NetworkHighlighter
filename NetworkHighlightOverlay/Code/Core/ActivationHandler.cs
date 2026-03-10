using System;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class ActivationHandler : MonoBehaviour
    {
        private static ActivationHandler _instance;

        private Manager _manager;
        private ModSettings _settings;
        private UuiButtonController _uuiButtonController;
        private Action _highlightRulesChangedHandler;
        private Action _settingsChangedHandler;
        private bool _isActive;
        private bool _lastUseUuiButton;

        public event Action<bool> ActivationChanged;

        public bool IsActive => _isActive;

        public static ActivationHandler GetInstance() => _instance;

        public void Initialize(Manager manager, ModSettings settings, UuiButtonController uuiButtonController)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
            _settings = settings ?? throw new ArgumentNullException("settings");
            _uuiButtonController = uuiButtonController ?? throw new ArgumentNullException("uuiButtonController");
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
            {
                SyncUuiPressedState(isActive);
                return;
            }

            _isActive = isActive;
            if (isActive) _manager.OnActivated();
            else _manager.OnDeactivated();

            RaiseActivationChanged(isActive);
            SyncUuiPressedState(isActive);
        }

        public void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            _manager.RenderIfActive(cameraInfo);
        }

        public void HandleSegmentCreated(ushort segment, NetInfo info, bool creationSucceeded)
        {
            if (_manager == null || !creationSucceeded || segment == 0 || info == null || info.m_netAI == null)
                return;

            _manager.OnSegmentCreated(segment);
        }

        public void HandleSegmentReleased(ushort segment)
        {
            if (_manager == null || segment == 0)
                return;

            _manager.OnSegmentReleased(segment);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            EnsureInitialized();
            _highlightRulesChangedHandler = OnHighlightRulesChanged;
            _settings.HighlightRulesChanged += _highlightRulesChangedHandler;

            _settingsChangedHandler = OnSettingsChanged;
            _settings.SettingsChanged += _settingsChangedHandler;

            UpdateUuiRegistration(true);
        }

        private void Update()
        {
            if (!_settings.ToggleOverlayHotkey.IsKeyUp()) return;

            SetActive(!IsActive);
        }

        private void OnDestroy()
        {
            if (object.ReferenceEquals(_instance, this))
            {
                _instance = null;
            }

            SetActive(false);
            UnsubscribeFromSettingsEvents();
            _uuiButtonController?.UnregisterUui();
        }

        private void OnHighlightRulesChanged()
        {
            _manager.OnHighlightRulesChanged();
        }

        private void OnSettingsChanged()
        {
            UpdateUuiRegistration(false);
        }

        private void SyncUuiPressedState(bool isActive)
        {
            if (!_settings.UseUuiButton) return;

            _uuiButtonController.SetPressed(isActive);
        }

        private void UpdateUuiRegistration(bool forceSync)
        {
            bool useUuiButton = _settings.UseUuiButton;
            if (!forceSync && useUuiButton == _lastUseUuiButton)
                return;

            _lastUseUuiButton = useUuiButton;
            if (useUuiButton)
            {
                _uuiButtonController.RegisterUui(SetActive);
                SyncUuiPressedState(IsActive);
                return;
            }

            _uuiButtonController.UnregisterUui();
        }

        private void UnsubscribeFromSettingsEvents()
        {
            if (_settings != null && _highlightRulesChangedHandler != null)
            {
                _settings.HighlightRulesChanged -= _highlightRulesChangedHandler;
                _highlightRulesChangedHandler = null;
            }

            if (_settings != null && _settingsChangedHandler != null)
            {
                _settings.SettingsChanged -= _settingsChangedHandler;
                _settingsChangedHandler = null;
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
            if (_manager == null || _settings == null || _uuiButtonController == null)
                throw new InvalidOperationException("ActivationHandler.Initialize must be called before Start.");
        }
    }
}
