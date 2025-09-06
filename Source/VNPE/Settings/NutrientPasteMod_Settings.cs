using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;



namespace VNPE
{


    public class NutrientPasteMod_Settings : ModSettings

    {

        public const float nutrientPasteDripperThresholdBase = 0.26f;
        public static float nutrientPasteDripperThreshold = nutrientPasteDripperThresholdBase;

        private static Vector2 scrollPosition = Vector2.zero;



        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<float>(ref nutrientPasteDripperThreshold, "nutrientPasteDripperThreshold", nutrientPasteDripperThresholdBase, true);

        }

        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();

            var scrollContainer = inRect.ContractedBy(10);
            scrollContainer.height -= ls.CurHeight;
            scrollContainer.y += ls.CurHeight;
            Widgets.DrawBoxSolid(scrollContainer, Color.grey);
            var innerContainer = scrollContainer.ContractedBy(1);
            Widgets.DrawBoxSolid(innerContainer, new ColorInt(42, 43, 44).ToColor);
            var frameRect = innerContainer.ContractedBy(5);
            frameRect.y += 15;
            frameRect.height -= 15;
            var contentRect = frameRect;
            contentRect.x = 0;
            contentRect.y = 0;
            contentRect.width -= 20;
            contentRect.height = 500;

            Listing_Standard ls2 = new Listing_Standard();

            Widgets.BeginScrollView(frameRect, ref scrollPosition, contentRect, true);
            ls2.Begin(contentRect.AtZero());


            var thresholdLabel = ls2.LabelPlusButton("VNPE_NutrientDripperThreshold".Translate() + ": " + nutrientPasteDripperThreshold.ToStringPercent(), "VNPE_NutrientDripperThresholdDesc".Translate());
            nutrientPasteDripperThreshold = (float)Math.Round(ls2.Slider(nutrientPasteDripperThreshold, 0f, 1f), 2);

            if (ls2.Settings_Button("VNPE_Reset".Translate(), new Rect(0f, thresholdLabel.position.y + 35, 250f, 29f)))
            {
                nutrientPasteDripperThreshold = nutrientPasteDripperThresholdBase;
            }

            ls2.End();
            Widgets.EndScrollView();
            Write();
        }
    }
}
