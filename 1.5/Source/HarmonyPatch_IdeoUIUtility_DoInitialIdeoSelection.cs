using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), "DoInitialIdeoSelection")]
static class HarmonyPatch_IdeoUIUtility_DoInitialIdeoSelection
{
    private static readonly MethodInfo addCustomIdeoInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility), nameof(CreateMoreIdeoligionsUtility.AddCustomIdeo));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction code = codes[i];

            // Catch any custom ideos created by the player
            if (code.opcode == OpCodes.Callvirt && codes[i - 1].opcode == OpCodes.Ldfld)
            {
#if DEBUG
                Log.Warning($"[DoInitialIdeoSelection] Patching create button");
#endif
                Label jump = generator.DefineLabel();
                code.labels.Add(jump);

                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(OpCodes.Brfalse, jump);
                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(OpCodes.Call, addCustomIdeoInfo);
            }

            yield return code;
        }
    }
}
