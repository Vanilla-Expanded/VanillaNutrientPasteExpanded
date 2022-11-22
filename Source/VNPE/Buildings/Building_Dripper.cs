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
        public float nutrientPasteNutrition;

        public override Color DrawColor => Position.GetRoom(Map).IsPrisonCell ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
        public override Color DrawColorTwo => base.DrawColor;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            facilityComp = GetComp<CompFacility>();
            resourceComp = GetComp<CompResource>();
            nutrientPasteNutrition = ThingDefOf.MealNutrientPaste.statBases.GetStatValueFromList(StatDefOf.Nutrition, 0f);
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
                            var wanted = occupant.needs.food.NutritionWanted;
                            if (wanted >= nutrientPasteNutrition)
                            {
                                net.DrawAmongStorage(1, net.storages);
                                var thing = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
                                var ingestedNum = thing.Ingested(occupant, nutrientPasteNutrition);
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
