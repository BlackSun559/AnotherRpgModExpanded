namespace AnotherRpgModExpanded.RPGModule;

internal class PerkNode : Node
{
    public PerkNode(Perk _perk, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0,
        int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type, _unlocked, _value,
        _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetPerk = _perk;
    }

    public Perk GetPerk { get; }
}