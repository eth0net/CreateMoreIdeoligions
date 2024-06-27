using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.PostOpen))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_Page_ConfigureIdeo_PostOpen
{
    private static readonly MethodInfo findIdeoManagerInfo =
        AccessTools.PropertyGetter(typeof(Find), nameof(Find.IdeoManager));

    private static readonly MethodInfo selectOrMakeNewIdeoInfo =
        AccessTools.Method(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.SelectOrMakeNewIdeo));

    private static readonly MethodInfo clearCustomIdeosInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.ClearCustomIdeos));

    private static readonly MethodInfo loadCustomIdeosInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.LoadCustomIdeos));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [..instructions];

        for (var i = 0; i < codes.Count; i++)
        {
            var code = codes[i];
            if (code.opcode == OpCodes.Ldarg_0 && codes[i + 2].Calls(selectOrMakeNewIdeoInfo))
            {
#if DEBUG
                Log.Warning("[PostOpen] Patching to load custom ideos");
#endif
                yield return new CodeInstruction(OpCodes.Call, findIdeoManagerInfo);
                yield return new CodeInstruction(OpCodes.Call, loadCustomIdeosInfo);
            }

            if (code.opcode == OpCodes.Ldstr)
            {
#if DEBUG
                Log.Warning("[PostOpen] Patching to clear custom ideos");
#endif
                yield return new CodeInstruction(OpCodes.Call, clearCustomIdeosInfo);
            }

            yield return code;
        }
    }
}
