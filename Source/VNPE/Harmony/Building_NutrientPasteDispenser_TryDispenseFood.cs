using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;
using Verse.Sound;

namespace VNPE
{
    [HarmonyPatch(typeof(Building_NutrientPasteDispenser))]
    [HarmonyPatch("TryDispenseFood", MethodType.Normal)]
    public static class Building_NutrientPasteDispenser_TryDispenseFood
    {
        public static bool Prefix(Building_NutrientPasteDispenser __instance, ref Thing __result)
        {
            if (__instance is Building_NutrientPasteTap nutrientPasteTap)
            {
                __result = nutrientPasteTap.TryDispenseFoodOverride();
                return false;
            }

            if (!__instance.CanDispenseNow)
            {
                __result = null;
                return false;
            }

            if (!Building_NutrientPasteDispenser_GetGizmos.HasEnoughFeedstockInHoppers(__instance))
            {
                var comp = __instance.GetComp<CompResource>();
                if (comp != null && comp.PipeNet is PipeNet net && net.Stored >= 1)
                {
                    __instance.def.building.soundDispense.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map));
                    net.DrawAmongStorage(1, net.storages);

                    __result = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
                    return false;
                }
            }

            return true;
        }
    }
}