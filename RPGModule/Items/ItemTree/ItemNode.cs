using System.Collections.Generic;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace AnotherRpgModExpanded.Items;

//What to save : 
//[Position,{ID,{Neighboor:1},state,level,maxlevel,{specific node values:4:5}]
//Convert to Json, to save
internal class ItemNode
{
    private static readonly int MININT = 0;
    protected string m_Desc = "Blank Node used to make more node, if you see that, report it :p";
    protected int m_ID;
    protected List<int> m_ID_Neightboor;
    protected bool m_isAscend = false;
    protected int m_Level;

    //0 = invisible, 1 = unknow, 2 = locked, 3 = unlocked, 4 = active;
    protected int m_LockState;

    protected int m_MaxLevel;

    protected string m_Name = "Blank";
    protected NodeCategory m_NodeCategory = NodeCategory.Other;

    protected Vector2 m_position;

    protected int m_RequiredPoints;
    protected ItemSkillTree m_SkillTreeParent;

    public float power;


    public float rarityWeight = 1;

    public ItemNode()
    {
        m_ID_Neightboor = new List<int>();
        m_SkillTreeParent = new ItemSkillTree();
    }

    public List<int> GetNeighboor => m_ID_Neightboor;

    public ItemSkillTree GetParent => m_SkillTreeParent;

    public int GetId => m_ID;

    public virtual NodeCategory GetNodeCategory => m_NodeCategory;

    public virtual bool IsAscend => m_isAscend;

    public int GetRequiredPoints => m_RequiredPoints;

    public virtual string GetName => m_Name;
    public virtual string GetDesc => m_Desc;

    public int GetLevel => m_Level;
    public int GetMaxLevel => m_MaxLevel;

    public int SetLevel
    {
        set => m_Level = value.Clamp(0, m_MaxLevel);
    }

    public int SetMaxLevel
    {
        set => m_MaxLevel = value.Clamp(0, int.MaxValue);
    }

    public int SetRequired
    {
        set => m_RequiredPoints = value.Clamp(1, int.MaxValue);
    }

    public int GetState => m_LockState;

    public Vector2 GetPos => m_position;

    public virtual void SetPower(float value)
    {
    }

    public void SetPos(float posx, float posy)
    {
        m_position = new Vector2(posx, posy);
    }

    public virtual void Reset()
    {
        m_Level = 0;

        if (m_LockState > 1)
            m_LockState = 2;
    }

    public virtual string GetSaveValue()
    {
        return "";
    }

    public virtual void LoadValue(string saveValue)
    {
    }

    public virtual void ShareNeightboor()
    {
        foreach (var Node in m_ID_Neightboor) m_SkillTreeParent.GetNode(Node).AddNeightboor(m_ID);
    }

    public virtual void AddNeightboor(int ID)
    {
        if (!m_ID_Neightboor.Contains(ID))
            m_ID_Neightboor.Add(ID);
    }

    public void ForceLockNode(int lockValue)
    {
        m_LockState = lockValue;
    }

    public virtual void UnlockStep(int step)
    {
        if (m_LockState < step.Clamp(MININT, 3))
            m_LockState = step.Clamp(MININT, 3);
        step -= 1;

        if (step > 0)
            foreach (var ID in m_ID_Neightboor)
                m_SkillTreeParent.GetNode(ID).UnlockStep(step);
    }

    public virtual void Activate()
    {
        m_LockState = 4;
        m_Level = 0;
    }

    public virtual ItemReason CanAddLevel(int Points)
    {
        if (m_LockState < 3)
            return ItemReason.Locked;

        if (m_Level >= m_MaxLevel)
            return ItemReason.MaxLevel;

        if (Points < m_RequiredPoints)
            return ItemReason.NotEnougtPoint;
        return ItemReason.CanUpgrade;
    }

    public virtual void AddLevel()
    {
        if (m_LockState == 3)
        {
            Activate();
            UnlockStep(4);
        }

        m_Level++;
    }

    /// <summary>
    ///     Passive :
    ///     Called each frame to modify weapons only attribute;
    /// </summary>
    /// <param name="item"></param>
    public virtual void Passive(Item item)
    {
    }

    /// <summary>
    ///     Passive :
    ///     Called each frame to modify Player linked attributes;
    /// </summary>
    /// <param name="item"></param>
    /// <param name="Player"></param>
    public virtual void PlayerPassive(Item item, Player Player)
    {
    }

    //;ID,Neighboor1:2,state,level,maxlevel,required,posx:posy,specificvalue1:2:3:5:7;
    public virtual void Init(ItemSkillTree IST, int ID, int maxlevel, int required, Vector2 pos)
    {
        m_ID_Neightboor = new List<int>();
        m_SkillTreeParent = IST;
        m_ID = ID;
        m_Level = 0;
        m_LockState = MININT;
        m_MaxLevel = maxlevel;
        m_RequiredPoints = required;
        m_position = pos;
    }
}

internal class ItemNodeAdvanced : ItemNode
{
    public virtual void OnShoot(EntitySource_ItemUse_WithAmmo source, Item item, Player Player, ref Vector2 position,
        ref Vector2 Velocity, ref int type, ref int damage, ref float knockBack)
    {
    }

    //virtual public void On
}