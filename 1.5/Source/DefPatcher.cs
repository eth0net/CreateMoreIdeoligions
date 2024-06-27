using RimWorld;
using Verse;

namespace CreateMoreIdeoligions;

[StaticConstructorOnStartup]
static class DefPatcher
{
    static DefPatcher()
    {
#if DEBUG
        Log.Message("CreateMoreIdeoligions: Patching IdeoDefs");
#endif
        PreceptDefOf.PreferredXenotype.maxCount = CreateMoreIdeoligionsSettings.preferredXenotypeLimit;
    }
}
