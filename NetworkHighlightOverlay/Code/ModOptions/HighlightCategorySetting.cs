using System;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public struct HighlightCategorySetting : IEquatable<HighlightCategorySetting>
    {
        public HighlightCategorySetting(bool isEnabled, float hue)
        {
            IsEnabled = isEnabled;
            Hue = hue;
        }

        public bool IsEnabled { get; private set; }
        public float Hue { get; private set; }

        public HighlightCategorySetting WithEnabled(bool isEnabled)
        {
            return new HighlightCategorySetting(isEnabled, Hue);
        }

        public HighlightCategorySetting WithHue(float hue)
        {
            return new HighlightCategorySetting(IsEnabled, hue);
        }

        public bool Equals(HighlightCategorySetting other)
        {
            return IsEnabled == other.IsEnabled && Hue.Equals(other.Hue);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HighlightCategorySetting))
            {
                return false;
            }

            HighlightCategorySetting other = (HighlightCategorySetting)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IsEnabled ? 1 : 0;
                hashCode = (hashCode * 397) ^ Hue.GetHashCode();
                return hashCode;
            }
        }
    }
}
