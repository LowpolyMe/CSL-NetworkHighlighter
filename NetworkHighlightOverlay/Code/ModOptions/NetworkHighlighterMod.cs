using ColossalFramework.UI;
using ICities;
using NetworkHighlightOverlay.Code.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public class NetworkHighlighterMod : IUserMod
    {
        public string Name => "Network Highlighter";
        public string Description =>
            "Highlights various networks (paths, roads, rails, etc.) including hidden/invisible ones.";

        private Texture2D _hueTexture;
        private Texture2D _valueTexture;
        private Texture2D _widthTexture;
        private readonly List<HueSliderBinding> _hueSliderBindings = new List<HueSliderBinding>();
        private readonly List<CheckboxBinding> _checkboxBindings = new List<CheckboxBinding>();
        private bool _isApplyingSettingsToUi;
        private bool _isSubscribedToSettings;

        private struct HueSliderBinding
        {
            public readonly UISlider Slider;
            public readonly Func<float> GetValue;

            public HueSliderBinding(UISlider slider, Func<float> getValue)
            {
                Slider = slider;
                GetValue = getValue;
            }
        }

        private struct CheckboxBinding
        {
            public readonly UICheckBox Checkbox;
            public readonly Func<bool> GetValue;

            public CheckboxBinding(UICheckBox checkbox, Func<bool> getValue)
            {
                Checkbox = checkbox;
                GetValue = getValue;
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            Debug.Log("[NetworkHighlightOverlay][Options] OnSettingsUI called");
            EnsureSettingsSubscription();
            ClearUiBindings();

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
            ClearUiBindings();

            // local container so we don't mess with Skyve/vanilla layout flags
            UIPanel tabRoot = CreateRootPanel(rootComponent);
            UITabstrip tabStrip = CreateUITabstrip(tabRoot);
            UITabContainer tabContainer = CreateUITabContainer(tabRoot, tabStrip);

            tabStrip.tabPages = tabContainer;
            tabStrip.selectedIndex = -1; 

            #region TAB COLORS
            UIPanel colorsPanel;
            UIHelper colorsHelper = UIUtility.CreateTab(
                tabContainer,
                tabStrip,
                "Colors",
                Color.white,
                out colorsPanel);
            if (colorsHelper != null && colorsPanel != null)
            {
                UIPanel columnsRoot;
                UIPanel leftColumn;
                UIPanel rightColumn;
                CreateColorColumns(colorsPanel, out columnsRoot, out leftColumn, out rightColumn);

                UIHelper leftHelper = new UIHelper(leftColumn);
                UIHelper rightHelper = new UIHelper(rightColumn);
                int sliderIndex = 0;

                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Highlight Strength",
                    () => ModSettings.HighlightStrength,
                    value => ModSettings.HighlightStrength = value,
                    _valueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Highlight Thickness",
                    () => ModSettings.HighlightWidth,
                    value => ModSettings.HighlightWidth = value,
                    _widthTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Pedestrian paths",
                    () => ModSettings.PedestrianPathsHue,
                    value => ModSettings.PedestrianPathsHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Pink paths",
                    () => ModSettings.PinkPathsHue,
                    value => ModSettings.PinkPathsHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Terraforming networks",
                    () => ModSettings.TerraformingNetworksHue,
                    value => ModSettings.TerraformingNetworksHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Roads",
                    () => ModSettings.RoadsHue,
                    value => ModSettings.RoadsHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Highways",
                    () => ModSettings.HighwaysHue,
                    value => ModSettings.HighwaysHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Train tracks",
                    () => ModSettings.TrainTracksHue,
                    value => ModSettings.TrainTracksHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Metro tracks",
                    () => ModSettings.MetroTracksHue,
                    value => ModSettings.MetroTracksHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Tram and Trolley tracks",
                    () => ModSettings.TramTracksHue,
                    value => ModSettings.TramTracksHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Monorail tracks",
                    () => ModSettings.MonorailTracksHue,
                    value => ModSettings.MonorailTracksHue = value,
                    _hueTexture);
                AddBoundHueSlider(
                    GetColumnHelper(sliderIndex++, leftHelper, rightHelper),
                    "Cable car paths",
                    () => ModSettings.CableCarsHue,
                    value => ModSettings.CableCarsHue = value,
                    _hueTexture);

                UpdateColorColumnsLayout(colorsPanel, columnsRoot, leftColumn, rightColumn);
                colorsPanel.eventSizeChanged += (component, size) =>
                    UpdateColorColumnsLayout(colorsPanel, columnsRoot, leftColumn, rightColumn);
            }
            #endregion
            
            #region TAB FILTERS
            UIPanel filtersPanel;
            UIHelper filtersHelper = UIUtility.CreateTab(tabContainer, tabStrip, "Filters", Color.white, out filtersPanel);
            if (filtersHelper != null)
            {
                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight pedestrian paths",
                    () => ModSettings.HighlightPedestrianPaths,
                    value => ModSettings.HighlightPedestrianPaths = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight pink paths",
                    () => ModSettings.HighlightPinkPaths,
                    value => ModSettings.HighlightPinkPaths = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight terraforming networks",
                    () => ModSettings.HighlightTerraformingNetworks,
                    value => ModSettings.HighlightTerraformingNetworks = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight roads",
                    () => ModSettings.HighlightRoads,
                    value => ModSettings.HighlightRoads = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight highways",
                    () => ModSettings.HighlightHighways,
                    value => ModSettings.HighlightHighways = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight train tracks",
                    () => ModSettings.HighlightTrainTracks,
                    value => ModSettings.HighlightTrainTracks = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight metro tracks",
                    () => ModSettings.HighlightMetroTracks,
                    value => ModSettings.HighlightMetroTracks = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight tram tracks",
                    () => ModSettings.HighlightTramTracks,
                    value => ModSettings.HighlightTramTracks = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight monorail tracks",
                    () => ModSettings.HighlightMonorailTracks,
                    value => ModSettings.HighlightMonorailTracks = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight cable car paths",
                    () => ModSettings.HighlightCableCars,
                    value => ModSettings.HighlightCableCars = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight bridges",
                    () => ModSettings.HighlightBridges,
                    value => ModSettings.HighlightBridges = value);

                AddBoundCheckbox(
                    filtersHelper,
                    "Highlight tunnels",
                    () => ModSettings.HighlightTunnels,
                    value => ModSettings.HighlightTunnels = value);
            }
            #endregion
            
            #region TAB DANGERZONE
            UIPanel dangerPanel;
            UIHelper dangerHelper = UIUtility.CreateTab(tabContainer, tabStrip, "DANGER ZONE", Color.red, out dangerPanel);
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
            ApplySettingsToUi();
            
        }

        private void EnsureSettingsSubscription()
        {
            if (_isSubscribedToSettings)
                return;

            ModSettings.SettingsChanged += OnModSettingsChanged;
            _isSubscribedToSettings = true;
        }

        private void OnModSettingsChanged(Config config)
        {
            ApplySettingsToUi();
        }

        private void ClearUiBindings()
        {
            _hueSliderBindings.Clear();
            _checkboxBindings.Clear();
        }

        private void ApplySettingsToUi()
        {
            _isApplyingSettingsToUi = true;
            try
            {
                int sliderCount = _hueSliderBindings.Count;
                for (int i = 0; i < sliderCount; i++)
                {
                    HueSliderBinding binding = _hueSliderBindings[i];
                    if (binding.Slider == null || binding.GetValue == null)
                        continue;

                    float targetValue = Mathf.Clamp01(binding.GetValue());
                    if (Mathf.Abs(binding.Slider.value - targetValue) > 0.0001f)
                    {
                        binding.Slider.value = targetValue;
                    }
                }

                int checkboxCount = _checkboxBindings.Count;
                for (int i = 0; i < checkboxCount; i++)
                {
                    CheckboxBinding binding = _checkboxBindings[i];
                    if (binding.Checkbox == null || binding.GetValue == null)
                        continue;

                    bool targetValue = binding.GetValue();
                    if (binding.Checkbox.isChecked != targetValue)
                    {
                        binding.Checkbox.isChecked = targetValue;
                    }
                }
            }
            finally
            {
                _isApplyingSettingsToUi = false;
            }
        }

        private void AddBoundHueSlider(
            UIHelper helper,
            string label,
            Func<float> getValue,
            Action<float> setValue,
            Texture2D backgroundTexture)
        {
            if (helper == null || getValue == null || setValue == null)
                return;

            OnValueChanged onChanged = value =>
            {
                if (_isApplyingSettingsToUi)
                    return;

                setValue(value);
            };

            UISlider slider = UIUtility.CreateHueSlider(helper, label, getValue(), onChanged, backgroundTexture);
            if (slider == null)
                return;

            _hueSliderBindings.Add(new HueSliderBinding(slider, getValue));
        }

        private void AddBoundCheckbox(
            UIHelperBase helper,
            string label,
            Func<bool> getValue,
            Action<bool> setValue)
        {
            if (helper == null || getValue == null || setValue == null)
                return;

            OnCheckChanged onChanged = value =>
            {
                if (_isApplyingSettingsToUi)
                    return;

                setValue(value);
            };

            object checkboxObject = helper.AddCheckbox(label, getValue(), onChanged);
            UICheckBox checkbox = checkboxObject as UICheckBox;
            if (checkbox == null)
                return;

            _checkboxBindings.Add(new CheckboxBinding(checkbox, getValue));
        }

        private static UIHelper GetColumnHelper(int index, UIHelper leftHelper, UIHelper rightHelper)
        {
            return (index % 2 == 0) ? leftHelper : rightHelper;
        }

        private static UIPanel CreateRootPanel(UIComponent rootComponent)
        {
            UIPanel tabRoot = rootComponent.AddUIComponent<UIPanel>();
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

        private static void CreateColorColumns(UIPanel colorsPanel, out UIPanel columnsRoot,
            out UIPanel leftColumn, out UIPanel rightColumn)
        {
            columnsRoot = colorsPanel.AddUIComponent<UIPanel>();
            columnsRoot.name = "NHO_Colors_Columns";
            columnsRoot.autoLayout = false;
            columnsRoot.clipChildren = false;
            columnsRoot.relativePosition = Vector3.zero;

            leftColumn = columnsRoot.AddUIComponent<UIPanel>();
            leftColumn.name = "NHO_Colors_ColumnLeft";
            leftColumn.autoLayout = true;
            leftColumn.autoLayoutDirection = LayoutDirection.Vertical;
            leftColumn.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            leftColumn.autoSize = true;

            rightColumn = columnsRoot.AddUIComponent<UIPanel>();
            rightColumn.name = "NHO_Colors_ColumnRight";
            rightColumn.autoLayout = true;
            rightColumn.autoLayoutDirection = LayoutDirection.Vertical;
            rightColumn.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            rightColumn.autoSize = true;
        }

        private static void UpdateColorColumnsLayout(UIPanel colorsPanel, UIPanel columnsRoot,
            UIPanel leftColumn, UIPanel rightColumn)
        {
            if (colorsPanel == null || columnsRoot == null || leftColumn == null || rightColumn == null)
            {
                return;
            }

            float availableWidth = Mathf.Max(0f, colorsPanel.width);
            float horizontalPadding = colorsPanel.autoLayoutPadding != null
                ? colorsPanel.autoLayoutPadding.left
                : 0f;
            float columnGap = horizontalPadding;
            float columnWidth = Mathf.Max(0f, (availableWidth - (horizontalPadding * 2f) - columnGap) * 0.5f);

            columnsRoot.width = availableWidth;
            leftColumn.width = columnWidth;
            rightColumn.width = columnWidth;
            leftColumn.relativePosition = new Vector3(horizontalPadding, 0f);
            rightColumn.relativePosition = new Vector3(horizontalPadding + columnWidth + columnGap, 0f);

            float columnsHeight = Mathf.Max(leftColumn.height, rightColumn.height);
            columnsRoot.height = columnsHeight;
        }

    }
}
