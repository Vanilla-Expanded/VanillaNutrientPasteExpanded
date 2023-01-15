using System.Collections.Generic;
using Verse;

namespace VNPE
{
    public class CompRegisterToGrinder : ThingComp
    {
        private List<Building_NutrientGrinder> grinders;

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            for (int i = 0; i < grinders.Count; i++)
            {
                grinders[i].UnregisterHopper(parent);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                grinders = new List<Building_NutrientGrinder>();
                var adjs = GenAdj.CellsAdjacentCardinal(parent);

                foreach (var cell in adjs)
                {
                    var things = cell.GetThingList(parent.Map);
                    foreach (var thing in things)
                    {
                        if (thing is Building_NutrientGrinder grinder && !grinders.Contains(grinder))
                        {
                            grinders.Add(grinder);
                        }
                    }
                }

                for (int i = 0; i < grinders.Count; i++)
                {
                    grinders[i].RegisterHopper(parent);
                }
            });
        }
    }
}