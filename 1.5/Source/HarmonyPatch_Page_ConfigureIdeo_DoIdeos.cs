using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.DoIdeos))]
static class HarmonyPatch_Page_ConfigureIdeo_DoIdeos
{
    private static readonly MethodInfo playerPrimaryIdeoNotSharedInfo = AccessTools.PropertyGetter(typeof(IdeoUIUtility), nameof(IdeoUIUtility.PlayerPrimaryIdeoNotShared));

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction code in instructions)
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
