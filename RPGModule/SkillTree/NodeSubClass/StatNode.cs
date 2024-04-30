namespace AnotherRpgModExpanded.RPGModule;

internal class StatNode : Node
{
    public StatNode(Stat _statType, bool _flat, NodeType _type, bool _unlocked = false, float _value = 1,
        int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type,
        _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetStatType = _statType;
        GetFlat = _flat;
    }

    public bool GetFlat { get; }

    public Stat GetStatType { get; }

    public float GetDamage()
    {
        return value * level;
    }
}