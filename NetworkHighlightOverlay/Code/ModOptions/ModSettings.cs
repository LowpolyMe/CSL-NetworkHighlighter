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

        private static readonly Observable<float> _pedestrianPathsHue;
        private static readonly Observable<float> _pinkPathsHue;
        private static readonly Observable<float> _terraformingNetworksHue;
        private static readonly Observable<float> _roadsHue;
        private static readonly Observable<float> _highwaysHue;
        private static readonly Observable<float> _trainTracksHue;
        private static readonly Observable<float> _metroTracksHue;
        private static readonly Observable<float> _tramTracksHue;
        private static readonly Observable<float> _monorailHue;
        private static readonly Observable<float> _cableCarsHue;

        private static readonly Observable<bool> _highlightPedestrianPaths;
        private static readonly Observable<bool> _highlightPinkPaths;
        private static readonly Observable<bool> _highlightTerraformingNetworks;
        private static readonly Observable<bool> _highlightRoads;
        private static readonly Observable<bool> _highlightHighways;
        private static readonly Observable<bool> _highlightTrainTracks;
        private static readonly Observable<bool> _highlightMetroTracks;
        private static readonly Observable<bool> _highlightTramTracks;
        private static readonly Observable<bool> _highlightMonorailTracks;
        private static readonly Observable<bool> _highlightCableCars;
        private static readonly Observable<bool> _highlightBridges;
        private static readonly Observable<bool> _highlightTunnels;

        private static bool _suppressSaveAndRaise;

        public static Observable<long> ChangeVersion => _changeVersion;

        public static Observable<float> PanelXState => _panelX;
        public static Observable<float> PanelYState => _panelY;
        public static Observable<float> HighlightStrengthState => _highlightStrength;
        public static Observable<float> HighlightWidthState => _highlightWidth;

        public static Observable<float> PedestrianPathsHueState => _pedestrianPathsHue;
        public static Observable<float> PinkPathsHueState => _pinkPathsHue;
        public static Observable<float> TerraformingNetworksHueState => _terraformingNetworksHue;
        public static Observable<float> RoadsHueState => _roadsHue;
        public static Observable<float> HighwaysHueState => _highwaysHue;
        public static Observable<float> TrainTracksHueState => _trainTracksHue;
        public static Observable<float> MetroTracksHueState => _metroTracksHue;
        public static Observable<float> TramTracksHueState => _tramTracksHue;
        public static Observable<float> MonorailTracksHueState => _monorailHue;
        public static Observable<float> CableCarsHueState => _cableCarsHue;

        public static Observable<bool> HighlightPedestrianPathsState => _highlightPedestrianPaths;
        public static Observable<bool> HighlightPinkPathsState => _highlightPinkPaths;
        public static Observable<bool> HighlightTerraformingNetworksState => _highlightTerraformingNetworks;
        public static Observable<bool> HighlightRoadsState => _highlightRoads;
        public static Observable<bool> HighlightHighwaysState => _highlightHighways;
        public static Observable<bool> HighlightTrainTracksState => _highlightTrainTracks;
        public static Observable<bool> HighlightMetroTracksState => _highlightMetroTracks;
        public static Observable<bool> HighlightTramTracksState => _highlightTramTracks;
        public static Observable<bool> HighlightMonorailTracksState => _highlightMonorailTracks;
        public static Observable<bool> HighlightCableCarsState => _highlightCableCars;
        public static Observable<bool> HighlightBridgesState => _highlightBridges;
        public static Observable<bool> HighlightTunnelsState => _highlightTunnels;

        static ModSettings()
        {
            _config = SettingsLoader.Load();

            _panelX = new Observable<float>(_config.PanelX);
            _panelY = new Observable<float>(_config.PanelY);

            _highlightStrength = new Observable<float>(_config.HighlightStrength);
            _highlightWidth = new Observable<float>(_config.HighlightWidth);

            _pedestrianPathsHue = new Observable<float>(_config.PedestrianPathsHue);
            _pinkPathsHue = new Observable<float>(_config.PinkPathsHue);
            _terraformingNetworksHue = new Observable<float>(_config.TerraformingNetworksHue);
            _roadsHue = new Observable<float>(_config.RoadsHue);
            _highwaysHue = new Observable<float>(_config.HighwaysHue);
            _trainTracksHue = new Observable<float>(_config.TrainTracksHue);
            _metroTracksHue = new Observable<float>(_config.MetroTracksHue);
            _tramTracksHue = new Observable<float>(_config.TramTracksHue);
            _monorailHue = new Observable<float>(_config.MonorailHue);
            _cableCarsHue = new Observable<float>(_config.CableCarsHue);

            _highlightPedestrianPaths = new Observable<bool>(_config.HighlightPedestrianPaths);
            _highlightPinkPaths = new Observable<bool>(_config.HighlightPinkPaths);
            _highlightTerraformingNetworks = new Observable<bool>(_config.HighlightTerraformingNetworks);
            _highlightRoads = new Observable<bool>(_config.HighlightRoads);
            _highlightHighways = new Observable<bool>(_config.HighlightHighways);
            _highlightTrainTracks = new Observable<bool>(_config.HighlightTrainTracks);
            _highlightMetroTracks = new Observable<bool>(_config.HighlightMetroTracks);
            _highlightTramTracks = new Observable<bool>(_config.HighlightTramTracks);
            _highlightMonorailTracks = new Observable<bool>(_config.HighlightMonorailTracks);
            _highlightCableCars = new Observable<bool>(_config.HighlightCableCars);
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

            BindFloat(_pedestrianPathsHue, (config, value) => config.PedestrianPathsHue = value);
            BindFloat(_pinkPathsHue, (config, value) => config.PinkPathsHue = value);
            BindFloat(_terraformingNetworksHue, (config, value) => config.TerraformingNetworksHue = value);
            BindFloat(_roadsHue, (config, value) => config.RoadsHue = value);
            BindFloat(_highwaysHue, (config, value) => config.HighwaysHue = value);
            BindFloat(_trainTracksHue, (config, value) => config.TrainTracksHue = value);
            BindFloat(_metroTracksHue, (config, value) => config.MetroTracksHue = value);
            BindFloat(_tramTracksHue, (config, value) => config.TramTracksHue = value);
            BindFloat(_monorailHue, (config, value) => config.MonorailHue = value);
            BindFloat(_cableCarsHue, (config, value) => config.CableCarsHue = value);

            BindBool(_highlightPedestrianPaths, (config, value) => config.HighlightPedestrianPaths = value);
            BindBool(_highlightPinkPaths, (config, value) => config.HighlightPinkPaths = value);
            BindBool(_highlightTerraformingNetworks, (config, value) => config.HighlightTerraformingNetworks = value);
            BindBool(_highlightRoads, (config, value) => config.HighlightRoads = value);
            BindBool(_highlightHighways, (config, value) => config.HighlightHighways = value);
            BindBool(_highlightTrainTracks, (config, value) => config.HighlightTrainTracks = value);
            BindBool(_highlightMetroTracks, (config, value) => config.HighlightMetroTracks = value);
            BindBool(_highlightTramTracks, (config, value) => config.HighlightTramTracks = value);
            BindBool(_highlightMonorailTracks, (config, value) => config.HighlightMonorailTracks = value);
            BindBool(_highlightCableCars, (config, value) => config.HighlightCableCars = value);
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

        private static void SaveAndRaise()
        {
            if (_suppressSaveAndRaise)
                return;

            SettingsLoader.Save(_config);
            _changeVersion.Update(IncrementVersion);
        }

        private static long IncrementVersion(long version)
        {
            return version == long.MaxValue ? 0L : version + 1L;
        }

        private static void SetFloat(Observable<float> state, float value)
        {
            if (Mathf.Approximately(state.Value, value))
                return;

            state.Value = value;
        }

        private static void SetBool(Observable<bool> state, bool value)
        {
            if (state.Value == value)
                return;

            state.Value = value;
        }

        #region GUI Settings
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
        #endregion

        #region Hues (float) + Colors
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
            get => _pedestrianPathsHue.Value;
            set => SetFloat(_pedestrianPathsHue, value);
        }

        public static Color PedestrianPathColor
        {
            get => ColorConversion.FromHue(PedestrianPathsHue, HighlightStrength);
            set => PedestrianPathsHue = ColorConversion.ToHue(value);
        }

        public static float PinkPathsHue
        {
            get => _pinkPathsHue.Value;
            set => SetFloat(_pinkPathsHue, value);
        }

        public static Color PinkPathColor
        {
            get => ColorConversion.FromHue(PinkPathsHue, HighlightStrength);
            set => PinkPathsHue = ColorConversion.ToHue(value);
        }

        public static float TerraformingNetworksHue
        {
            get => _terraformingNetworksHue.Value;
            set => SetFloat(_terraformingNetworksHue, value);
        }

        public static Color TerraformingNetworksColor
        {
            get => ColorConversion.FromHue(TerraformingNetworksHue, HighlightStrength);
            set => TerraformingNetworksHue = ColorConversion.ToHue(value);
        }

        public static float RoadsHue
        {
            get => _roadsHue.Value;
            set => SetFloat(_roadsHue, value);
        }

        public static Color RoadsColor
        {
            get => ColorConversion.FromHue(RoadsHue, HighlightStrength);
            set => RoadsHue = ColorConversion.ToHue(value);
        }

        public static float HighwaysHue
        {
            get => _highwaysHue.Value;
            set => SetFloat(_highwaysHue, value);
        }

        public static Color HighwaysColor
        {
            get => ColorConversion.FromHue(HighwaysHue, HighlightStrength);
            set => HighwaysHue = ColorConversion.ToHue(value);
        }

        public static float TrainTracksHue
        {
            get => _trainTracksHue.Value;
            set => SetFloat(_trainTracksHue, value);
        }

        public static Color TrainTracksColor
        {
            get => ColorConversion.FromHue(TrainTracksHue, HighlightStrength);
            set => TrainTracksHue = ColorConversion.ToHue(value);
        }

        public static float MetroTracksHue
        {
            get => _metroTracksHue.Value;
            set => SetFloat(_metroTracksHue, value);
        }

        public static Color MetroTracksColor
        {
            get => ColorConversion.FromHue(MetroTracksHue, HighlightStrength);
            set => MetroTracksHue = ColorConversion.ToHue(value);
        }

        public static float TramTracksHue
        {
            get => _tramTracksHue.Value;
            set => SetFloat(_tramTracksHue, value);
        }

        public static Color TramTracksColor
        {
            get => ColorConversion.FromHue(TramTracksHue, HighlightStrength);
            set => TramTracksHue = ColorConversion.ToHue(value);
        }

        public static float MonorailTracksHue
        {
            get => _monorailHue.Value;
            set => SetFloat(_monorailHue, value);
        }

        public static Color MonorailTracksColor
        {
            get => ColorConversion.FromHue(MonorailTracksHue, HighlightStrength);
            set => MonorailTracksHue = ColorConversion.ToHue(value);
        }

        public static float CableCarsHue
        {
            get => _cableCarsHue.Value;
            set => SetFloat(_cableCarsHue, value);
        }

        public static Color CableCarColor
        {
            get => ColorConversion.FromHue(CableCarsHue, HighlightStrength);
            set => CableCarsHue = ColorConversion.ToHue(value);
        }
        #endregion

        #region Highlight toggles
        public static bool HighlightPedestrianPaths
        {
            get => _highlightPedestrianPaths.Value;
            set => SetBool(_highlightPedestrianPaths, value);
        }

        public static bool HighlightPinkPaths
        {
            get => _highlightPinkPaths.Value;
            set => SetBool(_highlightPinkPaths, value);
        }

        public static bool HighlightTerraformingNetworks
        {
            get => _highlightTerraformingNetworks.Value;
            set => SetBool(_highlightTerraformingNetworks, value);
        }

        public static bool HighlightRoads
        {
            get => _highlightRoads.Value;
            set => SetBool(_highlightRoads, value);
        }

        public static bool HighlightHighways
        {
            get => _highlightHighways.Value;
            set => SetBool(_highlightHighways, value);
        }

        public static bool HighlightTrainTracks
        {
            get => _highlightTrainTracks.Value;
            set => SetBool(_highlightTrainTracks, value);
        }

        public static bool HighlightMetroTracks
        {
            get => _highlightMetroTracks.Value;
            set => SetBool(_highlightMetroTracks, value);
        }

        public static bool HighlightTramTracks
        {
            get => _highlightTramTracks.Value;
            set => SetBool(_highlightTramTracks, value);
        }

        public static bool HighlightMonorailTracks
        {
            get => _highlightMonorailTracks.Value;
            set => SetBool(_highlightMonorailTracks, value);
        }

        public static bool HighlightCableCars
        {
            get => _highlightCableCars.Value;
            set => SetBool(_highlightCableCars, value);
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
        #endregion

        #region Reset
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

                SetFloat(_pedestrianPathsHue, source.PedestrianPathsHue);
                SetFloat(_pinkPathsHue, source.PinkPathsHue);
                SetFloat(_terraformingNetworksHue, source.TerraformingNetworksHue);
                SetFloat(_roadsHue, source.RoadsHue);
                SetFloat(_highwaysHue, source.HighwaysHue);
                SetFloat(_trainTracksHue, source.TrainTracksHue);
                SetFloat(_metroTracksHue, source.MetroTracksHue);
                SetFloat(_tramTracksHue, source.TramTracksHue);
                SetFloat(_monorailHue, source.MonorailHue);
                SetFloat(_cableCarsHue, source.CableCarsHue);

                SetBool(_highlightPedestrianPaths, source.HighlightPedestrianPaths);
                SetBool(_highlightPinkPaths, source.HighlightPinkPaths);
                SetBool(_highlightTerraformingNetworks, source.HighlightTerraformingNetworks);
                SetBool(_highlightRoads, source.HighlightRoads);
                SetBool(_highlightHighways, source.HighlightHighways);
                SetBool(_highlightTrainTracks, source.HighlightTrainTracks);
                SetBool(_highlightMetroTracks, source.HighlightMetroTracks);
                SetBool(_highlightTramTracks, source.HighlightTramTracks);
                SetBool(_highlightMonorailTracks, source.HighlightMonorailTracks);
                SetBool(_highlightCableCars, source.HighlightCableCars);
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
        #endregion
    }
}
