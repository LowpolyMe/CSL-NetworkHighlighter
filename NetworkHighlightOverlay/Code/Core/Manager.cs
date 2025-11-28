using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public class Manager
    {
        private bool _isEnabled;

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
                    RebuildCache();
                }
                else
                {
                    Clear();
                }
            }
        }

        private static readonly Manager _instance = new Manager();
        public static Manager Instance => _instance;


        private readonly Dictionary<ushort, Color> _highlightedSegments = new Dictionary<ushort, Color>();

        private Manager()
        {
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
            if (TryGetHighlightColor(ref segment, out var color))
            {
                _highlightedSegments[id] = color;
            }
        }

        public void RenderIfActive(RenderManager.CameraInfo cameraInfo)
        {
            if (!_isEnabled)
                return;

            NetManager netManager = NetManager.instance;

            foreach (var kvp in _highlightedSegments)
            {
                ushort id = kvp.Key;
                Color color = kvp.Value;

                ref NetSegment segment = ref netManager.m_segments.m_buffer[id];

                if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                    continue;

                RenderSegmentOverlay(
                    cameraInfo,
                    ref segment,
                    color
                );
            }
        }

        private static void RenderSegmentOverlay(
            RenderManager.CameraInfo cameraInfo,
            ref NetSegment segment,
            Color color)
        {
            NetInfo info = segment.Info;
            if (info == null)
                return;

            Bezier3 bezier;
            bezier.a = NetManager.instance.m_nodes.m_buffer[segment.m_startNode].m_position;
            bezier.d = NetManager.instance.m_nodes.m_buffer[segment.m_endNode].m_position;
            NetSegment.CalculateMiddlePoints(
                bezier.a, segment.m_startDirection,
                bezier.d, segment.m_endDirection,
                true, true, out bezier.b, out bezier.c);

            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(
                cameraInfo,
                color,
                bezier,
                info.m_halfWidth * 2f,
                -100000f,   // no cut at start
                -100000f,   // no cut at end
                -100f,      // minY
                1280f,      // maxY
                false,
                false);
        }

        /// <summary>
        /// Central rule: given a segment and current ModSettings,
        /// decide if it should be highlighted and with which color.
        /// </summary>
        private bool TryGetHighlightColor(ref NetSegment segment, out Color color)
        {
            var info = segment.Info;
            color = default;

            if (info == null)
            {
                return false;
            }

            var ai = info.m_netAI;
            if (ai == null)
            {
                return false;
            }

            // 1) PEDESTRIAN
            if (ai is PedestrianPathAI || ai is PedestrianWayAI)
            {
                if (!ModSettings.HighlightPedestrianPaths)
                    return false;

                color = ModSettings.PedestrianPathColor;
                return true;
            }

            if (ai is PedestrianBridgeAI)
            {
                if (!ModSettings.HighlightPedestrianPaths ||
                    !ModSettings.HighlightBridges)
                    return false;

                color = ModSettings.PedestrianPathColor;
                return true;
            }

            if (ai is PedestrianTunnelAI)
            {
                if (!ModSettings.HighlightPedestrianPaths ||
                    !ModSettings.HighlightTunnels)
                    return false;

                color = ModSettings.PedestrianPathColor;
                return true;
            }

            // 2) TRAIN
            if (ai is TrainTrackAI || ai is TrainTrackBridgeAI || ai is TrainTrackTunnelAI)
            {
                if (!ModSettings.HighlightTrainTracks)
                    return false;

                if (ai is TrainTrackBridgeAI && !ModSettings.HighlightBridges)
                    return false;

                if (ai is TrainTrackTunnelAI && !ModSettings.HighlightTunnels)
                    return false;

                color = ModSettings.TrainTracksColor;
                return true;
            }

            // 3) METRO
            if (ai is MetroTrackAI)
            {
                if (!ModSettings.HighlightMetroTracks)
                    return false;

                color = ModSettings.MetroTracksColor;
                return true;
            }

            // 4) MONORAIL
            if (ai is MonorailTrackAI)
            {
                if (!ModSettings.HighlightMonorailTracks)
                    return false;

                color = ModSettings.MonorailTracksColor;
                return true;
            }

            // 5) CABLE CAR
            if (ai is CableCarPathAI)
            {
                if (!ModSettings.HighlightCableCars)
                    return false;

                color = ModSettings.CableCarColor;
                return true;
            }

            // 6) ROADS (including those with tram lanes)
            if (ai is RoadAI || ai is RoadBridgeAI || ai is RoadTunnelAI)
            {
                bool isHighway   = IsHighway(info);
                bool hasTram     = HasTramLanes(info);
                bool hasCarLike  = HasCarLikeLanes(info);

                bool isBridge = ai is RoadBridgeAI;
                bool isTunnel = ai is RoadTunnelAI;

                // Respect global bridge/tunnel toggles
                if (isBridge && !ModSettings.HighlightBridges)
                    return false;

                if (isTunnel && !ModSettings.HighlightTunnels)
                    return false;

                // --- Pure tram track (no car lanes) ---
                if (hasTram && !hasCarLike)
                {
                    if (!ModSettings.HighlightTramTracks)
                        return false;

                    color = ModSettings.TramTracksColor;
                    return true;
                }

                // --- Mixed road + tram lanes ---
                if (hasTram && hasCarLike && ModSettings.HighlightTramTracks)
                {
                    // choose tram color when tram highlight is on
                    color = ModSettings.TramTracksColor;
                    return true;
                }

                // --- Highways vs. normal roads ---
                if (isHighway)
                {
                    if (!ModSettings.HighlightHighways)
                        return false; // don't fall back to roads

                    color = ModSettings.HighwaysColor;
                    return true;
                }

                // Non-highway roads with car-like lanes
                if (hasCarLike && ModSettings.HighlightRoads)
                {
                    color = ModSettings.RoadsColor;
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool IsHighway(NetInfo info)
        {
            var ai = info?.m_netAI;
            if (ai == null)
                return false;
            return ai.IsHighway();
        }

        private static bool HasTramLanes(NetInfo info)
        {
            if (info?.m_lanes == null)
                return false;

            foreach (var lane in info.m_lanes)
            {
                if ((lane.m_vehicleType & VehicleInfo.VehicleType.Tram) != 0)
                    return true;
            }

            return false;
        }

        private static bool HasCarLikeLanes(NetInfo info)
        {
            if (info?.m_lanes == null)
                return false;

            // you can extend this mask if you want to treat buses/taxis/etc as "roads"
            const VehicleInfo.VehicleType carLikeMask = VehicleInfo.VehicleType.Car;

            foreach (var lane in info.m_lanes)
            {
                if ((lane.m_vehicleType & carLikeMask) != 0)
                    return true;
            }

            return false;
        }
    }
}
