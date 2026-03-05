namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class ModCompositionRoot
    {
        private static readonly ModSettings _settings = new ModSettings();

        public static ModSettings Settings => _settings;
    }
}
