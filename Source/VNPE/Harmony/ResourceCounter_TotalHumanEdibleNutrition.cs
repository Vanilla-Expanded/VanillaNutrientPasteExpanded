using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(ResourceCounter))]
    [HarmonyPatch("TotalHumanEdibleNutrition", MethodType.Getter)]
    public static class ResourceCounter_TotalHumanEdibleNutrition
    {
        public static void Postfix(Map ___map, ref float __result)
        {
            var manager = ___map.GetComponent<PipeNetManager>();
            if (manager == null)
                return;

            for (int i = 0; i < manager.pipeNets.Count; ++i)
            {
                var net = manager.pipeNets[i];
                __result += (int)net.Stored;
            }
        }
    }
}
