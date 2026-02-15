using System;
using System.Collections.Generic;
using ColossalFramework;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class ModSettings
    {
        private const string KeybindingsFileName = "NetworkHighlightOverlay_Keybindings";
        private const string ToggleOverlayHotkeyName = "NetworkHighlightOverlay_ToggleOverlay";

        private static readonly Config _config;
        private static readonly SavedInputKey _toggleOverlayHotkey;

        private static readonly Observable<long> _changeVersion = new Observable<long>(0L);

        private static readonly Observable<float> _panelX;
        private static readonly Observable<float> _panelY;

        private static readonly Observable<float> _highlightStrength;
        private static readonly Observable<float> _highlightWidth;

        private static readonly Observable<bool> _highlightBridges;
        private static readonly Observable<bool> _highlightTunnels;
        private static readonly Observable<bool> _useUuiButton;

        private static readonly Dictionary<HighlightCategoryId, Observable<HighlightCategorySetting>> _categoryStates =
            new Dictionary<HighlightCategoryId, Observable<HighlightCategorySetting>>();

        private static bool _suppressSaveAndRaise;

        public static Observable<long> ChangeVersion => _changeVersion;
        public static Observable<float> HighlightStrengthState => _highlightStrength;
        public static Observable<bool> UseUuiButtonState => _useUuiButton;
        public static SavedInputKey ToggleOverlayHotkey => _toggleOverlayHotkey;

        public static Observable<HighlightCategorySetting> GetCategoryState(HighlightCategoryId categoryId)
        {
            return _categoryStates[categoryId];
        }

        static ModSettings()
        {
            EnsureKeybindingsSettingsFile();
            _toggleOverlayHotkey = new SavedInputKey(
                ToggleOverlayHotkeyName,
                KeybindingsFileName,
                KeyCode.F9,
                false,
                false,
                false,
                true);

            _config = SettingsLoader.Load();

            _panelX = new Observable<float>(_config.PanelX);
            _panelY = new Observable<float>(_config.PanelY);

            _highlightStrength = new Observable<float>(_config.HighlightStrength);
            _highlightWidth = new Observable<float>(_config.HighlightWidth);

            _highlightBridges = new Observable<bool>(_config.HighlightBridges);
            _highlightTunnels = new Observable<bool>(_config.HighlightTunnels);
            _useUuiButton = new Observable<bool>(_config.UseUuiButton);

            InitializeCategoryStates();
            SubscribeToStateChanges();
        }

        private static void SubscribeToStateChanges()
        {
            BindFloat(_panelX, (config, value) => config.PanelX = value);
            BindFloat(_panelY, (config, value) => config.PanelY = value);

            BindFloat(_highlightStrength, (config, value) => config.HighlightStrength = value);
            BindFloat(_highlightWidth, (config, value) => config.HighlightWidth = value);

            BindAllCategoryStates();

            BindBool(_highlightBridges, (config, value) => config.HighlightBridges = value);
            BindBool(_highlightTunnels, (config, value) => config.HighlightTunnels = value);
            BindBool(_useUuiButton, (config, value) => config.UseUuiButton = value);
        }

        private static void EnsureKeybindingsSettingsFile()
        {
            if (GameSettings.FindSettingsFileByName(KeybindingsFileName) != null) return;

            SettingsFile keybindingsFile = new SettingsFile { fileName = KeybindingsFileName };
            GameSettings.AddSettingsFile(new SettingsFile[] { keybindingsFile });
        }

        private static void InitializeCategoryStates()
        {
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int i = 0; i < categoryCount; i++)
            {
                HighlightCategoryDefinition definition = categoryDefinitions[i];
                HighlightCategorySetting initialState = definition.ReadState(_config);
                _categoryStates[definition.Id] = new Observable<HighlightCategorySetting>(initialState);
            }
        }

        private static void BindAllCategoryStates()
        {
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int i = 0; i < categoryCount; i++)
            {
                HighlightCategoryDefinition definition = categoryDefinitions[i];
                Observable<HighlightCategorySetting> state = _categoryStates[definition.Id];
                BindCategory(state, (config, value) => definition.WriteState(config, value));
            }
        }

        private static void BindFloat(Observable<float> state, Action<Config, float> apply)
        {
            state.Subscribe((previousValue, currentValue) =>
            {
                apply(_config, currentValue);
                SaveAndRaise();
            });
        }

        private static void BindBool(Observable<bool> state, Action<Config, bool> apply)
        {
            state.Subscribe((previousValue, currentValue) =>
            {
                apply(_config, currentValue);
                SaveAndRaise();
            });
        }

        private static void BindCategory(
            Observable<HighlightCategorySetting> state,
            Action<Config, HighlightCategorySetting> apply)
        {
            state.Subscribe((previousValue, currentValue) =>
            {
                apply(_config, currentValue);
                SaveAndRaise();
            });
        }

        private static void SaveAndRaise()
        {
            if (_suppressSaveAndRaise) return;

            SettingsLoader.Save(_config);
            _changeVersion.Update(IncrementVersion);
        }

        private static long IncrementVersion(long version)
        {
            return version == long.MaxValue ? 0L : version + 1L;
        }

        private static void SetCategory(
            Observable<HighlightCategorySetting> state,
            HighlightCategorySetting value)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.Equals(value)) return;

            state.Value = value;
        }

        private static void SetCategoryEnabledState(Observable<HighlightCategorySetting> state, bool isEnabled)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.IsEnabled == isEnabled) return;

            state.Value = currentValue.WithEnabled(isEnabled);
        }

        private static void SetCategoryHueState(Observable<HighlightCategorySetting> state, float hue)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (Mathf.Approximately(currentValue.Hue, hue)) return;

            state.Value = currentValue.WithHue(hue);
        }

        public static bool GetCategoryEnabled(HighlightCategoryId categoryId)
        {
            return _categoryStates[categoryId].Value.IsEnabled;
        }

        public static void SetCategoryEnabled(HighlightCategoryId categoryId, bool value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryEnabledState(state, value);
        }

        public static float GetCategoryHue(HighlightCategoryId categoryId)
        {
            return _categoryStates[categoryId].Value.Hue;
        }

        public static void SetCategoryHue(HighlightCategoryId categoryId, float value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryHueState(state, value);
        }

        private static Color GetCategoryColor(HighlightCategoryId categoryId)
        {
            return ColorConversion.FromHue(GetCategoryHue(categoryId), HighlightStrength);
        }

        public static float PanelX
        {
            get => _panelX.Value;
            set => _panelX.Value = value;
        }

        public static float PanelY
        {
            get => _panelY.Value;
            set => _panelY.Value = value;
        }

        public static float HighlightStrength
        {
            get => _highlightStrength.Value;
            set => _highlightStrength.Value = value;
        }

        public static float HighlightWidth
        {
            get => _highlightWidth.Value;
            set => _highlightWidth.Value = value;
        }

        public static float PedestrianPathsHue
        {
            get => GetCategoryHue(HighlightCategoryId.PedestrianPaths);
            set => SetCategoryHue(HighlightCategoryId.PedestrianPaths, value);
        }

        public static Color PedestrianPathColor
        {
            get => GetCategoryColor(HighlightCategoryId.PedestrianPaths);
            set => SetCategoryHue(HighlightCategoryId.PedestrianPaths, ColorConversion.ToHue(value));
        }

        public static float PinkPathsHue
        {
            get => GetCategoryHue(HighlightCategoryId.PinkPaths);
            set => SetCategoryHue(HighlightCategoryId.PinkPaths, value);
        }

        public static Color PinkPathColor
        {
            get => GetCategoryColor(HighlightCategoryId.PinkPaths);
            set => SetCategoryHue(HighlightCategoryId.PinkPaths, ColorConversion.ToHue(value));
        }

        public static float TerraformingNetworksHue
        {
            get => GetCategoryHue(HighlightCategoryId.TerraformingNetworks);
            set => SetCategoryHue(HighlightCategoryId.TerraformingNetworks, value);
        }

        public static Color TerraformingNetworksColor
        {
            get => GetCategoryColor(HighlightCategoryId.TerraformingNetworks);
            set => SetCategoryHue(HighlightCategoryId.TerraformingNetworks, ColorConversion.ToHue(value));
        }

        public static float RoadsHue
        {
            get => GetCategoryHue(HighlightCategoryId.Roads);
            set => SetCategoryHue(HighlightCategoryId.Roads, value);
        }

        public static Color RoadsColor
        {
            get => GetCategoryColor(HighlightCategoryId.Roads);
            set => SetCategoryHue(HighlightCategoryId.Roads, ColorConversion.ToHue(value));
        }

        public static float HighwaysHue
        {
            get => GetCategoryHue(HighlightCategoryId.Highways);
            set => SetCategoryHue(HighlightCategoryId.Highways, value);
        }

        public static Color HighwaysColor
        {
            get => GetCategoryColor(HighlightCategoryId.Highways);
            set => SetCategoryHue(HighlightCategoryId.Highways, ColorConversion.ToHue(value));
        }

        public static float TrainTracksHue
        {
            get => GetCategoryHue(HighlightCategoryId.TrainTracks);
            set => SetCategoryHue(HighlightCategoryId.TrainTracks, value);
        }

        public static Color TrainTracksColor
        {
            get => GetCategoryColor(HighlightCategoryId.TrainTracks);
            set => SetCategoryHue(HighlightCategoryId.TrainTracks, ColorConversion.ToHue(value));
        }

        public static float MetroTracksHue
        {
            get => GetCategoryHue(HighlightCategoryId.MetroTracks);
            set => SetCategoryHue(HighlightCategoryId.MetroTracks, value);
        }

        public static Color MetroTracksColor
        {
            get => GetCategoryColor(HighlightCategoryId.MetroTracks);
            set => SetCategoryHue(HighlightCategoryId.MetroTracks, ColorConversion.ToHue(value));
        }

        public static float TramTracksHue
        {
            get => GetCategoryHue(HighlightCategoryId.TramTracks);
            set => SetCategoryHue(HighlightCategoryId.TramTracks, value);
        }

        public static Color TramTracksColor
        {
            get => GetCategoryColor(HighlightCategoryId.TramTracks);
            set => SetCategoryHue(HighlightCategoryId.TramTracks, ColorConversion.ToHue(value));
        }

        public static float MonorailTracksHue
        {
            get => GetCategoryHue(HighlightCategoryId.MonorailTracks);
            set => SetCategoryHue(HighlightCategoryId.MonorailTracks, value);
        }

        public static Color MonorailTracksColor
        {
            get => GetCategoryColor(HighlightCategoryId.MonorailTracks);
            set => SetCategoryHue(HighlightCategoryId.MonorailTracks, ColorConversion.ToHue(value));
        }

        public static float CableCarsHue
        {
            get => GetCategoryHue(HighlightCategoryId.CableCars);
            set => SetCategoryHue(HighlightCategoryId.CableCars, value);
        }

        public static Color CableCarColor
        {
            get => GetCategoryColor(HighlightCategoryId.CableCars);
            set => SetCategoryHue(HighlightCategoryId.CableCars, ColorConversion.ToHue(value));
        }

        public static bool HighlightPedestrianPaths
        {
            get => GetCategoryEnabled(HighlightCategoryId.PedestrianPaths);
            set => SetCategoryEnabled(HighlightCategoryId.PedestrianPaths, value);
        }

        public static bool HighlightPinkPaths
        {
            get => GetCategoryEnabled(HighlightCategoryId.PinkPaths);
            set => SetCategoryEnabled(HighlightCategoryId.PinkPaths, value);
        }

        public static bool HighlightTerraformingNetworks
        {
            get => GetCategoryEnabled(HighlightCategoryId.TerraformingNetworks);
            set => SetCategoryEnabled(HighlightCategoryId.TerraformingNetworks, value);
        }

        public static bool HighlightRoads
        {
            get => GetCategoryEnabled(HighlightCategoryId.Roads);
            set => SetCategoryEnabled(HighlightCategoryId.Roads, value);
        }

        public static bool HighlightHighways
        {
            get => GetCategoryEnabled(HighlightCategoryId.Highways);
            set => SetCategoryEnabled(HighlightCategoryId.Highways, value);
        }

        public static bool HighlightTrainTracks
        {
            get => GetCategoryEnabled(HighlightCategoryId.TrainTracks);
            set => SetCategoryEnabled(HighlightCategoryId.TrainTracks, value);
        }

        public static bool HighlightMetroTracks
        {
            get => GetCategoryEnabled(HighlightCategoryId.MetroTracks);
            set => SetCategoryEnabled(HighlightCategoryId.MetroTracks, value);
        }

        public static bool HighlightTramTracks
        {
            get => GetCategoryEnabled(HighlightCategoryId.TramTracks);
            set => SetCategoryEnabled(HighlightCategoryId.TramTracks, value);
        }

        public static bool HighlightMonorailTracks
        {
            get => GetCategoryEnabled(HighlightCategoryId.MonorailTracks);
            set => SetCategoryEnabled(HighlightCategoryId.MonorailTracks, value);
        }

        public static bool HighlightCableCars
        {
            get => GetCategoryEnabled(HighlightCategoryId.CableCars);
            set => SetCategoryEnabled(HighlightCategoryId.CableCars, value);
        }

        public static bool HighlightBridges
        {
            get => _highlightBridges.Value;
            set => _highlightBridges.Value = value;
        }

        public static bool HighlightTunnels
        {
            get => _highlightTunnels.Value;
            set => _highlightTunnels.Value = value;
        }

        public static bool UseUuiButton
        {
            get => _useUuiButton.Value;
            set => _useUuiButton.Value = value;
        }

        public static void ResetToDefaults()
        {
            ApplyConfig(new Config());
        }

        private static void ApplyConfig(Config source)
        {
            _suppressSaveAndRaise = true;
            try
            {
                _highlightStrength.Value = source.HighlightStrength;
                _highlightWidth.Value = source.HighlightWidth;

                ApplyAllCategoryStates(source);

                _highlightBridges.Value = source.HighlightBridges;
                _highlightTunnels.Value = source.HighlightTunnels;
                _useUuiButton.Value = source.UseUuiButton;

                _panelX.Value = source.PanelX;
                _panelY.Value = source.PanelY;
            }
            finally
            {
                _suppressSaveAndRaise = false;
            }

            SaveAndRaise();
        }

        private static void ApplyAllCategoryStates(Config source)
        {
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int i = 0; i < categoryCount; i++)
            {
                HighlightCategoryDefinition definition = categoryDefinitions[i];
                Observable<HighlightCategorySetting> state = _categoryStates[definition.Id];
                SetCategory(state, definition.ReadState(source));
            }
        }
    }
}
