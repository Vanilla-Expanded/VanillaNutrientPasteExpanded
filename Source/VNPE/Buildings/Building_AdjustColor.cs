using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    public class Building_AdjustColor : Building
    {
        public override Color DrawColor => !this.IsSociallyProper(null, false) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetInspectString());
            if (!this.IsSociallyProper(null, false))
                stringBuilder.AppendLine("InPrisonCell".Translate());
            return stringBuilder.ToString().Trim();
        }

        public override void Notify_ColorChanged()
        {
            Log.Message("changing color");
            base.Notify_ColorChanged();
        }
    }

    public class Building_AdjustStorageColor : Building_Storage
    {
        public override Color DrawColor => !this.IsSociallyProper(null, false) ? Building_Bed.SheetColorForPrisoner : base.DrawColor;

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetInspectString());
            if (!this.IsSociallyProper(null, false))
                stringBuilder.AppendLine("InPrisonCell".Translate());
            return stringBuilder.ToString().Trim();
        }
    }
}
