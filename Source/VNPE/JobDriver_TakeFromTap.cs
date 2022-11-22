using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VNPE
{
    public class JobDriver_TakeFromTap : JobDriver
    {
        public override string GetReport() => JobUtility.GetResolvedJobReportRaw(job.def.reportString, ThingDefOf.MealNutrientPaste.label, ThingDefOf.MealNutrientPaste, "", "", "", "");

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        private Thing IngestibleSource => job.GetTarget(TargetIndex.A).Thing;
        private float ChewDurationMultiplier
        {
            get
            {
                Thing ingestibleSource = IngestibleSource;
                return ingestibleSource.def.ingestible != null && !ingestibleSource.def.ingestible.useEatingSpeedStat ? 1f : 1f / pawn.GetStatValue(StatDefOf.EatingSpeed);
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.A);

            Toil toil = ToilMaker.MakeToil("TakeMealFromNutrientPaste");
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = Building_NutrientPasteDispenser.CollectDuration;
            toil.initAction = () =>
            {
                Pawn actor = toil.actor;
                Thing thing = ((Building_NutrientPasteTap)job.GetTarget(TargetIndex.A).Thing).TryDispenseFood();
                if (thing == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
                else
                {
                    actor.carryTracker.TryStartCarry(thing);
                    job.SetTarget(TargetIndex.A, (LocalTargetInfo)actor.carryTracker.CarriedThing);
                }
            };
            yield return toil;

            yield return Toils_Ingest.CarryIngestibleToChewSpot(pawn, TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);

            yield return Toils_Ingest.ChewIngestible(pawn, ChewDurationMultiplier, TargetIndex.A, TargetIndex.B).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

            yield return Toils_Ingest.FinalizeIngest(pawn, TargetIndex.A);
        }
    }
}
