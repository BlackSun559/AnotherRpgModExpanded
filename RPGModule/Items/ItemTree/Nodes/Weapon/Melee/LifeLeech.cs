using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class LifeLeech : ItemNode
{
    public float leech;
    protected new string m_Desc = "+ 0.X% Life Leech";

    protected new string m_Name = "Life Leech";
    public new float rarityWeight = 0.2f;

    public override NodeCategory GetNodeCategory => NodeCategory.Flat;

    public override string GetName => m_Name;

    public override string GetDesc => "Restore " + leech * GetLevel.Clamp(1, GetMaxLevel) + "% Health each attack";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().Leech += leech * GetLevel * 0.01f;
    }

    public override void SetPower(float value)
    {
        leech = Mathf.Round(value * 0.2f, 2).Clamp(0.2f, 50);
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