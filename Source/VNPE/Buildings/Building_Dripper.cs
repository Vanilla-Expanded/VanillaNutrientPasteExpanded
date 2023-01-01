using System.Linq;
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

        public override Color DrawColor => Position.IsInPrisonCell(Map) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            facilityComp = GetComp<CompFacility>();
            resourceComp = GetComp<CompResource>();
        }

        public override void TickRare()
        {
            var pos = Position;
            var net = resourceComp.PipeNet;
            var linkeds = facilityComp.LinkedBuildings;
            for (int i = 0; i < linkeds.Count; i++)
            {
                var linked = linkeds[i];
                // Get linked bed
                if (linked is Building_Bed bed)
                {
                    var occupants = bed.CurOccupants.ToList();
                    for (int o = 0; o < occupants.Count; o++)
                    {
                        var occupant = occupants[o];
                        // Make sure we only apply effect to pawn right next to the dripper
                        if (occupant.Position.AdjacentToCardinal(pos))
                        {
                            if (occupant.needs.food.CurLevelPercentage <= 0.4)
                            {
                                net.DrawAmongStorage(1, net.storages);
                                var thing = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
                                var ingestedNum = thing.Ingested(occupant, occupant.needs.food.NutritionWanted);
                                occupant.needs.food.CurLevel += ingestedNum;
                                occupant.records.AddTo(RecordDefOf.NutritionEaten, ingestedNum);
                            }
                        }
                    }
                }
            }
            base.TickRare();
        }
    }
}
