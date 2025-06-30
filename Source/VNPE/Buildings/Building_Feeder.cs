using RimWorld;
using UnityEngine;
using Verse;
using System;

namespace VNPE
{
    public class Building_Feeder : Building_Storage
    {
        public override Color DrawColor => Spawned && Position.IsInPrisonCell(Map) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        public override void PostMake()
        {
            try
            {
                base.PostMake();
            }
            catch (Exception ex)
            {
                Log.Warning($"VNPE: Error in Building_Feeder.PostMake: {ex.Message}");
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            try
            {
                base.SpawnSetup(map, respawningAfterLoad);
            }
            catch (Exception ex)
            {
                Log.Warning($"VNPE: Error in Building_Feeder.SpawnSetup: {ex.Message}");
            }
        }
    }
}