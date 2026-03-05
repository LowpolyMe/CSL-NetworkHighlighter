using HarmonyLib;
using System.Reflection;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch]
    public static class NetManagerCreateSegmentPatch
    {
        static MethodBase TargetMethod() => PatchTargets.ResolveCreateSegment();
        
        static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            ActivationHandler activationHandler = ActivationHandler.GetInstance();
            if (activationHandler == null)
                return;

            activationHandler.HandleSegmentCreated(segment, info, __result);
        }
    }
}
