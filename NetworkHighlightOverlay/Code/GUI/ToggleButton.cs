using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using System;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        #region Fields
        private UIView _view;
        private ModSettings _settings;
        private HighlightCategoryId _categoryId;
        private ToggleButtonAtlas _toggleButtonAtlas;
        private string _spriteName;
        private Action<ToggleButton, HighlightCategoryId> _onHueEditRequested;
        private UISprite _icon;
        #endregion

        public void Initialize(
            HighlightCategoryDefinition categoryDefinition,
            ModSettings settings,
            ToggleButtonAtlas toggleButtonAtlas,
            Action<ToggleButton, HighlightCategoryId> onHueEditRequested)
        {
            if (categoryDefinition == null)
                throw new ArgumentNullException("categoryDefinition");

            if (settings == null)
                throw new ArgumentNullException("settings");

            name = "NHO_ToggleButton_" + categoryDefinition.ToggleLabel.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = categoryDefinition.ToggleLabel + "\nRight-click to change color";
            _settings = settings;
            _categoryId = categoryDefinition.Id;
            if (toggleButtonAtlas == null)
                throw new ArgumentNullException("toggleButtonAtlas");

            if (onHueEditRequested == null)
                throw new ArgumentNullException("onHueEditRequested");

            _toggleButtonAtlas = toggleButtonAtlas;
            _spriteName = categoryDefinition.SpriteName;
            _onHueEditRequested = onHueEditRequested;
            playAudioEvents = true;

            CacheView();

            eventSizeChanged -= OnButtonSizeChanged;
            eventSizeChanged += OnButtonSizeChanged;

            SetupVisuals();
            RefreshFromSettings();
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            if (IsRightMouseClick(p))
                return;

            base.OnClick(p);
            _settings.SetCategoryEnabled(_categoryId, !_settings.GetCategoryEnabled(_categoryId));
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            if (TryHandleHueEditMouseDown(p)) return;

            base.OnMouseDown(p);
        }

        public override void OnDestroy()
        {
            _onHueEditRequested = null;
            eventSizeChanged -= OnButtonSizeChanged;
            base.OnDestroy();
        }

        public void RefreshFromSettings()
        {
            ApplyVisualState();
        }

        private void SetupVisuals()
        {
            atlas = _toggleButtonAtlas.GetOrCreate();
            ToggleButtonVisual.ApplyBackgroundSprites(this);
            _icon = ToggleButtonVisual.EnsureIcon(this, _icon, _view.defaultAtlas, _spriteName);
        }

        private void OnButtonSizeChanged(UIComponent component, Vector2 value)
        {
            ToggleButtonVisual.UpdateIconLayout(this, _icon);
        }

        private void ApplyVisualState()
        {
            opacity = 1f;
            isInteractive = true;
            normalBgSprite = _settings.GetCategoryEnabled(_categoryId)
                ? ToggleButtonAtlas.ActiveSpriteName
                : ToggleButtonAtlas.InactiveSpriteName;
            focusedBgSprite = normalBgSprite;
            ToggleButtonVisual.ApplyBackgroundColors(this, GetConfiguredColor());
        }

        private static bool IsRightMouseClick(UIMouseEventParameter p)
        {
            if (p == null)
                return false;

            return (p.buttons & UIMouseButton.Right) == UIMouseButton.Right;
        }

        private bool TryHandleHueEditMouseDown(UIMouseEventParameter p)
        {
            if (!IsRightMouseClick(p))
                return false;

            _onHueEditRequested(this, _categoryId);

            p?.Use();

            return true;
        }

        private Color GetConfiguredColor()
        {
            Color colorFromConfig = _settings.GetCategoryColor(_categoryId);
            colorFromConfig.a = 1f;
            return colorFromConfig;
        }

        private void CacheView()
        {
            if (_view != null)
                return;

            _view = UIView.GetAView();
            if (_view == null)
                throw new InvalidOperationException("ToggleButton requires an active UIView.");
        }

        private static class ToggleButtonVisual
        {
            private static readonly Vector2 IconSize = new Vector2(30f, 30f);

            public static void ApplyBackgroundSprites(ToggleButton button)
            {
                button.hoveredBgSprite = ToggleButtonAtlas.HoveredSpriteName;
                button.pressedBgSprite = ToggleButtonAtlas.PressedSpriteName;
                button.disabledBgSprite = ToggleButtonAtlas.InactiveSpriteName;
            }

            public static void ApplyBackgroundColors(ToggleButton button, Color activeColor)
            {
                button.color = activeColor;
                button.hoveredColor = activeColor;
                button.pressedColor = activeColor;
                button.focusedColor = activeColor;
                button.disabledColor = activeColor;
            }

            public static UISprite EnsureIcon(
                ToggleButton button,
                UISprite existingIcon,
                UITextureAtlas iconAtlas,
                string spriteName)
            {
                if (existingIcon != null)
                    return existingIcon;

                if (iconAtlas == null)
                    throw new InvalidOperationException("ToggleButton requires a default icon atlas.");

                UISprite icon = button.AddUIComponent<UISprite>();
                if (icon == null)
                    throw new InvalidOperationException("ToggleButton failed to create its icon sprite.");

                icon.name = button.name + "_Icon";
                icon.atlas = iconAtlas;
                icon.isInteractive = false;

                if (string.IsNullOrEmpty(spriteName)) return icon;
                
                icon.spriteName = spriteName;
                UpdateIconLayout(button, icon);

                return icon;
            }

            public static void UpdateIconLayout(ToggleButton button, UISprite icon)
            {
                if (icon == null)
                    throw new ArgumentNullException("icon");

                UITextureAtlas.SpriteInfo spriteInfo = icon.spriteInfo;
                if (spriteInfo == null)
                    throw new InvalidOperationException(
                        "ToggleButton icon sprite '" + icon.spriteName + "' was not found in the atlas.");

                Vector2 targetSize = ComputeTargetSizeWithAspectRatioIntact(spriteInfo.pixelSize);
                icon.size = new Vector2(targetSize.x, targetSize.y);
                icon.relativePosition = new Vector3(
                    (button.width - targetSize.x) * 0.5f,
                    (button.height - targetSize.y) * 0.5f);
            }

            private static Vector2 ComputeTargetSizeWithAspectRatioIntact(Vector2 pixelSize)
            {
                float targetWidth = IconSize.x;
                float targetHeight = IconSize.y;

                if (!(pixelSize.x > 0f) || !(pixelSize.y > 0f))
                    return new Vector2(targetWidth, targetHeight);

                if (pixelSize.x >= pixelSize.y)
                {
                    targetWidth = IconSize.x;
                    targetHeight = IconSize.x * (pixelSize.y / pixelSize.x);
                }
                else
                {
                    targetHeight = IconSize.y;
                    targetWidth = IconSize.y * (pixelSize.x / pixelSize.y);
                }

                return new Vector2(targetWidth, targetHeight);
            }
        }
    }
}
