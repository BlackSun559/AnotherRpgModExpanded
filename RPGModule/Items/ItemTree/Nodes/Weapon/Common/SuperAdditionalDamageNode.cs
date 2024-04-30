using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class SuperAdditionalDamageNode : ItemNode
{
    public int FlatDamage;
    protected new string m_Desc = "Add";
    protected new string m_Name = "(Rare) Additional Damage";
    public new float rarityWeight = 0.2f;

    public override NodeCategory GetNodeCategory => NodeCategory.Flat;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + FlatDamage * GetLevel.Clamp(1, GetMaxLevel) + " Damage";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer += FlatDamage * GetLevel;
    }

    public override void SetPower(float value)
    {
        FlatDamage = ((int)(value * 2)).Clamp(2, 999);
        m_MaxLevel = 1;
        m_RequiredPoints = 2 + Mathf.FloorInt(value * 0.2f);
        power = value;
    }

    public override void LoadValue(string saveValue)
    {
        power = saveValue.SafeFloatParse();
        SetPower(power);
    }

    public override string GetSaveValue()
    {
        return power.ToString();
    }
}