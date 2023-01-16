using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_Feeder : Building_Storage
    {
        public override Color DrawColor => Spawned && Position.IsInPrisonCell(Map) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
    }
}