using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MoreCustomIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.PostOpen))]
static class HarmonyPatch_Page_ConfigureIdeo_PostOpen
{
    private static readonly MethodInfo findIdeoManagerInfo = AccessTools.PropertyGetter(typeof(Find), nameof(Find.IdeoManager));

    private static readonly MethodInfo selectOrMakeNewIdeoInfo = AccessTools.Method(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.SelectOrMakeNewIdeo));

    private static readonly MethodInfo clearCustomIdeosInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.ClearCustomIdeos));

    private static readonly MethodInfo loadCustomIdeosInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.LoadCustomIdeos));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction code = codes[i];
            if (code.opcode == OpCodes.Ldarg_0 && codes[i + 2].Calls(selectOrMakeNewIdeoInfo))
            {
                yield return new CodeInstruction(OpCodes.Call, findIdeoManagerInfo);
                yield return new CodeInstruction(OpCodes.Call, loadCustomIdeosInfo);
            }

            if (code.opcode == OpCodes.Ldstr)
            {
                yield return new CodeInstruction(OpCodes.Call, clearCustomIdeosInfo);
            }

            yield return code;
        }
    }
}
