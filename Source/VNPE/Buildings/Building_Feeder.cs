using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_Feeder : Building_Storage
    {
        public override Color DrawColor
        {
            get
            {
                var pos = def.hasInteractionCell ? InteractionCell : Position;
                return pos.IsInPrisonCell(Map) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;
            }
        }

        public override Color DrawColorTwo => base.DrawColor;
    }
}
