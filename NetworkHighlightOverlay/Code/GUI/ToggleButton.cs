using ColossalFramework.UI;
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
        private IDisposable _bindingSubscription;
        private Action<ToggleButton, ToggleBinding> _onHueEditRequested;
        private UISprite _icon;
        #endregion

        public void Initialize(
            string spriteName,
            ToggleBinding binding,
            string toggleTooltip,
            Action<ToggleButton, ToggleBinding> onHueEditRequested)
        {
            name = "NHO_ToggleButton_" + toggleTooltip.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = toggleTooltip;
            _binding = binding;
            _spriteName = spriteName;
            _onHueEditRequested = onHueEditRequested;
            playAudioEvents = true;

            CacheView();

            eventSizeChanged -= OnButtonSizeChanged;
            eventSizeChanged += OnButtonSizeChanged;

            SubscribeToBindingChanges();
            SetupVisuals();
            ApplyVisualState();
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
            if (TryHandleHueEditMouseDown(p)) return;

            base.OnMouseDown(p);
        }

        public override void OnDestroy()
        {
            _onHueEditRequested = null;
            UnsubscribeFromBindingChanges();
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
        }

        private void OnButtonSizeChanged(UIComponent component, Vector2 value)
        {
            ToggleButtonVisual.UpdateIconLayout(this, _icon);
        }

        private void ApplyVisualState()
        {
            if (_binding == null)
                return;

            opacity = 1f;
            isInteractive = true;
            normalBgSprite = _binding.Value
                ? ToggleButtonAtlas.ActiveSpriteName
                : ToggleButtonAtlas.InactiveSpriteName;
            focusedBgSprite = normalBgSprite;
            ToggleButtonVisual.ApplyBackgroundColors(this, GetConfiguredColor());
        }

        private void SubscribeToBindingChanges()
        {
            if (_bindingSubscription != null || _binding == null)
                return;

            _bindingSubscription = _binding.Subscribe(OnBindingChanged, false);
        }

        private void UnsubscribeFromBindingChanges()
        {
            if (_bindingSubscription == null)
                return;

            _bindingSubscription.Dispose();
            _bindingSubscription = null;
        }

        private void OnBindingChanged()
        {
            ApplyVisualState();
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

            _onHueEditRequested?.Invoke(this, _binding);

            p?.Use();

            return true;
        }

        private Color GetConfiguredColor()
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
                    return null;

                UISprite icon = button.AddUIComponent<UISprite>();
                if (icon == null)
                    return null;

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
