using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public static class ColorConversion
    {
        public static float ToHue(Color color)
        {
            Color.RGBToHSV(color, out float h, out float _, out float _);
            return h;
        }
        
        public static Color FromHue(float hue, float strength)
        {
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            
            color.a = Mathf.Clamp01(strength);

            return color;
        }
    }
}