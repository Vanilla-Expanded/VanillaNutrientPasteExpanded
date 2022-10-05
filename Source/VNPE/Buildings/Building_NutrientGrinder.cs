using System.Collections.Generic;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    public class Building_NutrientGrinder : Building
    {
        private List<IntVec3> hoppers;

        public CompPowerTrader powerComp;
        public CompResource resourceComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            resourceComp = GetComp<CompResource>();
            hoppers = new List<IntVec3>();

            var adjs = GenAdj.CellsAdjacentCardinal(this);
            foreach (var cell in adjs)
            {
                var ed = cell.GetFirstBuilding(map);
                if (ed != null && ed.def == ThingDefOf.Hopper)
                {
                    UpdateHopper(cell);
                }
            }
        }

        public void UpdateHopper(IntVec3 pos)
        {
            Log.Message($"Adding hopper {pos}");
            if (hoppers.Contains(pos))
                hoppers.Remove(pos);

            hoppers.Add(pos);
        }
    }
}
