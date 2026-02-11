using System.Collections.Generic;
using System;
using ColossalFramework;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public class Manager
    {
        private readonly Observable<bool> _isEnabled = new Observable<bool>(false);
        private readonly IDisposable _enabledSubscription;
        private readonly IDisposable _settingsSubscription;
        private readonly HighlightCache _cache = new HighlightCache();
        private readonly OverlayRenderer _renderer = new OverlayRenderer();
        private readonly List<KeyValuePair<ushort, Color>> _segmentSnapshot =
            new List<KeyValuePair<ushort, Color>>(1024);

        public Observable<bool> EnabledState => _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled.Value;
            set => _isEnabled.Value = value;
        }

        private static readonly Manager _instance = new Manager();
        public static Manager Instance => _instance;

        private Manager()
        {
            _enabledSubscription = _isEnabled.Subscribe(OnEnabledStateChanged);
            _settingsSubscription = ModSettings.ChangeVersion.Subscribe(OnSettingsChanged);
        }

        private void OnEnabledStateChanged(bool previousState, bool isEnabled)
        {
            if (isEnabled)
            {
                _cache.RebuildCache();
            }
            else
            {
                _cache.Clear();
            }
        }

        private void OnSettingsChanged(long previousVersion, long currentVersion)
        {
            if (_isEnabled.Value)
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
            if (!_isEnabled.Value)
                return;

            _cache.CopySegmentsTo(_segmentSnapshot);
            _renderer.Render(cameraInfo, _segmentSnapshot);
        }
    }
}
