using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class AdditionalDamageNodePercent : ItemNode
{
    public float Damage;
    protected new string m_Desc = "+ XX% damage";

    protected new string m_Name = "Bonus Damage";
    public new float rarityWeight = 0.8f;

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
        Damage = Mathf.Round(Mathf.Pow(value, 0.7f), 2).Clamp(2.5f, 10);
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