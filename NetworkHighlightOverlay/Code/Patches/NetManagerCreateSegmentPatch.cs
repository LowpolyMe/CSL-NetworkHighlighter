using HarmonyLib;
using ColossalFramework.Math;
using UnityEngine;
using System.Reflection;
using System;
using NetworkHighlightOverlay.Code.Core;

namespace NetworkHighlightOverlay.Code.Patches
{
    [HarmonyPatch]
    public static class NetManagerCreateSegmentPatch
    {
        static MethodBase TargetMethod()
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
        
        static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            if (!__result || info == null) return;

            NetAI ai = info.m_netAI;
            if (ai != null)
            {
                Manager.Instance.OnSegmentCreated(segment);
            }
        }
    }
}
