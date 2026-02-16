using System;
using System.Collections.Generic;
using ColossalFramework;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;
using NetworkHighlightOverlay.Utility;
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
        private static readonly Observable<long> _highlightRulesVersion = new Observable<long>(0L);

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
        public static Observable<long> HighlightRulesVersion => _highlightRulesVersion;
        public static Observable<float> HighlightStrengthState => _highlightStrength;
        public static Observable<bool> UseUuiButtonState => _useUuiButton;
        public static SavedInputKey ToggleOverlayHotkey => _toggleOverlayHotkey;

        public static Observable<HighlightCategorySetting> GetCategoryState(HighlightCategoryId categoryId) => _categoryStates[categoryId];

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
            BindState(_panelX, (config, value) => config.PanelX = value, false);
            BindState(_panelY, (config, value) => config.PanelY = value, false);

            BindState(_highlightStrength, (config, value) => config.HighlightStrength = value, true);
            BindState(_highlightWidth, (config, value) => config.HighlightWidth = value, false);

            BindAllCategoryStates();

            BindState(_highlightBridges, (config, value) => config.HighlightBridges = value, true);
            BindState(_highlightTunnels, (config, value) => config.HighlightTunnels = value, true);
            BindState(_useUuiButton, (config, value) => config.UseUuiButton = value, false);
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
                BindState(state, (config, value) => definition.WriteState(config, value), true);
            }
        }

        private static void BindState<TValue>(
            Observable<TValue> state,
            Action<Config, TValue> apply,
            bool affectsHighlightRules)
        {
            state.Subscribe((previousValue, currentValue) =>
            {
                apply(_config, currentValue);
                SaveAndRaise(affectsHighlightRules);
            });
        }

        private static void SaveAndRaise(bool affectsHighlightRules)
        {
            if (_suppressSaveAndRaise) return;

            SettingsLoader.Save(_config);
            _changeVersion.Update(IncrementVersion);
            if (affectsHighlightRules)
            {
                _highlightRulesVersion.Update(IncrementVersion);
            }
        }

        private static long IncrementVersion(long version) => version == long.MaxValue ? 0L : version + 1L;

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

        public static bool GetCategoryEnabled(HighlightCategoryId categoryId) => _categoryStates[categoryId].Value.IsEnabled;

        public static void SetCategoryEnabled(HighlightCategoryId categoryId, bool value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryEnabledState(state, value);
        }

        public static float GetCategoryHue(HighlightCategoryId categoryId) => _categoryStates[categoryId].Value.Hue;

        public static void SetCategoryHue(HighlightCategoryId categoryId, float value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryHueState(state, value);
        }

        public static Color GetCategoryColor(HighlightCategoryId categoryId) => ColorConversion.FromHue(GetCategoryHue(categoryId), HighlightStrength);

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

            SaveAndRaise(true);
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
