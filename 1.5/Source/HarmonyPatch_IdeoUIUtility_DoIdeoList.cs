using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
#if DEBUG
using System.Text;
#endif
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

    private static readonly MethodInfo addCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeosUtility), nameof(MoreCustomIdeosUtility.AddCustomIdeo));

    private static readonly MethodInfo isCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeosUtility), nameof(MoreCustomIdeosUtility.IsCustomIdeo));

    private static readonly MethodInfo isLastCustomIdeoInfo = AccessTools.Method(typeof(MoreCustomIdeosUtility), nameof(MoreCustomIdeosUtility.IsLastCustomIdeo));

    private static readonly MethodInfo ideosInViewOrderCustomInfo = AccessTools.Method(typeof(MoreCustomIdeosUtility), nameof(MoreCustomIdeosUtility.IdeosInViewOrderCustom));

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

public static class MoreCustomIdeosUtility
{
#if DEBUG
    private static string lastLog;
#endif
    private static readonly List<Ideo> customIdeos = [];

    public static void AddCustomIdeo(Ideo ideo) => customIdeos.AddDistinct(ideo);

    public static bool RemoveCustomIdeo(Ideo ideo) => customIdeos.Remove(ideo);

    public static bool IsCustomIdeo(Ideo ideo) => customIdeos.Contains(ideo);

    public static bool IsLastCustomIdeo(this IdeoManager manager, Ideo ideo) => manager.LastCustomIdeo() == ideo;

    public static Ideo LastCustomIdeo(this IdeoManager manager)
    {
        var ideos = manager.IdeosInViewOrderCustom().Where(IsCustomIdeo);
        return ideos.Count() > 0 ? ideos.Last() : null;
    }

    public static IEnumerable<Ideo> IdeosInViewOrderCustom(this IdeoManager manager)
    {
#if DEBUG
        StringBuilder logs = new("IdeosInViewOrderCustom\n\n");
#endif
        IEnumerable<Faction> factions = Find.FactionManager.AllFactionsInViewOrder;

        IEnumerable<Ideo> list = manager.IdeosListForReading.Where(ideo => !ideo.hidden).OrderBy(ideo =>
        {
            int factionIndex = 0;
            int ideoScore = int.MaxValue;

            foreach (Faction faction in factions)
            {
                FactionIdeosTracker factionIdeos = faction.ideos;

                if (factionIdeos != null && !ideo.hidden)
                {
                    if (faction.IsPlayer)
                    {
                        if (factionIdeos.IsPrimary(ideo))
                        {
#if DEBUG
                            logs.Append("Primary Ideo: " + ideo.ToString() + " Score: " + ideoScore + "\n");
#endif
                            return int.MinValue;
                        }

                        if (IsCustomIdeo(ideo))
                        {
#if DEBUG
                            logs.Append("Minor Ideo: " + ideo.ToString() + " Score: " + ideoScore + "\n");
#endif
                            return int.MinValue + 1;
                        }
                    }

                    if (factionIdeos.IsPrimary(ideo))
                    {
                        ideoScore = Math.Min(ideoScore, factionIndex);
                    }
                }

                factionIndex++;
            }
#if DEBUG
            logs.Append("Ideo: " + ideo.ToString() + " Score: " + ideoScore + "\n");
#endif
            return ideoScore;
        });
#if DEBUG
        string listStr = list.Join(ideo => ideo.ToString(), "\n");
        string playerIdeosStr = Faction.OfPlayer.ideos.AllIdeos.Join(ideo => ideo.ToString());
        string customIdeosStr = customIdeos.Join(ideo => ideo.ToString(), "\n");

        logs.Append("\n\n");
        logs.Append("All Ideos:\n\n");
        logs.Append(listStr);
        logs.Append("\n\n");
        logs.Append("Player Ideos:\n\n");
        logs.Append(playerIdeosStr);
        logs.Append("\n\n");
        logs.Append("Custom Ideos:\n\n");
        logs.Append(customIdeosStr);
        logs.Append("\n\n");


        string log = logs.ToString();

        if (lastLog != log)
        {
            Log.Warning("BOOM " + log);
            lastLog = log;
        }
#endif
        return list;
    }
}