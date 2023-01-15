using HarmonyLib;
using RimWorld;

namespace VNPE
{
    [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "CanDispenseNow", MethodType.Getter)]
    public static class Building_NutrientPasteDispenser_CanDispenseNow
    {
        public static bool Prefix(ref bool __result, Building_NutrientPasteDispenser __instance)
        {
            if (__instance is Building_NutrientPasteTap nutrientPasteTap)
            {
                __result = nutrientPasteTap.CanDispenseNowOverride;
                return false;
            }
            return true;
        }
    }
}