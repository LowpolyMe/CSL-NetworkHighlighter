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
        private ToggleBinding _binding;
        private string _spriteName;
        private bool _isSubscribedToSettings;
        private Action<ToggleButton, ToggleBinding> _onHueEditRequested;
        private UISprite _icon;
        #endregion

        public void Initialize(
            string spriteName,
            ToggleBinding binding,
            string tooltip,
            Action<ToggleButton, ToggleBinding> onHueEditRequested)
        {
            name = "NHO_ToggleButton_" + tooltip.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = tooltip;
            _binding = binding;
            _spriteName = spriteName;
            _onHueEditRequested = onHueEditRequested;
            playAudioEvents = true;

            CacheView();

            eventSizeChanged -= OnButtonSizeChanged;
            eventSizeChanged += OnButtonSizeChanged;

            SubscribeToSettingsChanges();
            SetupVisuals();
            UpdateVisual();
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            if (IsRightMouseClick(p))
                return;

            base.OnClick(p);
            if (_binding == null)
                return;

            _binding.Value = !_binding.Value;
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            if (TryHandleHueEditMouseDown(p))
            {
                return;
            }

            base.OnMouseDown(p);
        }

        public override void OnDestroy()
        {
            _onHueEditRequested = null;
            UnsubscribeFromSettingsChanges();
            eventSizeChanged -= OnButtonSizeChanged;
            base.OnDestroy();
        }

        private void SetupVisuals()
        {
            if (_view == null)
                return;

            atlas = ToggleButtonAtlas.GetOrCreate();
            ToggleButtonVisual.ApplyBackgroundSprites(this);
            _icon = ToggleButtonVisual.EnsureIcon(this, _icon, _view.defaultAtlas, _spriteName);
            UpdateToggleState(_binding != null && _binding.Value);
        }

        private void OnButtonSizeChanged(UIComponent component, Vector2 value)
        {
            ToggleButtonVisual.UpdateIconLayout(this, _icon);
        }

        private void UpdateVisual()
        {
            if (_binding == null)
                return;

            bool isOn = _binding.Value;

            opacity = 1f;
            isInteractive = true;

            UpdateToggleState(isOn);
            ToggleButtonVisual.ApplyBackgroundColors(this, GetColorFromConfig());
        }

        private void UpdateToggleState(bool isOn)
        {
            normalBgSprite = isOn
                ? ToggleButtonAtlas.ActiveSpriteName
                : ToggleButtonAtlas.InactiveSpriteName;
            focusedBgSprite = normalBgSprite;
        }

        private void SubscribeToSettingsChanges()
        {
            if (_isSubscribedToSettings)
                return;

            ModSettings.SettingsChanged += OnSettingsChanged;
            _isSubscribedToSettings = true;
        }

        private void UnsubscribeFromSettingsChanges()
        {
            if (!_isSubscribedToSettings)
                return;

            ModSettings.SettingsChanged -= OnSettingsChanged;
            _isSubscribedToSettings = false;
        }

        private void OnSettingsChanged(Config config)
        {
            UpdateVisual();
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

            if (_binding == null)
                return false;

            if (_onHueEditRequested != null)
            {
                _onHueEditRequested(this, _binding);
            }

            if (p != null)
            {
                p.Use();
            }

            return true;
        }

        private Color GetColorFromConfig()
        {
            if (_binding == null)
                return Color.white;

            Color colorFromConfig = _binding.ColorValue;
            colorFromConfig.a = 1f;
            return colorFromConfig;
        }

        private void CacheView()
        {
            if (_view != null)
                return;

            _view = UIView.GetAView();
        }

        private struct ToggleButtonVisual
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
                    return null;

                UISprite icon = button.AddUIComponent<UISprite>();
                if (icon == null)
                    return null;

                icon.name = button.name + "_Icon";
                icon.atlas = iconAtlas;
                icon.isInteractive = false;

                if (!string.IsNullOrEmpty(spriteName))
                {
                    icon.spriteName = spriteName;
                    UpdateIconLayout(button, icon);
                }

                return icon;
            }

            public static void UpdateIconLayout(ToggleButton button, UISprite icon)
            {
                if (icon == null)
                    return;

                UITextureAtlas.SpriteInfo spriteInfo = icon.spriteInfo;
                if (spriteInfo == null)
                    return;

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
