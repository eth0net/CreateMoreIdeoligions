using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Verse;

namespace CreateMoreIdeoligions;

/// <summary>
///     The settings for the mod
/// </summary>
public class CreateMoreIdeoligionsSettings : ModSettings
{
    /// <summary>
    ///     The limit for the number of preferred xenotypes
    /// </summary>
    public static int PreferredXenotypeLimit = 3;

    /// <summary>
    ///     Expose data to save/load
    /// </summary>
    public override void ExposeData()
    {
        Scribe_Values.Look(ref PreferredXenotypeLimit, "preferredXenotypeLimit", 3);
        base.ExposeData();
    }

    /// <summary>
    ///     Draw the settings window
    /// </summary>
    /// <param name="inRect"></param>
    public static void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new();
        listingStandard.Begin(inRect);

        PreferredXenotypeLimit = (int)listingStandard.SliderLabeled(
            "CreateMoreIdeoligions.PreferredXenotypeLimit".Translate() + ": " + PreferredXenotypeLimit,
            PreferredXenotypeLimit, 1, 20);

        listingStandard.Gap();

        if (listingStandard.ButtonText("CreateMoreIdeoligions.ResetSettings".Translate())) ResetSettings();

        listingStandard.End();
    }

    /// <summary>
    ///     Reset the settings to default
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static void ResetSettings()
    {
        PreferredXenotypeLimit = 3;
    }
}
