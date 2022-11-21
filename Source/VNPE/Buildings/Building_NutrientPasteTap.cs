using System.Text;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VNPE
{
    public class Building_NutrientPasteTap : Building
    {
        public CompResource resourceComp;
        public CompPowerTrader powerComp;

        public override Color DrawColor => !this.IsSociallyProper(null, false) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
        public override Color DrawColorTwo => base.DrawColor;

        public bool CanDispenseNow => powerComp.PowerOn && resourceComp.PipeNet is PipeNet net && net.Stored >= 1;

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

        public Thing TryDispenseFood()
        {
            var net = resourceComp.PipeNet;
            def.building.soundDispense.PlayOneShot(new TargetInfo(Position, Map));

            net.DrawAmongStorage(1, net.storages);

            return ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
        }
    }
}
