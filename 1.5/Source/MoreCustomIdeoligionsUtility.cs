using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
using System.Text;
#endif
using Verse;

namespace MoreCustomIdeoligions;

public static class MoreCustomIdeoligionsUtility
{
#if DEBUG
    private static string lastLog;
#endif
    private static readonly List<Ideo> customIdeos = [];

    private static Pawn pawn;

    public static Pawn Pawn
    {
        get => pawn;
        set
        {
#if DEBUG
            if (value != pawn)
            {
                Log.Message("Setting pawn to " + value);
            }
#endif
            pawn = value;
        }
    }

    public static void ClearCustomIdeos() => customIdeos.Clear();

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

    public static void LoadCustomIdeos(this IdeoManager manager)
    {
        foreach (Ideo ideo in customIdeos)
        {
            manager.Add(ideo);
        }
    }

    public static void SetPawnIdeo(this Pawn pawn, Ideo ideo)
    {
#if DEBUG
        Log.Warning($"Setting pawn {pawn} to ideo {ideo}");
#endif
        pawn?.ideo?.SetIdeo(ideo);
    }
}