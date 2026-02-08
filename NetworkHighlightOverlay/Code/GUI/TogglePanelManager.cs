using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public static class TogglePanelManager
    {
        private static TogglePanel _panel;

        public static void Create()
        {
            if (_panel != null)
                return;

            UIView view = UIView.GetAView();
            if (view == null)
                return;

            _panel = view.AddUIComponent(typeof(TogglePanel)) as TogglePanel;
            _panel.isVisible = false;
        }

        public static void Destroy()
        {
            if (_panel == null)
                return;

            UnityEngine.Object.Destroy(_panel.gameObject);
            _panel = null;
        }

        public static void SyncVisibility()
        {
            if (_panel == null)
                return;

            _panel.isVisible = Manager.Instance.IsEnabled;
        }
    }
}
