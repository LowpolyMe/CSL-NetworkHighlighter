using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public static class UIUtility
    {
        public static UIComponent GetRootComponent(UIHelperBase helper)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (helper is UIHelper uiHelper && uiHelper.self is UIComponent uiComponent)
                return uiComponent;

            Type helperType = helper.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Exception firstLookupException = null;
            
            foreach (PropertyInfo prop in helperType.GetProperties(flags))
            {
                if (!typeof(UIComponent).IsAssignableFrom(prop.PropertyType))
                    continue;

                try
                {
                    UIComponent value = prop.GetValue(helper, null) as UIComponent;
                    if (value != null)
                        return value;
                }
                catch (Exception ex)
                {
                    if (firstLookupException == null)
                    {
                        firstLookupException = ex;
                    }
                }
            }

            foreach (FieldInfo field in helperType.GetFields(flags))
            {
                if (!typeof(UIComponent).IsAssignableFrom(field.FieldType))
                    continue;

                try
                {
                    UIComponent value = field.GetValue(helper) as UIComponent;
                    if (value != null)
                        return value;
                }
                catch (Exception ex)
                {
                    if (firstLookupException == null)
                    {
                        firstLookupException = ex;
                    }
                }
            }

            if (firstLookupException != null)
                throw new InvalidOperationException(
                    "Could not resolve a root UIComponent from helper type '" + helperType.FullName + "'.",
                    firstLookupException);

            throw new InvalidOperationException(
                "Helper type '" + helperType.FullName + "' must expose a UIComponent field or property.");
        }

        public static UIHelper CreateTab(UITabContainer tabContainer, UITabstrip tabStrip, string title, Color tintColor,
            out UIPanel page)
        {
            
            page = CreateTabPage(tabContainer, title);
            UIButton tabButton = CreateTabButton(tabStrip, title, tintColor);

            tabStrip.AddTab(title, tabButton.gameObject, page.gameObject);
            
            return new UIHelper(page);
        }

        public static UIKeymappingsPanel AddKeymappingsPanel(UIHelper helper)
        {
            UIComponent component = helper.self as UIComponent;
            if (component == null)
                throw new InvalidOperationException("UIHelper.self must be a UIComponent for keymapping controls.");

            return component.gameObject.AddComponent<UIKeymappingsPanel>();
        }

        public static UISlider CreateHueSlider(UIHelper group, string label, float initialHue, OnValueChanged onChanged,
            Texture2D backgroundTexture)
        {
            if (backgroundTexture == null)
                throw new InvalidOperationException("Hue slider background texture is required.");

            object sliderObj = group.AddSlider(label, 0f, 1f, 0.01f, initialHue, onChanged);
            UISlider slider = sliderObj as UISlider;
            if (slider == null)
                throw new InvalidOperationException("UIHelper.AddSlider must return a UISlider.");

            slider.backgroundSprite = string.Empty;
            slider.color = Color.white;
            slider.clipChildren = true;

            UITextureSprite hueBar = slider.AddUIComponent<UITextureSprite>();
            hueBar.texture = backgroundTexture;
            hueBar.size = slider.size;
            hueBar.relativePosition = Vector3.zero;
            hueBar.zOrder = 0;
            slider.thumbObject.zOrder = hueBar.zOrder + 1;

            return slider;
        }

        private static UIButton CreateTabButton(UITabstrip tabStrip, string title, Color tintColor)
        {
            UIButton tabButton = tabStrip.AddUIComponent<UIButton>();
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

        private static UIPanel CreateTabPage(UITabContainer tabContainer, string title)
        {
            UIPanel page = tabContainer.AddUIComponent<UIPanel>();
            page.name = $"NHO_{title}_Page";
            page.autoLayout = true;
            page.autoLayoutDirection = LayoutDirection.Vertical;
            page.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            page.clipChildren = true;
            return page;
        }
    }
}
