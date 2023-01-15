using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VNPE
{
    [HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
    public static class ThingListGroupHelper_Includes
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var typeFromHandle = AccessTools.Method(typeof(Type), "GetTypeFromHandle");
            var isDerivedFrom = AccessTools.Method(typeof(ThingListGroupHelper_Includes), "IsDerivedFrom");
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.Calls(typeFromHandle) && codes[i - 1].opcode == OpCodes.Ldtoken && codes[i - 1].OperandIs(typeof(Building_NutrientPasteDispenser)))
                {
                    yield return new CodeInstruction(OpCodes.Call, isDerivedFrom);
                    i++;
                }
            }
        }

        public static bool IsDerivedFrom(Type type, Type toCheck)
        {
            return toCheck.IsAssignableFrom(type);
        }
    }
}