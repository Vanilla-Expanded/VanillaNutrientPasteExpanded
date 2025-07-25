using System.Collections.Generic;
using Verse;

namespace VNPE
{
    public class CompRegisterToGrinder : ThingComp
    {
        private List<Building_NutrientGrinder> grinders;

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);

            if (grinders != null)
            {
                // Remove null or destroyed grinders first
                grinders.RemoveAll(g => g == null || g.Destroyed);

                foreach (var grinder in grinders)
                {
                    if (grinder != null && !grinder.Destroyed)
                    {
                        // Defensive: Remove this hopper from the grinder, checking for nulls inside grinder implementation
                        grinder.UnregisterHopper(parent);
                    }
                }
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