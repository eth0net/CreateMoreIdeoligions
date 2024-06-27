using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Dialog_IdeosDuringLanding), MethodType.Constructor)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_Dialog_IdeosDuringLanding_Constructor
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertySetter(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.Pawn));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
#if DEBUG
        Log.Warning("[Dialog_IdeosDuringLanding] Patching to reset pawn");
#endif

        yield return new CodeInstruction(OpCodes.Ldnull);
        yield return new CodeInstruction(OpCodes.Call, pawnInfo);

        foreach (var code in instructions) yield return code;
    }
}