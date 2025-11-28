using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public static class ColorConversion
    {
        public static float ToHue(Color color)
        {
            Color.RGBToHSV(color, out float h, out _, out _);
            return h;
        }
        
        public static Color FromHue(float hue)
        {
            return Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}