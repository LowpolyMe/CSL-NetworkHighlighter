using System.Collections.Generic;
using System;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class Manager
    {
        private readonly HighlightCache _cache;
        private readonly OverlayRenderer _renderer;
        private readonly List<KeyValuePair<ushort, Color>> _segmentSnapshot =
            new List<KeyValuePair<ushort, Color>>(1024);
        private bool _hasCacheSnapshot;
        private bool _isCacheDirty;
        private bool _isActive;

        public Manager(ModSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _cache = new HighlightCache(settings);
            _renderer = new OverlayRenderer(settings);
            _hasCacheSnapshot = false;
            _isCacheDirty = false;
            _isActive = false;
        }

        public void OnActivated()
        {
            _isActive = true;

            if (!_hasCacheSnapshot || _isCacheDirty)
            {
                _cache.RebuildCache();
                _hasCacheSnapshot = true;
                _isCacheDirty = false;
            }
        }

        public void OnDeactivated()
        {
            _isActive = false;
        }

        public void ResetForLevelUnload()
        {
            _cache.Clear();
            _hasCacheSnapshot = false;
            _isCacheDirty = false;
            _isActive = false;
        }

        public void OnHighlightRulesChanged()
        {
            if (!_isActive)
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
            if (!_isActive)
            {
                MarkCacheDirty();
                return;
            }

            _cache.OnSegmentCreated(segmentId);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (!_isActive)
            {
                MarkCacheDirty();
                return;
            }

            _cache.OnSegmentReleased(segmentId);
        }

        public void RenderIfActive(RenderManager.CameraInfo cameraInfo)
        {
            if (!_isActive)
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
