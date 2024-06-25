using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreCustomIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), "DrawIdeoRow")]
static class HarmonyPatch_IdeoUIUtility_DrawIdeoRow
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertyGetter(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.Pawn));

    private static readonly MethodInfo setPawnIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.SetPawnIdeo));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction code = codes[i];

            if (code.opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Stsfld)
            {
#if DEBUG
                Log.Warning($"Patching DrawIdeoRow to update pawn ideo");
#endif
                yield return new CodeInstruction(OpCodes.Call, pawnInfo);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, setPawnIdeoInfo);
            }

            yield return code;
        }
    }
}