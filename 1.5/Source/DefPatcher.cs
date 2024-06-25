using RimWorld;
using Verse;

namespace MoreCustomIdeoligions;

[StaticConstructorOnStartup]
static class DefPatcher
{
    static DefPatcher()
    {
        PreceptDefOf.PreferredXenotype.maxCount = MoreCustomIdeoligionsSettings.preferredXenotypeLimit;
    }
}
