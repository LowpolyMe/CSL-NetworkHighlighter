using System.Reflection;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public static class UIUtility
    {
        public static UIComponent TryGetRootComponent(UIHelperBase helper)
        {
            if (helper == null)
                return null;

            // 1) Vanilla: ICities.UIHelperBase is actually ColossalFramework.UIHelper
            if (helper is UIHelper uiHelper && uiHelper.self is UIComponent uiComponent)
            {
                Debug.Log("[NetworkHighlightOverlay][Options] Root from vanilla UIHelper.self");
                return uiComponent;
            }

            // 2) Custom helpers (SkyveUIHelper etc.) via reflection
            var helperType = helper.GetType();
            Debug.Log("[NetworkHighlightOverlay][Options] Trying to resolve root for custom helper type: " +
                      helperType.FullName);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            foreach (var prop in helperType.GetProperties(flags))
            {
                if (!typeof(UIComponent).IsAssignableFrom(prop.PropertyType))
                    continue;

                try
                {
                    var value = prop.GetValue(helper, null) as UIComponent;
                    if (value != null)
                    {
                        Debug.Log("[NetworkHighlightOverlay][Options] Root from property '" + prop.Name + "'");
                        return value;
                    }
                }
                catch { /* ignore */ }
            }

            
            foreach (var field in helperType.GetFields(flags))
            {
                if (!typeof(UIComponent).IsAssignableFrom(field.FieldType))
                    continue;

                try
                {
                    var value = field.GetValue(helper) as UIComponent;
                    if (value != null)
                    {
                        Debug.Log("[NetworkHighlightOverlay][Options] Root from field '" + field.Name + "'");
                        return value;
                    }
                }
                catch { /* ignore */ }
            }

            Debug.LogWarning("[NetworkHighlightOverlay][Options] No UIComponent field/property found on helper type " +
                             helperType.FullName);
            return null;
        }

        public static UIHelper CreateTab(UITabContainer tabContainer, UITabstrip tabStrip, string title, Color tintColor,
            out UIPanel page)
        {
            
            page = Page(tabContainer, title);
            UIButton tabButton = UIButton(tabStrip, title, tintColor);

            tabStrip.AddTab(title, tabButton.gameObject, page.gameObject);
            
            return new UIHelper(page);
        }

        public static UISlider CreateHueSlider(UIHelper group, string label, float initialHue, OnValueChanged onChanged,
            Texture2D backgroundTexture)
        {

            object sliderObj = group.AddSlider(label, 0f, 1f, 0.01f, initialHue, onChanged);
            UISlider slider = sliderObj as UISlider;
            if (slider == null)
            {
                return null;
            }

            // Remove the default grey background
            slider.backgroundSprite = string.Empty;
            slider.color = Color.white;
            
            if (backgroundTexture != null)
            {
                slider.clipChildren = true;

                var hueBar = slider.AddUIComponent<UITextureSprite>();
                hueBar.texture = backgroundTexture;
                hueBar.size = slider.size;
                hueBar.relativePosition = Vector3.zero;
                hueBar.zOrder = 0;

                if (slider.thumbObject != null)
                {
                    slider.thumbObject.zOrder = hueBar.zOrder + 1;
                }
            }

            return slider;
        }

        private static UIButton UIButton(UITabstrip tabStrip, string title, Color tintColor)
        {
            var tabButton = tabStrip.AddUIComponent<UIButton>();
            tabButton.text = title;
            tabButton.textColor = tintColor;
            tabButton.autoSize = false;
            tabButton.width = 150f;
            tabButton.height = 30f;
            tabButton.textScale = 0.9f;
            tabButton.normalBgSprite = "ButtonMenu";
            tabButton.hoveredBgSprite = "ButtonMenuHovered";
            tabButton.pressedBgSprite = "ButtonMenuPressed";
            tabButton.disabledBgSprite = "ButtonMenuDisabled";
            return tabButton;
        }

        private static UIPanel Page(UITabContainer tabContainer, string title)
        {
            var page = tabContainer.AddUIComponent<UIPanel>();
            page.name = $"NHO_{title}_Page";
            page.autoLayout = true;
            page.autoLayoutDirection = LayoutDirection.Vertical;
            page.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            page.clipChildren = true;
            return page;
        }
    }
}
