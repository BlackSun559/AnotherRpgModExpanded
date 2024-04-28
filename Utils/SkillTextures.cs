using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule;

namespace AnotherRpgModExpanded.Utils;

public enum DamageNameTree : byte
{
    Melee,
    Ranged,
    Throw,
    Magic,
    Summon,
    Bow, //thorium
    Gun //thorium
}

internal class SkillTextures
{
    public static string GetItemTexture(ItemNode node)
    {
        var path = "AnotherRpgModExpanded/Textures/ItemTree/" + node.GetName;
        return path;
    }

    public static string GetTexture(Node node)
    {
        var path = "AnotherRpgModExpanded/Textures/SkillTree/" + node.GetNodeType + "/";

        var additional = "";

        switch (node.GetNodeType)
        {
            case NodeType.Class:
                additional += (node as ClassNode).GetClassType;
                break;
            case NodeType.Damage:
                additional += (node as DamageNode).GetDamageType;
                break;
            case NodeType.Speed:
                additional += (node as SpeedNode).GetDamageType;
                break;
            case NodeType.Leech:
                additional += (node as LeechNode).GetLeechType;
                break;
            case NodeType.Perk:
                additional += (node as PerkNode).GetPerk;
                break;
            case NodeType.Immunity:
                additional += (node as ImmunityNode).GetImmunity;
                break;
            case NodeType.Stats:
                additional += (node as StatNode).GetStatType;
                break;
            case NodeType.LimitBreak:
                additional += (node as LimitBreakNode).LimitBreakType;
                break;
        }

        path += additional;
        return path;
    }
}