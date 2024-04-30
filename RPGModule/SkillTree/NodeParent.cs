using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AnotherRpgModExpanded.RPGModule;

internal class NodeParent
{
    private static ushort TotalID;

    public readonly ushort ID;
    public List<NodeParent> connectedNeighboor; //used to check for drawing connection lines between neighboor

    public Vector2 menuPos;

    public NodeParent(Node node, Vector2 pos)
    {
        GetNeightboor = new List<NodeParent>();
        connectedNeighboor = new List<NodeParent>();
        GetNode = node;
        menuPos = pos;
        ID = TotalID;
        TotalID++;
    }

    public List<NodeParent> GetNeightboor { get; }

    public Node GetNode { get; }

    public bool GetEnable => GetNode.GetEnable;

    //comment to my future self as I will surely forget it : 
    // type var => var
    // is equal to 
    // type var {get{return var;}}
    public NodeType GetNodeType => GetNode.GetNodeType;

    public int GetLevel => GetNode.GetLevel;

    public int GetMaxLevel => GetNode.GetMaxLevel;

    public int GetCostPerLevel => GetNode.GetCostPerLevel;

    public int GetLevelRequirement => GetNode.GetLevelRequirement;

    public bool GetUnlock => GetNode.GetUnlock;

    public bool GetActivate => GetNode.GetActivate;

    public Reason CanUpgrade(int points, int level)
    {
        if (level < GetNode.GetLevelRequirement)
            return Reason.LevelRequirement;
        return GetNode.CanUpgrade(points);
    }

    public static void ResetID()
    {
        TotalID = 0;
    }

    public bool CanBeDisable()
    {
        if (GetNode.GetNodeType == NodeType.Class || GetNode.GetNodeType == NodeType.Perk)
            return true;
        return false;
    }

    public void ToggleEnable()
    {
        GetNode.ToggleEnable();
    }

    public void Unlock()
    {
        GetNode.Unlock();
    }

    public void AddNeighboor(NodeParent neighboor)
    {
        GetNeightboor.Add(neighboor);
        neighboor.AddNeighboorSimple(this);
    }

    public void AddNeighboorSimple(NodeParent neighboor)
    {
        GetNeightboor.Add(neighboor);
    }

    public void Upgrade(bool loading = false)
    {
        if (loading && GetNode.GetNodeType == NodeType.Class)
            (GetNode as ClassNode).loadingUpgrade();
        else
            GetNode.Upgrade();

        for (var i = 0; i < GetNeightboor.Count; i++) GetNeightboor[i].Unlock();
    }
}