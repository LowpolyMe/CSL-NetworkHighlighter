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
            Func<HighlightCategoryId, bool> isCategoryEnabled,
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

            if (!flags.IsRoadFamily) return false;

            return TrySelectRoadCategory(
                flags.IsRoadBridge,
                flags.IsRoadTunnel,
                flags.IsPedestrianStreet,
                flags.IsHighway,
                flags.HasTramOrTrolleyLanes,
                flags.HasMonorailLanes,
                flags.HasCarLanes,
                isCategoryEnabled,
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
            if (isCategoryEnabled == null) return false;

            if (isBridge && !highlightBridges) return false;

            if (isTunnel && !highlightTunnels) return false;

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
            Func<HighlightCategoryId, bool> isCategoryEnabled,
            out HighlightCategoryId categoryId,
            out bool isBridge,
            out bool isTunnel)
        {
            categoryId = default(HighlightCategoryId);
            isBridge = isRoadBridge;
            isTunnel = isRoadTunnel;

            if (isCategoryEnabled == null) return false;

            if (isPedestrianStreet)
            {
                if (hasTramOrTrolleyLanes &&
                    TrySelectIfEnabled(HighlightCategoryId.TramTracks, isCategoryEnabled, out categoryId))
                {
                    return true;
                }

                if (hasMonorailLanes &&
                    TrySelectIfEnabled(HighlightCategoryId.MonorailTracks, isCategoryEnabled, out categoryId))
                {
                    return true;
                }

                return TrySelectIfEnabled(HighlightCategoryId.PedestrianPaths, isCategoryEnabled, out categoryId);
            }

            if (isHighway) return TrySelectIfEnabled(HighlightCategoryId.Highways, isCategoryEnabled, out categoryId);

            if (!hasCarLanes && !hasTramOrTrolleyLanes && !hasMonorailLanes) return false;

            if (hasTramOrTrolleyLanes &&
                TrySelectIfEnabled(HighlightCategoryId.TramTracks, isCategoryEnabled, out categoryId))
            {
                return true;
            }

            if (hasMonorailLanes &&
                TrySelectIfEnabled(HighlightCategoryId.MonorailTracks, isCategoryEnabled, out categoryId))
            {
                return true;
            }

            if (hasCarLanes &&
                TrySelectIfEnabled(HighlightCategoryId.Roads, isCategoryEnabled, out categoryId))
            {
                return true;
            }

            return false;
        }

        private static bool TrySelectIfEnabled(
            HighlightCategoryId categoryId,
            Func<HighlightCategoryId, bool> isCategoryEnabled,
            out HighlightCategoryId selectedCategoryId)
        {
            selectedCategoryId = default(HighlightCategoryId);
            if (!isCategoryEnabled(categoryId)) return false;

            selectedCategoryId = categoryId;
            return true;
        }
    }
}
