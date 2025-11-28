using ColossalFramework;
using HarmonyLib;
using ICities;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Lifecycle
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _controllerObject;
        private Harmony _harmony;
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
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
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
