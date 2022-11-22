using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VNPE
{
    [HarmonyPatch(typeof(JobGiver_GetFood))]
    [HarmonyPatch("TryGiveJob", MethodType.Normal)]
    public static class Postfix_JobGiver_GetFood_TryGiveJob
    {
        private static bool IsFoodSourceOnMapSociallyProper(Thing t, Pawn getter, Pawn eater)
        {
            bool animalsCare = !getter.RaceProps.Animal;
            if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
                return false;

            return true;
        }

        public static void Postfix(Pawn pawn, ref Job __result)
        {
            // No jobs given
            if (__result == null)
            {
                var map = pawn.Map;
                var pos = pawn.Position;
                var fac = pawn.Faction;
                // Var for checks
                var pawnCanManipulate = pawn.RaceProps.ToolUser && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
                var minPref = pawn.needs.food.CurCategory >= HungerCategory.UrgentlyHungry ? FoodPreferability.RawBad : FoodPreferability.MealAwful;
                if (minPref == FoodPreferability.MealAwful && pawn.genes != null && pawn.genes.DontMindRawFood)
                    minPref = FoodPreferability.RawBad;
                // Check can use nutrient paste
                if (!pawnCanManipulate || ThingDefOf.MealNutrientPaste.ingestible.preferability < minPref || !pawn.WillEat_NewTemp(ThingDefOf.MealNutrientPaste))
                    return;

                // Get all paste tap on map
                var thingRequest = ThingRequest.ForDef(VThingDefOf.VNPE_NutrientPasteTap);
                var matchingReqs = map.listerThings.ThingsMatching(thingRequest);
                var traverseParams = TraverseParms.For(pawn);
                // Get the closest one
                Thing best = null;
                var maxOptimality = float.MinValue;
                for (int index = 0; index < matchingReqs.Count; ++index)
                {
                    var t = matchingReqs[index];
                    if (t.Faction == fac
                        && t.InteractionCell.Standable(map)
                        && IsFoodSourceOnMapSociallyProper(t, pawn, pawn)
                        && t is Building_NutrientPasteTap tap
                        && tap.CanDispenseNow)
                    {
                        var mDist = (pos - t.Position).LengthManhattan;
                        var optimality = FoodUtility.FoodOptimality(pawn, t, ThingDefOf.MealNutrientPaste, mDist);
                        if (optimality >= maxOptimality && map.reachability.CanReach(pos, t, PathEndMode.ClosestTouch, traverseParams) && t.Spawned)
                        {
                            best = t;
                            maxOptimality = optimality;
                        }
                    }
                }
                // If any tap, return new job on it
                if (best != null)
                    __result = JobMaker.MakeJob(VThingDefOf.VNPE_TakeFromTap, best);
            }
        }
    }
}
