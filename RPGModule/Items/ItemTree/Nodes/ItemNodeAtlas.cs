using System;
using System.Collections.Generic;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule.Items;

namespace AnotherRpgModExpanded.Utils;

internal class ItemNodeAtlas
{
    //Node availible for all items;
    public static List<int> CommonID = new()
    {
        500
    };

    //Node availible for armor
    public static List<int> ArmorID = new()
    {
        300
    };


    //Node avaible for weapons
    public static List<int> WeaponID = new()
    {
        0, 1, 2, 1000, 10000, 10005
    };

    //Node avaible for ranged Weapon only
    public static List<int> RangedID = new()
    {
        100
    };

    //Node avaible for melee Weapon only
    public static List<int> MeleeID = new()
    {
        50
    };

    //Node avaible for magic Weapon only
    public static List<int> MagicID = new()
    {
        150
    };

    //Node abaible for summon Weapon only
    public static List<int> SummonID = new()
    {
        0, 1, 1000, 10000, 10005
    };


    public static Dictionary<int, Type> Atlas = new()
    {
        { 0, typeof(AdditionalDamageNode) },
        { 1, typeof(AdditionalDamageNodePercent) },
        { 2, typeof(UseTimeNode) },
        { 50, typeof(LifeLeech) },


        { 100, typeof(AdditionalProjectile) },

        { 150, typeof(MagicCostReduction) },

        { 300, typeof(AdditionalDefenceNode) },


        { 500, typeof(BonusExpNode) },

        { 1000, typeof(SuperAdditionalDamageNode) },

        { 10000, typeof(AscendedAdditionalDamageNode) },
        { 10005, typeof(AscendedAdditionalDamageNodePercent) }
    };

    public static object GetCorrectNode(int AtlasID)
    {
        var Node = Activator.CreateInstance(Atlas[AtlasID]);
        return Node;
    }

    public static int GetID(string nodeType)
    {
        foreach (var entry in Atlas)
            if (entry.Value.Name == nodeType)
                return entry.Key;

        return 0;
    }

    protected static float RarityOffset(float power, float rarity)
    {
        return power * rarity / Mathf.Pow(1.5f, power * rarity);
    }

    public static int GetIDFromList(float power, List<int> IDS)
    {
        float totalWeight = 0;
        for (var i = 0; i < IDS.Count; i++)
        {
            var AID = IDS[i];
            totalWeight = RarityOffset(power, (GetCorrectNode(AID) as ItemNode).rarityWeight);
        }

        var rn = Mathf.Random(0, totalWeight);
        float checkingWeight = 0;
        for (var i = 0; i < IDS.Count; i++)
        {
            var AID = IDS[i];

            if (rn < checkingWeight + RarityOffset(power, (GetCorrectNode(AID) as ItemNode).rarityWeight))
                return AID;
            checkingWeight += RarityOffset(power, (GetCorrectNode(AID) as ItemNode).rarityWeight);
        }

        return 0;
    }

    public static List<int> GetAvailibleNodeList(ItemUpdate source, bool acceptAscent = true)
    {
        var IDS = new List<int>();
        IDS.AddRange(CommonID);

        switch (source.GetItemType)
        {
            case ItemType.Weapon:
                IDS.AddRange(WeaponID);
                switch (source.GetWeaponType)
                {
                    case WeaponType.ExtendedMelee:
                    case WeaponType.OtherMelee:
                    case WeaponType.Stab:
                    case WeaponType.Spear:
                    case WeaponType.Swing:
                        IDS.AddRange(MeleeID);
                        break;
                    case WeaponType.OtherRanged:
                    case WeaponType.Gun:
                    case WeaponType.Ranged:
                    case WeaponType.Bow:
                        IDS.AddRange(RangedID);
                        break;
                    case WeaponType.Summon:
                        IDS = new List<int>();
                        IDS.AddRange(CommonID);
                        IDS.AddRange(SummonID);
                        break;
                    case WeaponType.Magic:
                        IDS.AddRange(MagicID);
                        break;
                }

                break;
            case ItemType.Armor:
                IDS.AddRange(ArmorID);
                break;
        }

        if (!acceptAscent)
        {
            var IDR = new List<int>();
            foreach (var n in IDS)
                if (((ItemNode)GetCorrectNode(n)).IsAscend)
                    IDR.Add(n);

            foreach (var n in IDR) IDS.Remove(n);
        }

        return IDS;
    }
}