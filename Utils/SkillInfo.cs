using AnotherRpgModExpanded.RPGModule;
using AnotherRpgModExpanded.RPGModule.Entities;
using Terraria;
using Terraria.Localization;

namespace AnotherRpgModExpanded.Utils;

internal class SkillInfo
{
    public static string GetPerkDescription(Perk PerkType, int level = 1)
    {
        var desc = "";

        switch (PerkType)
        {
            case Perk.Masochist:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Masochist") + level + "%";
            case Perk.BloodMage:
                switch (level)
                {
                    case 1:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.BloodMage.1");
                    case 2:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.BloodMage.2");
                    case 3:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.BloodMage.3");
                }

                break;
            case Perk.Vampire:
                switch (level)
                {
                    case 1:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Vampire.1");
                    case 2:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Vampire.2");
                    case 3:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Vampire.3");
                }

                break;
            case Perk.Chlorophyll:
                switch (level)
                {
                    case 1:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Chlorophyll.1");
                    case 2:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Chlorophyll.2");
                    case 3:
                        return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Chlorophyll.3");
                }

                break;
            case Perk.DemonEater:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.DemonEater.1") + level * 5 +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.DemonEater.2");
            case Perk.Cupidon:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Cupidon.1") + level * 5 +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Cupidon.2");
            case Perk.StarGatherer:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.StarGatherer.1") + level * 5 +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.StarGatherer.2");
            case Perk.Biologist:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Biologist.1") + level * 20 +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Biologist.2");
            case Perk.Berserk:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Berserk.1") + level +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Berserk.2");
            case Perk.Survivalist:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Survivalist.1") + level * 20 +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Survivalist.2");
            case Perk.TheGambler:
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.TheGambler");
            case Perk.ManaOverBurst:
                var bonusmanacost = Main.player[Main.myPlayer].statMana * (0.1f + ((float)level - 1) * 0.15f);
                var multiplier = 1 + (1 - 1 / (bonusmanacost / 1000 + 1));
                return Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaOverBurst.1") +
                       (10 + 15 * (level - 1)) +
                       Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaOverBurst.2") +
                       Mathf.Round(multiplier * 100 - 100, 2) + "%";
        }

        return desc;
    }

    public static string GetClassDescription(ClassType classType)
    {
        var ClassInfo = JsonCharacterClass.GetJsonCharList.GetClass(classType);
        var desc = Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Onlyhave");


        var allDamage = true;
        var damage = ClassInfo.Damage[0];

        var id = 0;
        foreach (var d in ClassInfo.Damage)
        {
            id++;

            if (!allDamage || id > 4)
                break;

            if (d != damage)
                allDamage = false;
        }

        if (allDamage && ClassInfo.Damage[0] != 0)
            desc += "+ " + ClassInfo.Damage[0] * 100 +
                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.AllDamage");
        else
            for (var i = 0; i < ClassInfo.Damage.Length; i++)
                if (ClassInfo.Damage[i] != 0)
                {
                    if (ClassInfo.Damage[i] > 0)
                        desc += "+ " + Mathf.Round(ClassInfo.Damage[i] * 100, 2) + "% " + ((DamageNameTree)i) +
                                Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Damage1");
                    else
                        desc += "- " + Mathf.Round(-ClassInfo.Damage[i] * 100, 2) + "% " + ((DamageNameTree)i) +
                                Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Damage1");
                }

        if (ClassInfo.Speed != 0)
        {
            if (ClassInfo.Speed > 0)
                desc += "+ " + Mathf.Round(ClassInfo.Speed * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.MeleeSpeed");
            else
                desc += "- " + Mathf.Round(-ClassInfo.Speed * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.MeleeSpeed");
        }

        if (ClassInfo.Health != 0)
        {
            if (ClassInfo.Health > 0)
                desc += "+ " + Mathf.Round(ClassInfo.Health * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Health");
            else
                desc += "- " + Mathf.Round(-ClassInfo.Health * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Health");
        }

        if (ClassInfo.Armor != 0)
        {
            if (ClassInfo.Armor > 0)
                desc += "+ " + Mathf.Round(ClassInfo.Armor * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Armor");
            else
                desc += "- " + Mathf.Round(-ClassInfo.Armor * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Armor");
        }

        if (ClassInfo.MovementSpeed != 0)
        {
            if (ClassInfo.MovementSpeed > 0)
                desc += "+ " + Mathf.Round(ClassInfo.MovementSpeed * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.MovementSpeed");
            else
                desc += "- " + Mathf.Round(-ClassInfo.MovementSpeed * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.MovementSpeed");
        }

        if (ClassInfo.Dodge != 0)
        {
            if (ClassInfo.Dodge != 0)
                desc += "+ " + Mathf.Round(ClassInfo.Dodge * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Dodge");
            else
                desc += "- " + Mathf.Round(-ClassInfo.Dodge * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Dodge");
        }

        if (ClassInfo.Ammo != 0)
        {
            if (ClassInfo.Ammo < 0)
                desc += "+ " + Mathf.Round(-ClassInfo.Ammo * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Ammo");
            else
                desc += "- " + Mathf.Round(ClassInfo.Ammo * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Ammo");
        }

        if (ClassInfo.Summons != 0)
        {
            if (ClassInfo.Summons < 0)
                desc += "- " + Mathf.Round(-ClassInfo.Summons, 0) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Summons");
            else
                desc += "+ " + Mathf.Round(ClassInfo.Summons, 0) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Summons");
        }

        if (ClassInfo.ManaCost != 0)
        {
            if (ClassInfo.ManaCost < 0)
                desc += "- " + Mathf.Round(-ClassInfo.ManaCost * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaCost");
            else
                desc += "+ " + Mathf.Round(ClassInfo.ManaCost * 100, 2) +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaCost");
        }

        if (ClassInfo.ManaShield > 0 && (ClassInfo.ManaEfficiency > 0 || ClassInfo.ManaBaseEfficiency > 0))
        {
            var intelect = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetStat(Stat.Int);
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaShield.1") +
                    Mathf.Round(ClassInfo.ManaShield * 100, 2) +
                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaShield.2") +
                    Mathf.Round(ClassInfo.ManaBaseEfficiency + intelect * ClassInfo.ManaEfficiency, 2) +
                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.ManaShield.3");
        }

        if (classType == ClassType.AscendedShadowDancer)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.1");

        if (classType == ClassType.ShadowDancer)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.2");

        if (classType == ClassType.Assassin)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.3");

        if (classType == ClassType.Rogue)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.4");

        if (classType == ClassType.Shinobi)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.5");

        if (classType == ClassType.Ninja)
            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.throwvelocity.6");
        return desc;
    }

    public static string GetDesc(Node node)
    {
        var desc = "";

        switch (node.GetNodeType)
        {
            case NodeType.Class:
                desc += GetClassDescription((node as ClassNode).GetClassType);
                break;
            case NodeType.Damage:
                switch ((node as DamageNode).GetFlat)
                {
                    case true:

                        if (node.GetLevel > 0)
                            desc += "+ " + Mathf.Round(node.GetValue * node.GetLevel.Clamp(0, node.GetMaxLevel), 2) +
                                    " " + (node as DamageNode).GetDamageType +
                                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.DamageMultiplier");

                        if (node.GetLevel < node.GetMaxLevel)
                        {
                            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel");
                            desc += "\n+ " +
                                    Mathf.Round(node.GetValue * (node.GetLevel + 1).Clamp(0, node.GetMaxLevel), 2) +
                                    " " + (node as DamageNode).GetDamageType +
                                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.DamageMultiplier");
                        }

                        break;
                    default:
                        if (node.GetLevel > 0)
                            desc += "+ " +
                                    Mathf.Round(node.GetValue * node.GetLevel.Clamp(0, node.GetMaxLevel) * 100f, 2) +
                                    "% " + (node as DamageNode).GetDamageType +
                                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Damage2");

                        if (node.GetLevel < node.GetMaxLevel)
                        {
                            desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel");
                            desc += "\n+ " +
                                    Mathf.Round(node.GetValue * (node.GetLevel + 1).Clamp(0, node.GetMaxLevel) * 100f,
                                        2) + "% " + (node as DamageNode).GetDamageType +
                                    Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Damage2");
                        }

                        break;
                }

                break;
            case NodeType.Speed:
                if (node.GetLevel > 0)
                    desc += "+ " + Mathf.Round(node.GetValue * node.GetLevel.Clamp(0, node.GetMaxLevel) * 100f, 2) +
                            "% " + (node as SpeedNode).GetDamageType +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Speed");

                if (node.GetLevel < node.GetMaxLevel)
                {
                    desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel");
                    desc += "\n+ " +
                            Mathf.Round(node.GetValue * (node.GetLevel + 1).Clamp(0, node.GetMaxLevel) * 100f, 2) +
                            "% " + (node as SpeedNode).GetDamageType +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Speed");
                }

                break;
            case NodeType.Stats:

                if (node.GetLevel > 0)
                    desc += "+ " + Mathf.Round(node.GetValue * node.GetLevel.Clamp(0, node.GetMaxLevel), 2) + " " +
                            (node as StatNode).GetStatType + "";

                if (node.GetLevel < node.GetMaxLevel)
                {
                    desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel");
                    desc += "\n+ " + Mathf.Round(node.GetValue * (node.GetLevel + 1).Clamp(0, node.GetMaxLevel), 2) +
                            " " + (node as StatNode).GetStatType + "";
                }

                break;
            case NodeType.Leech:

                if (node.GetLevel > 0)
                    desc += "+ " + Mathf.Round(node.GetValue * node.GetLevel.Clamp(0, node.GetMaxLevel) * 100f, 2) +
                            "% " + (node as LeechNode).GetLeechType +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Leech");

                if (node.GetLevel < node.GetMaxLevel)
                {
                    desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel");
                    desc += "\n+ " +
                            Mathf.Round(node.GetValue * (node.GetLevel + 1).Clamp(0, node.GetMaxLevel) * 100f, 2) +
                            "% " + (node as LeechNode).GetLeechType +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.Leech");
                }

                break;
            case NodeType.Perk:

                if (node.GetLevel > 0)
                    desc += GetPerkDescription((node as PerkNode).GetPerk, node.GetLevel);

                if (node.GetLevel < node.GetMaxLevel)
                {
                    desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.NextLevel1");
                    desc += GetPerkDescription((node as PerkNode).GetPerk, node.GetLevel + 1);
                }

                break;
            case NodeType.Immunity:
                switch ((node as ImmunityNode).GetImmunity)
                {
                    default:
                        desc += "";
                        break;
                }

                break;
            case NodeType.LimitBreak:
                desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.LimitBreak.1");
                desc += "+ " + node.GetValue +
                        Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.LimitBreak.2");
                desc += Language.GetTextValue("Mods.AnotherRpgModExpanded.SkillInfo.LimitBreak.3");
                break;
        }

        return desc;
    }
}