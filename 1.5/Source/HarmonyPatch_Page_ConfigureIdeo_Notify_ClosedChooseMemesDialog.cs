using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.Notify_ClosedChooseMemesDialog))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_Page_ConfigureIdeo_Notify_ClosedChooseMemesDialog
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(Page_ConfigureIdeo __instance)
    {
        var ideo = CreateMoreIdeoligionsUtility.LastSelectedIdeo;
#if DEBUG
        Log.Warning($"Restoring selected ideo: {ideo}");
#endif
        __instance.SelectOrMakeNewIdeo(ideo);
        Faction.OfPlayer.ideos.SetPrimary(ideo);
    }
}