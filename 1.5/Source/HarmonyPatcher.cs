using HarmonyLib;
using Verse;

namespace CreateMoreIdeoligions;

[StaticConstructorOnStartup]
public static class HarmonyPatcher
{
    static HarmonyPatcher()
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new("eth0net.CreateMoreIdeoligions");
        harmony.PatchAll();
    }
}
