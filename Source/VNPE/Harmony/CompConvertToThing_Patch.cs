using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace VNPE.Harmony
{
    [HarmonyPatch(typeof(PipeSystem.CompConvertToThing), "OutputResource")]
    public static class CompConvertToThing_OutputResource
    {
        private static bool Prefix(PipeSystem.CompConvertToThing __instance)
        {
            try
            {
                // Check if parent is null
                if (__instance.parent == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.OutputResource: parent is null, skipping");
                    return false;
                }

                // Check if parent.Map is null (this is likely the source of the NullReferenceException)
                if (__instance.parent.Map == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.OutputResource: parent.Map is null, skipping");
                    return false;
                }

                // Check if parent.Position is valid
                if (!__instance.parent.Position.IsValid)
                {
                    Log.Warning("[VNPE] CompConvertToThing.OutputResource: parent.Position is invalid, skipping");
                    return false;
                }

                // If all checks pass, allow the original method to run
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] CompConvertToThing.OutputResource prefix error: {ex.Message}");
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(PipeSystem.CompResource), "PostSpawnSetup")]
    public static class CompResource_PostSpawnSetup
    {
        private static bool Prefix(PipeSystem.CompResource __instance, bool respawningAfterLoad)
        {
            try
            {
                // Check if parent is null
                if (__instance.parent == null)
                {
                    Log.Warning("[VNPE] CompResource.PostSpawnSetup: parent is null, skipping");
                    return false;
                }

                // Check if parent.Map is null (this is likely the source of the NullReferenceException)
                if (__instance.parent.Map == null)
                {
                    Log.Warning("[VNPE] CompResource.PostSpawnSetup: parent.Map is null, skipping");
                    return false;
                }

                // If all checks pass, allow the original method to run
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] CompResource.PostSpawnSetup prefix error: {ex.Message}");
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(PipeSystem.CompConvertToThing), "PostSpawnSetup")]
    public static class CompConvertToThing_PostSpawnSetup
    {
        private static bool Prefix(PipeSystem.CompConvertToThing __instance, bool respawningAfterLoad)
        {
            try
            {
                // Check if parent is null
                if (__instance.parent == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.PostSpawnSetup: parent is null, skipping");
                    return false;
                }

                // Check if parent.Map is null
                if (__instance.parent.Map == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.PostSpawnSetup: parent.Map is null, skipping");
                    return false;
                }

                // If all checks pass, allow the original method to run
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] CompConvertToThing.PostSpawnSetup prefix error: {ex.Message}");
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(PipeSystem.CompConvertToThing), "HeldThing", MethodType.Getter)]
    public static class CompConvertToThing_HeldThing
    {
        private static bool Prefix(PipeSystem.CompConvertToThing __instance, ref Thing __result)
        {
            try
            {
                // Check if parent is null
                if (__instance.parent == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.HeldThing: parent is null, returning null");
                    __result = null;
                    return false;
                }

                // Check if parent.Map is null
                if (__instance.parent.Map == null)
                {
                    Log.Warning("[VNPE] CompConvertToThing.HeldThing: parent.Map is null, returning null");
                    __result = null;
                    return false;
                }

                // Check if parent.Position is valid
                if (!__instance.parent.Position.IsValid)
                {
                    Log.Warning("[VNPE] CompConvertToThing.HeldThing: parent.Position is invalid, returning null");
                    __result = null;
                    return false;
                }

                // If all checks pass, allow the original method to run
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] CompConvertToThing.HeldThing prefix error: {ex.Message}");
                __result = null;
                return false;
            }
        }
    }
} 