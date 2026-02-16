using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch(typeof(ToolBase), "RenderOverlay")]
    public static class ToolRenderOverlayPatch
    {
        static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            Manager.Instance.RenderIfActive(cameraInfo);
        }
    }
}
