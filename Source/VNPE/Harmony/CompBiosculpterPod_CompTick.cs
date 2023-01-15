using System.Collections.Generic;
using HarmonyLib;
using PipeSystem;
using RimWorld;

namespace VNPE
{
    [HarmonyPatch(typeof(CompBiosculpterPod))]
    [HarmonyPatch("CompTick", MethodType.Normal)]
    public static class CompBiosculpterPod_CompTick
    {
        private const int interval = 250;
        private static readonly Dictionary<CompBiosculpterPod, CompResource> comps = new Dictionary<CompBiosculpterPod, CompResource>();
        private static readonly Dictionary<CompBiosculpterPod, int> intervals = new Dictionary<CompBiosculpterPod, int>();

        private static CompResource GetComp(CompBiosculpterPod comp)
        {
            if (comps.ContainsKey(comp))
            {
                return comps[comp];
            }
            else
            {
                var compResource = comp.parent.GetComp<CompResource>();
                if (compResource != null)
                    comps.Add(comp, compResource);

                return compResource;
            }
        }

        private static bool IsIntervalTick(CompBiosculpterPod comp)
        {
            if (!intervals.ContainsKey(comp))
                intervals.Add(comp, interval);

            intervals[comp]--;

            if (intervals[comp] <= 0)
            {
                intervals[comp] = interval;
                return true;
            }
            return false;
        }

        public static void Postfix(ref float ___liquifiedNutrition, CompBiosculpterPod __instance)
        {
            if (IsIntervalTick(__instance))
            {
                var compResource = GetComp(__instance);
                if (compResource != null && compResource.PipeNet is PipeNet net)
                {
                    var stored = net.Stored;
                    var needed = __instance.RequiredNutritionRemaining;
                    while (needed > 0 && stored > 0)
                    {
                        ___liquifiedNutrition += 0.9f;
                        net.DrawAmongStorage(1, net.storages);

                        stored--;
                        needed -= 0.9f;
                    }
                }
            }
        }
    }
}
