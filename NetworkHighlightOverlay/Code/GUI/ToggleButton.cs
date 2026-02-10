using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using System;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private static readonly Vector2 IconSize = new Vector2(30f, 30f);

        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _iconAtlas;
        private bool _isSubscribedToSettings;
        private Func<Color> _activeColorProvider;
        private UISprite _icon;

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
            SubscribeToSettingsChanges();
            SetupVisuals();
            UpdateVisual();
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            if (_binding == null)
                return;

            _binding.Value = !_binding.Value;
        }

        public override void OnDestroy()
        {
            UnsubscribeFromSettingsChanges();
            base.OnDestroy();
        }

        private void SetupVisuals()
        {
            UIView view = UIView.GetAView();

            if (view == null) return;
            
            _iconAtlas = view.defaultAtlas;
            atlas = ToggleButtonAtlas.GetOrCreate();

            AddToggleBackground();
            AddToggleIcon();
            
            UpdateToggleState(_binding != null && _binding.Value);
        }
        

        private void AddToggleIcon()
        {
            if (_icon != null)
                return;

            if (_iconAtlas == null)
                return;

            _icon = AddUIComponent<UISprite>();
            _icon.name = name + "_Icon";
            _icon.atlas = _iconAtlas;
            _icon.isInteractive = false;
            
            if (_icon == null || string.IsNullOrEmpty(_spriteName))
                return;

            _icon.spriteName = _spriteName;
            UpdateIconLayout();
        }

        private void UpdateIconLayout()
        {
            if (_icon == null)
                return;
            
            UITextureAtlas.SpriteInfo spriteInfo = _icon.spriteInfo;
            if (spriteInfo != null)
            {
                Vector2 targetSize = ComputeTargetSizeWithAspectRatioIntact(spriteInfo.pixelSize);
                _icon.size = new Vector2(targetSize.x, targetSize.y);
                _icon.relativePosition = new Vector3((width - targetSize.x) * 0.5f, (height - targetSize.y) * 0.5f);
            }
        }

        private Vector2 ComputeTargetSizeWithAspectRatioIntact(Vector2 pixelSize)
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

        private void OnButtonSizeChanged(UIComponent component, Vector2 value)
        {
            UpdateIconLayout();
        }

        private void UpdateVisual()
        {
            if (_binding == null)
                return;

            bool isOn = _binding.Value;

            opacity = 1f;
            isInteractive = true;
            
            UpdateToggleState(isOn);
        }

        private void AddToggleBackground()
        {
            hoveredBgSprite = ToggleButtonAtlas.HoveredSpriteName;
            pressedBgSprite = ToggleButtonAtlas.PressedSpriteName;
            disabledBgSprite = ToggleButtonAtlas.InactiveSpriteName;
            
            Color activeColor = GetColorFromConfig();
            color = activeColor;
            hoveredColor = activeColor;
            pressedColor = activeColor;
            focusedColor = activeColor;
            disabledColor = activeColor;
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

        private Color GetColorFromConfig()
        {
            if (_activeColorProvider == null)
                return Color.white;

            Color colorFromConfig = _activeColorProvider();
            colorFromConfig.a = 1f;
            return colorFromConfig;
        }
    }
}
