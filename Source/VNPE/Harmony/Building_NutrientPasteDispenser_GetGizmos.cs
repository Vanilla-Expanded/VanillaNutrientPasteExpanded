using System.Collections.Generic;
using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;
using Verse.Sound;

namespace VNPE
{
    [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "GetGizmos")]
    public static class Building_NutrientPasteDispenser_GetGizmos
    {
        public static bool HasEnoughFeedstockInHoppers(Building_NutrientPasteDispenser dispenser)
        {
            var num = 0f;
            var adj = dispenser.AdjCellsCardinalInBounds;
            var map = dispenser.Map;

            for (int i = 0; i < adj.Count; i++)
            {
                var cell = adj[i];
                var thingList = cell.GetThingList(map);

                Thing food = null;
                Thing hopper = null;

                for (int o = 0; o < thingList.Count; ++o)
                {
                    var thing = thingList[o];
                    if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing.def))
                        food = thing;
                    if (thing.IsHopper())
                        hopper = thing;
                }
                if (food != null && hopper != null)
                    num += food.stackCount * food.GetStatValue(StatDefOf.Nutrition);

                if (num >= dispenser.def.building.nutritionCostPerDispense)
                    return true;
            }
            return false;
        }

        public static void TryDropFood(Building_NutrientPasteDispenser dispenser, int amount)
        {
            if (!dispenser.powerComp.PowerOn || amount <= 0)
                return;

            var position = dispenser.Position;
            var map = dispenser.Map;
            var cell = dispenser.InteractionCell;

            var comp = dispenser.GetComp<CompResource>();
            // No feed in hoppers, use net resource
            if (!HasEnoughFeedstockInHoppers(dispenser))
            {
                if (comp != null && comp.PipeNet is PipeNet net && net.Stored >= 1)
                {
                    dispenser.def.building.soundDispense.PlayOneShot(new TargetInfo(position, map));

                    int stored = (int)net.Stored;
                    int totalCount = amount > stored ? stored : amount;
                    net.DrawAmongStorage(totalCount, net.storages);

                    var comps = new List<CompRegisterIngredients>();
                    for (int i = 0; i < net.storages.Count; i++)
                    {
                        if (net.storages[i].parent.TryGetComp<CompRegisterIngredients>() is CompRegisterIngredients storageIngredients)
                            comps.Add(storageIngredients);
                    }

                    while (totalCount > 0)
                    {
                        var meal = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
                        if (meal.TryGetComp<CompIngredients>() is CompIngredients ingredients)
                        {
                            for (int i = 0; i < comps.Count; i++)
                            {
                                var compI = comps[i];
                                for (int o = 0; o < compI.ingredients.Count; o++)
                                    ingredients.RegisterIngredient(compI.ingredients[o]);
                            }
                        }
                        var count = totalCount > meal.def.stackLimit ? meal.def.stackLimit : totalCount;
                        meal.stackCount = count;
                        totalCount -= count;

                        GenPlace.TryPlaceThing(meal, cell, map, ThingPlaceMode.Near);
                    }
                }
            }
            else
            {
                var countLeft = amount;
                while (HasEnoughFeedstockInHoppers(dispenser) && countLeft > 0)
                {
                    GenPlace.TryPlaceThing(dispenser.TryDispenseFood(), cell, map, ThingPlaceMode.Near);
                    countLeft--;
                }

                TryDropFood(dispenser, countLeft);
            }
        }

        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building_NutrientPasteDispenser __instance)
        {
            if (__instance is Building_NutrientPasteTap nutrientPasteTap)
            {
                foreach (Gizmo gizmo in __result)
                    yield return gizmo;
            }
            else
            {
                foreach (Gizmo gizmo in __result)
                    yield return gizmo;

                yield return new Command_Action
                {
                    action = () => TryDropFood(__instance, 5),
                    defaultLabel = "VNPE_Extract5".Translate(),
                    defaultDesc = "VNPE_Extract5".Translate(),
                    icon = Building_NutrientPasteTap.aIcon,
                };
                yield return new Command_Action
                {
                    action = () => TryDropFood(__instance, 10),
                    defaultLabel = "VNPE_Extract10".Translate(),
                    defaultDesc = "VNPE_Extract10".Translate(),
                    icon = Building_NutrientPasteTap.bIcon,
                };
                yield return new Command_Action
                {
                    action = () => TryDropFood(__instance, 20),
                    defaultLabel = "VNPE_Extract20".Translate(),
                    defaultDesc = "VNPE_Extract20".Translate(),
                    icon = Building_NutrientPasteTap.cIcon,
                };
            }
        }
    }
}