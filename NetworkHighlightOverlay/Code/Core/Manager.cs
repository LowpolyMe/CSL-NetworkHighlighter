using System.Collections.Generic;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public class Manager
    {
        private readonly HighlightCache _cache = new HighlightCache();
        private readonly OverlayRenderer _renderer = new OverlayRenderer();
        private readonly List<KeyValuePair<ushort, Color>> _segmentSnapshot =
            new List<KeyValuePair<ushort, Color>>(1024);
        private bool _hasCacheSnapshot;
        private bool _isCacheDirty;

        private static readonly Manager _instance = new Manager();
        public static Manager Instance => _instance;

        private Manager()
        {
            _hasCacheSnapshot = false;
            _isCacheDirty = false;
        }

        public void OnActivated()
        {
            if (!_hasCacheSnapshot || _isCacheDirty)
            {
                _cache.RebuildCache();
                _hasCacheSnapshot = true;
                _isCacheDirty = false;
            }
        }

        public void OnDeactivated()
        {
        }

        public void ResetForLevelUnload()
        {
            _cache.Clear();
            _hasCacheSnapshot = false;
            _isCacheDirty = false;
        }

        public void OnHighlightRulesChanged()
        {
            if (!ActivationHandler.IsActive)
            {
                MarkCacheDirty();
                return;
            }

            _cache.RebuildCache();
            _hasCacheSnapshot = true;
            _isCacheDirty = false;
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (!ActivationHandler.IsActive)
            {
                MarkCacheDirty();
                return;
            }

            _cache.OnSegmentCreated(segmentId);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (!ActivationHandler.IsActive)
            {
                MarkCacheDirty();
                return;
            }

            _cache.OnSegmentReleased(segmentId);
        }

        public void RenderIfActive(RenderManager.CameraInfo cameraInfo)
        {
            if (!ActivationHandler.IsActive)
                return;

            _cache.CopySegmentsTo(_segmentSnapshot);
            _renderer.Render(cameraInfo, _segmentSnapshot);
        }

        private void MarkCacheDirty()
        {
            _isCacheDirty = true;
        }
    }
}
