using ColossalFramework.UI;
using ICities;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public class NetworkHighlighterMod : IUserMod
    {
        public string Name => "Network Highlighter 1.1.2";
        public string Description =>
            "Highlights various networks (paths, roads, rails, etc.) including hidden/invisible ones.";

        private Texture2D _hueTexture;
        private Texture2D _valueTexture;
        private Texture2D _widthTexture;

        public void OnSettingsUI(UIHelperBase helper)
        {
            Debug.Log("[NetworkHighlightOverlay][Options] OnSettingsUI called");

            if (_hueTexture == null)
            {
                _hueTexture = ModResources.LoadTexture("HueGradient.png");
            }
            if (_valueTexture == null)
            {
                _valueTexture = ModResources.LoadTexture("ValueGradient.png");
            }
            if (_widthTexture == null)
            {
                _widthTexture = ModResources.LoadTexture("HighlightWidth.png");
            }
            
            // Some mods (e.g. Skyve) seem to replace the vanilla UIHelper with their own UIHelperBase implementation,
            // so we cannot rely on UIHelper.self always being available. I solved this by resolving the underlying UIComponent first
            // then building our own tabbed UI on top of it. There's probably a more elegant solution out there, but this one works.
            
            UIComponent rootComponent = UIUtility.TryGetRootComponent(helper);
            if (rootComponent == null)
            {
                Debug.LogWarning(
                    "[NetworkHighlightOverlay][Options] Could not resolve root UIComponent for helper of type " +
                    (helper != null ? helper.GetType().FullName : "null") +
                    ". Falling back to simple (non-tabbed) settings UI.");

                //BuildSimpleSettings(helper);   //todo fallback – no tabs, just groups
                return;
            }

            Debug.Log(
                "[NetworkHighlightOverlay][Options] Using root component '" + rootComponent.name +
                "' of type " + rootComponent.GetType().FullName +
                ", size=" + rootComponent.size);

            BuildTabbedSettingsUI(rootComponent);
        }


        private void BuildTabbedSettingsUI(UIComponent rootComponent)
        {
            // local container so we don't mess with Skyve/vanilla layout flags
            UIPanel tabRoot = CreateRootPanel(rootComponent);
            UITabstrip tabStrip = CreateUITabstrip(tabRoot);
            UITabContainer tabContainer = CreateUITabContainer(tabRoot, tabStrip);

            tabStrip.tabPages = tabContainer;
            tabStrip.selectedIndex = -1; 

            #region TAB COLORS
            UIHelper colorsHelper = UIUtility.CreateTab(tabContainer, tabStrip, "Colors", Color.white);
            if (colorsHelper != null)
            {
                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Highlight Strength",
                    ModSettings.HighlightStrength,
                    v => ModSettings.HighlightStrength = v,
                    _valueTexture);
                
                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Highlight Thickness",
                    ModSettings.HighlightWidth,
                    v => ModSettings.HighlightWidth = v,
                    _widthTexture);
                
                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Pedestrian paths",
                    ModSettings.PedestrianPathsHue,
                    v => ModSettings.PedestrianPathsHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Pink paths",
                    ModSettings.PinkPathsHue,
                    v => ModSettings.PinkPathsHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Roads",
                    ModSettings.RoadsHue,
                    v => ModSettings.RoadsHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Highways",
                    ModSettings.HighwaysHue,
                    v => ModSettings.HighwaysHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Train tracks",
                    ModSettings.TrainTracksHue,
                    v => ModSettings.TrainTracksHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Metro tracks",
                    ModSettings.MetroTracksHue,
                    v => ModSettings.MetroTracksHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Tram and Trolley tracks",
                    ModSettings.TramTracksHue,
                    v => ModSettings.TramTracksHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Monorail tracks",
                    ModSettings.MonorailTracksHue,
                    v => ModSettings.MonorailTracksHue = v,
                    _hueTexture);

                UIUtility.CreateHueSlider(
                    colorsHelper,
                    "Cable car paths",
                    ModSettings.CableCarsHue,
                    v => ModSettings.CableCarsHue = v,
                    _hueTexture);
            }
            #endregion
            
            #region TAB FILTERS
            var filtersHelper = UIUtility.CreateTab(tabContainer, tabStrip, "Filters", Color.white);
            if (filtersHelper != null)
            {
                filtersHelper.AddCheckbox(
                    "Highlight pedestrian paths",
                    ModSettings.HighlightPedestrianPaths,
                    v => ModSettings.HighlightPedestrianPaths = v);

                filtersHelper.AddCheckbox(
                    "Highlight pink paths",
                    ModSettings.HighlightPinkPaths,
                    v => ModSettings.HighlightPinkPaths = v);

                filtersHelper.AddCheckbox(
                    "Highlight roads",
                    ModSettings.HighlightRoads,
                    v => ModSettings.HighlightRoads = v);

                filtersHelper.AddCheckbox(
                    "Highlight highways",
                    ModSettings.HighlightHighways,
                    v => ModSettings.HighlightHighways = v);

                filtersHelper.AddCheckbox(
                    "Highlight train tracks",
                    ModSettings.HighlightTrainTracks,
                    v => ModSettings.HighlightTrainTracks = v);

                filtersHelper.AddCheckbox(
                    "Highlight metro tracks",
                    ModSettings.HighlightMetroTracks,
                    v => ModSettings.HighlightMetroTracks = v);

                filtersHelper.AddCheckbox(
                    "Highlight tram tracks",
                    ModSettings.HighlightTramTracks,
                    v => ModSettings.HighlightTramTracks = v);

                filtersHelper.AddCheckbox(
                    "Highlight monorail tracks",
                    ModSettings.HighlightMonorailTracks,
                    v => ModSettings.HighlightMonorailTracks = v);

                filtersHelper.AddCheckbox(
                    "Highlight cable car paths",
                    ModSettings.HighlightCableCars,
                    v => ModSettings.HighlightCableCars = v);

                filtersHelper.AddCheckbox(
                    "Highlight bridges",
                    ModSettings.HighlightBridges,
                    v => ModSettings.HighlightBridges = v);

                filtersHelper.AddCheckbox(
                    "Highlight tunnels",
                    ModSettings.HighlightTunnels,
                    v => ModSettings.HighlightTunnels = v);
            }
            #endregion
            
            #region TAB DANGERZONE
            UIHelper dangerHelper = UIUtility.CreateTab(tabContainer, tabStrip, "DANGER ZONE", Color.red);
            if (dangerHelper != null)
            {
                dangerHelper.AddSpace(20);
                dangerHelper.AddButton(
                    "Reset ALL settings to defaults",
                    () =>
                    {
                        ModSettings.ResetToDefaults();
                        if (tabRoot != null && tabRoot.parent != null)
                        {
                            tabRoot.parent.RemoveUIComponent(tabRoot);
                            UnityEngine.Object.Destroy(tabRoot.gameObject);
                        }
                        BuildTabbedSettingsUI(rootComponent);
                    });
            }
            #endregion

            tabStrip.selectedIndex = 0;
            
        }

        private static UIPanel CreateRootPanel(UIComponent rootComponent)
        {
            var tabRoot = rootComponent.AddUIComponent<UIPanel>();
            tabRoot.name = "NHO_TabRoot";
            tabRoot.autoLayout = false;
            tabRoot.clipChildren = true;
            tabRoot.relativePosition = new Vector3(10f, 10f);
            tabRoot.width  = Mathf.Max(0f, rootComponent.width  - 20f);
            tabRoot.height = Mathf.Max(0f, rootComponent.height - 20f);
            return tabRoot;
        }


        private static UITabContainer CreateUITabContainer(UIComponent parent, UITabstrip tabStrip)
        {
            var tabContainer = parent.AddUIComponent<UITabContainer>();
            tabContainer.name = "NHO_TabContainer";
            tabContainer.relativePosition = new Vector3(0f, tabStrip.height + 5f);
            tabContainer.width  = parent.width;
            tabContainer.height = Mathf.Max(0f, parent.height - tabStrip.height - 5f);
            return tabContainer;
        }

        private static UITabstrip CreateUITabstrip(UIComponent parent)
        {
            var tabStrip = parent.AddUIComponent<UITabstrip>();
            tabStrip.name = "NHO_TabStrip";
            tabStrip.width = parent.width;
            tabStrip.height = 30f;
            tabStrip.relativePosition = new Vector3(0f, 0f);
            tabStrip.padding = new RectOffset(5, 5, 0, 0); 
            return tabStrip;
        }

    }
}
