using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace CreateMoreIdeoligions;

[StaticConstructorOnStartup]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class DefPatcher
{
    static DefPatcher()
    {
#if DEBUG
        Log.Message("CreateMoreIdeoligions: Patching IdeoDefs");
#endif
        PreceptDefOf.PreferredXenotype.maxCount = CreateMoreIdeoligionsSettings.PreferredXenotypeLimit;
    }
}