using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class BonusExpNode : ItemNodeAdvanced
{
    protected new string m_Desc = "Add";
    protected new string m_Name = "Bonus Experience";
    protected new NodeCategory m_NodeCategory = NodeCategory.Other;

    public int PercentBonus;
    public new float rarityWeight = 0.4f;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + PercentBonus * GetLevel.Clamp(1, GetMaxLevel) + " % bonus Item Exp";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().BonusXp += 0.01f * PercentBonus * GetLevel;
    }

    public override void SetPower(float value)
    {
        PercentBonus = ((int)value * 10).Clamp(5, 150);
    }

    public override void LoadValue(string saveValue)
    {
        power = saveValue.SafeFloatParse();
        SetPower(power);
    }

    public override string GetSaveValue()
    {
        return "1,0";
        //return power.ToString();
    }
}