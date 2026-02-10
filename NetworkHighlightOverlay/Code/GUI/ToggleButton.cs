using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private static readonly Vector2 IconSize = new Vector2(30f, 30f);
        private const float HuePopupWidth = 190f;
        private const float HuePopupHeight = 42f;
        private const float HueSliderHeight = 18f;
        private const float HueSliderPadding = 10f;
        private const float HuePopupOffset = 6f;

        private static ToggleButton _openHuePopupOwner;
        private static Texture2D _hueGradientTexture;

        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _iconAtlas;
        private bool _isSubscribedToSettings;
        private bool _isApplyingHueValue;
        private int _huePopupOpenedFrame = -1;
        private UISprite _icon;
        private UIPanel _huePopup;
        private UISlider _hueSlider;

        public static void CloseOpenHuePopup()
        {
            if (_openHuePopupOwner != null)
            {
                _openHuePopupOwner.CloseHuePopup();
            }
        }

        public void Initialize(string spriteName, ToggleBinding binding, string tooltip)
        {
            name = "NHO_ToggleButton_" + tooltip.Replace(' ', '_');
            text = string.Empty;
            this.tooltip = tooltip;
            _binding = binding;
            _spriteName = spriteName;
            playAudioEvents = true;
            eventPositionChanged -= OnButtonPositionChanged;
            eventPositionChanged += OnButtonPositionChanged;
            eventVisibilityChanged -= OnButtonVisibilityChanged;
            eventVisibilityChanged += OnButtonVisibilityChanged;
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
            if (IsRightMouseClick(p))
            {
                ToggleHuePopup();
                if (p != null)
                {
                    p.Use();
                }
                return;
            }

            base.OnMouseDown(p);
        }

        public override void OnDestroy()
        {
            CloseHuePopup();
            UnsubscribeFromSettingsChanges();
            eventPositionChanged -= OnButtonPositionChanged;
            eventVisibilityChanged -= OnButtonVisibilityChanged;
            eventSizeChanged -= OnButtonSizeChanged;
            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();

            if (_huePopup == null)
                return;

            if (Time.frameCount == _huePopupOpenedFrame)
                return;

            if (!Input.GetMouseButtonDown(0) &&
                !Input.GetMouseButtonDown(1) &&
                !Input.GetMouseButtonDown(2))
            {
                return;
            }

            UIView view = UIView.GetAView();
            if (view == null)
            {
                CloseHuePopup();
                return;
            }

            UIComponent activeComponent = UIView.activeComponent;
            if (IsSelfOrChildOf(activeComponent, _huePopup))
                return;

            CloseHuePopup();
        }

        private void SetupVisuals()
        {
            UIView view = UIView.GetAView();

            if (view == null)
                return;

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
            UpdateHuePopupPosition();
        }

        private void OnButtonPositionChanged(UIComponent component, Vector2 value)
        {
            UpdateHuePopupPosition();
        }

        private void OnButtonVisibilityChanged(UIComponent component, bool value)
        {
            if (!value)
            {
                CloseHuePopup();
            }
        }

        private void UpdateVisual()
        {
            if (_binding == null)
                return;

            bool isOn = _binding.Value;

            opacity = 1f;
            isInteractive = true;

            UpdateToggleState(isOn);
            UpdateBackgroundColor();
            UpdateHueSliderValue();
        }

        private void AddToggleBackground()
        {
            hoveredBgSprite = ToggleButtonAtlas.HoveredSpriteName;
            pressedBgSprite = ToggleButtonAtlas.PressedSpriteName;
            disabledBgSprite = ToggleButtonAtlas.InactiveSpriteName;

            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
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

        private void ToggleHuePopup()
        {
            if (_binding == null || !_binding.CanAdjustHue)
                return;

            if (_huePopup != null)
            {
                CloseHuePopup();
                return;
            }

            OpenHuePopup();
        }

        private void OpenHuePopup()
        {
            UIView view = UIView.GetAView();
            if (view == null)
                return;

            if (_openHuePopupOwner != null && _openHuePopupOwner != this)
            {
                _openHuePopupOwner.CloseHuePopup();
            }

            _huePopup = view.AddUIComponent(typeof(UIPanel)) as UIPanel;
            if (_huePopup == null)
                return;

            _huePopup.name = name + "_HuePopup";
            _huePopup.backgroundSprite = "GenericPanel";
            _huePopup.color = new Color32(35, 35, 35, 235);
            _huePopup.width = HuePopupWidth;
            _huePopup.height = HuePopupHeight;
            _huePopup.isInteractive = true;
            _huePopup.clipChildren = true;

            CreateHueSlider();
            UpdateHuePopupPosition();
            UpdateHueSliderValue();
            _huePopupOpenedFrame = Time.frameCount;

            _openHuePopupOwner = this;
        }

        private void CreateHueSlider()
        {
            if (_huePopup == null)
                return;

            _hueSlider = _huePopup.AddUIComponent<UISlider>();
            _hueSlider.name = _huePopup.name + "_Slider";
            _hueSlider.minValue = 0f;
            _hueSlider.maxValue = 1f;
            _hueSlider.stepSize = 0.01f;
            _hueSlider.width = HuePopupWidth - (HueSliderPadding * 2f);
            _hueSlider.height = HueSliderHeight;
            _hueSlider.relativePosition = new Vector3(
                HueSliderPadding,
                (HuePopupHeight - HueSliderHeight) * 0.5f);
            _hueSlider.backgroundSprite = string.Empty;
            _hueSlider.color = Color.white;

            UIButton thumb = _hueSlider.AddUIComponent<UIButton>();
            thumb.width = 14f;
            thumb.height = HueSliderHeight + 6f;
            thumb.normalBgSprite = "SliderBudget";
            thumb.hoveredBgSprite = "SliderBudgetHovered";
            thumb.pressedBgSprite = "SliderBudgetPressed";
            thumb.disabledBgSprite = "SliderBudget";
            _hueSlider.thumbObject = thumb;

            Texture2D texture = GetHueGradientTexture();
            if (texture != null)
            {
                UITextureSprite hueBar = _hueSlider.AddUIComponent<UITextureSprite>();
                hueBar.texture = texture;
                hueBar.size = _hueSlider.size;
                hueBar.relativePosition = Vector3.zero;
                hueBar.zOrder = 0;

                if (_hueSlider.thumbObject != null)
                {
                    _hueSlider.thumbObject.zOrder = hueBar.zOrder + 1;
                }
            }

            _hueSlider.eventValueChanged += OnHueSliderValueChanged;
        }

        private void UpdateHuePopupPosition()
        {
            if (_huePopup == null)
                return;

            UIView view = UIView.GetAView();
            if (view == null)
                return;

            Vector2 resolution = view.GetScreenResolution();
            float x = absolutePosition.x + width + HuePopupOffset;
            float y = absolutePosition.y + (height - HuePopupHeight) * 0.5f;

            if (x + HuePopupWidth > resolution.x)
            {
                x = absolutePosition.x - HuePopupWidth - HuePopupOffset;
            }

            x = Mathf.Clamp(x, 0f, Mathf.Max(0f, resolution.x - HuePopupWidth));
            y = Mathf.Clamp(y, 0f, Mathf.Max(0f, resolution.y - HuePopupHeight));

            _huePopup.absolutePosition = new Vector3(x, y);
        }

        private void UpdateHueSliderValue()
        {
            if (_hueSlider == null || _binding == null || !_binding.CanAdjustHue)
                return;

            float hue = Mathf.Clamp01(_binding.HueValue);
            if (Mathf.Abs(_hueSlider.value - hue) <= 0.0001f)
                return;

            _isApplyingHueValue = true;
            _hueSlider.value = hue;
            _isApplyingHueValue = false;
        }

        private void OnHueSliderValueChanged(UIComponent component, float value)
        {
            if (_isApplyingHueValue || _binding == null || !_binding.CanAdjustHue)
                return;

            _binding.HueValue = Mathf.Clamp01(value);
        }

        private void CloseHuePopup()
        {
            if (_hueSlider != null)
            {
                _hueSlider.eventValueChanged -= OnHueSliderValueChanged;
                _hueSlider = null;
            }

            if (_huePopup != null)
            {
                UnityEngine.Object.Destroy(_huePopup.gameObject);
                _huePopup = null;
            }

            _isApplyingHueValue = false;
            _huePopupOpenedFrame = -1;

            if (_openHuePopupOwner == this)
            {
                _openHuePopupOwner = null;
            }
        }

        private static Texture2D GetHueGradientTexture()
        {
            if (_hueGradientTexture == null)
            {
                _hueGradientTexture = ModResources.LoadTexture("HueGradient.png");
            }

            return _hueGradientTexture;
        }

        private static bool IsRightMouseClick(UIMouseEventParameter p)
        {
            if (p == null)
                return false;

            return (p.buttons & UIMouseButton.Right) == UIMouseButton.Right;
        }

        private static bool IsSelfOrChildOf(UIComponent component, UIComponent parent)
        {
            if (component == null || parent == null)
                return false;

            UIComponent current = component;
            while (current != null)
            {
                if (current == parent)
                    return true;

                current = current.parent;
            }

            return false;
        }

        private Color GetColorFromConfig()
        {
            if (_binding == null)
                return Color.white;

            Color colorFromConfig = _binding.ColorValue;
            colorFromConfig.a = 1f;
            return colorFromConfig;
        }
    }
}
