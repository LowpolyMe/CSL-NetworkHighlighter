using UnityEngine;
using UnifiedUI.Helpers;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.GUI;
using NetworkHighlightOverlay.Code.Utility;

namespace NetworkHighlightOverlay.Code.UI
{
    public static class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";

        private static UUICustomButton _button;
        private static bool _syncing;

        public static void RegisterUui()
        {
            if (_button != null && _button.Button != null)
            {
                return;
            }


            Texture2D iconTexture = ModResources.LoadTexture("UUIIcon.png");

            _button = UUIHelpers.RegisterCustomButton(
                name: ButtonName,
                groupName: null,
                tooltip: "Toggle Network Highlight Overlay",
                icon: iconTexture,
                onToggle: OnButtonToggled,
                onToolChanged: null,
                hotkeys: null
            );
            
            _syncing = true;
            _button.IsPressed = Manager.Instance.IsEnabled;
            _syncing = false;
        }

        private static void OnButtonToggled(bool isPressed)
        {
            if (_syncing)
            {
                return;
            }
            Manager.Instance.IsEnabled = isPressed;
            TogglePanelManager.SyncVisibility();
        }
        
        public static void SyncFromManager()
        {
            if (_button == null)
            {
                return;
            }

            bool desired = Manager.Instance.IsEnabled;
            if (_button.IsPressed == desired)
            {
                return;
            }

            _syncing = true;
            _button.IsPressed = desired;
            _syncing = false;
        }
    }
}
