namespace AnotherRpgModExpanded.RPGModule;

internal class Node
{
    protected bool activate;
    protected bool Ascended;

    protected bool enable;


    protected int level;
    protected int levelRequirement;

    protected int maxLevel = 1;
    protected NodeParent Parent;
    protected int pointsPerLevel = 1;
    protected NodeType Type;
    protected bool unlocked;
    protected float value;

    public Node(NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1,
        int _pointsPerLevel = 1, bool ascended = false)
    {
        Type = _type;
        value = _value;
        maxLevel = _maxLevel;
        pointsPerLevel = _pointsPerLevel;
        activate = false;
        levelRequirement = _levelrequirement;
        unlocked = _unlocked;
        Ascended = ascended;
    }

    public NodeType GetNodeType => Type;

    public NodeParent GetParent => Parent;

    public int GetLevel => level;

    public float GetValue => value;

    public int GetMaxLevel => maxLevel;

    public int GetCostPerLevel => pointsPerLevel;

    public int GetLevelRequirement => levelRequirement;

    public bool GetUnlock => unlocked;

    public bool GetActivate => activate;

    public bool GetEnable => enable;

    public bool GetAscended => Ascended;

    public void SetParrent(NodeParent Parent)
    {
        this.Parent = Parent;
    }

    public virtual void ToggleEnable()
    {
        enable = !enable;
    }

    public Reason CanUpgrade(int points)
    {
        if (points < pointsPerLevel)
            return Reason.NoEnoughtPoints;

        if (level >= maxLevel)
            return Reason.MaxLevelReach;

        if (!unlocked)
            return Reason.NotUnlocked;
        return Reason.CanUpgrade;
    }

    public virtual void Upgrade()
    {
        if (!activate)
        {
            activate = true;
            enable = true;
        }

        level++;
    }

    public virtual void Unlock()
    {
        unlocked = true;
    }
}