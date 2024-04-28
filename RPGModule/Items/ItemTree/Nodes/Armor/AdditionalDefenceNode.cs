using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class AdditionalDefenceNode : ItemNode
{
    public int FlatDef;
    protected new string m_Desc = "Add";
    protected new string m_Name = "Additional Defence";


    public override NodeCategory GetNodeCategory => NodeCategory.Flat;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + FlatDef * GetLevel.Clamp(1, GetMaxLevel) + " Defences";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().DefenceFlatBuffer += FlatDef * GetLevel;
    }

    public override void SetPower(float value)
    {
        FlatDef = ((int)Mathf.Pow(value * 0.8, 0.8f)).Clamp(1, 999);
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