using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DrawIdeoPlate))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_IdeoUIUtility_DrawIdeoPlate
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertySetter(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.Pawn));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [..instructions];

        for (var i = 0; i < codes.Count; i++)
        {
            var code = codes[i];

            if (i > 1 && codes[i - 2].opcode == OpCodes.Ldarg_1 && codes[i - 1].opcode == OpCodes.Call)
            {
#if DEBUG
                Log.Warning("[DrawIdeoPlate] Patching to save pawn");
#endif
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Call, pawnInfo);
            }

            yield return code;
        }
    }
}
