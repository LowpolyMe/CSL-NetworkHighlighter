using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class HighlightCache
    {
        private readonly Dictionary<ushort, Color> _highlightedSegments = new Dictionary<ushort, Color>();

        public void CopySegmentsTo(List<KeyValuePair<ushort, Color>> destination)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            destination.Clear();

            lock (_highlightedSegments)
            {
                int count = _highlightedSegments.Count;
                if (destination.Capacity < count)
                    destination.Capacity = count;

                foreach (KeyValuePair<ushort, Color> entry in _highlightedSegments)
                {
                    destination.Add(entry);
                }
            }
        }

        public void Clear()
        {
            lock (_highlightedSegments)
            {
                _highlightedSegments.Clear();
            }
        }

        public void RebuildCache()
        {
            lock (_highlightedSegments)
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
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (segmentId == 0)
                return;

            ref NetSegment segment = ref NetManager.instance.m_segments.m_buffer[segmentId];
            if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                return;

            lock (_highlightedSegments)
            {
                TryAddSegmentInternal(segmentId, ref segment);
            }
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (segmentId == 0)
                return;

            lock (_highlightedSegments)
            {
                _highlightedSegments.Remove(segmentId);
            }
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
