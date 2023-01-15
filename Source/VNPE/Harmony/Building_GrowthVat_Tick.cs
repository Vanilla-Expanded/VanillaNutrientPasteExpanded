using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(Building_GrowthVat))]
    [HarmonyPatch("Tick", MethodType.Normal)]
    public static class Building_GrowthVat_Tick
    {
        public static void Postfix(ref float ___containedNutrition, Building_GrowthVat __instance)
        {
            if (__instance.IsHashIntervalTick(250))
            {
                var compResource = __instance.GetComp<CompResource>();
                if (compResource != null && compResource.PipeNet is PipeNet net)
                {
                    var stored = net.Stored;
                    var needed = __instance.NutritionNeeded;
                    while (needed > 0.9f && stored > 0)
                    {
                        ___containedNutrition += 0.9f;
                        net.DrawAmongStorage(1, net.storages);

                        stored--;
                        needed -= 0.9f;
                    }
                }
            }
        }
    }
}
