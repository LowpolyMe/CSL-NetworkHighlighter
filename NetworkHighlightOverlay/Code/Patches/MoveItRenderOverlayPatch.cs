using System.Reflection;
using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch]
    public static class MoveItRenderOverlayPatch
    {
        private static MethodBase _targetMethod;

        static bool Prepare()
        {
            _targetMethod = PatchTargets.ResolveMoveItRenderOverlay();
            return _targetMethod != null;
        }

        static MethodBase TargetMethod() => _targetMethod;

        static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            RuntimeHooks.RenderOverlay(cameraInfo);
        }
    }
}
