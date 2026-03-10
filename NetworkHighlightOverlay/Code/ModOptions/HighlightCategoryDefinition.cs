namespace NetworkHighlightOverlay.Code.ModOptions
{
    public sealed class HighlightCategoryDefinition
    {
        public HighlightCategoryDefinition(
            HighlightCategoryId id,
            string toggleLabel,
            string colorSliderLabel,
            string filterLabel,
            string spriteName)
        {
            Id = id;
            ToggleLabel = toggleLabel;
            ColorSliderLabel = colorSliderLabel;
            FilterLabel = filterLabel;
            SpriteName = spriteName;
        }

        public HighlightCategoryId Id { get; }
        public string ToggleLabel { get; }
        public string ColorSliderLabel { get; }
        public string FilterLabel { get; }
        public string SpriteName { get; }
    }
}
