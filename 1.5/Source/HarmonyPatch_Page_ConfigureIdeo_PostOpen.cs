using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreCustomIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.PostOpen))]
static class HarmonyPatch_Page_ConfigureIdeo_PostOpen
{
    private static readonly MethodInfo clearCustomIdeosInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.ClearCustomIdeos));

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction code in instructions)
        {
            if (code.opcode == OpCodes.Ldstr)
            {
                yield return new CodeInstruction(OpCodes.Call, clearCustomIdeosInfo);
            }

            yield return code;
        }
    }
}
