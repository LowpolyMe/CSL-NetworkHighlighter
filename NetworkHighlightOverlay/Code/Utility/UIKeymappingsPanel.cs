using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public sealed class UIKeymappingsPanel : UICustomControl
    {
        private static readonly string KeyBindingTemplate = "KeyBindingTemplate";

        private SavedInputKey _editingBinding;
        private int _count;

        public UIComponent AddKeymapping(string label, SavedInputKey savedInputKey)
        {
            UIPanel row = component.AttachUIComponent(UITemplateManager.GetAsGameObject(KeyBindingTemplate)) as UIPanel;
            if (row == null)
                throw new System.InvalidOperationException("KeyBindingTemplate must create a UIPanel.");

            if ((_count++ % 2) == 1)
                row.backgroundSprite = null;

            UILabel nameLabel = row.Find<UILabel>("Name");
            UIButton bindingButton = row.Find<UIButton>("Binding");
            if (nameLabel == null || bindingButton == null)
                throw new System.InvalidOperationException("Key binding template is missing required controls.");

            bindingButton.eventKeyDown += OnBindingKeyDown;
            bindingButton.eventMouseDown += OnBindingMouseDown;
            bindingButton.eventVisibilityChanged += OnBindingVisibilityChanged;

            nameLabel.text = label;
            bindingButton.text = savedInputKey.ToLocalizedString("KEYNAME");
            bindingButton.objectUserData = savedInputKey;
            return bindingButton;
        }

        private static bool IsModifierKey(KeyCode code)
        {
            return code == KeyCode.LeftControl ||
                   code == KeyCode.RightControl ||
                   code == KeyCode.LeftShift ||
                   code == KeyCode.RightShift ||
                   code == KeyCode.LeftAlt ||
                   code == KeyCode.RightAlt;
        }

        private static bool IsControlDown()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

        private static bool IsShiftDown()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        private static bool IsAltDown()
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private static bool IsUnbindableMouseButton(UIMouseButton button)
        {
            return button == UIMouseButton.Left || button == UIMouseButton.Right;
        }

        private static KeyCode ButtonToKeycode(UIMouseButton button)
        {
            if (button == UIMouseButton.Left) return KeyCode.Mouse0;
            if (button == UIMouseButton.Right) return KeyCode.Mouse1;
            if (button == UIMouseButton.Middle) return KeyCode.Mouse2;
            if (button == UIMouseButton.Special0) return KeyCode.Mouse3;
            if (button == UIMouseButton.Special1) return KeyCode.Mouse4;
            if (button == UIMouseButton.Special2) return KeyCode.Mouse5;
            if (button == UIMouseButton.Special3) return KeyCode.Mouse6;
            return KeyCode.None;
        }

        private static void OnBindingVisibilityChanged(UIComponent component, bool isVisible)
        {
            if (isVisible && component.objectUserData is SavedInputKey savedInputKey)
                (component as UIButton).text = savedInputKey.ToLocalizedString("KEYNAME");
        }

        private void OnBindingKeyDown(UIComponent component, UIKeyEventParameter parameter)
        {
            if (_editingBinding == null || IsModifierKey(parameter.keycode))
                return;

            parameter.Use();
            UIView.PopModal();

            InputKey value = parameter.keycode == KeyCode.Escape
                ? _editingBinding.value
                : SavedInputKey.Encode(parameter.keycode, parameter.control, parameter.shift, parameter.alt);
            if (parameter.keycode == KeyCode.Backspace)
                value = SavedInputKey.Empty;

            _editingBinding.value = value;
            (parameter.source as UITextComponent).text = _editingBinding.ToLocalizedString("KEYNAME");
            _editingBinding = null;
        }

        private void OnBindingMouseDown(UIComponent component, UIMouseEventParameter parameter)
        {
            if (_editingBinding == null)
            {
                parameter.Use();
                _editingBinding = (SavedInputKey)parameter.source.objectUserData;
                UIButton button = parameter.source as UIButton;
                button.buttonsMask = UIMouseButton.Left |
                                     UIMouseButton.Right |
                                     UIMouseButton.Middle |
                                     UIMouseButton.Special0 |
                                     UIMouseButton.Special1 |
                                     UIMouseButton.Special2 |
                                     UIMouseButton.Special3;
                button.text = "Press any key";
                parameter.source.Focus();
                UIView.PushModal(parameter.source);
                return;
            }

            if (IsUnbindableMouseButton(parameter.buttons))
                return;

            parameter.Use();
            UIView.PopModal();

            InputKey value = SavedInputKey.Encode(
                ButtonToKeycode(parameter.buttons),
                IsControlDown(),
                IsShiftDown(),
                IsAltDown());
            _editingBinding.value = value;

            UIButton sourceButton = parameter.source as UIButton;
            sourceButton.text = _editingBinding.ToLocalizedString("KEYNAME");
            sourceButton.buttonsMask = UIMouseButton.Left;
            _editingBinding = null;
        }
    }
}
