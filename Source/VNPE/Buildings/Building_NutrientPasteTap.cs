using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VNPE
{
    [StaticConstructorOnStartup]
    public class Building_NutrientPasteTap : Building_NutrientPasteDispenser
    {
        public CompResource resourceComp;
        public override Color DrawColor => !this.IsSociallyProper(null, false) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "CanDispenseNow", MethodType.Getter)]
        public static class Building_NutrientPasteDispenser_CanDispenseNow_Patch
        {
            public static bool Prefix(ref bool __result, Building_NutrientPasteDispenser __instance)
            {
                if (__instance is Building_NutrientPasteTap nutrientPasteTap)
                {
                    __result = nutrientPasteTap.CanDispenseNowOverride;
                    return false;
                }
                return true;
            }
        }

        public bool CanDispenseNowOverride => powerComp.PowerOn && resourceComp.PipeNet is PipeNet net && net.Stored >= 1;

        public override bool HasEnoughFeedstockInHoppers()
        {
            return resourceComp.PipeNet is PipeNet net && net.Stored >= 1;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            resourceComp = GetComp<CompResource>();
            powerComp = GetComp<CompPowerTrader>();
        }

        public override string GetInspectString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(base.GetInspectString());

            if (!this.IsSociallyProper(null, false))
                builder.AppendLine("InPrisonCell".Translate());

            return builder.ToString().Trim();
        }

        [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "TryDispenseFood")]
        public static class Building_NutrientPasteDispenser_TryDispenseFood_Patch
        {
            public static bool Prefix(ref Thing __result, Building_NutrientPasteDispenser __instance)
            {
                if (__instance is Building_NutrientPasteTap nutrientPasteTap)
                {
                    __result = nutrientPasteTap.TryDispenseFoodOverride();
                    return false;
                }
                return true;
            }
        }

        public Thing TryDispenseFoodOverride()
        {
            var net = resourceComp.PipeNet;
            def.building.soundDispense.PlayOneShot(new TargetInfo(Position, Map));
            net.DrawAmongStorage(1, net.storages);

            var meal = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
            if (meal.TryGetComp<CompIngredients>() is CompIngredients ingredients)
            {
                for (int i = 0; i < net.storages.Count; i++)
                {
                    var parent = net.storages[i].parent;
                    if (parent.TryGetComp<CompRegisterIngredients>() is CompRegisterIngredients storageIngredients)
                    {
                        for (int o = 0; o < storageIngredients.ingredients.Count; o++)
                            ingredients.RegisterIngredient(storageIngredients.ingredients[o]);
                    }
                }
            }

            return meal;
        }


        private static readonly Texture2D aIcon = ContentFinder<Texture2D>.Get("UI/Extract5Meals");
        private static readonly Texture2D bIcon = ContentFinder<Texture2D>.Get("UI/Extract10Meals");
        private static readonly Texture2D cIcon = ContentFinder<Texture2D>.Get("UI/Extract20Meals");

        private void TryDropFood(int amount)
        {
            if (!powerComp.PowerOn || amount <= 0)
                return;

            var net = resourceComp.PipeNet;
            var stored = net.Stored;

            if (stored <= 0)
                return;

            var map = Map;
            var cell = InteractionCell;

            int totalCount = (int)(amount > stored ? stored : amount);
            net.DrawAmongStorage(totalCount, net.storages);
            def.building.soundDispense.PlayOneShot(new TargetInfo(cell, map));

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
                        var comp = comps[i];
                        for (int o = 0; o < comp.ingredients.Count; o++)
                            ingredients.RegisterIngredient(comp.ingredients[o]);
                    }
                }

                var stack = totalCount > meal.def.stackLimit ? meal.def.stackLimit : totalCount;
                meal.stackCount = stack;
                totalCount -= stack;

                GenPlace.TryPlaceThing(meal, cell, map, ThingPlaceMode.Near);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            yield return new Command_Action
            {
                action = () => TryDropFood(5),
                defaultLabel = "VNPE_Extract5".Translate(),
                defaultDesc = "VNPE_Extract5".Translate(),
                icon = aIcon,
            };
            yield return new Command_Action
            {
                action = () => TryDropFood(10),
                defaultLabel = "VNPE_Extract10".Translate(),
                defaultDesc = "VNPE_Extract10".Translate(),
                icon = bIcon,
            };
            yield return new Command_Action
            {
                action = () => TryDropFood(20),
                defaultLabel = "VNPE_Extract20".Translate(),
                defaultDesc = "VNPE_Extract20".Translate(),
                icon = cIcon,
            };
        }

        [HarmonyPatch(typeof(Building_NutrientPasteDispenser), "GetGizmos")]
        public static class Building_NutrientPasteDispenser_GetGizmos_Patch
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
                        if (IsAcceptableFeedstock(thing.def))
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
                        icon = aIcon,
                    };
                    yield return new Command_Action
                    {
                        action = () => TryDropFood(__instance, 10),
                        defaultLabel = "VNPE_Extract10".Translate(),
                        defaultDesc = "VNPE_Extract10".Translate(),
                        icon = bIcon,
                    };
                    yield return new Command_Action
                    {
                        action = () => TryDropFood(__instance, 20),
                        defaultLabel = "VNPE_Extract20".Translate(),
                        defaultDesc = "VNPE_Extract20".Translate(),
                        icon = cIcon,
                    };
                }
            }
        }
    }
}
