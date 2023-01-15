using HarmonyLib;
using PipeSystem;
using RimWorld;

namespace VNPE
{
    [HarmonyPatch(typeof(Building_NutrientPasteDispenser))]
    [HarmonyPatch("HasEnoughFeedstockInHoppers", MethodType.Normal)]
    public static class Building_NutrientPasteDispenser_HasEnoughFeedstockInHoppers
    {
        public static void Postfix(Building_NutrientPasteDispenser __instance, ref bool __result)
        {
            if (!__result)
            {
                var comp = __instance.GetComp<CompResource>();
                if (comp != null && comp.PipeNet is PipeNet net && net.Stored >= 1)
                    __result = true;
            }
        }
    }
}