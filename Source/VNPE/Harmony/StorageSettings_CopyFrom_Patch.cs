using HarmonyLib;
using RimWorld;
using Verse;
using System;

namespace VNPE.Harmony
{
    [HarmonyPatch(typeof(StorageSettings), "CopyFrom")]
    public static class StorageSettings_CopyFrom_Patch
    {
        private static bool Prefix(StorageSettings __instance, StorageSettings other)
        {
            try
            {
                // Check if the source StorageSettings is null
                if (other == null)
                {
                    return false;
                }

                // Check if the target StorageSettings is null
                if (__instance == null)
                {
                    return false;
                }

                // If both are valid, allow the original method to run
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] StorageSettings.CopyFrom prefix error: {ex.Message}");
                return false;
            }
        }
    }
} 