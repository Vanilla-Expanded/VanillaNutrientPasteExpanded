using RimWorld;
using Verse;

namespace VNPE
{
    [DefOf]
    public static class VThingDefOf
    {
        public static ThingDef VNPE_NutrientPasteTap;
        public static ThingDef VNPE_NutrientPasteFeeder;
        public static ThingDef VNPE_NutrientPasteDripper;
        public static EffecterDef EatVegetarian;
        public static JobDef VNPE_TakeFromTap;

        static VThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(VThingDefOf));
    }
}
