using HarmonyLib;
using Verse;

namespace VNPE
{
    [StaticConstructorOnStartup]
    public class Init
    {
        static Init()
        {
            Harmony harmony = new Harmony("VanillaExpanded.VNutrientE");
            harmony.PatchAll();
        }
    }
}
