namespace NetworkHighlightOverlay.Code.ModOptions
{
    public sealed class HighlightCategoryDefinition
    {
        public HighlightCategoryDefinition(
            HighlightCategoryId id,
            string toggleLabel,
            string colorSliderLabel,
            string filterLabel,
            string spriteName,
            SteamHelper.DLC requiredDlc)
        {
            Id = id;
            ToggleLabel = toggleLabel;
            ColorSliderLabel = colorSliderLabel;
            FilterLabel = filterLabel;
            SpriteName = spriteName;
            RequiredDlc = requiredDlc;
        }

        public HighlightCategoryId Id { get; }
        public string ToggleLabel { get; }
        public string ColorSliderLabel { get; }
        public string FilterLabel { get; }
        public string SpriteName { get; }
        public SteamHelper.DLC RequiredDlc { get; }
    }
}
