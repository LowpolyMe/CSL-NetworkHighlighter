using System;
using System.Reflection;
using HarmonyLib;
using NetworkHighlightOverlay.Code.Core;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Patches
{
    /// <summary>
    /// MoveIt appears to use their own RenderOverlay method, which unfortunately means we need to find
    /// and patch it to get our highlights to still show up while move it is active.
    /// according to https://github.com/Quboid/CS-MoveIt/blob/master/MoveIt/MoveItTool.cs 01/12/2026
    /// this should be void RenderOverlay(RenderManager.CameraInfo cameraInfo)
    /// will need to be updated when move it makes changes here
    /// </summary>
    [HarmonyPatch]
    public static class MoveItRenderOverlayPatch
    {
        static MethodBase TargetMethod()
        {
            try
            {
                var toolType = AccessTools.TypeByName("MoveIt.MoveItTool");
                if (toolType == null)
                {
                    return null;
                }

                return AccessTools.Method(
                    toolType,
                    "RenderOverlay",
                    new[] { typeof(RenderManager.CameraInfo) }
                );
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[NetworkHighlightOverlay] Failed to locate MoveIt.MoveItTool.RenderOverlay: {e}"
                );
                return null;
            }
        }
        
        //using pre- instead of postfix so moveit highlights are still displayed on top
        static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            try
            {
                Manager.Instance?.RenderIfActive(cameraInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[NetworkHighlightOverlay] Error in Move It RenderOverlay postfix: {e}"
                );
            }
        }
    }
}