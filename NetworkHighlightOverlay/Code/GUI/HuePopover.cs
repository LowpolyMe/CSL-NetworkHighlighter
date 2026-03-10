using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using NetworkHighlightOverlay.Code.Utility;
using System;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class HuePopover : UIPanel
    {
        #region Constants
        private const float PopoverWidth = 190f;
        private const float PopoverHeight = 42f;
        private const float SliderHeight = 18f;
        private const float SliderPadding = 10f;
        private const float PopoverOffset = 6f;

        private Texture2D _hueGradientTexture;
        #endregion

        #region Fields
        private ToggleButton _anchor;
        private ModSettings _settings;
        private HighlightCategoryId _categoryId;
        private bool _hasCategorySelection;
        private UIView _view;
        private UISlider _hueSlider;
        private bool _isApplyingHueValue;
        private bool _waitForMouseReleaseAfterOpen;
        #endregion

        public bool IsOpen => isVisible && _anchor != null && _settings != null && _hasCategorySelection;

        public override void Awake()
        {
            base.Awake();
            name = "NHO_HuePopover";
            backgroundSprite = "GenericPanel";
            color = new Color32(35, 35, 35, 235);
            width = PopoverWidth;
            height = PopoverHeight;
            isInteractive = true;
            clipChildren = true;
            isVisible = false;

            CreateSlider();
        }

        public override void Start()
        {
            base.Start();
            CacheView();
        }

        public override void OnDestroy()
        {
            if (_hueSlider != null)
            {
                _hueSlider.eventValueChanged -= OnHueSliderValueChanged;
                _hueSlider = null;
            }

            if (_hueGradientTexture != null)
            {
                UnityEngine.Object.Destroy(_hueGradientTexture);
                _hueGradientTexture = null;
            }

            base.OnDestroy();
        }

        public void Open(ToggleButton toggleButton, ModSettings settings, HighlightCategoryId categoryId)
        {
            if (toggleButton == null)
                throw new ArgumentNullException("toggleButton");

            if (settings == null)
                throw new ArgumentNullException("settings");

            _anchor = toggleButton;
            _settings = settings;
            _categoryId = categoryId;
            _hasCategorySelection = true;
            _waitForMouseReleaseAfterOpen = IsAnyMouseButtonHeld();
            isVisible = true;
            BringToFront();
            UpdatePosition();
            RefreshFromSettings();
        }

        public void Close()
        {
            _anchor = null;
            _settings = null;
            _hasCategorySelection = false;
            _isApplyingHueValue = false;
            _waitForMouseReleaseAfterOpen = false;
            isVisible = false;
        }

        public void RefreshFromSettings()
        {
            UpdatePosition();
            SyncSliderFromSettings();
        }

        private void CreateSlider()
        {
            _hueSlider = AddUIComponent<UISlider>();
            _hueSlider.name = name + "_Slider";
            _hueSlider.minValue = 0f;
            _hueSlider.maxValue = 1f;
            _hueSlider.stepSize = 0.01f;
            _hueSlider.width = PopoverWidth - (SliderPadding * 2f);
            _hueSlider.height = SliderHeight;
            _hueSlider.relativePosition = new Vector3(
                SliderPadding,
                (PopoverHeight - SliderHeight) * 0.5f);
            _hueSlider.backgroundSprite = string.Empty;
            _hueSlider.color = Color.white;

            UIButton thumb = _hueSlider.AddUIComponent<UIButton>();
            thumb.width = 14f;
            thumb.height = SliderHeight + 6f;
            thumb.normalBgSprite = "SliderBudget";
            thumb.hoveredBgSprite = "SliderBudgetHovered";
            thumb.pressedBgSprite = "SliderBudgetPressed";
            thumb.disabledBgSprite = "SliderBudget";
            _hueSlider.thumbObject = thumb;

            Texture2D texture = GetHueGradientTexture();
            UITextureSprite hueBar = _hueSlider.AddUIComponent<UITextureSprite>();
            hueBar.texture = texture;
            hueBar.size = _hueSlider.size;
            hueBar.relativePosition = Vector3.zero;
            hueBar.zOrder = 0;
            _hueSlider.thumbObject.zOrder = hueBar.zOrder + 1;

            _hueSlider.eventValueChanged += OnHueSliderValueChanged;
        }

        public bool IsAnchoredTo(ToggleButton button) => IsOpen && _anchor == button;

        public bool ShouldCloseForCurrentMouseClick()
        {
            if (!IsOpen)
                return false;

            if (_waitForMouseReleaseAfterOpen)
            {
                if (IsAnyMouseButtonHeld())
                    return false;

                _waitForMouseReleaseAfterOpen = false;
            }

            if (!IsAnyMouseButtonDownThisFrame())
                return false;

            return !containsMouse;
        }

        private void UpdatePosition()
        {
            Vector2 resolution = _view.GetScreenResolution();
            float x = _anchor.absolutePosition.x + _anchor.width + PopoverOffset;
            float y = _anchor.absolutePosition.y + (_anchor.height - PopoverHeight) * 0.5f;

            if (x + PopoverWidth > resolution.x)
            {
                x = _anchor.absolutePosition.x - PopoverWidth - PopoverOffset;
            }

            x = Mathf.Clamp(x, 0f, Mathf.Max(0f, resolution.x - PopoverWidth));
            y = Mathf.Clamp(y, 0f, Mathf.Max(0f, resolution.y - PopoverHeight));
            absolutePosition = new Vector3(x, y);
        }

        private void SyncSliderFromSettings()
        {
            if (!_hasCategorySelection)
                return;

            float hue = Mathf.Clamp01(_settings.GetCategoryHue(_categoryId));
            if (Mathf.Abs(_hueSlider.value - hue) <= 0.0001f)
                return;

            _isApplyingHueValue = true;
            _hueSlider.value = hue;
            _isApplyingHueValue = false;
        }

        private void OnHueSliderValueChanged(UIComponent component, float value)
        {
            if (_isApplyingHueValue || !_hasCategorySelection)
                return;

            _settings.SetCategoryHue(_categoryId, Mathf.Clamp01(value));
        }

        private Texture2D GetHueGradientTexture()
        {
            if (_hueGradientTexture == null)
            {
                _hueGradientTexture = ModResources.LoadTexture("HueGradient.png");
                if (_hueGradientTexture == null)
                    throw new InvalidOperationException("Missing required texture: Resources/HueGradient.png");
            }

            return _hueGradientTexture;
        }

        private static bool IsAnyMouseButtonHeld() => Input.GetMouseButton(0) ||
                                                      Input.GetMouseButton(1) ||
                                                      Input.GetMouseButton(2);

        private static bool IsAnyMouseButtonDownThisFrame() => Input.GetMouseButtonDown(0) ||
                                                                Input.GetMouseButtonDown(1) ||
                                                                Input.GetMouseButtonDown(2);

        private void CacheView()
        {
            if (_view != null)
                return;

            _view = UIView.GetAView();
            if (_view == null)
                throw new InvalidOperationException("HuePopover requires an active UIView.");
        }
    }
}
