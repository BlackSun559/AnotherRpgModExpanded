using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AnotherRpgModExpanded.RPGModule;

internal class NodeList //prefer to make this class to stock in different set each type of node to speedup
    //looking for the entire tree would take lot of time
{
    private readonly List<DamageNode> magicDamage;
    private readonly List<SpeedNode> magicSpeed;

    private readonly List<DamageNode> meleeDamage;

    private readonly List<SpeedNode> meleeSpeed;
    public List<NodeParent> nodeList;
    private readonly List<DamageNode> rangedDamage;
    private readonly List<SpeedNode> rangedSpeed;

    private readonly List<DamageNode> summonDamage;
    private readonly List<DamageNode> throwDamage;

    public NodeList()
    {
        nodeList = new List<NodeParent>();

        meleeDamage = new List<DamageNode>();
        rangedDamage = new List<DamageNode>();
        magicDamage = new List<DamageNode>();
        summonDamage = new List<DamageNode>();
        throwDamage = new List<DamageNode>();

        meleeSpeed = new List<SpeedNode>();
        rangedSpeed = new List<SpeedNode>();
        magicSpeed = new List<SpeedNode>();
        GetLeech = new List<LeechNode>();

        GetStatsList = new List<StatNode>();
        GetLBList = new List<LimitBreakNode>();

        GetClasses = new List<ClassNode>();
        GetPerks = new List<PerkNode>();
        GetImmunities = new List<ImmunityNode>();
    }

    public List<LeechNode> GetLeech { get; }

    public List<StatNode> GetStatsList { get; }

    public List<LimitBreakNode> GetLBList { get; }

    public List<ClassNode> GetClasses { get; }

    public List<PerkNode> GetPerks { get; }

    public List<ImmunityNode> GetImmunities { get; }

    public PerkNode GetPerk(Perk perk)
    {
        foreach (var Pnode in GetPerks)
            if (Pnode.GetPerk == perk)
                return Pnode;
        return null;
    }

    public List<DamageNode> GetDamageList(DamageType _type)
    {
        switch (_type)
        {
            case DamageType.Magic:
                return magicDamage;
            case DamageType.Melee:
                return meleeDamage;
            case DamageType.Ranged:
                return rangedDamage;
            case DamageType.Throw:
                return throwDamage;
            default:
                return summonDamage;
        }
    }

    public void AddNode(DamageNode node)
    {
        var nodeParent = new NodeParent(node, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        switch (node.GetDamageType)
        {
            case DamageType.Magic:
                magicDamage.Add(node);
                break;
            case DamageType.Melee:
                meleeDamage.Add(node);
                break;
            case DamageType.Ranged:
                rangedDamage.Add(node);
                break;
            case DamageType.Throw:
                throwDamage.Add(node);
                break;
            default:
                summonDamage.Add(node);
                break;
        }
    }

    public void AddNode(SpeedNode node)
    {
        var nodeParent = new NodeParent(node, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        switch (node.GetDamageType)
        {
            case DamageType.Magic:
                magicSpeed.Add(node);
                break;
            case DamageType.Melee:
                meleeSpeed.Add(node);
                break;
            case DamageType.Ranged:
                rangedSpeed.Add(node);
                break;
            default:
                return;
        }
    }

    public void AddNode(PerkNode perk)
    {
        var nodeParent = new NodeParent(perk, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetPerks.Add(perk);
    }

    public void AddNode(LeechNode leech)
    {
        var nodeParent = new NodeParent(leech, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetLeech.Add(leech);
    }

    public void AddNode(ImmunityNode immunity)
    {
        var nodeParent = new NodeParent(immunity, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetImmunities.Add(immunity);
    }

    public void AddNode(ClassNode classnode)
    {
        var nodeParent = new NodeParent(classnode, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetClasses.Add(classnode);
    }

    public void AddNode(StatNode statnode)
    {
        var nodeParent = new NodeParent(statnode, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetStatsList.Add(statnode);
    }

    public void AddNode(LimitBreakNode LBnode)
    {
        var nodeParent = new NodeParent(LBnode, Vector2.Zero);
        nodeParent.GetNode.SetParrent(nodeParent);
        nodeList.Add(nodeParent);
        GetLBList.Add(LBnode);
    }
}