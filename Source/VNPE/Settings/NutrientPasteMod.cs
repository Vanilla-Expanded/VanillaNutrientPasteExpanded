using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VNPE
{

    public class NutrientPasteMod : Mod
    {
        public NutrientPasteMod(ModContentPack content) : base(content)
        {
           
            settings = GetSettings<NutrientPasteMod_Settings>();
        }

        public static NutrientPasteMod_Settings settings;

        public override string SettingsCategory()
        {
            return "VE - Nutrient Paste";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }

    }
}