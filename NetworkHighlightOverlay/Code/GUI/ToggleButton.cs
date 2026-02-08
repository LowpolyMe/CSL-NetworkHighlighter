using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class ToggleButton : UIButton
    {
        private ToggleBinding _binding;
        private string _spriteName;
        private UITextureAtlas _atlas;
        private static readonly Color OnColor = Color.white;
        private static readonly Color OffColor = Color.gray;
        private static readonly Color OnHoverColor = new Color(0.26f, 0.7f, 1f);
        private static readonly Color OffHoverColor = new Color(0.7f, 0.7f, 0.7f);

        public void Initialize(string spriteName, ToggleBinding binding, string tooltip)
        {
            name = "NHO_ToggleButton_" + tooltip.Replace(' ', '_');
            this.tooltip = tooltip;
            _binding = binding;
            _spriteName = spriteName;
            playAudioEvents = true;
            SetupSprite();
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

        private void SetupSprite()
        {
            if (_atlas == null)
            {
                UIView view = UIView.GetAView();
                if (view != null)
                {
                    _atlas = view.defaultAtlas;
                }
            }

            if (_atlas == null || string.IsNullOrEmpty(_spriteName))
                return;

            atlas = _atlas;
            normalFgSprite = _spriteName;
            hoveredFgSprite = _spriteName;
            pressedFgSprite = _spriteName;
            focusedFgSprite = _spriteName;
            disabledFgSprite = _spriteName;
        }

        private void UpdateVisual()
        {
            if (_binding == null)
                return;

            bool isOn = _binding.Value;
            
            color = isOn ? OnColor : OffColor;
            hoveredColor = isOn ? OnHoverColor : OffHoverColor;
            pressedColor = hoveredColor;
            focusedColor = hoveredColor;
            opacity = 1f;
            isInteractive = true;
        }
    }
}
