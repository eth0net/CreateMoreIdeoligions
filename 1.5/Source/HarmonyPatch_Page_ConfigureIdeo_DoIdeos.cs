using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.DoIdeos))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_Page_ConfigureIdeo_DoIdeos
{
    private static readonly MethodInfo playerPrimaryIdeoNotSharedInfo =
        AccessTools.PropertyGetter(typeof(IdeoUIUtility), nameof(IdeoUIUtility.PlayerPrimaryIdeoNotShared));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            if (code.Calls(playerPrimaryIdeoNotSharedInfo))
            {
#if DEBUG
                Log.Warning("[DoIdeos] Patching to always show create");
#endif
                yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                continue;
            }

            yield return code;
        }
    }
}