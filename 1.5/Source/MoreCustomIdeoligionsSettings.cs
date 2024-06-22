using RimWorld;
using UnityEngine;
using Verse;

namespace MoreCustomIdeoligions;

/// <summary>
/// The settings for the mod
/// </summary>
public class MoreCustomIdeoligionsSettings : ModSettings
{
    // /// <summary>
    // /// The global multiplier applied to the reaction speed
    // /// </summary>
    // public static float globalReactionSpeedMultiplier = 1f;

    // /// <summary>
    // /// The global multiplier applied to the reaction heat bonus
    // /// </summary>
    // public static float globalReactionHeatBonusMultiplier = 1f;

    /// <summary>
    /// Expose data to save/load
    /// </summary>
    public override void ExposeData()
    {
        // Scribe_Values.Look(ref globalReactionSpeedMultiplier, "globalReactionSpeedMultiplier", 1f);
        // Scribe_Values.Look(ref globalReactionHeatBonusMultiplier, "globalReactionHeatBonusMultiplier", 1f);
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

        // listing_Standard.Label("MoreCustomIdeoligions.GlobalReactionSpeedMultiplier".Translate() + ": " + globalReactionSpeedMultiplier);
        // globalReactionSpeedMultiplier = listing_Standard.Slider(globalReactionSpeedMultiplier, 0.001f, 1000f);

        // listing_Standard.Label("MoreCustomIdeoligions.GlobalReactionHeatBonusMultiplier".Translate() + ": " + globalReactionHeatBonusMultiplier);
        // globalReactionHeatBonusMultiplier = listing_Standard.Slider(globalReactionHeatBonusMultiplier, 0.001f, 1000f);

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
        // globalReactionSpeedMultiplier = 1f;
        // globalReactionHeatBonusMultiplier = 1f;
    }
}
