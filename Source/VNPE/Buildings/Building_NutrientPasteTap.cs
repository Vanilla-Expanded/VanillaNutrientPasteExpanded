using System.Text;
using HarmonyLib;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VNPE
{
    public class Building_NutrientPasteTap : Building_NutrientPasteDispenser
    {
        public CompResource resourceComp;
        public override Color DrawColor => !this.IsSociallyProper(null, false) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "CanDispenseNow", MethodType.Getter)]
        public static class Building_NutrientPasteDispenser_CanDispenseNow_Patch
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

        public bool CanDispenseNowOverride => powerComp.PowerOn && resourceComp.PipeNet is PipeNet net && net.Stored >= 1;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            resourceComp = GetComp<CompResource>();
            powerComp = GetComp<CompPowerTrader>();
        }

        public override string GetInspectString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(base.GetInspectString());

            if (!this.IsSociallyProper(null, false))
                builder.AppendLine("InPrisonCell".Translate());

            return builder.ToString().Trim();
        }

        [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "TryDispenseFood")]
        public static class Building_NutrientPasteDispenser_TryDispenseFood_Patch
        {
            public static bool Prefix(ref Thing __result, Building_NutrientPasteDispenser __instance)
            {
                if (__instance is Building_NutrientPasteTap nutrientPasteTap)
                {
                    __result = nutrientPasteTap.TryDispenseFoodOverride();
                    return false;
                }
                return true;
            }
        }

        public Thing TryDispenseFoodOverride()
        {
            var net = resourceComp.PipeNet;
            def.building.soundDispense.PlayOneShot(new TargetInfo(Position, Map));
            net.DrawAmongStorage(1, net.storages);
            return ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
        }
    }
}
