using AnotherRpgModExpanded.Utils;

namespace AnotherRpgModExpanded.RPGModule.Entities;

internal class StatData
{
    public StatData(int _natural, int _level = 0, int _xp = 0)
    {
        NaturalLevel = _natural;
        GetXP = _xp;
        AddLevel = _level;
    }

    public int AddLevel { get; private set; }

    public int NaturalLevel { get; private set; }

    public int GetLevel => AddLevel + NaturalLevel;
    public int GetXP { get; private set; }

    public int XpForLevel()
    {
        return Mathf.CeilInt(Mathf.Pow(AddLevel * 0.04f, 0.75f)) + 1;
    }

    public void AddXp(int _xp)
    {
        GetXP += _xp;
        while (GetXP >= XpForLevel())
        {
            GetXP -= XpForLevel();
            AddLevel = AddLevel + 1;
        }
    }

    public void LevelUp()
    {
        NaturalLevel++;
    }
}