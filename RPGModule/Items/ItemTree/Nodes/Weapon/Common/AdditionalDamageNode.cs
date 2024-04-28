using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class AdditionalDamageNode : ItemNode
{
    public int FlatDamage;
    protected new string m_Desc = "Add";
    protected new string m_Name = "Additional Damage";


    public override NodeCategory GetNodeCategory => NodeCategory.Flat;

    public override string GetName => m_Name;

    public override string GetDesc => "Add " + FlatDamage * GetLevel.Clamp(1, GetMaxLevel) + " Damage";

    public override void Passive(Item item)
    {
        item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer += FlatDamage * GetLevel;
    }

    public override void SetPower(float value)
    {
        FlatDamage = ((int)Mathf.Pow(value * 0.8, 0.8f)).Clamp(1, 999);
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