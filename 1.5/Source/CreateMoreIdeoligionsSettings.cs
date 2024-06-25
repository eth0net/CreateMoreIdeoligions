using RimWorld;
using UnityEngine;
using Verse;

namespace CreateMoreIdeoligions;

/// <summary>
/// The settings for the mod
/// </summary>
public class CreateMoreIdeoligionsSettings : ModSettings
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

        preferredXenotypeLimit = (int)listing_Standard.SliderLabeled("CreateMoreIdeoligions.PreferredXenotypeLimit".Translate() + ": " + preferredXenotypeLimit, preferredXenotypeLimit, 1, 20);

        listing_Standard.Gap();

        if (listing_Standard.ButtonText("CreateMoreIdeoligions.ResetSettings".Translate()))
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
