using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class DragHandle : UIDragHandle
    {
        public static bool IsCtrlDown => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        public override void Awake()
        {
            base.Awake();
            tooltip = "Hold CTRL to move";
        }

        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            if (!IsCtrlDown)
                return;

            base.OnMouseMove(p);
        }
    }
}
