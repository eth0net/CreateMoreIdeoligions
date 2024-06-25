using RimWorld;
using UnityEngine;
using Verse;

namespace MoreCustomIdeoligions;

/// <summary>
/// The settings for the mod
/// </summary>
public class MoreCustomIdeoligionsSettings : ModSettings
{
    /// <summary>
    /// The limit for the number of preferred xenotypes
    /// </summary>
    public static int preferredXenotypeLimit = 3;

    /// <summary>
    /// Expose data to save/load
    /// </summary>
    public override void ExposeData()
    {
        Scribe_Values.Look(ref preferredXenotypeLimit, "preferredXenotypeLimit", 3);
        base.ExposeData();
    }

    /// <summary>
    /// Draw the settings window
    /// </summary>
    /// <param name="inRect"></param>
    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listing_Standard = new();
        listing_Standard.Begin(inRect);

        preferredXenotypeLimit = (int)listing_Standard.SliderLabeled("MoreCustomIdeoligions.PreferredXenotypeLimit".Translate() + ": " + preferredXenotypeLimit, preferredXenotypeLimit, 1, 20);

        listing_Standard.Gap();

        if (listing_Standard.ButtonText("MoreCustomIdeoligions.ResetSettings".Translate()))
        {
            ResetSettings();
        }

        listing_Standard.End();
    }

    /// <summary>
    /// Reset the settings to default
    /// </summary>
    public void ResetSettings()
    {
        preferredXenotypeLimit = 3;
    }
}
