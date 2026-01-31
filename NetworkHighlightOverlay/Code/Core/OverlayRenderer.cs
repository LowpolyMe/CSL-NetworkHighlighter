using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public sealed class OverlayRenderer
    {
        public void Render(RenderManager.CameraInfo cameraInfo, List<KeyValuePair<ushort, Color>> highlightedSegments)
        {
            NetManager netManager = NetManager.instance;

            int count = highlightedSegments.Count;
            for (int i = 0; i < count; i++)
            {
                KeyValuePair<ushort, Color> kvp = highlightedSegments[i];
                ushort id = kvp.Key;
                Color color = kvp.Value;

                ref NetSegment segment = ref netManager.m_segments.m_buffer[id];

                if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                    continue;

                RenderSegmentOverlay(cameraInfo, ref segment, color);
            }
        }

        private static void RenderSegmentOverlay(RenderManager.CameraInfo cameraInfo, ref NetSegment segment, Color color)
        {
            NetInfo info = segment.Info;
            if (info == null)
                return;

            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(
                cameraInfo,
                color,
                GetBezierPoints(segment),
                GetHighlightWidth(info),
                -100000f,
                -100000f,
                -100f,
                1280f,
                false,
                false);
        }

        private static Bezier3 GetBezierPoints(NetSegment segment)
        {
            Bezier3 bezier;
            bezier.a = NetManager.instance.m_nodes.m_buffer[segment.m_startNode].m_position;
            bezier.d = NetManager.instance.m_nodes.m_buffer[segment.m_endNode].m_position;

            NetSegment.CalculateMiddlePoints(
                bezier.a, segment.m_startDirection,
                bezier.d, segment.m_endDirection,
                true, true, out bezier.b, out bezier.c);
            return bezier;
        }

        private static float GetHighlightWidth(NetInfo info)
        {
            float widthFactor = ModSettings.HighlightWidth;
            float highlightWidth = info.m_halfWidth * 2f * widthFactor;
            return highlightWidth;
        }
    }
}
