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
            ModSettings.SettingsChanged += _ => OnSettingsChanged();
        }

        private void OnSettingsChanged()
        {
            if (_isEnabled)
                RebuildCache();
            else
                Clear();
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

            // 6) ROADS (including tram/trolley lanes)
            if (ai is RoadAI || ai is RoadBridgeAI || ai is RoadTunnelAI)
            {
                bool isHighway   = IsHighway(info);
                bool hasTramLike = HasTramOrTrolleyLanes(info);
                bool hasCarLike  = HasCarLikeLanes(info);

                bool isBridge = ai is RoadBridgeAI;
                bool isTunnel = ai is RoadTunnelAI;

                // Respect global bridge/tunnel toggles
                if (isBridge && !ModSettings.HighlightBridges)
                    return false;

                if (isTunnel && !ModSettings.HighlightTunnels)
                    return false;

                // --- Highways 
                if (isHighway)
                {
                    if (!ModSettings.HighlightHighways)
                        return false;

                    color = ModSettings.HighwaysColor;
                    return true;
                }

                //non-highway roads

                // exclude roads that do not have car or tram or trolley lanes
                if (!hasCarLike && !hasTramLike)
                    return false;

                // --- Pure tram/trolley (no car lanes) ---
                if (hasTramLike && !hasCarLike)
                {
                    if (!ModSettings.HighlightTramTracks)
                        return false;

                    color = ModSettings.TramTracksColor;
                    return true;
                }

                // --- Mixed road + tram/trolley ---
                if (hasTramLike && hasCarLike)
                {
                    // 1) Both true → tram/trolley color 
                    if (ModSettings.HighlightTramTracks)
                    {
                        color = ModSettings.TramTracksColor;
                        return true;
                    }

                    // 2) Only roads true → treat as road
                    if (ModSettings.HighlightRoads)
                    {
                        color = ModSettings.RoadsColor;
                        return true;
                    }

                    // neither roads nor tram enabled → no highlight
                    return false;
                }

                // --- Plain road (car-like lanes only) ---
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

        private static bool HasTramOrTrolleyLanes(NetInfo info)
        {
            if (info?.m_lanes == null)
                return false;

            const VehicleInfo.VehicleType tramLikeMask =
                VehicleInfo.VehicleType.Tram |
                VehicleInfo.VehicleType.Trolleybus;

            foreach (var lane in info.m_lanes)
            {
                if ((lane.m_vehicleType & tramLikeMask) != 0)
                    return true;
            }

            return false;
        }

        private static bool HasCarLikeLanes(NetInfo info)
        {
            if (info?.m_lanes == null)
                return false;

            foreach (var lane in info.m_lanes)
            {
                if ((lane.m_vehicleType & VehicleInfo.VehicleType.Car) != 0)
                    return true;
            }

            return false;
        }
    }
}
