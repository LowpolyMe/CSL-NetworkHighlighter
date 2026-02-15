using UnityEngine;
using NetworkHighlightOverlay.Code.ModOptions;

namespace NetworkHighlightOverlay.Code.Core
{
    public class ActivationHandler : MonoBehaviour
    {
        private void Update()
        {
            if (ModSettings.ToggleOverlayHotkey.IsPressed())
            {
                Manager.Instance.IsEnabled = !Manager.Instance.IsEnabled;
            }
        }
        //todo: also activate when current tool is road draw tool IF enabled in options menu
        //todo: also activate when current tool is pedestrian path draw tool IF enabled in options menu
        //todo: also activate when current tool is ANY network draw tool IF enabled in options menu
    }
}
