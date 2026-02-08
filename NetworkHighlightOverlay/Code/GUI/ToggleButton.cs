using ColossalFramework.UI;
using UnityEngine;
using System;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private const string BackgroundSpriteName = "OptionBasePressed";
        private static readonly Color MutedBlendColor = new Color(0.26f, 0.26f, 0.26f);
        private static readonly Color IconTintColor = Color.white;
        private static readonly Vector2 IconSize = new Vector2(22f, 22f);
        private static readonly Color FallbackOnBackgroundColor = new Color(0.20f, 0.62f, 0.98f);

        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _atlas;
        private UISprite _icon;
        private Func<Color> _activeColorProvider;

        public void Initialize(string spriteName, ToggleBinding binding, string tooltip, Func<Color> activeColorProvider)
        {
            name = "NHO_ToggleButton_" + tooltip.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = tooltip;
            _binding = binding;
            _spriteName = spriteName;
            _activeColorProvider = activeColorProvider;
            playAudioEvents = true;
            eventSizeChanged -= OnButtonSizeChanged;
            eventSizeChanged += OnButtonSizeChanged;
            SetupVisuals();
            UpdateVisual();
        }

        public void Refresh()
        {
            UpdateVisual();
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            if (_binding == null)
                return;

            _binding.Value = !_binding.Value;
            UpdateVisual();
        }

        private void SetupVisuals()
        {
            if (_atlas == null)
            {
                UIView view = UIView.GetAView();
                if (view != null)
                {
                    _atlas = view.defaultAtlas;
                }
            }

            if (_atlas == null)
                return;

            atlas = _atlas;

            normalBgSprite = BackgroundSpriteName;
            hoveredBgSprite = BackgroundSpriteName;
            pressedBgSprite = BackgroundSpriteName;
            focusedBgSprite = BackgroundSpriteName;
            disabledBgSprite = BackgroundSpriteName;

            normalFgSprite = string.Empty;
            hoveredFgSprite = string.Empty;
            pressedFgSprite = string.Empty;
            focusedFgSprite = string.Empty;
            disabledFgSprite = string.Empty;

            EnsureIcon();
            UpdateIconSprite();
            UpdateIconLayout();
        }

        private void EnsureIcon()
        {
            if (_icon != null)
                return;

            _icon = AddUIComponent<UISprite>();
            _icon.name = name + "_Icon";
            _icon.atlas = _atlas;
            _icon.color = IconTintColor;
            _icon.isInteractive = false;
        }

        private void UpdateIconSprite()
        {
            if (_icon == null || string.IsNullOrEmpty(_spriteName))
                return;

            _icon.spriteName = _spriteName;
        }

        private void UpdateIconLayout()
        {
            if (_icon == null)
                return;

            _icon.size = IconSize;
            _icon.relativePosition = new Vector3(
                (width - IconSize.x) * 0.5f,
                (height - IconSize.y) * 0.5f);
        }

        private void OnButtonSizeChanged(UIComponent component, Vector2 value)
        {
            UpdateIconLayout();
        }

        private void UpdateVisual()
        {
            if (_binding == null)
                return;

            bool isOn = _binding.Value;
            Color activeColor = ResolveActiveColor();
            Color activeHoverColor = Color.Lerp(activeColor, Color.white, 0.2f);
            Color mutedColor = GetMutedColor(activeColor);
            Color mutedHoverColor = Color.Lerp(mutedColor, activeColor, 0.15f);

            color = isOn ? activeColor : mutedColor;
            hoveredColor = isOn ? activeHoverColor : mutedHoverColor;
            pressedColor = hoveredColor;
            focusedColor = hoveredColor;
            disabledColor = mutedColor;

            if (_icon != null)
            {
                _icon.color = IconTintColor;
            }

            opacity = 1f;
            isInteractive = true;
        }

        private Color ResolveActiveColor()
        {
            if (_activeColorProvider == null)
                return FallbackOnBackgroundColor;

            Color colorFromConfig = _activeColorProvider();
            colorFromConfig.a = 1f;
            return colorFromConfig;
        }

        private static Color GetMutedColor(Color source)
        {
            Color muted = Color.Lerp(source, MutedBlendColor, 0.6f);
            muted.a = 1f;
            return muted;
        }
    }
}
