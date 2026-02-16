using System;
using System.Reflection;
using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Patches
{
    /// <summary>
    /// MoveIt uses its own RenderOverlay method, so this patch keeps the overlay visible
    /// while MoveIt is active.
    /// </summary>
    [HarmonyPatch]
    public static class MoveItRenderOverlayPatch
    {
        private static MethodBase _target;

        static bool Prepare()
        {
            Type toolType = AccessTools.TypeByName("MoveIt.MoveItTool");
            if (toolType == null)
            {
                Debug.Log("[NetworkHighlightOverlay] No MoveIt detected.");
                return false;
            }

            _target = AccessTools.Method(
                toolType,
                "RenderOverlay",
                new[] { typeof(RenderManager.CameraInfo) });

            if (_target == null)
            {
                Debug.LogWarning(
                    "[NetworkHighlightOverlay] MoveIt.MoveItTool.RenderOverlay not found - signature may have changed.");
                return false;
            }

            Debug.Log("[NetworkHighlightOverlay] MoveIt detected - patching MoveIt.MoveItTool.RenderOverlay.");
            return true;
        }

        static MethodBase TargetMethod() => _target;

        // Use prefix so MoveIt highlights still render above this overlay.
        static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            try
            {
                Manager.Instance.RenderIfActive(cameraInfo);
            }
            catch (Exception exception)
            {
                Debug.LogError("[NetworkHighlightOverlay] Error in MoveIt RenderOverlay prefix: " + exception);
            }
        }
    }
}
