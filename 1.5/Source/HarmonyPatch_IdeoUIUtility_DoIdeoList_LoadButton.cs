using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_IdeoUIUtility_DoIdeoList_LoadButton
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static MethodBase TargetMethod()
    {
        // The method is tucked away in a nested class, so we need to find it first
        // It's defined as a delegate in IdeoUIUtility.DoIdeoList for the Load button
        // In 1.5, the method is in IdeoUIUtility.<>c__DisplayClass61_0.<DoIdeoList>b__1
        return (from type in typeof(IdeoUIUtility).GetNestedTypes(AccessTools.all)
            from method in type.GetMethods(AccessTools.all)
            where method.Name.Contains("DoIdeoList") && method.ReturnType == typeof(void)
            let parameters = method.GetParameters()
            where parameters.Length == 1 && parameters[0].ParameterType == typeof(Ideo)
            select method).FirstOrDefault();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(Ideo ideo)
    {
#if DEBUG
        Log.Warning($"[<DoIdeoList>b__1] Adding custom ideo: {ideo}");
#endif
        CreateMoreIdeoligionsUtility.AddCustomIdeo(ideo);
    }
}
