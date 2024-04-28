using System;
using System.Collections.Generic;
using System.Linq;
using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace AnotherRpgModExpanded.Items;

internal class Branch
{
    private readonly List<int> m_nodeID;

    public Branch()
    {
        m_nodeID = new List<int>();
    }

    public void Add(int ID)
    {
        m_nodeID.Add(ID);
    }

    public int GetLast()
    {
        if (IsEmpty())
            return 0;
        return m_nodeID.Last();
    }

    public int GetLastandBefore()
    {
        if (m_nodeID.Count > 1)
            return m_nodeID[m_nodeID.Count - Mathf.RandomInt(1, 2)];

        return GetLast();
    }

    public bool IsEmpty()
    {
        if (m_nodeID.Count == 0) return true;

        return false;
    }

    public static float GetPosition(int index)
    {
        switch (index)
        {
            case 1:
                return -75;
            case 2:
                return 75;
            case 3:
                return -175;
            case 4:
                return 175;
            case 5:
                return -300;
            case 6:
                return 300;
            default:
                return 0;
        }
    }

    public static int GetIdFromPosition(float PosX)
    {
        switch ((int)PosX)
        {
            case -75:
                return 1;
            case 75:
                return 2;
            case -175:
                return 3;
            case 175:
                return 4;
            case -300:
                return 5;
            case 300:
                return 6;
            default:
                return 0;
        }
    }

    public static int GetRandomNearbyBranches(int index, int limit, List<Branch> BList, bool noEmpty)
    {
        var branches = GetNearbyBranchesID(index);
        var idToDel = new List<int>();
        foreach (var id in branches)
        {
            if (noEmpty && (id >= BList.Count || (id < BList.Count && BList[id].IsEmpty())))
                idToDel.Add(id);

            if (id > limit) idToDel.Add(id);
        }

        foreach (var id in idToDel) branches.Remove(id);

        if (branches.Count == 0)
        {
            branches = GetNearbyBranchesID(index);

            return GetRandomNearbyBranches(branches[Mathf.RandomInt(0, branches.Count - 1)], limit, BList, noEmpty);
        }

        if (branches.Count == 1) return branches[0];

        return branches[Mathf.RandomInt(0, 1)];
    }

    public static List<int> GetNearbyBranchesID(int index)
    {
        var branches = new List<int>();
        switch (index)
        {
            case 0:
                branches = new List<int> { 1, 2 };
                break;
            case 1:
                branches = new List<int> { 0, 3 };
                break;
            case 3:
                branches = new List<int> { 1, 5 };
                break;
            case 5:
                branches = new List<int> { 3 };
                break;
            case 2:
                branches = new List<int> { 0, 4 };
                break;
            case 4:
                branches = new List<int> { 2, 6 };
                break;
            case 6:
                branches = new List<int> { 4 };
                break;
            default:
                return new List<int> { 0 };
        }

        return branches;
    }
}

internal class ItemSkillTree
{
    private const int MAXBRANCH = 7;
    private const int MINBRANCH = 3;
    protected int m_ascendPoints;

    //How to save : 
    //optimised to : 
    //;ID,Neighboor1:2,state,level,maxlevel,required,posx:posy,specificvalue1:2:3:5:7;
    //Convert to Json, to save


    protected int m_evolutionPoints;
    protected ItemUpdate m_ItemSource;
    protected int m_maxAscendPoints;
    protected int m_maxEvolutionPoints;

    protected List<ItemNode> m_nodeList;

    public ItemSkillTree()
    {
        m_ItemSource = new ItemUpdate();
        m_nodeList = new List<ItemNode>();
    }

    public int EvolutionPoints
    {
        get => m_evolutionPoints;
        set => m_evolutionPoints = value;
    }

    public int AscendPoints
    {
        get => m_ascendPoints;
        set => m_ascendPoints = value;
    }

    public int MaxEvolutionPoints
    {
        get => m_maxEvolutionPoints;
        set => m_maxEvolutionPoints = value;
    }

    public int MaxAscendPoints
    {
        get => m_maxAscendPoints;
        set => m_maxAscendPoints = value;
    }

    public int GetUsedPoints
    {
        get
        {
            var value = 0;
            foreach (var IN in m_nodeList)
                if (IN.GetState >= 4)
                    value++;
            return value;
        }
    }

    private int HaveNodeAtPos(Vector2 pos)
    {
        for (var i = 0; i < m_nodeList.Count; i++)
            if (m_nodeList[i].GetPos == pos)
                return i;
        return -1;
    }

