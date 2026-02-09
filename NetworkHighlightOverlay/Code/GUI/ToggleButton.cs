using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private static readonly Color ButtonTintColor = Color.white;
        private static readonly Color IconTintColor = Color.white;
        private static readonly Vector2 IconSize = new Vector2(22f, 22f);

        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _iconAtlas;
        private bool _isSubscribedToSettings;
        private UISprite _icon;

        public void Initialize(string spriteName, ToggleBinding binding, string tooltip)
        {
            name = "NHO_ToggleButton_" + tooltip.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = tooltip;
            _binding = binding;
            _spriteName = spriteName;
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
            if (view != null)
            {
                _iconAtlas = view.defaultAtlas;
            }

            atlas = ToggleButtonAtlas.GetOrCreate();

            ConfigureBackgroundSprites();
            UpdateNormalBackgroundSprite(_binding != null && _binding.Value);
            
            EnsureIcon();
            UpdateIconSprite();
            UpdateIconLayout();
        }

        private void EnsureIcon()
        {
            if (_icon != null)
                return;

            if (_iconAtlas == null)
                return;

            _icon = AddUIComponent<UISprite>();
            _icon.name = name + "_Icon";
            _icon.atlas = _iconAtlas;
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
            UpdateNormalBackgroundSprite(isOn);
            color = ButtonTintColor;
            hoveredColor = ButtonTintColor;
            pressedColor = ButtonTintColor;
            focusedColor = ButtonTintColor;
            disabledColor = ButtonTintColor;

            if (_icon != null)
            {
                _icon.color = IconTintColor;
            }

            opacity = 1f;
            isInteractive = true;
        }

        private void ConfigureBackgroundSprites()
        {
            hoveredBgSprite = ToggleButtonAtlas.HoveredSpriteName;
            pressedBgSprite = ToggleButtonAtlas.PressedSpriteName;
            focusedBgSprite = hoveredBgSprite;
            disabledBgSprite = ToggleButtonAtlas.InactiveSpriteName;
        }

        private void UpdateNormalBackgroundSprite(bool isOn)
        {
            normalBgSprite = isOn
                ? ToggleButtonAtlas.ActiveSpriteName
                : ToggleButtonAtlas.InactiveSpriteName;
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
    }
}
