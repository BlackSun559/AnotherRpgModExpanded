using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class AscendedAdditionalDamageNodePercent : ItemNode
{
    public int Damage;
    protected new string m_Desc = "+ XX% damage";
    protected new bool m_isAscend = true;
    protected new string m_Name = "(Ascended) Bonus Damage";
    public new float rarityWeight = 0.05f;

    public override bool IsAscend => m_isAscend;

    public override NodeCategory GetNodeCategory => NodeCategory.Multiplier;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + Damage * GetLevel.Clamp(1, GetMaxLevel) + "% Damage ";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().DamageBuffer +=
            item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer * (Damage * GetLevel) * 0.01f;
    }

    public override void SetPower(float value)
    {
        Damage = ((int)Mathf.Pow(value * 10, 1.2f)).Clamp(8, 999);
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