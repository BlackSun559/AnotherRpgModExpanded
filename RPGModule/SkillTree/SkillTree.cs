using System;
using System.Collections.Generic;
using AnotherRpgModExpanded.RPGModule.Entities;
using Microsoft.Xna.Framework;
using Terraria;

namespace AnotherRpgModExpanded.RPGModule;

internal class SkillTree
{
    public static readonly int SKILLTREEVERSION = 2;
    public ClassNode ActiveClass;
    public NodeList nodeList;

    public SkillTree()
    {
        nodeList = new NodeList();
        var NodeSaved = JsonSkillTree.GetJsonNodeList;

        NodeType nodeT;
        ClassType classT;
        DamageType damageT;
        LeechType leechT;
        Immunity immunityT;
        Stat StatT;
        Perk perkT;

        var i = 0;
        foreach (var actualNode in NodeSaved.jsonList)
        {
            nodeT = (NodeType)Enum.Parse(typeof(NodeType), actualNode.baseType);
            switch (nodeT)
            {
                case NodeType.Damage:
                    damageT = (DamageType)Enum.Parse(typeof(DamageType), actualNode.specificType);
                    nodeList.AddNode(new DamageNode(damageT, actualNode.flatDamage, NodeType.Damage,
                        actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
                case NodeType.Class:
                    classT = (ClassType)Enum.Parse(typeof(ClassType), actualNode.specificType);
                    nodeList.AddNode(new ClassNode(classT, NodeType.Class, actualNode.unlocked,
                        actualNode.valuePerLevel, actualNode.levelRequirement, 1, actualNode.pointsPerLevel,
                        actualNode.ascended));
                    break;
                case NodeType.Speed:
                    damageT = (DamageType)Enum.Parse(typeof(DamageType), actualNode.specificType);
                    nodeList.AddNode(new SpeedNode(damageT, NodeType.Speed, actualNode.unlocked,
                        actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
                case NodeType.Immunity:
                    immunityT = (Immunity)Enum.Parse(typeof(Immunity), actualNode.specificType);
                    nodeList.AddNode(new ImmunityNode(immunityT, NodeType.Immunity, actualNode.unlocked,
                        actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
                case NodeType.Leech:
                    leechT = (LeechType)Enum.Parse(typeof(LeechType), actualNode.specificType);
                    nodeList.AddNode(new LeechNode(leechT, NodeType.Leech, actualNode.unlocked,
                        actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
                case NodeType.Perk:
                    perkT = (Perk)Enum.Parse(typeof(Perk), actualNode.specificType);
                    nodeList.AddNode(new PerkNode(perkT, NodeType.Perk, actualNode.unlocked, actualNode.valuePerLevel,
                        actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel,
                        actualNode.ascended));
                    break;
                case NodeType.Stats:
                    StatT = (Stat)Enum.Parse(typeof(Stat), actualNode.specificType);
                    nodeList.AddNode(new StatNode(StatT, actualNode.flatDamage, NodeType.Stats, actualNode.unlocked,
                        actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
                case NodeType.LimitBreak:
                    nodeList.AddNode(new LimitBreakNode(actualNode.specificType, NodeType.LimitBreak,
                        actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel,
                        actualNode.pointsPerLevel, actualNode.ascended));
                    break;
            }

            nodeList.nodeList[i].menuPos = new Vector2(actualNode.posX, actualNode.posY);
            i++;
        }

        i = 0;
        foreach (var actualNode in NodeSaved.jsonList)
        {
            foreach (var nbID in actualNode.neigthboorlist) nodeList.nodeList[i].AddNeighboor(nodeList.nodeList[nbID]);

            i++;
        }
    }

    public int GetStats(Stat stat)
    {
        var value = 0;
        foreach (var node in nodeList.GetStatsList)
            if (node.GetStatType == stat)
                value += (int)(node.GetValue * node.GetLevel);
        foreach (var node in nodeList.GetLBList) value += (int)(node.GetValue * node.GetLevel);
        return value;
    }

    private float CalcDamage(List<DamageNode> _list, bool flat)
    {
        float value = 0;
        foreach (var node in _list)
            if (node.GetFlat == flat)
                value += node.GetValue * node.GetLevel;
        return value;
    }

    private float CalcSpeed(List<SpeedNode> _list)
    {
        float value = 0;
        foreach (var node in _list) value += node.GetValue * node.GetLevel;
        return value;
    }

    private float CalcLeech(List<LeechNode> _list, LeechType _type)
    {
        float value = 0;
        foreach (var node in _list)
            if (node.GetLeechType == LeechType.Both || node.GetLeechType == _type)
                value += node.GetValue * node.GetLevel;
        return value;
    }

    public int GetSummonSlot()
    {
        var slot = 0;

        if (ActiveClass == null)
            return 0;
        slot = JsonCharacterClass.GetJsonCharList.GetClass(ActiveClass.GetClassType).Summons;
        return slot;
    }

    private float GetClassDamage(DamageType _type)
    {
        float value = 1;
        var pEntity = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();

        if (ActiveClass == null) return 1;

        var actualClass = JsonCharacterClass.GetJsonCharList.GetClass(ActiveClass.GetClassType);
        value *= 1 + actualClass.Damage[(int)_type];

        if (_type == DamageType.Ranged)
        {
            if (pEntity.HaveBow())
                value *= 1 + actualClass.Damage[5];

            if (pEntity.HaveRangedWeapon() && !pEntity.HaveBow())
                value *= 1 + actualClass.Damage[6];
        }

        return value;
    }

    public float GetDamageMult(DamageType _type)
    {
        float value = 0;

        value += CalcDamage(nodeList.GetDamageList(_type), false);
        value += GetClassDamage(_type);

        return value;
    }

    public int GetDamageFlat(DamageType _type)
    {
        float value = 0;

        value += CalcDamage(nodeList.GetDamageList(_type), true);

        return (int)value;
    }

    public float GetDamageSpeed(DamageType _type)
    {
        float value = 0;

        value += CalcDamage(nodeList.GetDamageList(_type), true);

        return value;
    }

    public float GetLeech(LeechType _leechType)
    {
        float value = 0;

        value += CalcLeech(nodeList.GetLeech, _leechType);

        return value;
    }

    public bool HavePerk(Perk _perk)
    {
        var list = nodeList.GetPerks;
        for (var i = 0; i < list.Count; i++)
            if (list[i].GetPerk == _perk && list[i].GetEnable)
                return true;
        return false;
    }

    public bool IsLimitBreak()
    {
        var list = nodeList.GetLBList;
        for (var i = 0; i < list.Count; i++)
            if (list[i].GetEnable)
                return true;
        return false;
    }

    public bool HaveImmunity(Immunity _immunity)
    {
        var list = nodeList.GetImmunities;
        for (var i = 0; i < list.Count; i++)
            if (list[i].GetImmunity == _immunity && list[i].GetEnable)
                return true;
        return false;
    }

    public void ResetConnection()
    {
        var count = nodeList.nodeList.Count;
        for (var i = 0; i < count; i++) nodeList.nodeList[i].connectedNeighboor = new List<NodeParent>();
    }

    public void Init()
    {
        NodeParent.ResetID();
        nodeList.nodeList[0].Upgrade();
    }
}