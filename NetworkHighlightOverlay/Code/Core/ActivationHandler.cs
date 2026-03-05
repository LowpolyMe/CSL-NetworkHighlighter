using System;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.UI;
using NetworkHighlightOverlay.Utility;
using UnifiedUI.Util;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class ActivationHandler : MonoBehaviour
    {
        private static ActivationHandler _instance;
        private readonly Observable<bool> _isActive = new Observable<bool>(false);

        private Manager _manager;
        private ModSettings _settings;
        private UuiButtonController _uuiButtonController;
        private IDisposable _highlightRulesSubscription;
        private IDisposable _useUuiButtonSubscription;

        public bool IsActive => _isActive.Value;

        public static ActivationHandler GetInstance() => _instance;

        public void Initialize(Manager manager, ModSettings settings, UuiButtonController uuiButtonController)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
            _settings = settings ?? throw new ArgumentNullException("settings");
            _uuiButtonController = uuiButtonController ?? throw new ArgumentNullException("uuiButtonController");
        }

        public IDisposable Subscribe(Action<bool, bool> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            return _isActive.Subscribe(callback, true);
        }

        public void SetActive(bool isActive)
        {
            if (_isActive.SetValue(isActive))
            {
                if (isActive) _manager.OnActivated();
                else _manager.OnDeactivated();
            }

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
            _highlightRulesSubscription = _settings.HighlightRulesVersion.Subscribe(OnHighlightRulesChanged);
            _useUuiButtonSubscription = _settings.UseUuiButtonState.Subscribe(OnUseUuiButtonChanged, true);
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

            DisposeSubscriptions();
            _uuiButtonController?.UnregisterUui();
            SetActive(false);
        }

        private void OnHighlightRulesChanged(long previousVersion, long currentVersion)
        {
            _manager.OnHighlightRulesChanged();
        }

        private void OnUseUuiButtonChanged(bool previousValue, bool useUuiButton)
        {
            if (useUuiButton)
            {
                _uuiButtonController.RegisterUui(SetActive);
                SyncUuiPressedState(IsActive);
                return;
            }

            _uuiButtonController.UnregisterUui();
        }

        private void SyncUuiPressedState(bool isActive)
        {
            if (!_settings.UseUuiButton) return;

            _uuiButtonController.SetPressed(isActive);
        }

        private void DisposeSubscriptions()
        {
            DisposeSubscription(ref _highlightRulesSubscription);
            DisposeSubscription(ref _useUuiButtonSubscription);
        }

        private static void DisposeSubscription(ref IDisposable subscription)
        {
            if (subscription == null)
                return;

            subscription.Dispose();
            subscription = null;
        }

        private void EnsureInitialized()
        {
            if (_manager == null || _settings == null || _uuiButtonController == null)
                throw new InvalidOperationException("ActivationHandler.Initialize must be called before Start.");
        }
    }
}