    public static string ConvertToString(ItemSkillTree skilltree)
    {
        var save = "";
        for (var i = 0; i < skilltree.GetSize; i++)
        {
            if (i > 0)
                save += ";";
            save += ItemNodeAtlas.GetID(skilltree.GetNode(i).GetType().Name) + "|";
            for (var j = 0; j < skilltree.GetNode(i).GetNeighboor.Count; j++)
            {
                save += skilltree.GetNode(i).GetNeighboor[j];

                if (j < skilltree.GetNode(i).GetNeighboor.Count - 1)
                    save += ':';
            }

            save += "|" + skilltree.GetNode(i).GetState + "|" + skilltree.GetNode(i).GetLevel + "|" +
                    skilltree.GetNode(i).GetMaxLevel + "|" + skilltree.GetNode(i).GetRequiredPoints + "|" +
                    skilltree.GetNode(i).GetPos.X + ":" + skilltree.GetNode(i).GetPos.Y + "|";

            save += skilltree.GetNode(i).GetSaveValue();
        }

        return save;
    }

    public static ItemSkillTree ConvertToTree(string save, ItemUpdate source, int evoPoint, int AscPoint)
    {
        string[] nodeListSave;
        string[] nodeDetails;
        var tree = new ItemSkillTree();
        nodeListSave = save.Split(';');
        tree.m_ItemSource = source;
        string[] a;
        ItemNode bufferNode;

        tree.MaxEvolutionPoints = evoPoint;
        tree.MaxAscendPoints = AscPoint;
        tree.EvolutionPoints = tree.MaxEvolutionPoints;
        tree.AscendPoints = tree.MaxAscendPoints;

        foreach (var nodeSave in nodeListSave)
        {
            bufferNode = null;
            nodeDetails = nodeSave.Split('|');

            if (nodeDetails.Length != 8)
            {
                AnotherRpgModExpanded.Instance.Logger.Error(source.ItemName);
                AnotherRpgModExpanded.Instance.Logger.Error("Item tree corrupted, reseting tree");
                AnotherRpgModExpanded.Instance.Logger.Error(nodeSave);
                tree = new ItemSkillTree();
                tree.MaxEvolutionPoints = evoPoint;
                tree.MaxAscendPoints = AscPoint;
                tree.Init(source);
                tree.Reset(true);
                tree.EvolutionPoints = tree.MaxEvolutionPoints;
                tree.AscendPoints = tree.MaxAscendPoints;

                if (tree.AscendPoints > 0)
                    tree.ExtendTree(
                        Mathf.CeilInt(Mathf.Pow(source.BaseCap / 3f, 0.95)).Clamp(5, 99) * tree.AscendPoints);
                return tree;
            }

            bufferNode = (ItemNode)ItemNodeAtlas.GetCorrectNode(int.Parse(nodeDetails[0]));

            a = nodeDetails[6].Split(':');
            bufferNode.Init(tree, tree.GetSize, int.Parse(nodeDetails[4]), int.Parse(nodeDetails[5]),
                new Vector2(int.Parse(a[0]), int.Parse(a[1])));

            //Ignore it if there is no neighboor
            if (nodeDetails[1] != "")
            {
                a = nodeDetails[1].Split(':');
                foreach (var neightboor in a) bufferNode.AddNeightboor(int.Parse(neightboor));
            }

            bufferNode.ForceLockNode(int.Parse(nodeDetails[2]));
            bufferNode.SetLevel = int.Parse(nodeDetails[3]);


            if (nodeDetails[7] != "")
                bufferNode.LoadValue(nodeDetails[7]);
            tree.AddNode(bufferNode);

            if (bufferNode.IsAscend)
                tree.AscendPoints -= bufferNode.GetLevel * bufferNode.GetRequiredPoints;
            else
                tree.EvolutionPoints -= bufferNode.GetLevel * bufferNode.GetRequiredPoints;
        }

        return tree;
    }

    public void Init(ItemUpdate source)
    {
        m_ItemSource = source;
        m_nodeList = new List<ItemNode>();
        GenerateTree();
    }

    #region StatsFunctions

    public void ApplyFlatPassives(Item item)
    {
        foreach (var n in m_nodeList)
            if (n.GetLevel > 0 && n.GetNodeCategory == NodeCategory.Flat)
                n.Passive(item);
    }

    public void ApplyMultiplierPassives(Item item)
    {
        foreach (var n in m_nodeList)
            if (n.GetLevel > 0 && n.GetNodeCategory == NodeCategory.Multiplier)
                n.Passive(item);
    }

    public void ApplyOtherPassives(Item item)
    {
        foreach (var n in m_nodeList)
            if (n.GetLevel > 0 && n.GetNodeCategory == NodeCategory.Other)
                n.Passive(item);
    }

