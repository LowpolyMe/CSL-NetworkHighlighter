using System.Reflection;
using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch]
    public static class NetManagerReleaseSegmentPatch
    {
        static MethodBase TargetMethod() => typeof(NetManager).GetMethod(
            "ReleaseSegment",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[] { typeof(ushort), typeof(bool) },
            null);

        static void Prefix(ushort segment)
        {
            ActivationHandler activationHandler = ActivationHandler.GetInstance();
            if (activationHandler == null)
                return;

            activationHandler.HandleSegmentReleased(segment);
        }
    }
}
