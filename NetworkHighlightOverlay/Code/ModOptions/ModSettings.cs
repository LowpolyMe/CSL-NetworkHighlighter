using System;
using System.Collections.Generic;
using ColossalFramework;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public sealed class ModSettings
    {
        private const string KeybindingsFileName = "NetworkHighlightOverlay_Keybindings";
        private const string ToggleOverlayHotkeyName = "NetworkHighlightOverlay_ToggleOverlay";
        private static readonly InputKey DefaultToggleOverlayHotkey = SavedInputKey.Encode(KeyCode.F9, false, false, false);

        public static readonly ModSettings Shared = new ModSettings();

        private readonly Config _config;
        private readonly SavedInputKey _toggleOverlayHotkey;
        private readonly Dictionary<HighlightCategoryId, HighlightCategorySetting> _categoryStates =
            new Dictionary<HighlightCategoryId, HighlightCategorySetting>();

        public event Action SettingsChanged;
        public event Action HighlightRulesChanged;

        public SavedInputKey ToggleOverlayHotkey => _toggleOverlayHotkey;

        private ModSettings()
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

            InitializeCategoryStates();
        }

        private void EnsureKeybindingsSettingsFile()
        {
            if (GameSettings.FindSettingsFileByName(KeybindingsFileName) != null) return;

            SettingsFile keybindingsFile = new SettingsFile { fileName = KeybindingsFileName };
            GameSettings.AddSettingsFile(new SettingsFile[] { keybindingsFile });
        }

        private void InitializeCategoryStates()
        {
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int i = 0; i < categoryCount; i++)
            {
                HighlightCategoryDefinition definition = categoryDefinitions[i];
                HighlightCategorySetting initialState = ReadCategoryState(_config, definition.Id);
                _categoryStates[definition.Id] = initialState;
            }
        }

        private void SaveAndRaise(bool affectsHighlightRules)
        {
            SettingsLoader.Save(_config);
            RaiseChangedEvents(affectsHighlightRules);
        }

        private void RaiseChangedEvents(bool affectsHighlightRules)
        {
            Action settingsChanged = SettingsChanged;
            if (settingsChanged != null)
            {
                settingsChanged();
            }

            if (!affectsHighlightRules)
                return;

            Action highlightRulesChanged = HighlightRulesChanged;
            if (highlightRulesChanged != null)
            {
                highlightRulesChanged();
            }
        }

        private void SetCategory(
            HighlightCategoryId categoryId,
            HighlightCategorySetting value)
        {
            HighlightCategorySetting currentValue = _categoryStates[categoryId];
            if (currentValue.Equals(value)) return;

            _categoryStates[categoryId] = value;
            WriteCategoryState(_config, categoryId, value);
            SaveAndRaise(true);
        }

        private void SetCategoryEnabledState(HighlightCategoryId categoryId, bool isEnabled)
        {
            HighlightCategorySetting currentValue = _categoryStates[categoryId];
            if (currentValue.IsEnabled == isEnabled) return;

            SetCategory(categoryId, currentValue.WithEnabled(isEnabled));
        }

        private void SetCategoryHueState(HighlightCategoryId categoryId, float hue)
        {
            HighlightCategorySetting currentValue = _categoryStates[categoryId];
            if (Mathf.Approximately(currentValue.Hue, hue)) return;

            SetCategory(categoryId, currentValue.WithHue(hue));
        }

        public bool GetCategoryEnabled(HighlightCategoryId categoryId) => _categoryStates[categoryId].IsEnabled;

        public void SetCategoryEnabled(HighlightCategoryId categoryId, bool value)
        {
            SetCategoryEnabledState(categoryId, value);
        }

        public float GetCategoryHue(HighlightCategoryId categoryId) => _categoryStates[categoryId].Hue;

        public void SetCategoryHue(HighlightCategoryId categoryId, float value)
        {
            SetCategoryHueState(categoryId, value);
        }

        public Color GetCategoryColor(HighlightCategoryId categoryId) => ColorConversion.FromHue(GetCategoryHue(categoryId), HighlightStrength);

        public float PanelX
        {
            get => _config.PanelX;
            set
            {
                if (Mathf.Approximately(_config.PanelX, value))
                    return;

                _config.PanelX = value;
                SaveAndRaise(false);
            }
        }

        public float PanelY
        {
            get => _config.PanelY;
            set
            {
                if (Mathf.Approximately(_config.PanelY, value))
                    return;

                _config.PanelY = value;
                SaveAndRaise(false);
            }
        }

        public float HighlightStrength
        {
            get => _config.HighlightStrength;
            set
            {
                if (Mathf.Approximately(_config.HighlightStrength, value))
                    return;

                _config.HighlightStrength = value;
                SaveAndRaise(true);
            }
        }

        public float HighlightWidth
        {
            get => _config.HighlightWidth;
            set
            {
                if (Mathf.Approximately(_config.HighlightWidth, value))
                    return;

                _config.HighlightWidth = value;
                SaveAndRaise(false);
            }
        }

        public bool HighlightBridges
        {
            get => _config.HighlightBridges;
            set
            {
                if (_config.HighlightBridges == value)
                    return;

                _config.HighlightBridges = value;
                SaveAndRaise(true);
            }
        }

        public bool HighlightTunnels
        {
            get => _config.HighlightTunnels;
            set
            {
                if (_config.HighlightTunnels == value)
                    return;

                _config.HighlightTunnels = value;
                SaveAndRaise(true);
            }
        }

        public bool UseUuiButton
        {
            get => _config.UseUuiButton;
            set
            {
                if (_config.UseUuiButton == value)
                    return;

                _config.UseUuiButton = value;
                SaveAndRaise(false);
            }
        }

        public void ResetToDefaults()
        {
            _toggleOverlayHotkey.value = DefaultToggleOverlayHotkey;
            ApplyConfig(new Config());
        }

        private void ApplyConfig(Config source)
        {
            _config.HighlightStrength = source.HighlightStrength;
            _config.HighlightWidth = source.HighlightWidth;

            ApplyAllCategoryStates(source);

            _config.HighlightBridges = source.HighlightBridges;
            _config.HighlightTunnels = source.HighlightTunnels;
            _config.UseUuiButton = source.UseUuiButton;

            _config.PanelX = source.PanelX;
            _config.PanelY = source.PanelY;

            SaveAndRaise(true);
        }

        private void ApplyAllCategoryStates(Config source)
        {
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int i = 0; i < categoryCount; i++)
            {
                HighlightCategoryDefinition definition = categoryDefinitions[i];
                HighlightCategorySetting state = ReadCategoryState(source, definition.Id);
                _categoryStates[definition.Id] = state;
                WriteCategoryState(_config, definition.Id, state);
            }
        }

        private static HighlightCategorySetting ReadCategoryState(Config config, HighlightCategoryId categoryId)
        {
            switch (categoryId)
            {
                case HighlightCategoryId.PedestrianPaths:
                    return new HighlightCategorySetting(config.HighlightPedestrianPaths, config.PedestrianPathsHue);
                case HighlightCategoryId.PinkPaths:
                    return new HighlightCategorySetting(config.HighlightPinkPaths, config.PinkPathsHue);
                case HighlightCategoryId.TerraformingNetworks:
                    return new HighlightCategorySetting(config.HighlightTerraformingNetworks, config.TerraformingNetworksHue);
                case HighlightCategoryId.Roads:
                    return new HighlightCategorySetting(config.HighlightRoads, config.RoadsHue);
                case HighlightCategoryId.Highways:
                    return new HighlightCategorySetting(config.HighlightHighways, config.HighwaysHue);
                case HighlightCategoryId.RaceRoads:
                    return new HighlightCategorySetting(config.HighlightRaceRoads, config.RaceRoadsHue);
                case HighlightCategoryId.AirportRoads:
                    return new HighlightCategorySetting(config.HighlightAirportRoads, config.AirportRoadsHue);
                case HighlightCategoryId.TrainTracks:
                    return new HighlightCategorySetting(config.HighlightTrainTracks, config.TrainTracksHue);
                case HighlightCategoryId.MetroTracks:
                    return new HighlightCategorySetting(config.HighlightMetroTracks, config.MetroTracksHue);
                case HighlightCategoryId.TramTracks:
                    return new HighlightCategorySetting(config.HighlightTramTracks, config.TramTracksHue);
                case HighlightCategoryId.MonorailTracks:
                    return new HighlightCategorySetting(config.HighlightMonorailTracks, config.MonorailHue);
                case HighlightCategoryId.CableCars:
                    return new HighlightCategorySetting(config.HighlightCableCars, config.CableCarsHue);
                default:
                    throw new ArgumentOutOfRangeException("categoryId");
            }
        }

        private static void WriteCategoryState(Config config, HighlightCategoryId categoryId, HighlightCategorySetting state)
        {
            switch (categoryId)
            {
                case HighlightCategoryId.PedestrianPaths:
                    config.HighlightPedestrianPaths = state.IsEnabled;
                    config.PedestrianPathsHue = state.Hue;
                    return;
                case HighlightCategoryId.PinkPaths:
                    config.HighlightPinkPaths = state.IsEnabled;
                    config.PinkPathsHue = state.Hue;
                    return;
                case HighlightCategoryId.TerraformingNetworks:
                    config.HighlightTerraformingNetworks = state.IsEnabled;
                    config.TerraformingNetworksHue = state.Hue;
                    return;
                case HighlightCategoryId.Roads:
                    config.HighlightRoads = state.IsEnabled;
                    config.RoadsHue = state.Hue;
                    return;
                case HighlightCategoryId.Highways:
                    config.HighlightHighways = state.IsEnabled;
                    config.HighwaysHue = state.Hue;
                    return;
                case HighlightCategoryId.RaceRoads:
                    config.HighlightRaceRoads = state.IsEnabled;
                    config.RaceRoadsHue = state.Hue;
                    return;
                case HighlightCategoryId.AirportRoads:
                    config.HighlightAirportRoads = state.IsEnabled;
                    config.AirportRoadsHue = state.Hue;
                    return;
                case HighlightCategoryId.TrainTracks:
                    config.HighlightTrainTracks = state.IsEnabled;
                    config.TrainTracksHue = state.Hue;
                    return;
                case HighlightCategoryId.MetroTracks:
                    config.HighlightMetroTracks = state.IsEnabled;
                    config.MetroTracksHue = state.Hue;
                    return;
                case HighlightCategoryId.TramTracks:
                    config.HighlightTramTracks = state.IsEnabled;
                    config.TramTracksHue = state.Hue;
                    return;
                case HighlightCategoryId.MonorailTracks:
                    config.HighlightMonorailTracks = state.IsEnabled;
                    config.MonorailHue = state.Hue;
                    return;
                case HighlightCategoryId.CableCars:
                    config.HighlightCableCars = state.IsEnabled;
                    config.CableCarsHue = state.Hue;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("categoryId");
            }
        }
    }
}
