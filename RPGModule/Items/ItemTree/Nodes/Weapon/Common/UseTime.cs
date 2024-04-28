using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class UseTimeNode : ItemNode
{
    protected new string m_Desc = "+ XX% Attack Speed";
    protected new string m_Name = "Attack Speed";
    protected new NodeCategory m_NodeCategory = NodeCategory.Multiplier;
    public new float rarityWeight = 0.5f;

    public int UseTimeReduction;

    public override string GetName => m_Name;

    public override string GetDesc => "+ " + UseTimeReduction * GetLevel.Clamp(1, GetMaxLevel) + "% Attack Speed";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().UseTimeBuffer -=
            item.GetGlobalItem<ItemUpdate>().UseTimeBuffer * 0.01f * UseTimeReduction * GetLevel;
    }

    public override void SetPower(float value)
    {
        UseTimeReduction = Mathf.RoundInt(value).Clamp(1, 10);
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