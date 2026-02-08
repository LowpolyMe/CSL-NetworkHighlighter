using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private const string BackgroundSpriteName = "OptionBasePressed";
        private static readonly Color OnBackgroundColor = new Color(0.20f, 0.62f, 0.98f);
        private static readonly Color OffBackgroundColor = new Color(0.42f, 0.42f, 0.42f);
        private static readonly Color OnHoverBackgroundColor = new Color(0.28f, 0.70f, 1.00f);
        private static readonly Color OffHoverBackgroundColor = new Color(0.42f, 0.42f, 0.42f);
        private static readonly Color IconTintColor = Color.white;
        private static readonly Vector2 IconSize = new Vector2(22f, 22f);

        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _atlas;
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
            color = isOn ? OnBackgroundColor : OffBackgroundColor;
            hoveredColor = isOn ? OnHoverBackgroundColor : OffHoverBackgroundColor;
            pressedColor = hoveredColor;
            focusedColor = hoveredColor;
            disabledColor = OffBackgroundColor;

            if (_icon != null)
            {
                _icon.color = IconTintColor;
            }

            opacity = 1f;
            isInteractive = true;
        }
    }
}
