using HarmonyLib;
using System;
using ICities;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;
using NetworkHighlightOverlay.Code.UI;
using NetworkHighlightOverlay.Code.GUI;
using NetworkHighlightOverlay.Code.ModOptions;

namespace NetworkHighlightOverlay.Code.Lifecycle
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _controllerObject;
        private Harmony _harmony;
        private IDisposable _useUuiSubscription;
        private static bool _patched;
        
        private const string HarmonyId = "com.lowpolyme.NetworkHighlightOverlay";

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            PatchHarmony();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            UnpatchHarmony();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CreateRendererObject();
            Manager.Instance.RebuildCache();
            SubscribeToUuiSetting();
            TogglePanel.Create();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            DisposeUuiSettingSubscription();
            UuiButtonController.UnregisterUui();
            TogglePanel.Destroy();
            ToggleButtonAtlas.Clear();
            DestroyRendererObject();
            Manager.Instance.Clear();
        }

        #region Helpers

        private void PatchHarmony()
        {
            if (_patched)
                return;
            
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll();

            _patched = true;
        }

        private void UnpatchHarmony()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;
            }

            _patched = false;
        }

        private void CreateRendererObject()
        {
            if (_controllerObject != null) return;
            
            _controllerObject = new GameObject("PathHighlightRenderer");
            _controllerObject.AddComponent<ActivationHandler>();
            GameObject.DontDestroyOnLoad(_controllerObject);
        }

        private void DestroyRendererObject()
        {
            if (_controllerObject == null) return;
            
            GameObject.Destroy(_controllerObject);
            _controllerObject = null;
        }

        private void SubscribeToUuiSetting()
        {
            if (_useUuiSubscription != null)
            {
                return;
            }

            _useUuiSubscription = ModSettings.UseUuiButtonState.Subscribe(OnUseUuiButtonChanged, true);
        }

        private void OnUseUuiButtonChanged(bool previousValue, bool currentValue)
        {
            if (currentValue)
            {
                UuiButtonController.RegisterUui();
                return;
            }

            UuiButtonController.UnregisterUui();
        }

        private void DisposeUuiSettingSubscription()
        {
            if (_useUuiSubscription == null)
            {
                return;
            }

            _useUuiSubscription.Dispose();
            _useUuiSubscription = null;
        }

        #endregion
        
    }
}
