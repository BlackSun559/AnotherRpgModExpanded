using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class RarityWeightManager
{
    public RarityWeight[] weights;

    public RarityWeightManager(RarityWeight[] weights)
    {
        this.weights = weights;
    }

    public Rarity DrawRarity()
    {
        var bossKilled = WorldManager.BossDefeated;

        float Weight = 0;
        for (var i = 0; i < weights.Length; i++)
            Weight += weights[i].weight;

        var rn = Mathf.Random(0, Weight);

        if (WorldManager.Ascended)
        {
            if (Main.hardMode)
                rn *= Mathf.Pow(0.9f, bossKilled);
            else
                rn *= Mathf.Pow(0.95f, bossKilled);
        }

        else
        {
            if (Main.hardMode)
                rn *= Mathf.Pow(0.95f, bossKilled);
            else
                rn *= Mathf.Pow(0.975f, bossKilled);
        }

        float actualWeight = 0;
        for (var i = weights.Length - 1; i >= 0; i--)
        {
            actualWeight += weights[i].weight;

            if (rn < actualWeight)
                return weights[i].rarity;
        }

        return weights[0].rarity;
    }
}