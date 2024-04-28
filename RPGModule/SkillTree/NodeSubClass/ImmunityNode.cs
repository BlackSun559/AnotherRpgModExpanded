namespace AnotherRpgModExpanded.RPGModule;

internal class ImmunityNode : Node
{
    public ImmunityNode(Immunity _immunity, NodeType _type, bool _unlocked = false, float _value = 1,
        int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type,
        _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetImmunity = _immunity;
    }

    public Immunity GetImmunity { get; }
}