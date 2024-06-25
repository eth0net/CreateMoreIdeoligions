using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DrawIdeoPlate))]
static class HarmonyPatch_IdeoUIUtility_DrawIdeoPlate
{
    private static readonly MethodInfo pawnInfo = AccessTools.PropertySetter(typeof(CreateMoreIdeoligionsUtility), nameof(CreateMoreIdeoligionsUtility.Pawn));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction code = codes[i];

            if (i > 1 && codes[i - 2].opcode == OpCodes.Ldarg_1 && codes[i - 1].opcode == OpCodes.Call)
            {
#if DEBUG
                Log.Warning($"Patching DrawIdeoPlate to save pawn");
#endif
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Call, pawnInfo);
            }

            yield return code;
        }
    }
}
