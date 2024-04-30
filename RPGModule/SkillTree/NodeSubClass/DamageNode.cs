namespace AnotherRpgModExpanded.RPGModule;

internal class DamageNode : Node
{
    public DamageNode(DamageType _damageType, bool _flat, NodeType _type, bool _unlocked = false, float _value = 1,
        int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type,
        _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetDamageType = _damageType;
        GetFlat = _flat;
    }

    public bool GetFlat { get; }

    public DamageType GetDamageType { get; }

    public float GetDamage()
    {
        return value * level;
    }
}