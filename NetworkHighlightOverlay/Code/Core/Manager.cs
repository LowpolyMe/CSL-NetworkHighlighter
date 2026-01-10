using ColossalFramework;
using NetworkHighlightOverlay.Code.ModOptions;

namespace NetworkHighlightOverlay.Code.Core
{
    public class Manager
    {
        private bool _isEnabled;
        private readonly HighlightCache _cache = new HighlightCache();
        private readonly OverlayRenderer _renderer = new OverlayRenderer();

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;

                if (_isEnabled)
                {
                    _cache.RebuildCache();
                }
                else
                {
                    _cache.Clear();
                }
            }
        }

        private static readonly Manager _instance = new Manager();
        public static Manager Instance => _instance;

        private Manager()
        {
            ModSettings.SettingsChanged += _ => OnSettingsChanged();
        }

        private void OnSettingsChanged()
        {
            if (_isEnabled)
                _cache.RebuildCache();
            else
                _cache.Clear();
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public void RebuildCache()
        {
            _cache.RebuildCache();
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            _cache.OnSegmentCreated(segmentId);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            _cache.OnSegmentReleased(segmentId);
        }

        public void RenderIfActive(RenderManager.CameraInfo cameraInfo)
        {
            if (!_isEnabled)
                return;

            _renderer.Render(cameraInfo, _cache.GetSegments());
        }
    }
}
