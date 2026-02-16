using HarmonyLib;
using ICities;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;
using NetworkHighlightOverlay.Code.GUI;

namespace NetworkHighlightOverlay.Code.Lifecycle
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _controllerObject;
        private Harmony _harmony;
        
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
            TogglePanel.Create();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            TogglePanel.Destroy();
            ToggleButtonAtlas.Clear();
            DestroyRendererObject();
            Manager.Instance.ResetForLevelUnload();
        }

        #region Helpers

        private void PatchHarmony()
        {
            if (_harmony != null)
                return;
            
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll();
        }

        private void UnpatchHarmony()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;
            }
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

        #endregion
        
    }
}
