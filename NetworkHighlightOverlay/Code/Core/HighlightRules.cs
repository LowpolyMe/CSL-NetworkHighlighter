using System;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Core
{
    public static class HighlightRules
    {
        private const string PinkPathNetworkName = "Pedestrian Connection";
        private const string TerraformingToken = "terraforming";
        private const string PedestrianStreetClassName = "Pedestrian Street";

        public static bool TryGetHighlightColor(ref NetSegment segment, ModSettings settings, out Color color)
        {
            color = default(Color);
            if (settings == null) return false;

            NetInfo info = segment.Info;
            if (info == null) return false;

            NetAI ai = info.m_netAI;
            if (ai == null) return false;

            HighlightCategoryId categoryId;
            bool isBridge;
            bool isTunnel;

            const VehicleInfo.VehicleType TramLikeMask =
                VehicleInfo.VehicleType.Tram |
                VehicleInfo.VehicleType.Trolleybus;

            bool isRoadFamily = ai is RoadAI || ai is RoadBridgeAI || ai is RoadTunnelAI;
            bool isRoadBridge = ai is RoadBridgeAI;
            bool isRoadTunnel = ai is RoadTunnelAI;
            bool isPedestrianStreet = false;
            bool isHighway = false;
            bool hasTramOrTrolleyLanes = false;
            bool hasMonorailLanes = false;
            bool hasCarLanes = false;
            bool isRaceRoad = false;
            bool isEventRoad = false;
            bool isPitLane = false;
            bool isAirportTaxiway = ai is AirportAreaTaxiwayAI;
            bool isAirportRunway = ai is AirportAreaRunwayAI;

            if (isRoadFamily)
            {
                VehicleInfo.VehicleType laneVehicleTypes = GetLaneVehicleTypes(info);
                isPedestrianStreet = IsPedestrianStreet(info);
                isHighway = IsHighway(info);
                hasTramOrTrolleyLanes = (laneVehicleTypes & TramLikeMask) != 0;
                hasMonorailLanes = (laneVehicleTypes & VehicleInfo.VehicleType.Monorail) != 0;
                hasCarLanes = (laneVehicleTypes & VehicleInfo.VehicleType.Car) != 0;
                isRaceRoad = IsRaceRoad(info);
                isEventRoad = IsEventRoad(info);
                isPitLane = IsPitLane(info);
            }

            HighlightSelection.SegmentFlags flags = new HighlightSelection.SegmentFlags
            {
                IsPinkPath = IsPinkPath(info, ai),
                IsTerraformingNetwork = IsTerraformingNetwork(info),
                IsPedestrianPath = ai is PedestrianPathAI || ai is PedestrianWayAI || ai is PedestrianZoneRoadAI,
                IsPedestrianBridge = ai is PedestrianBridgeAI || ai is PedestrianZoneBridgeAI,
                IsPedestrianTunnel = ai is PedestrianTunnelAI,
                IsTrainTrack = ai is TrainTrackAI,
                IsTrainBridge = ai is TrainTrackBridgeAI,
                IsTrainTunnel = ai is TrainTrackTunnelAI,
                IsMetroTrack = ai is MetroTrackAI,
                IsMetroBridge = ai is MetroTrackBridgeAI,
                IsMetroTunnel = ai is MetroTrackTunnelAI,
                IsMonorailTrack = ai is MonorailTrackAI,
                IsCableCarPath = ai is CableCarPathAI,
                IsRaceRoad = isRaceRoad,
                IsEventRoad = isEventRoad,
                IsPitLane = isPitLane,
                IsAirportTaxiway = isAirportTaxiway,
                IsAirportRunway = isAirportRunway,
                IsRoadFamily = isRoadFamily,
                IsRoadBridge = isRoadBridge,
                IsRoadTunnel = isRoadTunnel,
                IsPedestrianStreet = isPedestrianStreet,
                IsHighway = isHighway,
                HasTramOrTrolleyLanes = hasTramOrTrolleyLanes,
                HasMonorailLanes = hasMonorailLanes,
                HasCarLanes = hasCarLanes
            };

            bool didSelect = HighlightSelection.TrySelectCategory(
                flags,
                settings.GetCategoryEnabled,
                out categoryId,
                out isBridge,
                out isTunnel);

            if (!didSelect) return false;

            bool isEnabled = HighlightSelection.IsCategoryEnabledForSegment(
                categoryId,
                isBridge,
                isTunnel,
                settings.HighlightBridges,
                settings.HighlightTunnels,
                settings.GetCategoryEnabled);

            if (!isEnabled) return false;

            color = settings.GetCategoryColor(categoryId);
            return true;
        }

        private static bool IsPinkPath(NetInfo info, NetAI ai)
        {
            if (info == null || ai == null) return false;

            if (!(ai is PedestrianPathAI)) return false;

            return string.Equals(info.name, PinkPathNetworkName, StringComparison.Ordinal);
        }

        private static bool IsTerraformingNetwork(NetInfo info)
        {
            if (info == null) return false;

            string infoName = info.name;
            if (string.IsNullOrEmpty(infoName)) return false;

            if (infoName.IndexOf(TerraformingToken, StringComparison.OrdinalIgnoreCase) < 0) return false;

            return info.m_flattenTerrain;
        }

        private static bool IsPedestrianStreet(NetInfo info)
        {
            if (info == null || info.m_class == null) return false;

            string className = info.m_class.name;
            if (string.IsNullOrEmpty(className)) return false;

            return string.Equals(className, PedestrianStreetClassName, StringComparison.Ordinal);
        }

        private static bool IsRaceRoad(NetInfo netInfo)
        {
            if (netInfo == null) return false;

            return netInfo.GetService() == ItemClass.Service.Race &&
                   netInfo.GetClassLevel() == ItemClass.Level.Level4;
        }

        private static bool IsEventRoad(NetInfo netInfo)
        {
            if (netInfo == null) return false;

            return netInfo.GetService() == ItemClass.Service.Race &&
                   netInfo.GetClassLevel() == ItemClass.Level.Level3;
        }

        private static bool IsPitLane(NetInfo netInfo)
        {
            if (netInfo == null) return false;

            if (netInfo.GetService() != ItemClass.Service.Race) return false;

            ItemClass.Level classLevel = netInfo.GetClassLevel();
            return classLevel == ItemClass.Level.Level2 ||
                   classLevel == ItemClass.Level.Level1;
        }

        private static bool IsHighway(NetInfo info)
        {
            NetAI ai = info == null ? null : info.m_netAI;
            if (ai == null) return false;

            return ai.IsHighway();
        }

        private static VehicleInfo.VehicleType GetLaneVehicleTypes(NetInfo info)
        {
            if (info == null || info.m_lanes == null) return VehicleInfo.VehicleType.None;

            VehicleInfo.VehicleType laneVehicleTypes = VehicleInfo.VehicleType.None;
            foreach (NetInfo.Lane lane in info.m_lanes)
            {
                laneVehicleTypes |= lane.m_vehicleType;
            }

            return laneVehicleTypes;
        }
    }
}
