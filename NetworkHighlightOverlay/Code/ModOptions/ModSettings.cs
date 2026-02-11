using System;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class ModSettings
    {
        private static readonly Config _config;
        private static readonly Observable<long> _changeVersion = new Observable<long>(0L);

        private static readonly Observable<float> _panelX;
        private static readonly Observable<float> _panelY;

        private static readonly Observable<float> _highlightStrength;
        private static readonly Observable<float> _highlightWidth;

        private static readonly Observable<HighlightCategorySetting> _pedestrianPaths;
        private static readonly Observable<HighlightCategorySetting> _pinkPaths;
        private static readonly Observable<HighlightCategorySetting> _terraformingNetworks;
        private static readonly Observable<HighlightCategorySetting> _roads;
        private static readonly Observable<HighlightCategorySetting> _highways;
        private static readonly Observable<HighlightCategorySetting> _trainTracks;
        private static readonly Observable<HighlightCategorySetting> _metroTracks;
        private static readonly Observable<HighlightCategorySetting> _tramTracks;
        private static readonly Observable<HighlightCategorySetting> _monorailTracks;
        private static readonly Observable<HighlightCategorySetting> _cableCars;

        private static readonly Observable<bool> _highlightBridges;
        private static readonly Observable<bool> _highlightTunnels;

        private static bool _suppressSaveAndRaise;

        public static Observable<long> ChangeVersion => _changeVersion;

        public static Observable<float> PanelXState => _panelX;

        public static Observable<float> PanelYState => _panelY;

        public static Observable<float> HighlightStrengthState => _highlightStrength;

        public static Observable<float> HighlightWidthState => _highlightWidth;

        public static Observable<HighlightCategorySetting> PedestrianPathsState => _pedestrianPaths;

        public static Observable<HighlightCategorySetting> PinkPathsState => _pinkPaths;

        public static Observable<HighlightCategorySetting> TerraformingNetworksState => _terraformingNetworks;

        public static Observable<HighlightCategorySetting> RoadsState => _roads;

        public static Observable<HighlightCategorySetting> HighwaysState => _highways;

        public static Observable<HighlightCategorySetting> TrainTracksState => _trainTracks;

        public static Observable<HighlightCategorySetting> MetroTracksState => _metroTracks;

        public static Observable<HighlightCategorySetting> TramTracksState => _tramTracks;

        public static Observable<HighlightCategorySetting> MonorailTracksState => _monorailTracks;

        public static Observable<HighlightCategorySetting> CableCarsState => _cableCars;

        public static Observable<bool> HighlightBridgesState => _highlightBridges;

        public static Observable<bool> HighlightTunnelsState => _highlightTunnels;

        static ModSettings()
        {
            _config = SettingsLoader.Load();

            _panelX = new Observable<float>(_config.PanelX);
            _panelY = new Observable<float>(_config.PanelY);

            _highlightStrength = new Observable<float>(_config.HighlightStrength);
            _highlightWidth = new Observable<float>(_config.HighlightWidth);

            _pedestrianPaths = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightPedestrianPaths, _config.PedestrianPathsHue));
            _pinkPaths = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightPinkPaths, _config.PinkPathsHue));
            _terraformingNetworks = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightTerraformingNetworks, _config.TerraformingNetworksHue));
            _roads = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightRoads, _config.RoadsHue));
            _highways = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightHighways, _config.HighwaysHue));
            _trainTracks = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightTrainTracks, _config.TrainTracksHue));
            _metroTracks = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightMetroTracks, _config.MetroTracksHue));
            _tramTracks = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightTramTracks, _config.TramTracksHue));
            _monorailTracks = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightMonorailTracks, _config.MonorailHue));
            _cableCars = new Observable<HighlightCategorySetting>(
                new HighlightCategorySetting(_config.HighlightCableCars, _config.CableCarsHue));

            _highlightBridges = new Observable<bool>(_config.HighlightBridges);
            _highlightTunnels = new Observable<bool>(_config.HighlightTunnels);

            SubscribeToStateChanges();
        }

        private static void SubscribeToStateChanges()
        {
            BindFloat(_panelX, (config, value) => config.PanelX = value);
            BindFloat(_panelY, (config, value) => config.PanelY = value);

            BindFloat(_highlightStrength, (config, value) => config.HighlightStrength = value);
            BindFloat(_highlightWidth, (config, value) => config.HighlightWidth = value);

            BindCategory(_pedestrianPaths, (config, value) =>
            {
                config.HighlightPedestrianPaths = value.IsEnabled;
                config.PedestrianPathsHue = value.Hue;
            });

            BindCategory(_pinkPaths, (config, value) =>
            {
                config.HighlightPinkPaths = value.IsEnabled;
                config.PinkPathsHue = value.Hue;
            });

            BindCategory(_terraformingNetworks, (config, value) =>
            {
                config.HighlightTerraformingNetworks = value.IsEnabled;
                config.TerraformingNetworksHue = value.Hue;
            });

            BindCategory(_roads, (config, value) =>
            {
                config.HighlightRoads = value.IsEnabled;
                config.RoadsHue = value.Hue;
            });

            BindCategory(_highways, (config, value) =>
            {
                config.HighlightHighways = value.IsEnabled;
                config.HighwaysHue = value.Hue;
            });

            BindCategory(_trainTracks, (config, value) =>
            {
                config.HighlightTrainTracks = value.IsEnabled;
                config.TrainTracksHue = value.Hue;
            });

            BindCategory(_metroTracks, (config, value) =>
            {
                config.HighlightMetroTracks = value.IsEnabled;
                config.MetroTracksHue = value.Hue;
            });

            BindCategory(_tramTracks, (config, value) =>
            {
                config.HighlightTramTracks = value.IsEnabled;
                config.TramTracksHue = value.Hue;
            });

            BindCategory(_monorailTracks, (config, value) =>
            {
                config.HighlightMonorailTracks = value.IsEnabled;
                config.MonorailHue = value.Hue;
            });

            BindCategory(_cableCars, (config, value) =>
            {
                config.HighlightCableCars = value.IsEnabled;
                config.CableCarsHue = value.Hue;
            });

            BindBool(_highlightBridges, (config, value) => config.HighlightBridges = value);
            BindBool(_highlightTunnels, (config, value) => config.HighlightTunnels = value);
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

        private static void SetFloat(Observable<float> state, float value)
        {
            if (Mathf.Approximately(state.Value, value)) return;

            state.Value = value;
        }

        private static void SetBool(Observable<bool> state, bool value)
        {
            if (state.Value == value) return;

            state.Value = value;
        }

        private static void SetCategory(
            Observable<HighlightCategorySetting> state,
            HighlightCategorySetting value)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.Equals(value)) return;

            state.Value = value;
        }

        private static void SetCategoryEnabled(Observable<HighlightCategorySetting> state, bool isEnabled)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (currentValue.IsEnabled == isEnabled) return;

            state.Value = currentValue.WithEnabled(isEnabled);
        }

        private static void SetCategoryHue(Observable<HighlightCategorySetting> state, float hue)
        {
            HighlightCategorySetting currentValue = state.Value;
            if (Mathf.Approximately(currentValue.Hue, hue)) return;

            state.Value = currentValue.WithHue(hue);
        }

        public static float PanelX
        {
            get => _panelX.Value;
            set => SetFloat(_panelX, value);
        }

        public static float PanelY
        {
            get => _panelY.Value;
            set => SetFloat(_panelY, value);
        }

        public static float HighlightStrength
        {
            get => _highlightStrength.Value;
            set => SetFloat(_highlightStrength, value);
        }

        public static float HighlightWidth
        {
            get => _highlightWidth.Value;
            set => SetFloat(_highlightWidth, value);
        }

        public static float PedestrianPathsHue
        {
            get => _pedestrianPaths.Value.Hue;
            set => SetCategoryHue(_pedestrianPaths, value);
        }

        public static Color PedestrianPathColor
        {
            get => ColorConversion.FromHue(PedestrianPathsHue, HighlightStrength);
            set => PedestrianPathsHue = ColorConversion.ToHue(value);
        }

        public static float PinkPathsHue
        {
            get => _pinkPaths.Value.Hue;
            set => SetCategoryHue(_pinkPaths, value);
        }

        public static Color PinkPathColor
        {
            get => ColorConversion.FromHue(PinkPathsHue, HighlightStrength);
            set => PinkPathsHue = ColorConversion.ToHue(value);
        }

        public static float TerraformingNetworksHue
        {
            get => _terraformingNetworks.Value.Hue;
            set => SetCategoryHue(_terraformingNetworks, value);
        }

        public static Color TerraformingNetworksColor
        {
            get => ColorConversion.FromHue(TerraformingNetworksHue, HighlightStrength);
            set => TerraformingNetworksHue = ColorConversion.ToHue(value);
        }

        public static float RoadsHue
        {
            get => _roads.Value.Hue;
            set => SetCategoryHue(_roads, value);
        }

        public static Color RoadsColor
        {
            get => ColorConversion.FromHue(RoadsHue, HighlightStrength);
            set => RoadsHue = ColorConversion.ToHue(value);
        }

        public static float HighwaysHue
        {
            get => _highways.Value.Hue;
            set => SetCategoryHue(_highways, value);
        }

        public static Color HighwaysColor
        {
            get => ColorConversion.FromHue(HighwaysHue, HighlightStrength);
            set => HighwaysHue = ColorConversion.ToHue(value);
        }

        public static float TrainTracksHue
        {
            get => _trainTracks.Value.Hue;
            set => SetCategoryHue(_trainTracks, value);
        }

        public static Color TrainTracksColor
        {
            get => ColorConversion.FromHue(TrainTracksHue, HighlightStrength);
            set => TrainTracksHue = ColorConversion.ToHue(value);
        }

        public static float MetroTracksHue
        {
            get => _metroTracks.Value.Hue;
            set => SetCategoryHue(_metroTracks, value);
        }

        public static Color MetroTracksColor
        {
            get => ColorConversion.FromHue(MetroTracksHue, HighlightStrength);
            set => MetroTracksHue = ColorConversion.ToHue(value);
        }

        public static float TramTracksHue
        {
            get => _tramTracks.Value.Hue;
            set => SetCategoryHue(_tramTracks, value);
        }

        public static Color TramTracksColor
        {
            get => ColorConversion.FromHue(TramTracksHue, HighlightStrength);
            set => TramTracksHue = ColorConversion.ToHue(value);
        }

        public static float MonorailTracksHue
        {
            get => _monorailTracks.Value.Hue;
            set => SetCategoryHue(_monorailTracks, value);
        }

        public static Color MonorailTracksColor
        {
            get => ColorConversion.FromHue(MonorailTracksHue, HighlightStrength);
            set => MonorailTracksHue = ColorConversion.ToHue(value);
        }

        public static float CableCarsHue
        {
            get => _cableCars.Value.Hue;
            set => SetCategoryHue(_cableCars, value);
        }

        public static Color CableCarColor
        {
            get => ColorConversion.FromHue(CableCarsHue, HighlightStrength);
            set => CableCarsHue = ColorConversion.ToHue(value);
        }

        public static bool HighlightPedestrianPaths
        {
            get => _pedestrianPaths.Value.IsEnabled;
            set => SetCategoryEnabled(_pedestrianPaths, value);
        }

        public static bool HighlightPinkPaths
        {
            get => _pinkPaths.Value.IsEnabled;
            set => SetCategoryEnabled(_pinkPaths, value);
        }

        public static bool HighlightTerraformingNetworks
        {
            get => _terraformingNetworks.Value.IsEnabled;
            set => SetCategoryEnabled(_terraformingNetworks, value);
        }

        public static bool HighlightRoads
        {
            get => _roads.Value.IsEnabled;
            set => SetCategoryEnabled(_roads, value);
        }

        public static bool HighlightHighways
        {
            get => _highways.Value.IsEnabled;
            set => SetCategoryEnabled(_highways, value);
        }

        public static bool HighlightTrainTracks
        {
            get => _trainTracks.Value.IsEnabled;
            set => SetCategoryEnabled(_trainTracks, value);
        }

        public static bool HighlightMetroTracks
        {
            get => _metroTracks.Value.IsEnabled;
            set => SetCategoryEnabled(_metroTracks, value);
        }

        public static bool HighlightTramTracks
        {
            get => _tramTracks.Value.IsEnabled;
            set => SetCategoryEnabled(_tramTracks, value);
        }

        public static bool HighlightMonorailTracks
        {
            get => _monorailTracks.Value.IsEnabled;
            set => SetCategoryEnabled(_monorailTracks, value);
        }

        public static bool HighlightCableCars
        {
            get => _cableCars.Value.IsEnabled;
            set => SetCategoryEnabled(_cableCars, value);
        }

        public static bool HighlightBridges
        {
            get => _highlightBridges.Value;
            set => SetBool(_highlightBridges, value);
        }

        public static bool HighlightTunnels
        {
            get => _highlightTunnels.Value;
            set => SetBool(_highlightTunnels, value);
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
                SetFloat(_highlightStrength, source.HighlightStrength);
                SetFloat(_highlightWidth, source.HighlightWidth);

                SetCategory(_pedestrianPaths,
                    new HighlightCategorySetting(source.HighlightPedestrianPaths, source.PedestrianPathsHue));
                SetCategory(_pinkPaths,
                    new HighlightCategorySetting(source.HighlightPinkPaths, source.PinkPathsHue));
                SetCategory(_terraformingNetworks,
                    new HighlightCategorySetting(source.HighlightTerraformingNetworks, source.TerraformingNetworksHue));
                SetCategory(_roads,
                    new HighlightCategorySetting(source.HighlightRoads, source.RoadsHue));
                SetCategory(_highways,
                    new HighlightCategorySetting(source.HighlightHighways, source.HighwaysHue));
                SetCategory(_trainTracks,
                    new HighlightCategorySetting(source.HighlightTrainTracks, source.TrainTracksHue));
                SetCategory(_metroTracks,
                    new HighlightCategorySetting(source.HighlightMetroTracks, source.MetroTracksHue));
                SetCategory(_tramTracks,
                    new HighlightCategorySetting(source.HighlightTramTracks, source.TramTracksHue));
                SetCategory(_monorailTracks,
                    new HighlightCategorySetting(source.HighlightMonorailTracks, source.MonorailHue));
                SetCategory(_cableCars,
                    new HighlightCategorySetting(source.HighlightCableCars, source.CableCarsHue));

                SetBool(_highlightBridges, source.HighlightBridges);
                SetBool(_highlightTunnels, source.HighlightTunnels);

                SetFloat(_panelX, source.PanelX);
                SetFloat(_panelY, source.PanelY);
            }
            finally
            {
                _suppressSaveAndRaise = false;
            }

            SaveAndRaise();
        }
    }
}
