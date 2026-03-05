using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch(typeof(ToolBase), "RenderOverlay")]
    public static class ToolRenderOverlayPatch
    {
        static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            ActivationHandler activationHandler = ActivationHandler.GetInstance();
            if (activationHandler == null)
                return;

            activationHandler.RenderOverlay(cameraInfo);
        }
    }
}
