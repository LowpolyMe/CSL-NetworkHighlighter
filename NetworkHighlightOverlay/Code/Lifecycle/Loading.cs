using HarmonyLib;
using ICities;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.GUI;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.UI;
using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Lifecycle
{
    public class Loading : LoadingExtensionBase
    {
        private readonly ModSettings _settings = ModCompositionRoot.Settings;
        private Manager _manager;
        private UuiButtonController _uuiButtonController;
        private ToggleButtonAtlas _toggleButtonAtlas;
        private GameObject _controllerObject;
        private ActivationHandler _activationHandler;
        private TogglePanel _togglePanel;
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
            ReleaseRuntime();
            UnpatchHarmony();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CreateRuntime();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            ReleaseRuntime();
        }

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

        private void CreateRuntime()
        {
            ReleaseRuntime();

            _manager = new Manager(_settings);
            _uuiButtonController = new UuiButtonController();
            _toggleButtonAtlas = new ToggleButtonAtlas();

            CreateControllerObject();
            CreateTogglePanel();
        }

        private void ReleaseRuntime()
        {
            DestroyTogglePanel();
            DestroyControllerObject();

            ToggleButtonAtlas atlas = _toggleButtonAtlas;
            _toggleButtonAtlas = null;
            if (atlas != null)
            {
                atlas.Dispose();
            }

            Manager manager = _manager;
            _manager = null;
            if (manager != null)
            {
                manager.ResetForLevelUnload();
            }

            _uuiButtonController = null;
        }

        private void CreateControllerObject()
        {
            _controllerObject = new GameObject("PathHighlightRenderer");
            _activationHandler = _controllerObject.AddComponent<ActivationHandler>();
            _activationHandler.Initialize(_manager, _settings, _uuiButtonController);
            GameObject.DontDestroyOnLoad(_controllerObject);
        }

        private void DestroyControllerObject()
        {
            if (_controllerObject == null)
            {
                _activationHandler = null;
                return;
            }

            UnityEngine.Object.Destroy(_controllerObject);
            _controllerObject = null;
            _activationHandler = null;
        }

        private void CreateTogglePanel()
        {
            UIView view = UIView.GetAView();
            if (view == null)
                throw new InvalidOperationException("Loading requires an active UIView before creating the toggle panel.");

            if (_activationHandler == null || _toggleButtonAtlas == null)
                throw new InvalidOperationException("Loading must initialize runtime dependencies before creating the toggle panel.");

            TogglePanel panel = view.AddUIComponent(typeof(TogglePanel)) as TogglePanel;
            if (panel == null)
                throw new InvalidOperationException("Failed to create the toggle panel.");

            panel.isVisible = false;
            panel.Initialize(_settings, _activationHandler, _toggleButtonAtlas);
            _togglePanel = panel;
        }

        private void DestroyTogglePanel()
        {
            if (_togglePanel == null)
                return;

            _togglePanel.CloseHuePopover();
            UnityEngine.Object.Destroy(_togglePanel.gameObject);
            _togglePanel = null;
        }
    }
}
