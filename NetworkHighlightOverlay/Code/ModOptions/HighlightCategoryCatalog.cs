namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class HighlightCategoryCatalog
    {
        public static readonly HighlightCategoryDefinition[] All = new HighlightCategoryDefinition[]
        {
            new HighlightCategoryDefinition(
                HighlightCategoryId.PedestrianPaths,
                "Pedestrian paths",
                "Pedestrian paths",
                "Highlight pedestrian paths",
                "SubBarBeautificationPedestrianZoneEssentials",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.PinkPaths,
                "Pink paths",
                "Pink paths",
                "Highlight pink paths",
                "SubBarRoadsMaintenance",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TerraformingNetworks,
                "Terraforming networks",
                "Terraforming networks",
                "Highlight terraforming networks",
                "ToolbarIconLandscaping",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Roads,
                "Roads",
                "Roads",
                "Highlight roads",
                "SubBarRoadsSmall",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Highways,
                "Highways",
                "Highways",
                "Highlight highways",
                "SubBarRoadsHighway",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.RaceRoads,
                "Races and Parades",
                "Races and Parades",
                "Highlight races and parades roads",
                "SubBarRoadsRacesAndParades",
                SteamHelper.DLC.RacesAndParadesDLC),
            new HighlightCategoryDefinition(
                HighlightCategoryId.AirportRoads,
                "Airport road",
                "Airport road",
                "Highlight airport roads",
                "SubBarPublicTransportAirportArea",
                SteamHelper.DLC.AirportDLC),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TrainTracks,
                "Train tracks",
                "Train tracks",
                "Highlight train tracks",
                "SubBarPublicTransportTrain",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MetroTracks,
                "Metro tracks",
                "Metro tracks",
                "Highlight metro tracks",
                "SubBarPublicTransportMetro",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TramTracks,
                "Tram tracks",
                "Tram and Trolley tracks",
                "Highlight tram tracks",
                "SubBarPublicTransportTram",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MonorailTracks,
                "Monorail tracks",
                "Monorail tracks",
                "Highlight monorail tracks",
                "SubBarPublicTransportMonorail",
                SteamHelper.DLC.None),
            new HighlightCategoryDefinition(
                HighlightCategoryId.CableCars,
                "Cable cars",
                "Cable car paths",
                "Highlight cable car paths",
                "SubBarPublicTransportCableCar",
                SteamHelper.DLC.None)
        };
    }
}
