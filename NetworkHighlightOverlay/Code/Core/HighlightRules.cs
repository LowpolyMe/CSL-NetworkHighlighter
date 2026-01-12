using System;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public static class HighlightRules
    {
        public static bool TryGetHighlightColor(ref NetSegment segment, out Color color)
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

            if (IsPinkPath(info, ai))
            {
                if (!ModSettings.HighlightPedestrianPaths || !ModSettings.HighlightPinkPaths)
                    return false;

                color = ModSettings.PinkPathColor;
                return true;
            }

            if (IsTerraformingNetwork(info, ai))
            {
                if (!ModSettings.HighlightTerraformingNetworks)
                    return false;

                color = ModSettings.TerraformingNetworksColor;
                return true;
            }

            if (ai is PedestrianPathAI || ai is PedestrianWayAI || ai is PedestrianZoneRoadAI)
            {
                if (!ModSettings.HighlightPedestrianPaths)
                    return false;

                color = ModSettings.PedestrianPathColor;
                return true;
            }

            if (ai is PedestrianBridgeAI || ai is PedestrianZoneBridgeAI)
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

            if (ai is MetroTrackAI || ai is MetroTrackBridgeAI || ai is MetroTrackTunnelAI)
            {
                if (!ModSettings.HighlightMetroTracks)
                    return false;

                if (ai is MetroTrackBridgeAI && !ModSettings.HighlightBridges)
                    return false;

                if (ai is MetroTrackTunnelAI && !ModSettings.HighlightTunnels)
                    return false;

                color = ModSettings.MetroTracksColor;
                return true;
            }

            if (ai is MonorailTrackAI)
            {
                if (!ModSettings.HighlightMonorailTracks)
                    return false;

                color = ModSettings.MonorailTracksColor;
                return true;
            }

            if (ai is CableCarPathAI)
            {
                if (!ModSettings.HighlightCableCars)
                    return false;

                color = ModSettings.CableCarColor;
                return true;
            }

            if (ai is RoadAI || ai is RoadBridgeAI || ai is RoadTunnelAI)
            {
                bool isHighway = IsHighway(info);
                bool hasTramLike = HasTramOrTrolleyLanes(info);
                bool hasCarLike = HasCarLikeLanes(info);
                bool isPedestrianStreet = IsPedestrianStreet(info);

                bool isBridge = ai is RoadBridgeAI;
                bool isTunnel = ai is RoadTunnelAI;

                if (isBridge && !ModSettings.HighlightBridges)
                    return false;

                if (isTunnel && !ModSettings.HighlightTunnels)
                    return false;

                if (isPedestrianStreet)
                {
                    if (hasTramLike && ModSettings.HighlightTramTracks)
                    {
                        color = ModSettings.TramTracksColor;
                        return true;
                    }

                    if (!ModSettings.HighlightPedestrianPaths)
                        return false;

                    color = ModSettings.PedestrianPathColor;
                    return true;
                }

                if (isHighway)
                {
                    if (!ModSettings.HighlightHighways)
                        return false;

                    color = ModSettings.HighwaysColor;
                    return true;
                }

                if (!hasCarLike && !hasTramLike)
                    return false;

                if (hasTramLike && !hasCarLike)
                {
                    if (!ModSettings.HighlightTramTracks)
                        return false;

                    color = ModSettings.TramTracksColor;
                    return true;
                }

                if (hasTramLike && hasCarLike)
                {
                    if (ModSettings.HighlightTramTracks)
                    {
                        color = ModSettings.TramTracksColor;
                        return true;
                    }

                    if (ModSettings.HighlightRoads)
                    {
                        color = ModSettings.RoadsColor;
                        return true;
                    }

                    return false;
                }

                if (hasCarLike && ModSettings.HighlightRoads)
                {
                    color = ModSettings.RoadsColor;
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool IsPinkPath(NetInfo info, NetAI ai)
        {
            if (info == null || ai == null)
                return false;

            if (!(ai is PedestrianPathAI))
                return false;

            return string.Equals(info.name, "Pedestrian Connection", StringComparison.Ordinal);
        }

        private static bool IsTerraformingNetwork(NetInfo info, NetAI ai)
        {
            if (info == null || ai == null)
                return false;

            if (string.IsNullOrEmpty(info.name))
                return false;

            if (info.name.IndexOf("terraforming", StringComparison.OrdinalIgnoreCase) < 0)
                return false;

            return IsFlattenTerrainEnabled(info);
        }

        private static bool IsFlattenTerrainEnabled(NetInfo info)
        {
            if (info == null)
                return false;
            return info.m_flattenTerrain;
        }

        private static bool IsHighway(NetInfo info)
        {
            var ai = info == null ? null : info.m_netAI;
            if (ai == null)
                return false;
            return ai.IsHighway();
        }

        private static bool HasTramOrTrolleyLanes(NetInfo info)
        {
            if (info == null || info.m_lanes == null)
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
            if (info == null || info.m_lanes == null)
                return false;

            foreach (var lane in info.m_lanes)
            {
                if ((lane.m_vehicleType & VehicleInfo.VehicleType.Car) != 0)
                    return true;
            }

            return false;
        }

        private static bool IsPedestrianStreet(NetInfo info)
        {
            var itemClass = info == null ? null : info.m_class;
            var name = itemClass == null ? null : itemClass.name;
            if (string.IsNullOrEmpty(name))
                return false;

            return string.Equals(name, "Pedestrian Street", StringComparison.Ordinal);
        }
    }
}
