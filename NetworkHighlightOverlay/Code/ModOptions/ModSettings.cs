using System;
using System.Collections.Generic;
using ColossalFramework;
using NetworkHighlightOverlay.Code.Utility;
using NetworkHighlightOverlay.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public sealed class ModSettings
    {
        private const string KeybindingsFileName = "NetworkHighlightOverlay_Keybindings";
        private const string ToggleOverlayHotkeyName = "NetworkHighlightOverlay_ToggleOverlay";

        private readonly Config _config;
        private readonly SavedInputKey _toggleOverlayHotkey;

        private readonly Observable<long> _changeVersion = new Observable<long>(0L);
        private readonly Observable<long> _highlightRulesVersion = new Observable<long>(0L);

        private readonly Observable<float> _panelX;
        private readonly Observable<float> _panelY;

        private readonly Observable<float> _highlightStrength;
        private readonly Observable<float> _highlightWidth;

        private readonly Observable<bool> _highlightBridges;
        private readonly Observable<bool> _highlightTunnels;
        private readonly Observable<bool> _useUuiButton;

        private readonly Dictionary<HighlightCategoryId, Observable<HighlightCategorySetting>> _categoryStates =
            new Dictionary<HighlightCategoryId, Observable<HighlightCategorySetting>>();

        private bool _suppressSaveAndRaise;

        public event Action SettingsChanged;
        public event Action HighlightRulesChanged;

        public Observable<long> ChangeVersion => _changeVersion;
        public Observable<long> HighlightRulesVersion => _highlightRulesVersion;
        public Observable<float> HighlightStrengthState => _highlightStrength;
        public Observable<bool> UseUuiButtonState => _useUuiButton;
        public SavedInputKey ToggleOverlayHotkey => _toggleOverlayHotkey;

        public Observable<HighlightCategorySetting> GetCategoryState(HighlightCategoryId categoryId) => _categoryStates[categoryId];

        public ModSettings()
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

        private void SubscribeToStateChanges()
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
                HighlightCategorySetting initialState = definition.ReadState(_config);
                _categoryStates[definition.Id] = new Observable<HighlightCategorySetting>(initialState);
            }
        }

        private void BindAllCategoryStates()
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

        private void BindState<TValue>(
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

        private void SaveAndRaise(bool affectsHighlightRules)
        {
            if (_suppressSaveAndRaise) return;

            SettingsLoader.Save(_config);
            _changeVersion.Update(IncrementVersion);
            if (affectsHighlightRules)
            {
                _highlightRulesVersion.Update(IncrementVersion);
            }

            RaiseChangedEvents(affectsHighlightRules);
        }

        private long IncrementVersion(long version) => version == long.MaxValue ? 0L : version + 1L;

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
            Observable<HighlightCategorySetting> state,
            HighlightCategorySetting value)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.Equals(value)) return;

            state.Value = value;
        }

        private void SetCategoryEnabledState(Observable<HighlightCategorySetting> state, bool isEnabled)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.IsEnabled == isEnabled) return;

            state.Value = currentValue.WithEnabled(isEnabled);
        }

        private void SetCategoryHueState(Observable<HighlightCategorySetting> state, float hue)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (Mathf.Approximately(currentValue.Hue, hue)) return;

            state.Value = currentValue.WithHue(hue);
        }

        public bool GetCategoryEnabled(HighlightCategoryId categoryId) => _categoryStates[categoryId].Value.IsEnabled;

        public void SetCategoryEnabled(HighlightCategoryId categoryId, bool value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryEnabledState(state, value);
        }

        public float GetCategoryHue(HighlightCategoryId categoryId) => _categoryStates[categoryId].Value.Hue;

        public void SetCategoryHue(HighlightCategoryId categoryId, float value)
        {
            Observable<HighlightCategorySetting> state = _categoryStates[categoryId];
            SetCategoryHueState(state, value);
        }

        public Color GetCategoryColor(HighlightCategoryId categoryId) => ColorConversion.FromHue(GetCategoryHue(categoryId), HighlightStrength);

        public float PanelX
        {
            get => _panelX.Value;
            set => _panelX.Value = value;
        }

        public float PanelY
        {
            get => _panelY.Value;
            set => _panelY.Value = value;
        }

        public float HighlightStrength
        {
            get => _highlightStrength.Value;
            set => _highlightStrength.Value = value;
        }

        public float HighlightWidth
        {
            get => _highlightWidth.Value;
            set => _highlightWidth.Value = value;
        }

        public bool HighlightBridges
        {
            get => _highlightBridges.Value;
            set => _highlightBridges.Value = value;
        }

        public bool HighlightTunnels
        {
            get => _highlightTunnels.Value;
            set => _highlightTunnels.Value = value;
        }

        public bool UseUuiButton
        {
            get => _useUuiButton.Value;
            set => _useUuiButton.Value = value;
        }

        public void ResetToDefaults()
        {
            ApplyConfig(new Config());
        }

        private void ApplyConfig(Config source)
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

        private void ApplyAllCategoryStates(Config source)
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
