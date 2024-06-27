using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), "DrawIdeoRow")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_IdeoUIUtility_DrawIdeoRow
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertyGetter(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.Pawn));

    private static readonly MethodInfo setPawnIdeoInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.SetPawnIdeo));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [..instructions];

        for (var i = 0; i < codes.Count; i++)
        {
            var code = codes[i];

            if (code.opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Stsfld)
            {
#if DEBUG
                Log.Warning("[DrawIdeoRow] Patching to update pawn ideo");
#endif
                yield return new CodeInstruction(OpCodes.Call, pawnInfo);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, setPawnIdeoInfo);
            }

            yield return code;
        }
    }
}