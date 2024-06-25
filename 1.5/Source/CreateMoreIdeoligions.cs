using UnityEngine;
using Verse;

namespace CreateMoreIdeoligions;

/// <summary>
/// The main class for the mod
/// </summary>
public class CreateMoreIdeoligions : Mod
{
    /// <summary>
    /// The settings for the mod
    /// </summary>
    public CreateMoreIdeoligionsSettings settings;

    /// <summary>
    /// Constructor for the mod class to get the settings
    /// </summary>
    /// <param name="content"></param>
    public CreateMoreIdeoligions(ModContentPack content) : base(content)
    {
        settings = GetSettings<CreateMoreIdeoligionsSettings>();
    }

    /// <summary>
    /// Draw the settings window
    /// </summary>
    /// <param name="inRect"></param>
    public override void DoSettingsWindowContents(Rect inRect)
    {
        settings.DoSettingsWindowContents(inRect);
        base.DoSettingsWindowContents(inRect);
    }

    /// <summary>
    /// Add the settings category
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory() => "CreateMoreIdeoligions".Translate();
}