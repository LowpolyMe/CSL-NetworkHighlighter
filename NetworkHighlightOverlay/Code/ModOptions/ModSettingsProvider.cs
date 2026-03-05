namespace NetworkHighlightOverlay.Code.ModOptions
{
    [System.Obsolete("Use ModCompositionRoot.Settings from the composition root.")]
    public static class ModSettingsProvider
    {
        public static ModSettings GetSettings() => ModCompositionRoot.Settings;
    }
}
