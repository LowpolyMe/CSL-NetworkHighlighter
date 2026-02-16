using System;
using NetworkHighlightOverlay.Code.ModOptions;

namespace NetworkHighlightOverlay.Code.Core
{
    public static class HighlightSelection
    {
        public struct SegmentFlags
        {
            public bool IsPinkPath;
            public bool IsTerraformingNetwork;
            public bool IsPedestrianPath;
            public bool IsPedestrianBridge;
            public bool IsPedestrianTunnel;
            public bool IsTrainTrack;
            public bool IsTrainBridge;
            public bool IsTrainTunnel;
            public bool IsMetroTrack;
            public bool IsMetroBridge;
            public bool IsMetroTunnel;
            public bool IsMonorailTrack;
            public bool IsCableCarPath;
            public bool IsRoadFamily;
            public bool IsRoadBridge;
            public bool IsRoadTunnel;
            public bool IsPedestrianStreet;
            public bool IsHighway;
            public bool HasTramOrTrolleyLanes;
            public bool HasMonorailLanes;
            public bool HasCarLanes;
        }

        public static bool TrySelectCategory(
            SegmentFlags flags,
            out HighlightCategoryId categoryId,
            out bool isBridge,
            out bool isTunnel)
        {
            categoryId = default(HighlightCategoryId);
            isBridge = false;
            isTunnel = false;

            if (flags.IsPinkPath)
            {
                categoryId = HighlightCategoryId.PinkPaths;
                return true;
            }

            if (flags.IsTerraformingNetwork)
            {
                categoryId = HighlightCategoryId.TerraformingNetworks;
                return true;
            }

            if (flags.IsPedestrianPath)
            {
                categoryId = HighlightCategoryId.PedestrianPaths;
                return true;
            }

            if (flags.IsPedestrianBridge)
            {
                categoryId = HighlightCategoryId.PedestrianPaths;
                isBridge = true;
                return true;
            }

            if (flags.IsPedestrianTunnel)
            {
                categoryId = HighlightCategoryId.PedestrianPaths;
                isTunnel = true;
                return true;
            }

            if (flags.IsTrainTrack || flags.IsTrainBridge || flags.IsTrainTunnel)
            {
                categoryId = HighlightCategoryId.TrainTracks;
                isBridge = flags.IsTrainBridge;
                isTunnel = flags.IsTrainTunnel;
                return true;
            }

            if (flags.IsMetroTrack || flags.IsMetroBridge || flags.IsMetroTunnel)
            {
                categoryId = HighlightCategoryId.MetroTracks;
                isBridge = flags.IsMetroBridge;
                isTunnel = flags.IsMetroTunnel;
                return true;
            }

            if (flags.IsMonorailTrack)
            {
                categoryId = HighlightCategoryId.MonorailTracks;
                return true;
            }

            if (flags.IsCableCarPath)
            {
                categoryId = HighlightCategoryId.CableCars;
                return true;
            }

            if (!flags.IsRoadFamily)
            {
                return false;
            }

            return TrySelectRoadCategory(
                flags.IsRoadBridge,
                flags.IsRoadTunnel,
                flags.IsPedestrianStreet,
                flags.IsHighway,
                flags.HasTramOrTrolleyLanes,
                flags.HasMonorailLanes,
                flags.HasCarLanes,
                out categoryId,
                out isBridge,
                out isTunnel);
        }

        public static bool IsCategoryEnabledForSegment(
            HighlightCategoryId categoryId,
            bool isBridge,
            bool isTunnel,
            bool highlightBridges,
            bool highlightTunnels,
            Func<HighlightCategoryId, bool> isCategoryEnabled)
        {
            if (isCategoryEnabled == null)
            {
                return false;
            }

            if (isBridge && !highlightBridges)
            {
                return false;
            }

            if (isTunnel && !highlightTunnels)
            {
                return false;
            }

            if (categoryId == HighlightCategoryId.PinkPaths)
            {
                return isCategoryEnabled(HighlightCategoryId.PedestrianPaths) &&
                       isCategoryEnabled(HighlightCategoryId.PinkPaths);
            }

            return isCategoryEnabled(categoryId);
        }

        private static bool TrySelectRoadCategory(
            bool isRoadBridge,
            bool isRoadTunnel,
            bool isPedestrianStreet,
            bool isHighway,
            bool hasTramOrTrolleyLanes,
            bool hasMonorailLanes,
            bool hasCarLanes,
            out HighlightCategoryId categoryId,
            out bool isBridge,
            out bool isTunnel)
        {
            categoryId = default(HighlightCategoryId);
            isBridge = isRoadBridge;
            isTunnel = isRoadTunnel;

            if (isPedestrianStreet)
            {
                if (hasTramOrTrolleyLanes)
                {
                    categoryId = HighlightCategoryId.TramTracks;
                    return true;
                }

                if (hasMonorailLanes)
                {
                    categoryId = HighlightCategoryId.MonorailTracks;
                    return true;
                }

                categoryId = HighlightCategoryId.PedestrianPaths;
                return true;
            }

            if (isHighway)
            {
                categoryId = HighlightCategoryId.Highways;
                return true;
            }

            if (!hasCarLanes && !hasTramOrTrolleyLanes && !hasMonorailLanes)
            {
                return false;
            }

            if (hasTramOrTrolleyLanes)
            {
                categoryId = HighlightCategoryId.TramTracks;
                return true;
            }

            if (hasMonorailLanes)
            {
                categoryId = HighlightCategoryId.MonorailTracks;
                return true;
            }

            categoryId = HighlightCategoryId.Roads;
            return true;
        }
    }
}
