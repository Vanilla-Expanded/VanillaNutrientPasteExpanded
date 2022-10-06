using System.Collections.Generic;
using System.Text;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_NutrientGrinder : Building
    {
        private List<Thing> cachedHoppers;

        public CompPowerTrader powerComp;
        public CompResource resourceComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            resourceComp = GetComp<CompResource>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;

            yield return BuildCopyCommandUtility.FindAllowedDesignator(ThingDefOf.Hopper);
        }

        public override string GetInspectString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(base.GetInspectString());

            if (!this.IsSociallyProper(null, false))
                builder.AppendLine((string)"InPrisonCell".Translate());

            return builder.ToString().Trim();
        }

        public override void TickRare()
        {
            if (!powerComp.PowerOn || cachedHoppers.NullOrEmpty())
                return;
            // Produce two time only if first succeded
            // each 250 ticks -> 2 paste
            if (TryProducePaste())
                TryProducePaste();
        }

        private bool TryProducePaste()
        {
            var net = resourceComp.PipeNet;

            if (net == null || net.AvailableCapacity < 1 || !HasEnoughFeed())
                return false;

            var num = def.building.nutritionCostPerDispense - 0.00001f;
            while (num > 0)
            {
                var feed = FindFeedInAnyHopper();
                if (feed == null)
                {
                    Log.Error("Did not find enough food in hoppers while trying to grind.");
                    return false;
                }

                var count = Mathf.Min(feed.stackCount, Mathf.CeilToInt(num / feed.GetStatValue(StatDefOf.Nutrition)));
                num -= count * feed.GetStatValue(StatDefOf.Nutrition);
                feed.SplitOff(count);
            }

            net.DistributeAmongStorage(1);
            return true;
        }

        private Thing FindFeedInAnyHopper()
        {
            for (int h = 0; h < cachedHoppers.Count; h++)
            {
                var thingList = cachedHoppers[h].Position.GetThingList(Map);
                for (int t = 0; t < thingList.Count; ++t)
                {
                    Thing thing = thingList[t];
                    if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing.def))
                        return thing;
                }
            }
            return null;
        }

        private bool HasEnoughFeed()
        {
            var map = Map;
            var num = 0f;

            for (int h = 0; h < cachedHoppers.Count; h++)
            {
                var things = cachedHoppers[h].Position.GetThingList(map);

                for (int t = 0; t < things.Count; t++)
                {
                    var thing = things[t];
                    if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing.def))
                    {
                        num += thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition);
                        break;
                    }
                }

                if (num >= def.building.nutritionCostPerDispense)
                    return true;
            }

            return false;
        }

        public void RegisterHopper(Thing hopper)
        {
            if (cachedHoppers == null)
                cachedHoppers = new List<Thing>();

            if (!cachedHoppers.Contains(hopper))
                cachedHoppers.Add(hopper);
        }

        public void UnregisterHopper(Thing hopper)
        {
            if (cachedHoppers.Contains(hopper))
                cachedHoppers.Remove(hopper);
        }
    }
}