    public void ApplyPlayerPassive(Item item, Player Player)
    {
        foreach (var n in m_nodeList) n.PlayerPassive(item, Player);
    }

    public void OnShoot(EntitySource_ItemUse_WithAmmo source, Item item, Player Player, ref Vector2 position,
        ref Vector2 velocity, ref int type, ref int damage, ref float knockBack)
    {
        foreach (var n in m_nodeList)
            if (n is ItemNodeAdvanced m)
                m.OnShoot(source, item, Player, ref position, ref velocity, ref type, ref damage, ref knockBack);
    }

    #endregion

    #region GeneralIntractionFunction

    public void AddNode(ItemNode node)
    {
        m_nodeList.Add(node);
    }

    public int GetSize => m_nodeList.Count;

    public void Reset(bool Complete)
    {
        m_evolutionPoints = m_maxEvolutionPoints;
        m_ascendPoints = m_maxAscendPoints;

        if (Complete)
        {
            m_nodeList = new List<ItemNode>();
            Init(m_ItemSource);
        }
        else
        {
            for (var i = 0; i < m_nodeList.Count; i++) m_nodeList[i].Reset();
            m_nodeList[0].ForceLockNode(3);
        }
    }

    public void UpdateConnection()
    {
        foreach (var node in m_nodeList) node.ShareNeightboor();
        foreach (var node in m_nodeList) node.UnlockStep(node.GetState);
    }

    public void BuildConnection()
    {
        foreach (var node in m_nodeList) node.ShareNeightboor();
    }

    public ItemNode GetNode(int ID)
    {
        if (m_nodeList[ID] != null) return m_nodeList[ID];
        return null;
    }

    #endregion

    #region ExtendFunctions

    private Branch GetEntireBranch(int ID)
    {
        var branch = new Branch();

        foreach (var node in m_nodeList)
            if (Branch.GetIdFromPosition(node.GetPos.X) == Branch.GetPosition(ID))
                branch.Add(node.GetId);

        return branch;
    }

    private int GetBranchesCount()
    {
        var branches = 0;
        var detectedBranches = new bool[7];
        foreach (var node in m_nodeList)
            if (detectedBranches[Branch.GetIdFromPosition(node.GetPos.X)] == false)
            {
                detectedBranches[Branch.GetIdFromPosition(node.GetPos.X)] = true;
                branches++;
            }

        return branches;
    }

    private int GetYPos()
    {
        float higgestYPos = 0;

        foreach (var node in m_nodeList)
            if (node.GetPos.Y > higgestYPos)
                higgestYPos = node.GetPos.Y;

        return (int)(higgestYPos / 100);
    }

    #endregion

    #region Generation

    private int GetLowestBranch(List<Branch> Branches, int ypos)
    {
        var id = 0;
        var higgest = -1;
        foreach (var b in Branches)
        {
            id++;

            if ((int)(m_nodeList[b.GetLast()].GetPos.Y / 100) == ypos)
                higgest = id;
        }

        return id;
    }

    public void ExtendTree(int Node)
    {
        //Init all value to continue building the tree
        var yPos = GetYPos();
        var IDS = ItemNodeAtlas.GetAvailibleNodeList(m_ItemSource);
        var brancheAmm = Mathf.RandomInt(
            GetBranchesCount().Clamp(Mathf.RoundInt(MINBRANCH + m_ItemSource.GetCapLevel() * 0.02f), MAXBRANCH)
            , MAXBRANCH
        );

        var Branches = new List<Branch>();
        for (var i = 0; i < brancheAmm; i++) Branches.Add(GetEntireBranch(i));

        var emptyLevel = false;

        var brancheID = GetLowestBranch(Branches, yPos);

        if (brancheID == -1)
        {
            brancheID++;
            emptyLevel = true;
        }

        int connectionBranch;
        Vector2 pos;

        AnotherRpgModExpanded.Instance.Logger.Info(m_nodeList.Count);
        for (var i = 0; i < Node; i++)
        {
            //20% chance to add a new branches
            if (Mathf.RandomInt(0, 5) >= 4 && Branches.Count < brancheAmm) Branches.Add(new Branch());

            //if we reached all branches, then we goes to the layer
            if (brancheID >= Branches.Count)
            {
                emptyLevel = true;
                brancheID = 0;
                yPos++;
            }

            if (Mathf.RandomInt(0, 5) < 4 || (brancheID == Branches.Count - 1 && emptyLevel))
            {
                emptyLevel = false;

                connectionBranch = brancheID;

                if (Branches[brancheID].IsEmpty())
                    connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                else if (Mathf.RandomInt(0, 5) >= 4)
                    connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);


                pos = new Vector2(Branch.GetPosition(brancheID), yPos * 100);


                var a = HaveNodeAtPos(pos);
                if (a > 0)
                    m_nodeList[a].AddNeightboor(Branches[brancheID].GetLastandBefore());
                else
                    Branches[brancheID].Add(AddNewRandomNode(Branches[connectionBranch].GetLastandBefore(), pos, IDS));

                //add neightboor
                if (Mathf.RandomInt(0, 5) >= 4)
                {
                    connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                    var id = Branches[connectionBranch].GetLastandBefore();
                    if (!m_nodeList[Branches[brancheID].GetLast()].GetNeighboor.Contains(id))
                        m_nodeList[Branches[brancheID].GetLast()].AddNeightboor(id);
                }
            }

            brancheID += 1;
        }

