using HarmonyLib;
using ColossalFramework;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch(typeof(ToolBase), "RenderOverlay")]
    public static class ToolRenderOverlayPatch
    {
        static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            Manager.Instance?.RenderIfActive(cameraInfo);
        }
    }
}