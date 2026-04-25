using HarmonyLib;
using PipeSystem;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(CompConvertToThing))]
    [HarmonyPatch("MaxCanOutput", MethodType.Getter)]
    public static class CompConvertToThing_MaxCanOutput
    {
        public static void Postfix(ref int __result, CompConvertToThing __instance)
        {
            // Only run for the feeder, and only when the base says output is possible
            if (__result <= 0 || __instance.parent.def != VThingDefOf.VNPE_NutrientPasteFeeder)
                return;

            // If there's nothing on the feeder yet, production is always fine
            var heldThing = __instance.HeldThing;
            if (heldThing == null)
                return;

            var net = __instance.PipeNet;
            if (net == null)
                return;

            // Lets see if the meal we want to produce would be compatible with the meal already on the feeder.
            var wouldBeMeal = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
            if (wouldBeMeal.TryGetComp<CompIngredients>() is CompIngredients ingredients)
            {
                for (int i = 0; i < net.storages.Count; i++)
                {
                    var storage = net.storages[i].parent;
                    if (storage.TryGetComp<CompRegisterIngredients>() is CompRegisterIngredients si)
                        for (int o = 0; o < si.ingredients.Count; o++)
                            ingredients.RegisterIngredient(si.ingredients[o]);
                }
            }

            // If not, output should be blocked to avoid scattering meals on the ground.
            if (!heldThing.CanStackWith(wouldBeMeal))
                __result = 0;
        }
    }
}
