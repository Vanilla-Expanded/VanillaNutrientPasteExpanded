using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(CompConvertToThing))]
    [HarmonyPatch("OutputResource", MethodType.Normal)]
    public static class CompConvertToThing_OutputResource
    {
        public static bool Prefix(int amount, CompConvertToThing __instance)
        {
            var parent = __instance.parent;
            if (parent.def.defName == "VNPE_NutrientPasteFeeder")
            {
                var net = __instance.PipeNet;

                var meal = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
                if (meal.TryGetComp<CompIngredients>() is CompIngredients ingredients)
                {
                    for (int i = 0; i < net.storages.Count; i++)
                    {
                        var storage = net.storages[i].parent;
                        if (storage.TryGetComp<CompRegisterIngredients>() is CompRegisterIngredients storageIngredients)
                        {
                            for (int o = 0; o < storageIngredients.ingredients.Count; o++)
                                ingredients.RegisterIngredient(storageIngredients.ingredients[o]);
                        }
                    }
                }

                meal.stackCount = amount;
                GenPlace.TryPlaceThing(meal, parent.Position, parent.Map, ThingPlaceMode.Near);
                return false;
            }
            return true;
        }
    }
}
