using System;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public sealed class HighlightCategoryDefinition
    {
        private readonly Func<Config, bool> _readEnabled;
        private readonly Action<Config, bool> _writeEnabled;
        private readonly Func<Config, float> _readHue;
        private readonly Action<Config, float> _writeHue;

        public HighlightCategoryDefinition(
            HighlightCategoryId id,
            string toggleLabel,
            string colorSliderLabel,
            string filterLabel,
            string spriteName,
            Func<Config, bool> readEnabled,
            Action<Config, bool> writeEnabled,
            Func<Config, float> readHue,
            Action<Config, float> writeHue)
        {
            if (string.IsNullOrEmpty(toggleLabel))
            {
                throw new ArgumentException("Toggle label is required.", "toggleLabel");
            }

            if (string.IsNullOrEmpty(colorSliderLabel))
            {
                throw new ArgumentException("Color slider label is required.", "colorSliderLabel");
            }

            if (string.IsNullOrEmpty(filterLabel))
            {
                throw new ArgumentException("Filter label is required.", "filterLabel");
            }

            if (string.IsNullOrEmpty(spriteName))
            {
                throw new ArgumentException("Sprite name is required.", "spriteName");
            }

            if (readEnabled == null)
            {
                throw new ArgumentNullException("readEnabled");
            }

            if (writeEnabled == null)
            {
                throw new ArgumentNullException("writeEnabled");
            }

            if (readHue == null)
            {
                throw new ArgumentNullException("readHue");
            }

            if (writeHue == null)
            {
                throw new ArgumentNullException("writeHue");
            }

            Id = id;
            ToggleLabel = toggleLabel;
            ColorSliderLabel = colorSliderLabel;
            FilterLabel = filterLabel;
            SpriteName = spriteName;
            _readEnabled = readEnabled;
            _writeEnabled = writeEnabled;
            _readHue = readHue;
            _writeHue = writeHue;
        }

        public HighlightCategoryId Id { get; }
        public string ToggleLabel { get; }
        public string ColorSliderLabel { get; }
        public string FilterLabel { get; }
        public string SpriteName { get; }

        public HighlightCategorySetting ReadState(Config config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            return new HighlightCategorySetting(_readEnabled(config), _readHue(config));
        }

        public void WriteState(Config config, HighlightCategorySetting state)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _writeEnabled(config, state.IsEnabled);
            _writeHue(config, state.Hue);
        }
    }
}
