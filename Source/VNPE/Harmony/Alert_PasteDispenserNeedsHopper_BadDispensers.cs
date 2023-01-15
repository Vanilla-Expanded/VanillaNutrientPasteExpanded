using System.Collections.Generic;
using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(Alert_PasteDispenserNeedsHopper))]
    [HarmonyPatch("BadDispensers", MethodType.Getter)]
    public static class Alert_PasteDispenserNeedsHopper_BadDispensers
    {
        public static void Postfix(ref List<Thing> ___badDispensersResult)
        {
            ___badDispensersResult.RemoveAll(d => d.TryGetComp<CompResource>() is CompResource cr && cr.PipeNet.connectors.Count > 1);
        }
    }
}