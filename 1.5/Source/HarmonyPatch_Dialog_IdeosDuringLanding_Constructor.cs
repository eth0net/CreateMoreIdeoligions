using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreCustomIdeoligions;

[HarmonyPatch(typeof(Dialog_IdeosDuringLanding), MethodType.Constructor)]
static class HarmonyPatch_Dialog_IdeosDuringLanding_Constructor
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertySetter(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.Pawn));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldnull);
        yield return new CodeInstruction(OpCodes.Call, pawnInfo);

        foreach (CodeInstruction code in instructions)
        {
            yield return code;
        }
    }
}
