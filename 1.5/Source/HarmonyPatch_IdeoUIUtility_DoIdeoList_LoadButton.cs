using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace CreateMoreIdeoligions;

[HarmonyPatch]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_IdeoUIUtility_LoadButtons
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // The methods are tucked away in nested classes, so we need to find them first
        // One is defined as a delegate in IdeoUIUtility.DoIdeoList for the load button in the list
        // The other is defined as a delegate in IdeoUIUtility.DoInitialIdeoSelection for the central load button
        // In 1.5, the first method is IdeoUIUtility.<>c__DisplayClass61_0.<DoIdeoList>b__1
        // and the second is IdeoUIUtility.<>c.<DoInitialIdeoSelection>b__59_0
        return from type in typeof(IdeoUIUtility).GetNestedTypes(AccessTools.all)
            from method in type.GetMethods(AccessTools.all)
            where method.Name.Contains("DoIdeoList") || method.Name.Contains("DoInitialIdeoSelection")
            where method.ReturnType == typeof(void)
            let parameters = method.GetParameters()
            where parameters.Length == 1 && parameters[0].ParameterType == typeof(Ideo)
            select method;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(Ideo ideo)
    {
#if DEBUG
        Log.Warning($"[Ideo Load Button] Adding custom ideo: {ideo}");
#endif
        CreateMoreIdeoligionsUtility.AddCustomIdeo(ideo);
    }
}
