namespace AnotherRpgModExpanded.RPGModule;

internal class LeechNode : Node
{
    public LeechNode(LeechType _leechType, NodeType _type, bool _unlocked = false, float _value = 1,
        int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type,
        _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetLeechType = _leechType;
    }

    public LeechType GetLeechType { get; }

    public float GetLeech()
    {
        return value * level;
    }
}