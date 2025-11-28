using ColossalFramework.UI;
using ICities;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public class Options : IUserMod
    {
        public string Name => "Path / Network Highlight Overlay";
        public string Description => "Highlights various networks (paths, roads, rails, etc.).";

        private Texture2D _hueTexture;

        public void OnSettingsUI(UIHelperBase helper)
        {
            // Load gradient texture once
            if (_hueTexture == null)
                _hueTexture = ModResources.LoadTexture("HueGradient.png");

            var colorsGroupBase = helper.AddGroup("Highlight colors");
            var filtersGroupBase = helper.AddGroup("Filters");

            var colorsGroup = colorsGroupBase as UIHelper;
            var filtersGroup = filtersGroupBase as UIHelper;

            // --- Colors / hues ---
            if (colorsGroup != null)
            {
                CreateHueSlider(
                    colorsGroup,
                    "Pedestrian paths",
                    ModSettings.PedestrianPathsHue,
                    v => ModSettings.PedestrianPathsHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Roads",
                    ModSettings.RoadsHue,
                    v => ModSettings.RoadsHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Highways",
                    ModSettings.HighwaysHue,
                    v => ModSettings.HighwaysHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Train tracks",
                    ModSettings.TrainTracksHue,
                    v => ModSettings.TrainTracksHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Metro tracks",
                    ModSettings.MetroTracksHue,
                    v => ModSettings.MetroTracksHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Tram tracks",
                    ModSettings.TramTracksHue,
                    v => ModSettings.TramTracksHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Monorail tracks",
                    ModSettings.MonorailTracksHue,
                    v => ModSettings.MonorailTracksHue = v);

                CreateHueSlider(
                    colorsGroup,
                    "Cable car paths",
                    ModSettings.CableCarsHue,
                    v => ModSettings.CableCarsHue = v);
            }

            // --- Filters / toggles ---
            if (filtersGroup != null)
            {
                filtersGroup.AddCheckbox(
                    "Highlight pedestrian paths",
                    ModSettings.HighlightPedestrianPaths,
                    v => ModSettings.HighlightPedestrianPaths = v);

                filtersGroup.AddCheckbox(
                    "Highlight roads",
                    ModSettings.HighlightRoads,
                    v => ModSettings.HighlightRoads = v);

                filtersGroup.AddCheckbox(
                    "Highlight highways",
                    ModSettings.HighlightHighways,
                    v => ModSettings.HighlightHighways = v);

                filtersGroup.AddCheckbox(
                    "Highlight train tracks",
                    ModSettings.HighlightTrainTracks,
                    v => ModSettings.HighlightTrainTracks = v);

                filtersGroup.AddCheckbox(
                    "Highlight metro tracks",
                    ModSettings.HighlightMetroTracks,
                    v => ModSettings.HighlightMetroTracks = v);

                filtersGroup.AddCheckbox(
                    "Highlight tram tracks",
                    ModSettings.HighlightTramTracks,
                    v => ModSettings.HighlightTramTracks = v);

                filtersGroup.AddCheckbox(
                    "Highlight monorail tracks",
                    ModSettings.HighlightMonorailTracks,
                    v => ModSettings.HighlightMonorailTracks = v);

                filtersGroup.AddCheckbox(
                    "Highlight cable car paths",
                    ModSettings.HighlightCableCars,
                    v => ModSettings.HighlightCableCars = v);

                filtersGroup.AddCheckbox(
                    "Highlight bridges",
                    ModSettings.HighlightBridges,
                    v => ModSettings.HighlightBridges = v);

                filtersGroup.AddCheckbox(
                    "Highlight tunnels",
                    ModSettings.HighlightTunnels,
                    v => ModSettings.HighlightTunnels = v);
            }

            // Optional: add a reset button later if you like
            // helper.AddButton("Reset to defaults", () => { ModSettings.ResetToDefaults(); ... });
        }

        private UISlider CreateHueSlider(UIHelper group, string label, float initialHue, OnValueChanged onChanged)
        {
            // hook up slider
            var sliderObj = group.AddSlider(label, 0f, 1f, 0.01f, initialHue, onChanged);
            var slider = sliderObj as UISlider;
            if (slider == null)
                return null;

            // Remove the default grey background
            slider.backgroundSprite = string.Empty;
            slider.color = Color.white;

            // Add our hue gradient as a child so it moves with layout
            if (_hueTexture != null)
            {
                slider.clipChildren = true;

                var hueBar = slider.AddUIComponent<UITextureSprite>();
                hueBar.texture = _hueTexture;
                hueBar.size = slider.size;
                hueBar.relativePosition = Vector3.zero;
                hueBar.zOrder = 0;

                if (slider.thumbObject != null)
                {
                    slider.thumbObject.zOrder = hueBar.zOrder + 1;
                }
            }

            return slider;
        }
    }
}


