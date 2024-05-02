using System;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;
// ReSharper disable UnassignedField.Global

namespace AnotherRpgModExpanded;

[Flags]
public enum GamePlayFlag
{
    NONE = 0x0,
    NPCPROGRESS = 0x1,
    XPREDUTION = 0x2,
    NPCRARITY = 0x4,
    NPCMODIFIER = 0x8,
    BOSSMODIFIER = 0x10,
    BOSSCLUSTERED = 0x20,
    RPGPlayer = 0x40,
    ITEMRARITY = 0x80,
    ITEMMODIFIER = 0x100,
    LIMITNPCGROWTH = 0x200,
    DISPLAYNPCNAME = 0x400,
    BOSSKILLINCREASELEVEL = 0x800
}

public class VisualConfig : ModConfig
{
    [DefaultValue(true)]
    public bool DisplayNpcName;

    [DefaultValue(true)]
    public bool DisplayTownName;

    [Range(0.1f, 3f)] 
    [Increment(.25f)] 
    [DefaultValue(0.75f)]
    public float HealthBarScale;

    [Range(-50f, 110f)]
    [Increment(1f)]
    [DefaultValue(0)]
    public float HealthBarYOffSet;

    [DefaultValue(false)]
    public bool HideVanillaHb;

    [Range(0.1f, 3f)]
    [Increment(.25f)]
    [DefaultValue(0.75f)]
    public float UiScale;

    // You MUST specify a MultiPlayerSyncMode.
    public override ConfigScope Mode => ConfigScope.ClientSide;


    public override void OnLoaded()
    {
        AnotherRpgModExpanded.VisualConfig = this;
    }

    public override void OnChanged()
    {
        if (Main.netMode != NetmodeID.Server)
        {
            AnotherRpgModExpanded.VisualConfig = this;

            if (AnotherRpgModExpanded.Instance != null)
            {
                AnotherRpgModExpanded.Instance.HealthBar.Reset();
                AnotherRpgModExpanded.Instance.OpenSt.Reset();
                AnotherRpgModExpanded.Instance.OpenStatMenu.Reset();
            }
        }
    }
}

public class GamePlayConfig : ModConfig
{
    [DefaultValue(true)]
    public bool AscendLimit;

    [Range(0.1F, 10F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float AscendLimitPerBoss;

    [DefaultValue(true)]
    public bool ItemModifier;
    
    [DefaultValue(true)]
    public bool ItemRarity;

    [DefaultValue(true)]
    public bool ItemTree;

    [Range(0.1F, 50F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float ItemXpMultiplier;

    [Range(0.01f, 5F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float LifeLeechCd;

    [DefaultValue(true)]
    public bool RpgPlayer;

    [DefaultValue(false)]
    public bool UseCustomSkillTree;

    [DefaultValue(false)]
    public bool VanityGiveStat;

    [Range(0.1F, 50F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float XpMultiplier;

    [DefaultValue(true)]
    public bool XpReduction;

    [DefaultValue(10)]
    public int XpReductionDelta;

    // You MUST specify a MultiPlayerSyncMode.
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        AnotherRpgModExpanded.GpConfig = this;
    }

    public override void OnChanged()
    {
        AnotherRpgModExpanded.GpConfig = this;
    }
}

public class NpcConfig : ModConfig
{
    [DefaultValue(false)]
    public bool BossClustered;

    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [DefaultValue(1f)]
    public float BossHealthMultiplier;

    [DefaultValue(false)]
    public bool BossKillLevelIncrease;

    [DefaultValue(true)]
    public bool BossModifier;

    [DefaultValue(true)]
    public bool BossRarity;

    [DefaultValue(true)]
    public bool LimitNpcGrowth;

    [Range(0f, 200f)]
    [Increment(5f)]
    [DefaultValue(20f)]
    public float LimitNpcGrowthPercent;

    [Range(0, int.MaxValue)]
    [Increment(10)]
    [DefaultValue(20)]
    public int LimitNpcGrowthValue;

    [Range(0.1F, 50F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float NpcDamageMultiplier;

    [Range(1, int.MaxValue)]
    [Increment(5)]
    [DefaultValue(50)]
    public int NpcGrowthHardMode;

    [Range(1f, 10f)]
    [Increment(0.1f)]
    [DefaultValue(1.1f)]
    public float NpcGrowthHardModePercent;

    [Range(1, int.MaxValue)]
    [Increment(5)]
    [DefaultValue(15)]
    public int NpcGrowthValue;

    [Range(0.1F, 50F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float NpcHealthMultiplier;

    [Range(0.1F, 50F)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float NpcLevelMultiplier;
    
    [DefaultValue(true)]
    public bool NpcModifier;
    
    [DefaultValue(true)]
    public bool NpcProgress;

    [Range(1, 2500)]
    [Increment(10)]
    [DefaultValue(10)]
    public int NpcProjectileDamageLevel;

    [DefaultValue(true)]
    public bool NpcRarity;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        AnotherRpgModExpanded.NpcConfig = this;
    }

    public override void OnChanged()
    {
        AnotherRpgModExpanded.NpcConfig = this;
        JsonSkillTree.Load();
        JsonCharacterClass.Load();
    }
}

public class Config
{
    public static VisualConfig VConfig => AnotherRpgModExpanded.VisualConfig;

    public static GamePlayConfig GpConfig => AnotherRpgModExpanded.GpConfig;

    public static NpcConfig NpcConfig => AnotherRpgModExpanded.NpcConfig;
}