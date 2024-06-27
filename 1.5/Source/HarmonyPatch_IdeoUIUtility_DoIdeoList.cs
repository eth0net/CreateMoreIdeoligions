using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CreateMoreIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoIdeoList))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class HarmonyPatch_IdeoUIUtility_DoIdeoList
{
    private static readonly MethodInfo primaryIdeoInfo =
        AccessTools.PropertyGetter(typeof(FactionIdeosTracker), nameof(FactionIdeosTracker.PrimaryIdeo));

    private static readonly MethodInfo ideoManagerInfo =
        AccessTools.PropertyGetter(typeof(Find), nameof(Find.IdeoManager));

    private static readonly MethodInfo ideosInViewOrderInfo =
        AccessTools.PropertyGetter(typeof(IdeoManager), nameof(IdeoManager.IdeosInViewOrder));

    private static readonly FieldInfo selectedIdeoInfo =
        AccessTools.Field(typeof(IdeoUIUtility), nameof(IdeoUIUtility.selected));

    private static readonly MethodInfo removeCustomIdeoInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.RemoveCustomIdeo));

    private static readonly MethodInfo lastSelectedIdeoInfo =
        AccessTools.PropertySetter(typeof(CreateMoreIdeoligionsUtility),
            nameof(CreateMoreIdeoligionsUtility.LastSelectedIdeo));

    private static readonly MethodInfo isCustomIdeoInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.IsCustomIdeo));

    private static readonly MethodInfo isLastCustomIdeoInfo = AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
        nameof(CreateMoreIdeoligionsUtility.IsLastCustomIdeo));

    private static readonly MethodInfo ideosInViewOrderCustomInfo =
        AccessTools.Method(typeof(CreateMoreIdeoligionsUtility),
            nameof(CreateMoreIdeoligionsUtility.IdeosInViewOrderCustom));

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Label? label = null;

        List<CodeInstruction> codes = [..instructions];

        for (var i = 0; i < codes.Count; i++)
        {
            var code = codes[i];

            // Save the selected ideo before create
            if (code.opcode == OpCodes.Ldloc_0 && codes[i + 2].opcode == OpCodes.Ldnull)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching to save selected ideo before create");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, selectedIdeoInfo);
                yield return new CodeInstruction(OpCodes.Call, lastSelectedIdeoInfo);
            }

            // Replace IdeoManager.IdeosInViewOrder with MoreCustomIdeosUtility.IdeosInViewOrderCustom
            if (code.Calls(ideosInViewOrderInfo))
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching list method");
#endif
                yield return new CodeInstruction(OpCodes.Call, ideosInViewOrderCustomInfo);
                continue;
            }

            // Replace factionIdeos.PrimaryIdeo with IsCustomIdeo
            if (code.Calls(primaryIdeoInfo) && codes[i + 1].opcode == OpCodes.Bne_Un)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching custom ideo check");
#endif
                i += 1; // skip the next instruction
                yield return new CodeInstruction(OpCodes.Call, isCustomIdeoInfo);
                yield return new CodeInstruction(OpCodes.Brtrue, codes[i].operand);
                continue;
            }

            // Save the loop exit label for later use
            if (label == null && code.opcode == OpCodes.Br)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching to find loop label");
#endif
                label = (Label)code.operand;
            }

            // Exit the loop if the ideo is not a custom ideo, skipping delete button
            if (code.opcode == OpCodes.Ldloc_1)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching delete button conditions");
#endif
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                yield return new CodeInstruction(OpCodes.Call, isCustomIdeoInfo);
                yield return new CodeInstruction(OpCodes.Brfalse, label);
            }

            // Patch the delete button to delete the matching item, not the primary
            if (code.opcode == OpCodes.Stloc_S && codes[i - 1].opcode == OpCodes.Newobj)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching delete button ideo");
#endif
                yield return code;
                yield return codes[i + 1];
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(OpCodes.Call, removeCustomIdeoInfo);
                yield return new CodeInstruction(OpCodes.Pop);
                i += 4;
                continue;
            }

            // Exit the loop if the ideo is not the last custom ideo, skipping the faction header
            if (code.opcode == OpCodes.Ldloc_0 && codes[i - 1].opcode == OpCodes.Stfld &&
                codes[i + 1].opcode == OpCodes.Ldstr)
            {
#if DEBUG
                Log.Warning("[DoIdeoList] Patching skip faction ideos label");
#endif
                // replace with nop to preserve labels
                code.opcode = OpCodes.Nop;
                yield return code;

                // skip the header if not the last custom ideo
                yield return new CodeInstruction(OpCodes.Call, ideoManagerInfo);
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                yield return new CodeInstruction(OpCodes.Call, isLastCustomIdeoInfo);
                yield return new CodeInstruction(OpCodes.Brfalse, label);

                // insert the original instruction without the jump label
                yield return new CodeInstruction(OpCodes.Ldloc_0);
                continue;
            }

            yield return code;
        }
    }
}
