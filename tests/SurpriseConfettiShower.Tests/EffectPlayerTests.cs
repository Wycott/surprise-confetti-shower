using Xunit;

namespace SurpriseConfettiShower.Tests;

public class EffectPlayerTests
{
    [Theory]
    [InlineData(EffectKind.Confetti)]
    [InlineData(EffectKind.Fireworks)]
    [InlineData(EffectKind.Mayhem)]
    public void Resolve_PlainEffectsPlayAsChosen(EffectKind kind) =>
        Assert.Equal(kind, EffectPlayer.Resolve(kind, EffectKind.Fireworks, new Random(1)));

    [Fact]
    public void Resolve_AlternateStartsWithConfettiThenTakesTurns()
    {
        EffectKind? last = null;
        var played = new List<EffectKind>();
        for (var i = 0; i < 4; i++)
        {
            last = EffectPlayer.Resolve(EffectKind.Alternate, last, new Random(1));
            played.Add(last.Value);
        }

        Assert.Equal([EffectKind.Confetti, EffectKind.Fireworks, EffectKind.Confetti, EffectKind.Fireworks], played);
    }

    [Fact]
    public void Resolve_RandomPicksBothPlayableEffectsAndNothingElse()
    {
        var rng = new Random(1234);
        var seen = Enumerable.Range(0, 200)
            .Select(_ => EffectPlayer.Resolve(EffectKind.Random, null, rng))
            .ToHashSet();

        Assert.Equal([EffectKind.Confetti, EffectKind.Fireworks], seen.Order());
    }
}
