using HarmonyLib;
using Verse;

namespace MoreCustomIdeoligions;

[StaticConstructorOnStartup]
public static class HarmonyPatcher
{
    static HarmonyPatcher()
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new("eth0net.MoreCustomIdeoligions");
        harmony.PatchAll();
    }
}
