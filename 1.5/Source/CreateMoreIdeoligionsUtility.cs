using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RimWorld;
using Verse;
#if DEBUG
using HarmonyLib;
using System.Text;
#endif

namespace CreateMoreIdeoligions;

public static class CreateMoreIdeoligionsUtility
{
#if DEBUG
    private static string _lastLog;
#endif
    private static readonly List<Ideo> customIdeos = [];

    private static Ideo _lastSelectedIdeo;

    private static Pawn _pawn;

    public static Ideo LastSelectedIdeo
    {
        get => _lastSelectedIdeo;
        set
        {
#if DEBUG
            if (value != _lastSelectedIdeo) Log.Message("Setting last ideo to " + value);
#endif
            _lastSelectedIdeo = value;
        }
    }

    public static Pawn Pawn
    {
        get => _pawn;
        set
        {
#if DEBUG
            if (value != _pawn) Log.Message("Setting pawn to " + value);
#endif
            _pawn = value;
        }
    }

    public static void ClearCustomIdeos()
    {
        customIdeos.Clear();
    }

    public static void AddCustomIdeo(Ideo ideo)
    {
        customIdeos.AddDistinct(ideo);
    }

    public static bool RemoveCustomIdeo(Ideo ideo)
    {
        return customIdeos.Remove(ideo);
    }

    public static bool IsCustomIdeo(Ideo ideo)
    {
        return customIdeos.Contains(ideo);
    }

    public static bool IsLastCustomIdeo(this IdeoManager manager, Ideo ideo)
    {
        return manager.LastCustomIdeo() == ideo;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static Ideo LastCustomIdeo(this IdeoManager manager)
    {
        var ideos = manager.IdeosInViewOrderCustom().Where(IsCustomIdeo).ToList();
        return ideos.Any() ? ideos.Last() : null;
    }

    public static IEnumerable<Ideo> IdeosInViewOrderCustom(this IdeoManager manager)
    {
#if DEBUG
        StringBuilder logs = new("IdeosInViewOrderCustom\n\n");
#endif
        var factions = Find.FactionManager.AllFactionsInViewOrder;

#if DEBUG
        IEnumerable<Ideo> ideos = manager.
#else
        return manager.
#endif
            IdeosListForReading.Where(ideo => !ideo.hidden).OrderBy(ideo =>
            {
                var factionIndex = 0;
                var ideoScore = int.MaxValue;

                foreach (var faction in factions)
                {
                    var factionIdeos = faction.ideos;

                    if (factionIdeos != null && !ideo.hidden)
                    {
                        if (faction.IsPlayer)
                        {
//                             if (factionIdeos.IsPrimary(ideo))
//                             {
// #if DEBUG
//                                 logs.Append("Primary Ideo: " + ideo + " Score: " + ideoScore + "\n");
// #endif
//                                 return int.MinValue;
//                             }

                            if (IsCustomIdeo(ideo))
                            {
#if DEBUG
                                logs.Append("Minor Ideo: " + ideo + " Score: " + ideoScore + "\n");
#endif
                                return int.MinValue + 1;
                            }
                        }

                        if (factionIdeos.IsPrimary(ideo)) ideoScore = Math.Min(ideoScore, factionIndex);
                    }

                    factionIndex++;
                }
#if DEBUG
                logs.Append("Ideo: " + ideo + " Score: " + ideoScore + "\n");
#endif
                return ideoScore;
            });
#if DEBUG
        var list = ideos.ToList();
        var listStr = list.Join(ideo => ideo.ToString(), "\n");
        var playerIdeosStr = Faction.OfPlayer.ideos.AllIdeos.Join(ideo => ideo.ToString());
        var customIdeosStr = customIdeos.Join(ideo => ideo.ToString(), "\n");
        var playerPrimaryIdeo = Faction.OfPlayer.ideos.PrimaryIdeo;

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
        logs.Append($"Last Selected Ideo: {_lastSelectedIdeo}\n\n");
        logs.Append($"Player primary: {playerPrimaryIdeo}\n\n");

        var log = logs.ToString();

        if (_lastLog == log) return list;

        Log.Warning("BOOM " + log);
        _lastLog = log;

        return list;
#endif
    }

    public static void LoadCustomIdeos(this IdeoManager manager)
    {
        foreach (var ideo in customIdeos) manager.Add(ideo);
    }

    public static void SetPawnIdeo(this Pawn pawn, Ideo ideo)
    {
#if DEBUG
        Log.Warning($"Setting pawn {pawn} to ideo {ideo}");
#endif
        pawn?.ideo?.SetIdeo(ideo);
    }
}
