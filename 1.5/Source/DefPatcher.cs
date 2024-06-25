using RimWorld;
using Verse;

namespace CreateMoreIdeoligions;

[StaticConstructorOnStartup]
static class DefPatcher
{
    static DefPatcher()
    {
        PreceptDefOf.PreferredXenotype.maxCount = CreateMoreIdeoligionsSettings.preferredXenotypeLimit;
    }
}
