using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MoreCustomIdeoligions;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoIdeoList))]
static class HarmonyPatch_IdeoUIUtility_DoIdeoList
{
    private static readonly MethodInfo primaryIdeoInfo = AccessTools.PropertyGetter(typeof(FactionIdeosTracker), nameof(FactionIdeosTracker.PrimaryIdeo));

    private static readonly MethodInfo isMinorInfo = AccessTools.Method(typeof(FactionIdeosTracker), nameof(FactionIdeosTracker.IsMinor));

    private static readonly MethodInfo ideoManagerInfo = AccessTools.PropertyGetter(typeof(Find), nameof(Find.IdeoManager));

    private static readonly MethodInfo ideosInViewOrderInfo = AccessTools.PropertyGetter(typeof(IdeoManager), nameof(IdeoManager.IdeosInViewOrder));

    private static readonly MethodInfo selectOrMakeNewIdeoInfo = AccessTools.Method(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.SelectOrMakeNewIdeo));

    private static readonly MethodInfo addCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.AddCustomIdeo));

    private static readonly MethodInfo isCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.IsCustomIdeo));

    private static readonly MethodInfo isLastCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.IsLastCustomIdeo));

    private static readonly MethodInfo ideosInViewOrderCustomInfo = AccessTools.Method(typeof(MoreCustomIdeoligionsUtility), nameof(MoreCustomIdeoligionsUtility.IdeosInViewOrderCustom));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label? label = null;

        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction code = codes[i];

            // Catch any custom ideos created by the player
            if (code.opcode == OpCodes.Stsfld && codes[i - 1].opcode == OpCodes.Ldfld)
            {
#if DEBUG
                Log.Warning("Patching AddCustomIdeo into SelectOrMakeNewIdeo");
#endif
                Label jump = generator.DefineLabel();
                code.labels.Add(jump);

                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(OpCodes.Brfalse, jump);
                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(OpCodes.Call, addCustomIdeoInfo);
            }

            // Replace IdeoManager.IdeosInViewOrder with MoreCustomIdeosUtility.IdeosInViewOrderCustom
            if (code.Calls(ideosInViewOrderInfo))
            {
#if DEBUG
                Log.Warning("Patching IdeosInViewOrder");
#endif
                yield return new CodeInstruction(OpCodes.Call, ideosInViewOrderCustomInfo);
                continue;
            }

            // Replace factionIdeos.PrimaryIdeo with factionIdeos.Has
            if (code.Calls(primaryIdeoInfo) && codes[i + 1].opcode == OpCodes.Bne_Un)
            {
#if DEBUG
                Log.Warning("Patching PrimaryIdeo");
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
                Log.Warning("Found label");
#endif
                label = (Label)code.operand;
            }

            // Exit the loop if the ideo is not a custom ideo, skipping delete button
            if (code.opcode == OpCodes.Ldloc_1)
            {
#if DEBUG
                Log.Warning("Patching skip delete button");
#endif
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                yield return new CodeInstruction(OpCodes.Call, isCustomIdeoInfo);
                yield return new CodeInstruction(OpCodes.Brfalse, label);
            }

            // Patch the delete button to delete the matching item, not the primary
            if (code.opcode == OpCodes.Stloc_S && codes[i - 1].opcode == OpCodes.Newobj)
            {
#if DEBUG
                Log.Warning("Patching delete button ideo");
#endif
                yield return code;
                yield return codes[i + 1];
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                i += 4;
                continue;
            }

            // Exit the loop if the ideo is not the last custom ideo, skipping the faction header
            if (code.opcode == OpCodes.Ldloc_0 && codes[i - 1].opcode == OpCodes.Stfld && codes[i + 1].opcode == OpCodes.Ldstr)
            {
#if DEBUG
                Log.Warning("Patching skip faction ideos label");
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
