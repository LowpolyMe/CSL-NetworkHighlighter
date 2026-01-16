using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class HighlightCache
    {
        private readonly Dictionary<ushort, Color> _highlightedSegments = new Dictionary<ushort, Color>();

        public IDictionary<ushort, Color> GetSegments()
        {
            return _highlightedSegments;
        }

        public void Clear()
        {
            _highlightedSegments.Clear();
        }

        public void RebuildCache()
        {
            _highlightedSegments.Clear();

            NetManager netManager = NetManager.instance;
            var segments = netManager.m_segments;

            for (ushort i = 1; i < segments.m_size; i++)
            {
                ref NetSegment segment = ref segments.m_buffer[i];

                if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                    continue;

                TryAddSegmentInternal(i, ref segment);
            }
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (segmentId == 0)
                return;

            ref NetSegment segment = ref NetManager.instance.m_segments.m_buffer[segmentId];
            if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                return;

            TryAddSegmentInternal(segmentId, ref segment);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (segmentId == 0)
                return;

            _highlightedSegments.Remove(segmentId);
        }

        private void TryAddSegmentInternal(ushort id, ref NetSegment segment)
        {
            Color color;
            if (HighlightRules.TryGetHighlightColor(ref segment, out color))
            {
                _highlightedSegments[id] = color;
            }
        }
    }
}
