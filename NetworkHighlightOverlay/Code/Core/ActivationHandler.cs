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
        private static readonly Observable<bool> _isActive = new Observable<bool>(false);

        private IDisposable _highlightRulesSubscription;
        private IDisposable _useUuiButtonSubscription;

        public static bool IsActive => _isActive.Value;

        public static IDisposable Subscribe(Action<bool, bool> callback) => _isActive.Subscribe(callback, true);

        public static void SetActive(bool isActive)
        {
            bool hasChanged = _isActive.SetValue(isActive);
            if (hasChanged)
            {
                if (isActive)
                {
                    Manager.Instance.OnActivated();
                }
                else
                {
                    Manager.Instance.OnDeactivated();
                }
            }

            SyncUuiPressedState(isActive);
        }

        private void Start()
        {
            _highlightRulesSubscription = ModSettings.HighlightRulesVersion.Subscribe(OnHighlightRulesChanged);
            _useUuiButtonSubscription = ModSettings.UseUuiButtonState.Subscribe(OnUseUuiButtonChanged, true);
        }

        private void Update()
        {
            if (!ModSettings.ToggleOverlayHotkey.IsKeyUp()) return;

            SetActive(!IsActive);
        }

        private void OnDestroy()
        {
            DisposeSubscriptions();
            UuiButtonController.UnregisterUui();
            SetActive(false);
        }

        private void OnHighlightRulesChanged(long previousVersion, long currentVersion)
        {
            Manager.Instance.OnHighlightRulesChanged();
        }

        private void OnUseUuiButtonChanged(bool previousValue, bool useUuiButton)
        {
            if (useUuiButton)
            {
                UuiButtonController.RegisterUui(SetActive);
                SyncUuiPressedState(IsActive);
                return;
            }

            UuiButtonController.UnregisterUui();
        }

        private static void SyncUuiPressedState(bool isActive)
        {
            if (!ModSettings.UseUuiButton) return;

            UuiButtonController.SetPressed(isActive);
        }

        private void DisposeSubscriptions()
        {
            if (_highlightRulesSubscription != null)
            {
                _highlightRulesSubscription.Dispose();
                _highlightRulesSubscription = null;
            }

            if (_useUuiButtonSubscription != null)
            {
                _useUuiButtonSubscription.Dispose();
                _useUuiButtonSubscription = null;
            }
        }
    }
}
