using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_Feeder : Building_Storage
    {
        public override Color DrawColor => Position.GetRoom(Map).IsPrisonCell ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
        public override Color DrawColorTwo => base.DrawColor;
    }
}
