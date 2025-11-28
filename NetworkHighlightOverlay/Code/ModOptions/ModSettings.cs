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
        
  #region Hues (float) + Colors

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
            get => ColorConversion.FromHue(PedestrianPathsHue);
            set => PedestrianPathsHue = ColorConversion.ToHue(value);
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
            get => ColorConversion.FromHue(RoadsHue);
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
            get => ColorConversion.FromHue(HighwaysHue);
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
            get => ColorConversion.FromHue(TrainTracksHue);
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
            get => ColorConversion.FromHue(MetroTracksHue);
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
            get => ColorConversion.FromHue(TramTracksHue);
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
            get => ColorConversion.FromHue(MonorailTracksHue);
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
            get => ColorConversion.FromHue(CableCarsHue);
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
            // Hues
            _config.PedestrianPathsHue = 0.85f;
            _config.RoadsHue           = 0.85f;
            _config.HighwaysHue        = 0.85f;
            _config.TrainTracksHue     = 0.85f;
            _config.MetroTracksHue     = 0.85f;
            _config.TramTracksHue      = 0.85f;
            _config.MonorailHue        = 0.85f;
            _config.CableCarsHue       = 0.85f;

            // Toggles
            _config.HighlightPedestrianPaths = true;
            _config.HighlightRoads           = true;
            _config.HighlightHighways        = true;
            _config.HighlightTrainTracks     = true;
            _config.HighlightMetroTracks     = true;
            _config.HighlightTramTracks      = true;
            _config.HighlightMonorailTracks  = true;
            _config.HighlightCableCars       = true;
            _config.HighlightBridges         = true;
            _config.HighlightTunnels         = true;

            SaveAndRaise();
        }

        #endregion
        
    }
}