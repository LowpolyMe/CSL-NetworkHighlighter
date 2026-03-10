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
                "SubBarBeautificationPedestrianZoneEssentials"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.PinkPaths,
                "Pink paths",
                "Pink paths",
                "Highlight pink paths",
                "SubBarRoadsMaintenance"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TerraformingNetworks,
                "Terraforming networks",
                "Terraforming networks",
                "Highlight terraforming networks",
                "ToolbarIconLandscaping"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Roads,
                "Roads",
                "Roads",
                "Highlight roads",
                "SubBarRoadsSmall"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Highways,
                "Highways",
                "Highways",
                "Highlight highways",
                "SubBarRoadsHighway"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TrainTracks,
                "Train tracks",
                "Train tracks",
                "Highlight train tracks",
                "SubBarPublicTransportTrain"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MetroTracks,
                "Metro tracks",
                "Metro tracks",
                "Highlight metro tracks",
                "SubBarPublicTransportMetro"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TramTracks,
                "Tram tracks",
                "Tram and Trolley tracks",
                "Highlight tram tracks",
                "SubBarPublicTransportTram"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MonorailTracks,
                "Monorail tracks",
                "Monorail tracks",
                "Highlight monorail tracks",
                "SubBarPublicTransportMonorail"),
            new HighlightCategoryDefinition(
                HighlightCategoryId.CableCars,
                "Cable cars",
                "Cable car paths",
                "Highlight cable car paths",
                "SubBarPublicTransportCableCar")
        };
    }
}