        UpdateConnection();
    }

    private void GenerateTree()
    {
        var NodeGoal = Mathf.CeilInt(Mathf.Pow(m_ItemSource.BaseCap / 3f, 0.95) * 1.25f);
        var IDS = ItemNodeAtlas.GetAvailibleNodeList(m_ItemSource, false);
        var yPos = 0; //skilltree will allways goes down
        var minbranche = MINBRANCH;
        minbranche = Mathf.FloorInt(MINBRANCH + m_ItemSource.GetCapLevel() * 0.02f).Clamp(MINBRANCH, MAXBRANCH);
        var brancheAmm = Mathf.RandomInt(minbranche, MAXBRANCH);
        var Branches = new List<Branch>
        {
            new(),
            new(),
            new()
        };

        //branches placement : 
        // 3 1 0 2 4
        var brancheID = 0;
        var emptyLevel = true;
        int connectionBranch;
        Vector2 pos;
        for (var i = 0; i < NodeGoal; i++)
        {
            //20% chance to add a new branches
            if (Mathf.RandomInt(0, 5) >= 4 && Branches.Count < brancheAmm) Branches.Add(new Branch());

            //Used to init the first node

            if (i == 0)
            {
                Branches[0].Add(AddNewRandomNode(-1, new Vector2(0, 0), IDS));
                yPos++;
                emptyLevel = true;
            }
            else
            {
                //if we reached all branches, then we goes to the layer
                if (brancheID >= Branches.Count)
                {
                    emptyLevel = true;
                    brancheID = 0;
                    yPos++;
                }

                if (Mathf.RandomInt(0, 5) < 4 || (brancheID == Branches.Count - 1 && emptyLevel))
                {
                    emptyLevel = false;

                    connectionBranch = brancheID;

                    if (Branches[brancheID].IsEmpty())
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                    else if (Mathf.RandomInt(0, 5) >= 4)
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);


                    pos = new Vector2(Branch.GetPosition(brancheID), yPos * 100);


                    var a = HaveNodeAtPos(pos);
                    if (a > 0)
                        m_nodeList[a].AddNeightboor(Branches[brancheID].GetLastandBefore());
                    else
                        Branches[brancheID]
                            .Add(AddNewRandomNode(Branches[connectionBranch].GetLastandBefore(), pos, IDS));

                    //add neightboor
                    if (Mathf.RandomInt(0, 5) >= 4)
                    {
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                        var id = Branches[connectionBranch].GetLastandBefore();
                        if (!m_nodeList[Branches[brancheID].GetLast()].GetNeighboor.Contains(id))
                            m_nodeList[Branches[brancheID].GetLast()].AddNeightboor(id);
                    }
                }

                //Go through all the node one by one
                brancheID += 1;
            }
        }

        BuildConnection();
        m_nodeList[0].UnlockStep(3);
    }

    public int AddNewRandomNode(int Source, Vector2 position, List<int> IDList, bool Ascend = false)
    {
        var ID = IDList.Count;
        var Node = (ItemNode)ItemNodeAtlas.GetCorrectNode(IDList[Mathf.RandomInt(0, ID)]);

        if (!Ascend)
        {
            var power = (position.Y / 250 + Math.Abs(position.X) / 300 + Mathf.Random(-0.35f, 0.35f)).Clamp(0, 45);
            var level = 3 + (int)power;
            var requirement = Mathf.FloorInt(1 + power * 0.75f);
            Node.Init(this, m_nodeList.Count, level, requirement, position);
            Node.SetPower(1 + power);
            if (Source != -1) Node.AddNeightboor(Source);
            m_nodeList.Add(Node);
        }

        return m_nodeList.Count - 1;
    }

    #endregion
}