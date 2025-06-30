﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace VNPE
{
    [StaticConstructorOnStartup]
    public class Init
    {
        public static bool AnimalControls = false;

        static Init()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("VanillaExpanded.VNutrientE");
            harmony.PatchAll();
            // Add dripper to all beds
            var defs = DefDatabase<ThingDef>.AllDefsListForReading;
            foreach (var def in defs)
            {
                // Is it a bed?
                if (def.building != null
                    && def.building.buildingTags.Contains("Bed")
                    && !def.defName.Contains("Spot")
                    && def.GetCompProperties<CompProperties_AffectedByFacilities>() is CompProperties_AffectedByFacilities props
                    && !props.linkableFacilities.Contains(VThingDefOf.VNPE_NutrientPasteDripper))
                {
                    props.linkableFacilities.Add(VThingDefOf.VNPE_NutrientPasteDripper);
                }
            }

            var dripper = DefDatabase<ThingDef>.GetNamed("VNPE_NutrientPasteDripper");
            dripper.GetCompProperties<CompProperties_Facility>().ResolveReferences(dripper);

            // Animal control loaded?
            if (ModLister.HasActiveModWithName("Animal Controls"))
            {
                AnimalControls = true;
                Log.Message("[VNPE] Animal control detected");
            }
        }
    }
}