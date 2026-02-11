using System;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class ModSettings
    {
        private static readonly Config _config;
        
        
        public static event Action<Config> SettingsChanged;

        static ModSettings()
        {
            _config = SettingsLoader.Load();
        }
        
        private static void SaveAndRaise()
        {
            SettingsLoader.Save(_config);
            SettingsChanged?.Invoke(_config);
        }
        
        #region GUI Settings
        public static float PanelX
        {
            get => _config.PanelX;
            set
            {
                if (Mathf.Approximately(_config.PanelX, value))
                    return;

                _config.PanelX = value;
                SaveAndRaise();
            }
        }

        public static float PanelY
        {
            get => _config.PanelY;
            set
            {
                if (Mathf.Approximately(_config.PanelY, value))
                    return;

                _config.PanelY = value;
                SaveAndRaise();
            }
        }
        
        #endregion
        #region Hues (float) + Colors

        public static float HighlightStrength
        {
            get => _config.HighlightStrength;
            set
            {
                if (Mathf.Approximately(_config.HighlightStrength, value))
                    return;

                _config.HighlightStrength = value;
                SaveAndRaise();
            }
        }
        public static float HighlightWidth
        {
            get => _config.HighlightWidth;
            set
            {
                if (Mathf.Approximately(_config.HighlightWidth, value))
                    return;

                _config.HighlightWidth = value;
                SaveAndRaise();
            }
        }
        public static float PedestrianPathsHue
        {
            get => _config.PedestrianPathsHue;
            set
            {
                if (Mathf.Approximately(_config.PedestrianPathsHue, value))
                    return;

                _config.PedestrianPathsHue = value;
                SaveAndRaise();
            }
        }

        public static Color PedestrianPathColor
        {
            get => ColorConversion.FromHue(PedestrianPathsHue, HighlightStrength);
            set => PedestrianPathsHue = ColorConversion.ToHue(value);
        }

        public static float PinkPathsHue
        {
            get => _config.PinkPathsHue;
            set
            {
                if (Mathf.Approximately(_config.PinkPathsHue, value))
                    return;

                _config.PinkPathsHue = value;
                SaveAndRaise();
            }
        }

        public static Color PinkPathColor
        {
            get => ColorConversion.FromHue(PinkPathsHue, HighlightStrength);
            set => PinkPathsHue = ColorConversion.ToHue(value);
        }

        public static float TerraformingNetworksHue
        {
            get => _config.TerraformingNetworksHue;
            set
            {
                if (Mathf.Approximately(_config.TerraformingNetworksHue, value))
                    return;

                _config.TerraformingNetworksHue = value;
                SaveAndRaise();
            }
        }

        public static Color TerraformingNetworksColor
        {
            get => ColorConversion.FromHue(TerraformingNetworksHue, HighlightStrength);
            set => TerraformingNetworksHue = ColorConversion.ToHue(value);
        }

        public static float RoadsHue
        {
            get => _config.RoadsHue;
            set
            {
                if (Mathf.Approximately(_config.RoadsHue, value))
                    return;

                _config.RoadsHue = value;
                SaveAndRaise();
            }
        }

        public static Color RoadsColor
        {
            get => ColorConversion.FromHue(RoadsHue, HighlightStrength);
            set => RoadsHue = ColorConversion.ToHue(value);
        }

        public static float HighwaysHue
        {
            get => _config.HighwaysHue;
            set
            {
                if (Mathf.Approximately(_config.HighwaysHue, value))
                    return;

                _config.HighwaysHue = value;
                SaveAndRaise();
            }
        }

        public static Color HighwaysColor
        {
            get => ColorConversion.FromHue(HighwaysHue, HighlightStrength);
            set => HighwaysHue = ColorConversion.ToHue(value);
        }

        public static float TrainTracksHue
        {
            get => _config.TrainTracksHue;
            set
            {
                if (Mathf.Approximately(_config.TrainTracksHue, value))
                    return;

                _config.TrainTracksHue = value;
                SaveAndRaise();
            }
        }

        public static Color TrainTracksColor
        {
            get => ColorConversion.FromHue(TrainTracksHue, HighlightStrength);
            set => TrainTracksHue = ColorConversion.ToHue(value);
        }

        public static float MetroTracksHue
        {
            get => _config.MetroTracksHue;
            set
            {
                if (Mathf.Approximately(_config.MetroTracksHue, value))
                    return;

                _config.MetroTracksHue = value;
                SaveAndRaise();
            }
        }

        public static Color MetroTracksColor
        {
            get => ColorConversion.FromHue(MetroTracksHue, HighlightStrength);
            set => MetroTracksHue = ColorConversion.ToHue(value);
        }

        public static float TramTracksHue
        {
            get => _config.TramTracksHue;
            set
            {
                if (Mathf.Approximately(_config.TramTracksHue, value))
                    return;

                _config.TramTracksHue = value;
                SaveAndRaise();
            }
        }

        public static Color TramTracksColor
        {
            get => ColorConversion.FromHue(TramTracksHue, HighlightStrength);
            set => TramTracksHue = ColorConversion.ToHue(value);
        }

        public static float MonorailTracksHue
        {
            get => _config.MonorailHue;
            set
            {
                if (Mathf.Approximately(_config.MonorailHue, value))
                    return;

                _config.MonorailHue = value;
                SaveAndRaise();
            }
        }

        public static Color MonorailTracksColor
        {
            get => ColorConversion.FromHue(MonorailTracksHue, HighlightStrength);
            set => MonorailTracksHue = ColorConversion.ToHue(value);
        }

        public static float CableCarsHue
        {
            get => _config.CableCarsHue;
            set
            {
                if (Mathf.Approximately(_config.CableCarsHue, value))
                    return;

                _config.CableCarsHue = value;
                SaveAndRaise();
            }
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
            get => _config.HighlightPedestrianPaths;
            set
            {
                if (_config.HighlightPedestrianPaths == value)
                    return;

                _config.HighlightPedestrianPaths = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightPinkPaths
        {
            get => _config.HighlightPinkPaths;
            set
            {
                if (_config.HighlightPinkPaths == value)
                    return;

                _config.HighlightPinkPaths = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightTerraformingNetworks
        {
            get => _config.HighlightTerraformingNetworks;
            set
            {
                if (_config.HighlightTerraformingNetworks == value)
                    return;

                _config.HighlightTerraformingNetworks = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightRoads
        {
            get => _config.HighlightRoads;
            set
            {
                if (_config.HighlightRoads == value)
                    return;

                _config.HighlightRoads = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightHighways
        {
            get => _config.HighlightHighways;
            set
            {
                if (_config.HighlightHighways == value)
                    return;

                _config.HighlightHighways = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightTrainTracks
        {
            get => _config.HighlightTrainTracks;
            set
            {
                if (_config.HighlightTrainTracks == value)
                    return;

                _config.HighlightTrainTracks = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightMetroTracks
        {
            get => _config.HighlightMetroTracks;
            set
            {
                if (_config.HighlightMetroTracks == value)
                    return;

                _config.HighlightMetroTracks = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightTramTracks
        {
            get => _config.HighlightTramTracks;
            set
            {
                if (_config.HighlightTramTracks == value)
                    return;

                _config.HighlightTramTracks = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightMonorailTracks
        {
            get => _config.HighlightMonorailTracks;
            set
            {
                if (_config.HighlightMonorailTracks == value)
                    return;

                _config.HighlightMonorailTracks = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightCableCars
        {
            get => _config.HighlightCableCars;
            set
            {
                if (_config.HighlightCableCars == value)
                    return;

                _config.HighlightCableCars = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightBridges
        {
            get => _config.HighlightBridges;
            set
            {
                if (_config.HighlightBridges == value)
                    return;

                _config.HighlightBridges = value;
                SaveAndRaise();
            }
        }

        public static bool HighlightTunnels
        {
            get => _config.HighlightTunnels;
            set
            {
                if (_config.HighlightTunnels == value)
                    return;

                _config.HighlightTunnels = value;
                SaveAndRaise();
            }
        }

        #endregion

        #region Reset

        public static void ResetToDefaults()
        {
            ApplyConfig(new Config());

            SaveAndRaise();
        }

        private static void ApplyConfig(Config source)
        {
            _config.HighlightStrength = source.HighlightStrength;
            _config.HighlightWidth = source.HighlightWidth;

            _config.PedestrianPathsHue = source.PedestrianPathsHue;
            _config.PinkPathsHue = source.PinkPathsHue;
            _config.TerraformingNetworksHue = source.TerraformingNetworksHue;
            _config.RoadsHue = source.RoadsHue;
            _config.HighwaysHue = source.HighwaysHue;
            _config.TrainTracksHue = source.TrainTracksHue;
            _config.MetroTracksHue = source.MetroTracksHue;
            _config.TramTracksHue = source.TramTracksHue;
            _config.MonorailHue = source.MonorailHue;
            _config.CableCarsHue = source.CableCarsHue;

            _config.HighlightPedestrianPaths = source.HighlightPedestrianPaths;
            _config.HighlightPinkPaths = source.HighlightPinkPaths;
            _config.HighlightTerraformingNetworks = source.HighlightTerraformingNetworks;
            _config.HighlightRoads = source.HighlightRoads;
            _config.HighlightHighways = source.HighlightHighways;
            _config.HighlightTrainTracks = source.HighlightTrainTracks;
            _config.HighlightMetroTracks = source.HighlightMetroTracks;
            _config.HighlightTramTracks = source.HighlightTramTracks;
            _config.HighlightMonorailTracks = source.HighlightMonorailTracks;
            _config.HighlightCableCars = source.HighlightCableCars;
            _config.HighlightBridges = source.HighlightBridges;
            _config.HighlightTunnels = source.HighlightTunnels;
            _config.PanelX = source.PanelX;
            _config.PanelY = source.PanelY;
        }

        #endregion
        
    }
}
