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

        private static readonly Manager _instance = new Manager();
        public static Manager Instance => _instance;

        private Manager()
        {
        }

        public void OnActivated()
        {
            _cache.RebuildCache();
        }

        public void OnDeactivated()
        {
            _cache.Clear();
        }

        public void OnHighlightRulesChanged()
        {
            if (!ActivationHandler.IsActive)
            {
                return;
            }

            _cache.RebuildCache();
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (!ActivationHandler.IsActive)
            {
                return;
            }

            _cache.OnSegmentCreated(segmentId);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (!ActivationHandler.IsActive)
            {
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
    }
}
