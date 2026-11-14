using System.Windows;
using WinForms = System.Windows.Forms;

namespace SurpriseConfettiShower;

/// <summary>Plays one effect at a time, across every monitor at once.</summary>
public static class EffectPlayer
{
    private static readonly EffectKind[] Playable = [EffectKind.Confetti, EffectKind.Fireworks];

    // Kept in memory only, so Alternate starts again with confetti after a restart.
    private static EffectKind? _lastPlayed;

    /// <summary>
    /// Opens a click-through overlay on each monitor, each running its own particle simulation
    /// and closing itself when the animation ends. Does nothing while an effect is still
    /// playing, so two triggers close together cannot stack two showers on one screen.
    /// </summary>
    public static void Play(EffectKind kind)
    {
        // WPF removes a window from this collection as it closes, so it already tracks the overlays.
        if (Application.Current.Windows.OfType<EffectWindow>().Any())
        {
            return;
        }

        // Resolved only once the effect will really play, so a skipped trigger cannot break the alternation.
        var effect = Resolve(kind, _lastPlayed, System.Random.Shared);
        _lastPlayed = effect;

        foreach (var screen in WinForms.Screen.AllScreens)
        {
            new EffectWindow(screen.Bounds, effect).Show();
        }
    }

    /// <summary>Turns the chosen setting into the effect to play now.</summary>
    public static EffectKind Resolve(EffectKind chosen, EffectKind? lastPlayed, Random rng) => chosen switch
    {
        EffectKind.Alternate => lastPlayed == EffectKind.Confetti ? EffectKind.Fireworks : EffectKind.Confetti,
        EffectKind.Random => Playable[rng.Next(Playable.Length)],
        _ => chosen,
    };
}
