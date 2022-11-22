using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_Dripper : Building
    {
        public CompFacility facilityComp;
        public CompResource resourceComp;

        public override Color DrawColor => Position.GetRoom(Map).IsPrisonCell ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
        public override Color DrawColorTwo => base.DrawColor;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            facilityComp = GetComp<CompFacility>();
            resourceComp = GetComp<CompResource>();
        }

        public override void TickLong()
        {
            base.TickLong();
        }
    }
}
