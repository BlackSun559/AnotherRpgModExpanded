using AnotherRpgModExpanded.RPGModule.Items;

namespace AnotherRpgModExpanded.Items;

internal static class ItemExtraction
{
    public static long GetTotalEarnedXp(ItemUpdate item)
    {
        long xp = 0;
        var level = item.Level;
        var ascLevel = item.Ascension;


        for (var j = 0; j < item.Level; j++) xp += item.GetExpToNextLevel(j, ascLevel);

        return xp;
    }

    public static float GetExtractedXp(bool Destroy, ItemUpdate item)
    {
        float exp = 0;

        if (Destroy)
            exp = GetTotalEarnedXp(item) * 0.5f;

        else
            exp = GetTotalEarnedXp(item) * 0.25f;

        return exp;
    }
}