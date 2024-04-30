using AnotherRpgModExpanded.Utils;
using Terraria;

namespace AnotherRpgModExpanded.Items;

internal class MagicCostReduction : ItemNode
{
    protected new string m_Desc = "+ X Projectile";

    protected new bool m_isAscend = true;

    protected new string m_Name = "Manacost Reduction";
    protected new NodeCategory m_NodeCategory = NodeCategory.Other;
    public float manaCostReduction;
    public new float rarityWeight = 0.4f;


    public override string GetName => m_Name;

    public override string GetDesc => "- " + manaCostReduction * 100 * GetLevel.Clamp(1, GetMaxLevel) + "% Manacost";

    public override void PlayerPassive(Item item, Player Player)
    {
        Player.manaCost *= 1 - manaCostReduction * GetLevel;
    }

    public override void SetPower(float value)
    {
        manaCostReduction = Mathf.Round(value * 0.01f, 2).Clamp(0.01f, 0.5f);
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