using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_NutrientGrinder : Building
    {
        public CompPowerTrader powerComp;
        public CompResource resourceComp;

        private const int produceTicksNeeded = 400;

        private List<Thing> cachedHoppers;
        private Effecter effecter;
        private int nextTick = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick");
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

            if (Prefs.DevMode)
            {
                builder.AppendLine($"{cachedHoppers.Count} connected hopper(s)\n");
            }

            return builder.ToString().Trim();
        }

        public void RegisterHopper(Thing hopper)
        {
            if (cachedHoppers == null)
                cachedHoppers = new List<Thing>();

            if (!cachedHoppers.Contains(hopper))
                cachedHoppers.Add(hopper);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            resourceComp = GetComp<CompResource>();

            var adjCells = GenAdj.CellsAdjacentCardinal(this).ToList();
            for (int i = 0; i < adjCells.Count; i++)
            {
                var cell = adjCells[i];
                if (cell.GetFirstBuilding(map) is Building h && h.TryGetComp<CompRegisterToGrinder>() is CompRegisterToGrinder compRegisterToGrinder)
                {
                    this.RegisterHopper(h);
                }
            }

            if (!respawningAfterLoad)
                nextTick = Find.TickManager.TicksGame + produceTicksNeeded;
        }
        public override void Tick()
        {
            var tick = Find.TickManager.TicksGame;
            if (tick >= nextTick)
            {
                nextTick = tick + produceTicksNeeded;
                if (!powerComp.PowerOn || cachedHoppers.NullOrEmpty())
                    return;

                if (TryProducePaste() && effecter == null)
                {
                    effecter = VThingDefOf.EatVegetarian.Spawn();
                    effecter.Trigger(this, new TargetInfo(Position, Map));
                }
            }
            else if (tick >= nextTick - 150 && effecter != null)
            {
                effecter?.Cleanup();
                effecter = null;
            }

            if (effecter != null)
                effecter.EffectTick(this, new TargetInfo(Position, Map));
        }

        public void UnregisterHopper(Thing hopper)
        {
            if (cachedHoppers.Contains(hopper))
                cachedHoppers.Remove(hopper);
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

        private bool TryProducePaste()
        {
            var net = resourceComp.PipeNet;

            if (net == null || net.AvailableCapacity < 1 || !HasEnoughFeed())
                return false;

            var comps = new List<CompRegisterIngredients>();
            for (int i = 0; i < net.storages.Count; i++)
            {
                var storage = net.storages[i];
                if (storage.parent.GetComp<CompRegisterIngredients>() is CompRegisterIngredients comp)
                    comps.Add(comp);
            }
            var compsCount = comps.Count;

            var num = def.building.nutritionCostPerDispense - 0.00001f;
            while (num > 0)
            {
                var feed = FindFeedInAnyHopper();
                if (feed == null)
                {
                    Log.Error("Did not find enough food in hoppers while trying to grind.");
                    return false;
                }

                for (int i = 0; i < compsCount; i++)
                    comps[i].RegisterIngredient(feed.def);

                var count = Mathf.Min(feed.stackCount, Mathf.CeilToInt(num / feed.GetStatValue(StatDefOf.Nutrition)));
                num -= count * feed.GetStatValue(StatDefOf.Nutrition);
                feed.SplitOff(count);
            }

            net.DistributeAmongStorage(1);
            return true;
        }
    }
}