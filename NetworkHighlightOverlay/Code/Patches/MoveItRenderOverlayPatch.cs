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
        private static MethodBase _target;

        static bool Prepare()
        {
            var toolType = AccessTools.TypeByName("MoveIt.MoveItTool");
            if (toolType == null)
            {
                Debug.Log(
                    "[NetworkHighlightOverlay] No MoveIt detected."
                );
                return false;
            }
            
            _target = AccessTools.Method(
                toolType, 
                "RenderOverlay",
                new[] { typeof(RenderManager.CameraInfo) }
                );
            
            if (_target == null)
            {
                Debug.LogWarning(
                    "[NetworkHighlightOverlay] MoveIt.MoveItTool.RenderOverlay not found – " +
                    "signature may have changed."
                );
                return false;
            }
            
            Debug.Log(
                "[NetworkHighlightOverlay] MoveIt detected – patching MoveIt.MoveItTool.RenderOverlay."
            );
            return true;
        }

        static MethodBase TargetMethod() => _target;
        
        
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
                    $"[NetworkHighlightOverlay] Error in Move It RenderOverlay pretfix: {e}"
                );
            }
        }
    }
}