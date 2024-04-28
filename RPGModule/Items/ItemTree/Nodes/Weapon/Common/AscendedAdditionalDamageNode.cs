using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class AscendedAdditionalDamageNode : ItemNode
{
    public int FlatDamage;
    protected new string m_Desc = "Add";
    protected new bool m_isAscend = true;
    protected new string m_Name = "(Ascended) Additional Damage";
    public new float rarityWeight = 0.05f;

    public override bool IsAscend => m_isAscend;

    public override NodeCategory GetNodeCategory => NodeCategory.Flat;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + FlatDamage * GetLevel.Clamp(1, GetMaxLevel) + " Damage";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer += FlatDamage * GetLevel;
    }

    public override void SetPower(float value)
    {
        FlatDamage = ((int)value * 8).Clamp(8, 999);
        m_MaxLevel = 1;
        m_RequiredPoints = 1;
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