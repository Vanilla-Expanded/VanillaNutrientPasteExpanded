using System.Collections.Generic;
using Verse;

namespace VNPE
{
    public class CompRegisterToGrinder : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            TryUpdate(parent.Map);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            TryUpdate(map);
        }

        private void TryUpdate(Map map)
        {
            var grinders = new List<Building_NutrientGrinder>();
            var adjs = GenAdj.CellsAdjacentCardinal(parent);

            foreach (var cell in adjs)
            {
                var ed = cell.GetFirstBuilding(map);
                if (ed != null && ed is Building_NutrientGrinder grinder && !grinders.Contains(grinder))
                {
                    grinders.Add(grinder);
                }
            }

            var pos = parent.Position;
            for (int i = 0; i < grinders.Count; i++)
            {
                Log.Message($"Trying to add hopper {pos}");
                grinders[i].UpdateHopper(pos);
            }
        }
    }
}
