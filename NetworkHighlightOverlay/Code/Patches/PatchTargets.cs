using System;
using System.Reflection;
using ColossalFramework.Math;
using HarmonyLib;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Patches
{
    public static class PatchTargets
    {
        public static MethodBase ResolveMoveItRenderOverlay()
        {
            Type toolType = AccessTools.TypeByName("MoveIt.MoveItTool");
            if (toolType == null)
                return null;

            return AccessTools.Method(
                toolType,
                "RenderOverlay",
                new[] { typeof(RenderManager.CameraInfo) });
        }

        public static MethodBase ResolveCreateSegment()
        {
            Type[] args = new[]
            {
                typeof(ushort).MakeByRefType(),
                typeof(Randomizer).MakeByRefType(),
                typeof(NetInfo),
                typeof(TreeInfo),
                typeof(ushort),
                typeof(ushort),
                typeof(Vector3),
                typeof(Vector3),
                typeof(uint),
                typeof(uint),
                typeof(bool)
            };

            return AccessTools.Method(typeof(NetManager), "CreateSegment", args);
        }
    }
}
