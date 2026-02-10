using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class HuePopover : UIPanel
    {
        private const float PopoverWidth = 190f;
        private const float PopoverHeight = 42f;
        private const float SliderHeight = 18f;
        private const float SliderPadding = 10f;
        private const float PopoverOffset = 6f;

        private static Texture2D _hueGradientTexture;

        private ToggleButton _anchor;
        private ToggleBinding _binding;
        private UISlider _hueSlider;
        private bool _isApplyingHueValue;
        private int _openedFrame = -1;

        public bool IsOpen => isVisible && _anchor != null && _binding != null;
        public int OpenedFrame => _openedFrame;
        public ToggleButton Anchor => _anchor;

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

        public override void Update()
        {
            base.Update();

            if (!IsOpen)
                return;

            if (_anchor == null || _anchor.parent == null || !_anchor.isVisible)
            {
                Close();
                return;
            }

            UpdatePosition();
            SyncSliderFromBinding();
        }

        public override void OnDestroy()
        {
            if (_hueSlider != null)
            {
                _hueSlider.eventValueChanged -= OnHueSliderValueChanged;
                _hueSlider = null;
            }

            base.OnDestroy();
        }

        public void Open(ToggleButton anchor, ToggleBinding binding)
        {
            if (anchor == null || binding == null || !binding.CanAdjustHue)
                return;

            _anchor = anchor;
            _binding = binding;
            _openedFrame = Time.frameCount;
            isVisible = true;
            BringToFront();
            UpdatePosition();
            SyncSliderFromBinding();
        }

        public void Close()
        {
            _anchor = null;
            _binding = null;
            _openedFrame = -1;
            _isApplyingHueValue = false;
            isVisible = false;
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

        private void UpdatePosition()
        {
            if (_anchor == null)
                return;

            UIView view = UIView.GetAView();
            if (view == null)
                return;

            Vector2 resolution = view.GetScreenResolution();
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

        private void SyncSliderFromBinding()
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

        private static Texture2D GetHueGradientTexture()
        {
            if (_hueGradientTexture == null)
            {
                _hueGradientTexture = ModResources.LoadTexture("HueGradient.png");
            }

            return _hueGradientTexture;
        }
    }
}
