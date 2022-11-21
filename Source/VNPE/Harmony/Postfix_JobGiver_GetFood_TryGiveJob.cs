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
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            if (__result == null && !pawn.NonHumanlikeOrWildMan())
            {
                var map = pawn.Map;
                var pos = pawn.Position;
                var thingRequest = ThingRequest.ForDef(VThingDefOf.VNPE_NutrientPasteTap);
                var matchingReqs = map.listerThings.ThingsMatching(thingRequest);
                var traverseParams = TraverseParms.For(pawn);

                Thing thing = null;
                var maxOptimality = float.MinValue;
                for (int index = 0; index < matchingReqs.Count; ++index)
                {
                    var search = matchingReqs[index];
                    if (search.Faction == pawn.Faction && search is Building_NutrientPasteTap tap && tap.CanDispenseNow)
                    {
                        var mDist = (pos - search.Position).LengthManhattan;
                        var optimality = FoodUtility.FoodOptimality(pawn, search, ThingDefOf.MealNutrientPaste, mDist);
                        if (optimality >= maxOptimality && map.reachability.CanReach(pos, search, PathEndMode.ClosestTouch, traverseParams) && search.Spawned)
                        {
                            thing = search;
                            maxOptimality = optimality;
                        }
                    }
                }

                if (thing != null)
                    __result = JobMaker.MakeJob(VThingDefOf.VNPE_TakeFromTap, thing);
            }
        }
    }
}
