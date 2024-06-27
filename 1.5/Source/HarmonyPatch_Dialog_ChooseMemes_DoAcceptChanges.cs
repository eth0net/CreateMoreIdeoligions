using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Dialog_ChooseMemes), "DoAcceptChanges")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_Dialog_ChooseMemes_DoAcceptChanges
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(Dialog_ChooseMemes __instance)
    {
        if (!__instance.initialSelection || __instance.memeCategory != MemeCategory.Normal ||
            __instance.ideo == null) return;
#if DEBUG
        Log.Warning($"[DoAcceptChanges] Adding custom ideo: {__instance.ideo}");
#endif
        CreateMoreIdeoligionsUtility.AddCustomIdeo(__instance.ideo);
    }
}
