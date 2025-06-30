using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using System;

namespace VNPE.Harmony
{
    [HarmonyPatch(typeof(Frame), "CompleteConstruction")]
    public static class Frame_CompleteConstruction_Patch
    {
        static void Prefix(Frame __instance)
        {
            try
            {
                // Get the ThingDef of the building that will be completed
                ThingDef buildingDef = __instance.def.entityDefToBuild as ThingDef;
                if (buildingDef == null)
                {
                    return;
                }
                
                // Check if the completed building will have storage settings
                bool hasStorageTab = false;
                if (buildingDef.inspectorTabs != null)
                {
                    foreach (var tab in buildingDef.inspectorTabs)
                    {
                        if (tab.Name == "ITab_Storage")
                        {
                            hasStorageTab = true;
                            break;
                        }
                    }
                }

                if (hasStorageTab)
                {
                    // Try to get the storageSettings field
                    var storageSettingsField = __instance.GetType().GetField("storageSettings", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (storageSettingsField != null)
                    {
                        var currentSettings = storageSettingsField.GetValue(__instance);
                        
                        if (currentSettings == null)
                        {
                            // Create new storage settings
                            var newSettings = new StorageSettings(__instance);
                            
                            // Set default settings for nutrient paste feeders
                            if (buildingDef.defName == "VNPE_NutrientPasteFeeder")
                            {
                                // Allow nutrient paste meals
                                newSettings.filter.SetAllow(ThingDefOf.MealNutrientPaste, true);
                            }
                            
                            storageSettingsField.SetValue(__instance, newSettings);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[VNPE] Frame_CompleteConstruction_Patch error: {ex}");
            }
        }
    }
} 