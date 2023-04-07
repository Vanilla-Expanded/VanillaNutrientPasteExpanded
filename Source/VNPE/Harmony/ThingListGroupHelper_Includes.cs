using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
    public static class ThingListGroupHelper_Includes
    {
        public static void Postfix(ThingDef def, ThingRequestGroup group, ref bool __result)
        {
            // Make sure the nutrient paste tap is considered as Building_NutrientPasteDispenser
            if ((group == ThingRequestGroup.FoodSourceNotPlantOrTree || group == ThingRequestGroup.FoodSource)
                && def.defName == "VNPE_NutrientPasteTap")
            {
                __result = true;
            }
        }
    }
}