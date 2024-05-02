using System;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.UI;
using AnotherRpgModExpanded.Utils;
using MetroidMod.Common.Players;
using MetroidMod.Content.DamageClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnotherRpgModExpanded.RPGModule.Entities;

internal class RpgPlayer : ModPlayer
{
    private const ushort ActualSaveVersion = 2;
    public const float MainStatsMultiplier = 0.0025f;
    public const float SecondaryStatsMultiplier = 0.001f;
    private string _basename = "";
    private string _baseName = "";

    private float _damageToApply;
    public long EquippedItemMaxXp = 1;
    public long EquippedItemXp;
    private int _exp;
    private const float HealthRegenLeech = 0.02f;
    private bool _initiated;
    private DateTime _lastLeech = DateTime.MinValue;
    private int _level = 1;

    private float _lifeLeechDuration;
    private const float LifeLeechMaxDuration = 3;
    public float MVirtualRes;
    private float _manaRegenBuffer;
    private float _manaRegenPerSecond;
    private int _manaShieldDelay;
    public readonly float StatMultiplier = 1;

    private RPGStats _stats;

    private bool _xpLimitMessage;

    public bool Migrated { get; set; }

    public int GetSkillPoints { get; private set; } = 5;

    public SkillTree GetSkillTree { get; private set; }

    public int BaseArmor { get; private set; }

    public int FreePoints { get; private set; }

    public int TotalPoints { get; private set; }

    public float GetLifeLeechLeft => HealthRegenLeech * _lifeLeechDuration * Player.statLifeMax2;

    public void SpentSkillPoints(int value)
    {
        GetSkillPoints -= value;
        GetSkillPoints.Clamp(0, int.MaxValue);
    }

    public void SyncLevel(int level) //only use for sync
    {
        _level = level;
    }

    public void SyncStat(StatData data, Stat stat) //only use for sync
    {
        _stats.SetStats(stat, _level, data.GetLevel, data.GetXP);
    }

    public int GetStatXp(Stat s)
    {
        return _stats.GetStatXP(s);
    }

    public int GetStatXpMax(Stat s)
    {
        return _stats.GetStatXPMax(s);
    }

    public int GetStatImproved(Stat stat)
    {
        float vampireMultiplier = 1;
        float dayPerkMultiplier = 1;

        if (GetSkillTree.HavePerk(Perk.Vampire))
            switch (GetSkillTree.nodeList.GetPerk(Perk.Vampire).GetLevel)
            {
                case 1:
                    if (Main.dayTime)
                        vampireMultiplier = 0.5f;
                    else
                        vampireMultiplier = 1.25f;
                    break;
                case 2:
                    if (Main.dayTime)
                        vampireMultiplier = 0.75f;
                    else
                        vampireMultiplier = 1.5f;
                    break;
                case 3:
                    if (Main.dayTime)
                        vampireMultiplier = 0.9f;
                    else
                        vampireMultiplier = 2f;
                    break;
            }

        if (GetSkillTree.HavePerk(Perk.Chlorophyll))
            switch (GetSkillTree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel)
            {
                case 1:
                    if (Main.dayTime)
                        dayPerkMultiplier = 1.25f;
                    else
                        dayPerkMultiplier = 0.5f;
                    break;
                case 2:
                    if (Main.dayTime)
                        dayPerkMultiplier = 1.5f;
                    else
                        dayPerkMultiplier = 0.75f;
                    break;
                case 3:
                    if (Main.dayTime)
                        dayPerkMultiplier = 2f;
                    else
                        dayPerkMultiplier = 0.9f;
                    break;
            }

        return Mathf.CeilInt(GetStat(stat) * GetStatImprovementItem(stat) * vampireMultiplier * dayPerkMultiplier);
    }

    public int GetStat(Stat s)
    {
        return _stats.GetStat(s) + GetSkillTree.GetStats(s);
    }

    public int GetNaturalStat(Stat s)
    {
        return _stats.GetNaturalStat(s) + GetSkillTree.GetStats(s);
    }

    public int GetAddStat(Stat s)
    {
        return _stats.GetLevelStat(s);
    }

    public int GetLevel()
    {
        return _level;
    }

    public int GetExp()
    {
        return _exp;
    }

    private void DemonEaterPerk(NPC target, int damage)
    {
        if (GetSkillTree.HavePerk(Perk.DemonEater))
            if (damage > target.life)
            {
                var heal = (int)(0.05f * GetSkillTree.nodeList.GetPerk(Perk.DemonEater).GetLevel * Player.statLifeMax2);
                Player.statLife = (heal + Player.statLife).Clamp(Player.statLife, Player.statLifeMax2);

                CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + heal);
            }
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (GetSkillTree.HavePerk(Perk.Biologist))
        {
            var bonus = 1 + 0.2f * GetSkillTree.nodeList.GetPerk(Perk.Biologist).GetLevel;
            healValue = Mathf.RoundInt(healValue * bonus);
        }

        base.GetHealLife(item, quickHeal, ref healValue);
    }

    public override void
        OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit,
            int damageDone) /* tModPorter If you don't need the Item, consider using OnHitNPC instead */
    {
        DemonEaterPerk(target, item.damage);
        base.OnHitNPCWithItem(item, target, hit, damageDone);
    }

    public override void
        OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit,
            int damageDone) /* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
    {
        DemonEaterPerk(target, proj.damage);
        base.OnHitNPCWithProj(proj, target, hit, damageDone);
    }

    public int ManaShieldReduction(int damage)
    {
        if (GetSkillTree.ActiveClass == null || _manaShieldDelay > 0 || GetManaRatio() < 0.1f) return damage;

        var shieldInfo = GetManaShieldInfo();
        if (shieldInfo.DamageAbsorption == 0 || damage == 1)
            return damage;

        var defenseMult = 0.5f;
        if (Main.expertMode)
            defenseMult = 0.75f;

        var damageafterArmor = damage - Player.statDefense * defenseMult;

        var maxDamageAbsorbed = damageafterArmor * shieldInfo.DamageAbsorption;
        var manaCost = maxDamageAbsorbed / shieldInfo.ManaPerDamage;
        manaCost = manaCost.Clamp(0, Player.statMana);

        if (manaCost == 0)
            return damage;

        SoundEngine.PlaySound(SoundID.Drip);
        var reducedDamage = Mathf.CeilInt(manaCost * shieldInfo.ManaPerDamage);
        Player.statMana -= Mathf.CeilInt(manaCost);
        _manaShieldDelay = 120;
        Player.manaRegenDelay = 120;
        CombatText.NewText(Player.getRect(), new Color(64, 196, 255), "-" + Mathf.CeilInt(manaCost));

        return damage - reducedDamage;
    }

    private ManaShieldInfo GetManaShieldInfo()
    {
        var abs = JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).ManaShield;
        var mp = JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).ManaBaseEfficiency +
                 JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).ManaEfficiency *
                 _stats.GetStat(Stat.Int);
        return new ManaShieldInfo(abs, mp);
    }

    public float GetDefenceMult()
    {
        return (GetStatImproved(Stat.Vit) * 0.0025f + GetStatImproved(Stat.Cons) * 0.006f) * StatMultiplier + 1f;
    }

    public override void OnConsumeMana(Item item, int manaConsumed)
    {
        if (GetSkillTree.HavePerk(Perk.BloodMage))
            switch (GetSkillTree.nodeList.GetPerk(Perk.BloodMage).GetLevel)
            {
                case 1:
                    Player.statLife = (Player.statLife - manaConsumed).Clamp(1, Player.statLifeMax2);
                    break;
                case 2:
                    Player.statLife = (Player.statLife - manaConsumed * 3).Clamp(1, Player.statLifeMax2);
                    break;
                case 3:
                    Player.statLife = (Player.statLife - manaConsumed * 9).Clamp(1, Player.statLifeMax2);
                    break;
            }

        base.OnConsumeMana(item, manaConsumed);
    }

    private bool IsSaved()
    {
        foreach (var item in Player.armor)
        {
            if (item.netID > 0)
            {
                var instance = item.GetGlobalItem<ItemUpdate>();
                if (instance != null)
                    if (ModifierManager.HaveModifier(Modifier.Savior, instance.Modifier))
                        if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Savior, instance))
                        {
                            CombatText.NewText(Player.getRect(), new Color(64, 196, 255), "SAVED !", true, true);
                            return true;
                        }
            }
        }

        return false;
    }

    private float GetStatImprovementItem(Stat s)
    {
        float value = 1;
        if (!Config.GpConfig.ItemRarity)
            return value;
        var maxSlot = Player.armor.Length;
        if (!Config.GpConfig.VanityGiveStat) maxSlot = 9;

        for (var i = 0; i < maxSlot; i++)
        {
            var item = Player.armor[i];

            if (item.netID > 0)
            {
                var instance = item.GetGlobalItem<ItemUpdate>();
                if (instance != null)
                    value += instance.GetStat(s) * 0.01f;
            }
        }

        return value;
    }

    public float GetHealthPerHeart()
    {
        return (GetStatImproved(Stat.Vit) * 0.65f + GetStatImproved(Stat.Cons) * 0.325f) * StatMultiplier + 10;
    }

    public float GetManaPerStar()
    {
        return (GetStatImproved(Stat.Foc) * 0.2f + GetStatImproved(Stat.Int) * 0.05f) * StatMultiplier + 10;
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (_basename == "")
            _basename = Player.name;
        SendClientChanges(this);
        if (WorldManager.Instance != null)
            WorldManager.Instance.NetUpdateWorld();
        base.SyncPlayer(toWho, fromWho, newPlayer);
    }

    public override void CopyClientState(ModPlayer clientClone)
    {
        base.CopyClientState(clientClone);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        if (clientPlayer == null || _level <= 0)
            return;
        var classname = "Hobo";

        if (_basename == "")
            _basename = Player.name;

        if (GetSkillTree != null && GetSkillTree.ActiveClass != null)
            classname = "" + GetSkillTree.ActiveClass.GetClassType;
        var name = _basename + " the lvl." + _level + " " + classname;

        if (Config.GpConfig.RpgPlayer)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)Message.SyncLevel);
            packet.Write((byte)Player.whoAmI);
            packet.Write(_level);
            packet.Write(name);
            packet.Write(Player.statLife);
            packet.Write(Player.statLifeMax2);
            packet.Send();
        }

        base.SendClientChanges(clientPlayer);
    }

    public override void PreUpdateBuffs()
    {
        if (_basename == "")
            _basename = Player.name;
        if (Config.GpConfig.RpgPlayer)
        {
            if (!_initiated)
            {
                _initiated = true;

                GetSkillTree.Init();
            }

            if (Player.HasBuff(24) || Player.HasBuff(20) || Player.HasBuff(67))
                _damageToApply +=
                    (Mathf.Logx(Player.statLifeMax2, 1.002f) * 0.05f * NPCUtils.DELTATIME).Clamp(0,
                        Player.statLifeMax2 * 0.015f * NPCUtils.DELTATIME);

            if (Player.HasBuff(70) || Player.HasBuff(39) || Player.HasBuff(68))
                _damageToApply +=
                    (Mathf.Logx(Player.statLifeMax2, 1.002f) * 0.08f * NPCUtils.DELTATIME).Clamp(0,
                        Player.statLifeMax2 * 0.025f * NPCUtils.DELTATIME);

            if (Player.onFire2 || Player.HasBuff(38) || Player.breath <= 0)
                _damageToApply +=
                    (Mathf.Logx(Player.statLifeMax2, 1.002f) * 0.1f * NPCUtils.DELTATIME).Clamp(0,
                        Player.statLifeMax2 * 0.04f * NPCUtils.DELTATIME);

            if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Dbzmod])
                IncreaseDbzKi(Player);

            if (_damageToApply > 1)
            {
                var buffDamage = Mathf.FloorInt(_damageToApply);
                _damageToApply -= _damageToApply;
                ApplyReduction(ref buffDamage);
                Player.statLife -= buffDamage;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Player.GetCritChance(DamageClass.Melee) =
                    (int)(Player.GetCritChance(DamageClass.Melee) + GetCriticalChanceBonus());
                Player.GetCritChance(DamageClass.Throwing) =
                    (int)(Player.GetCritChance(DamageClass.Throwing) + GetCriticalChanceBonus());
                Player.GetCritChance(DamageClass.Magic) =
                    (int)(Player.GetCritChance(DamageClass.Magic) + GetCriticalChanceBonus());
                Player.GetCritChance(DamageClass.Ranged) =
                    (int)(Player.GetCritChance(DamageClass.Ranged) + GetCriticalChanceBonus());

                Player.maxMinions += GetSkillTree.GetSummonSlot();

                if (!Config.GpConfig.ItemTree)
                    Player.maxMinions += GetMaxMinion();

                Player.lifeRegen = Mathf.FloorInt(GetHealthRegen());
                Player.manaRegen = 0;

                //MODED DAMAGE
                if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Thorium])
                    UpdateThoriumDamage(Player);

                if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Dbzmod])
                    UpdateDbzDamage(Player);

                if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Metroid])
                    UpdateHunterDamage(Player);
            }
        }

        if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
        {
            var Item = Player.HeldItem.GetGlobalItem<ItemUpdate>();

            if (Item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
            {
                EquippedItemXp = Item.GetXp;
                EquippedItemMaxXp = Item.GetMaxXp;
            }
        }
    }

    private void IncreaseDbzKi(Player player)
    {
        var DBZ = ModLoader.GetMod("DBZMOD");
        //Player.GetModPlayer<DBZMOD.MyPlayer>().kiMaxMult *= Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 10)),1,10);
        //Player.GetModPlayer<DBZMOD.MyPlayer>().kiChargeRate += Mathf.FloorInt( Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 6)), 0, 25));
    }

    private void UpdateDbzDamage(Player player)
    {
        var DBZ = ModLoader.GetMod("DBZMOD");
        //Player.GetModPlayer<DBZMOD.MyPlayer>().kiDamage *= GetDamageMult(DamageType.KI);
        //Player.GetModPlayer<DBZMOD.MyPlayer>().kiCrit += (int)GetCriticalChanceBonus();
    }

    [JITWhenModsEnabled("ThoriumMod")]
    private void UpdateThoriumDamage(Player player)
    {
        /*
        Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicDamage *= GetDamageMult(DamageType.Symphonic);
        Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantBoost *= GetDamageMult(DamageType.Radiant);

        Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantCrit += (int)GetCriticalChanceBonus();
        Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicCrit += (int)GetCriticalChanceBonus();
        */
    }

    [JITWhenModsEnabled("MetroidMod")]
    private void UpdateHunterDamage(Player player)
    {
        player.GetModPlayer<HunterDamagePlayer>().HunterDamageMult = GetDamageMultiplier(DamageType.Ranged, 2);
        player.GetModPlayer<HunterDamagePlayer>().HunterCrit += (int)GetCriticalChanceBonus();
    }

    public void ApplyReduction(ref int damage, bool heal = false)
    {
        if (MVirtualRes > 0)
            CombatText.NewText(Player.getRect(), new Color(50, 26, 255, 1), "(" + damage + ")");
        damage = (int)(damage * (1 - MVirtualRes));
    }

    public void ApplyReduction(ref float damage, bool heal = false)
    {
        if (MVirtualRes > 0)
            CombatText.NewText(Player.getRect(), new Color(50, 26, 255, 1), "(" + damage + ")");
        damage = damage * (1 - MVirtualRes);
    }

    public float GetArmorPenetrationMult()
    {
        return 1f + _stats.GetStat(Stat.Dex) * 0.01f;
    }

    public int GetArmorPenetrationAdd()
    {
        return Mathf.FloorInt(_stats.GetStat(Stat.Dex) * 0.1f);
    }

    public override void PostUpdateEquips()
    {
        if (Config.GpConfig.RpgPlayer)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                MVirtualRes = 0;
                BaseArmor = Player.statDefense;

                Player.statLifeMax2 =
                    ((int)(GetHealthMultiplier() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10).Clamp(10,
                        int.MaxValue);
                /*
            if (Main.netMode == 1)
            {
                Player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10,10,Int16.MaxValue);
                if (Player.statLifeMax2>= Int16.MaxValue)
                {
                    float HealthVirtualMult = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue) / Player.statLifeMax2;
                    m_virtualRes = 1-(1f / HealthVirtualMult);
                }

            }
            else
            {
                Player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue);
            }
            */
                Player.statManaMax2 = (int)(Player.statManaMax2 * GetManaPerStar() / 20) + 10;
                Player.statDefense *= GetDefenceMult() * GetArmorMultiplier();
                Player.GetDamage(DamageClass.Melee) *= GetDamageMultiplier(DamageType.Melee, 2);
                Player.GetDamage(DamageClass.Throwing) *= GetDamageMultiplier(DamageType.Throw, 2);
                Player.GetDamage(DamageClass.Ranged) *= GetDamageMultiplier(DamageType.Ranged, 2);
                Player.GetDamage(DamageClass.Magic) *= GetDamageMultiplier(DamageType.Magic, 2);
                Player.GetDamage(DamageClass.Summon) *= GetDamageMultiplier(DamageType.Summon, 2);

                //if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Metroid])
                //{
                //    UpdateHunterDamage(Player);
                //}

                /*
                Player.HeldItem.ArmorPenetration = Mathf.FloorInt( Player.GetArmorPenetration<GenericDamageClass>()* GetArmorPenetrationMult());
                Player.armorPenetration += GetArmorPenetrationAdd();
                */
                _manaShieldDelay = (_manaShieldDelay - 1).Clamp(0, _manaShieldDelay);
                Player.manaCost *= (float)Math.Sqrt(GetDamageMultiplier(DamageType.Magic, 2));

                _manaRegenPerSecond = Mathf.FloorInt(GetManaRegen());
                var manaRegenTick = _manaRegenPerSecond / 60 + _manaRegenBuffer;
                var manaRegenThisTick = Mathf.FloorInt(manaRegenTick).Clamp(0, int.MaxValue);
                _manaRegenBuffer = (manaRegenTick - manaRegenThisTick).Clamp(0, int.MaxValue);
                Player.statMana = (Player.statMana + manaRegenThisTick).Clamp(Player.statMana, Player.statManaMax2);

                if (GetSkillTree.HavePerk(Perk.BloodMage))
                    switch (GetSkillTree.nodeList.GetPerk(Perk.BloodMage).GetLevel)
                    {
                        case 1:
                            Player.manaCost *= 0.5f;
                            break;
                        case 2:
                            Player.manaCost *= 0.25f;
                            break;
                        case 3:
                            Player.manaCost *= 0.1f;
                            break;
                    }

                if (GetSkillTree.ActiveClass != null)
                {
                    Player.accRunSpeed *= 1 + JsonCharacterClass.GetJsonCharList
                        .GetClass(GetSkillTree.ActiveClass.GetClassType).MovementSpeed;
                    Player.moveSpeed *= 1 + JsonCharacterClass.GetJsonCharList
                        .GetClass(GetSkillTree.ActiveClass.GetClassType).MovementSpeed;
                    Player.maxRunSpeed *= 1 + JsonCharacterClass.GetJsonCharList
                        .GetClass(GetSkillTree.ActiveClass.GetClassType).MovementSpeed;
                    Player.GetAttackSpeed<MeleeDamageClass>() *= 1 + JsonCharacterClass.GetJsonCharList
                        .GetClass(GetSkillTree.ActiveClass.GetClassType).Speed;
                    Player.manaCost *= 1 + JsonCharacterClass.GetJsonCharList
                        .GetClass(GetSkillTree.ActiveClass.GetClassType).ManaCost;
                }
            }

            PostUpdatePerk();
        }

        UpdateModifier();
        //Issue : After one use of item , Player can no longer do anything wiht item&inventory
        //ErrorLogger.Log(Player.can);
        CustomPostUpdates();

        if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
            MPPacketHandler.SendPlayerHealthSync(Mod, Player.whoAmI);
    }

    private void PostUpdatePerk()
    {
        if (GetSkillTree.HavePerk(Perk.Masochist))
        {
            int def = Player.statDefense;
            Player.statDefense *= 0;
            Player.GetDamage(DamageClass.Generic) *=
                1 + def * 0.01f * GetSkillTree.nodeList.GetPerk(Perk.Masochist).GetLevel;
        }

        if (GetSkillTree.HavePerk(Perk.Berserk))
        {
            var PerkMultiplier =
                1 + 0.01f * GetSkillTree.nodeList.GetPerk(Perk.Berserk).GetLevel * (1 - GetHealthRatio());
            Player.GetDamage(DamageClass.Generic) *= PerkMultiplier;
        }

        if (GetSkillTree.HavePerk(Perk.Survivalist))
        {
            Player.GetDamage(DamageClass.Generic) *= 0.5f;

            Player.statDefense *= 1 + .2f * GetSkillTree.nodeList.GetPerk(Perk.Survivalist).GetLevel;
        }

        if (GetSkillTree.HavePerk(Perk.ManaOverBurst))
        {
            var bonusmanacost = Player.statMana *
                                (0.1f + ((float)GetSkillTree.nodeList.GetPerk(Perk.ManaOverBurst).GetLevel - 1) *
                                    0.15f);
            var multiplier = 1 + (1 - 1 / (bonusmanacost / 1000 + 1));
            Player.GetDamage(DamageClass.Generic) *= multiplier;
        }
    }

    public void SpendPoints(Stat _stat, int ammount)
    {
        ammount = ammount.Clamp(1, FreePoints);
        _stats.UpgradeStat(_stat, ammount);
        FreePoints -= ammount;
    }

    public void ResetStats()
    {
        _stats.Reset(_level);
        FreePoints = TotalPoints;
    }

    public float GetCriticalChanceBonus()
    {
        var X =
            Mathf.Pow(GetStatImproved(Stat.Foc) * StatMultiplier + GetStatImproved(Stat.Dex) * StatMultiplier, 0.8f) *
            0.05f;
        X.Clamp(0, 75);
        return X;
    }

    public float GetCriticalDamage()
    {
        var X =
            Mathf.Pow(GetStatImproved(Stat.Agi) * StatMultiplier + GetStatImproved(Stat.Str) * StatMultiplier, 0.8f) *
            0.005f;
        return 1.4f + X;
    }

    public override void UpdateAutopause()
    {
        //Main.npcChatRelease;
        if (!Main.drawingPlayerChat)
            HandleTrigger();

        base.UpdateAutopause();
    }

    private void HandleTrigger()
    {
        if (Config.GpConfig.RpgPlayer)
        {
            if (AnotherRpgModExpanded.StatsHotKey.JustPressed)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);

                UI.Stats.Instance.LoadChar();
                UI.Stats.Visible = !UI.Stats.Visible;
            }

            if (AnotherRpgModExpanded.SkillTreeHotKey.JustPressed)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);

                SkillTreeUi.visible = !SkillTreeUi.visible;
                if (SkillTreeUi.visible)
                    SkillTreeUi.Instance.LoadSkillTree();
            }
        }

        if (AnotherRpgModExpanded.ItemTreeHotKey.JustPressed && Config.GpConfig.ItemTree)
        {
            if (ItemUpdate.NeedSavingStatic(Player.HeldItem))
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ItemTreeUi.visible = !ItemTreeUi.visible;
                if (ItemTreeUi.visible)
                {
                    if (ItemUpdate.HaveTree(Player.HeldItem))
                        ItemTreeUi.Instance.Open(Player.HeldItem.GetGlobalItem<ItemUpdate>());
                    else
                        ItemTreeUi.visible = false;
                }
            }
            else if (ItemTreeUi.visible)
            {
                ItemTreeUi.visible = false;
            }
        }

        if (AnotherRpgModExpanded.HelmetItemTreeHotKey.JustPressed && Config.GpConfig.ItemTree)
        {
            if (ItemUpdate.NeedSavingStatic(Player.armor[0]))
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ItemTreeUi.visible = !ItemTreeUi.visible;
                if (ItemTreeUi.visible)
                {
                    if (ItemUpdate.HaveTree(Player.armor[0]))
                        ItemTreeUi.Instance.Open(Player.armor[0].GetGlobalItem<ItemUpdate>());
                    else
                        ItemTreeUi.visible = false;
                }
            }
            else if (ItemTreeUi.visible)
            {
                ItemTreeUi.visible = false;
            }
        }

        if (AnotherRpgModExpanded.ChestItemTreeHotKey.JustPressed && Config.GpConfig.ItemTree)
        {
            if (ItemUpdate.NeedSavingStatic(Player.armor[1]))
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ItemTreeUi.visible = !ItemTreeUi.visible;
                if (ItemTreeUi.visible)
                {
                    if (ItemUpdate.HaveTree(Player.armor[1]))
                        ItemTreeUi.Instance.Open(Player.armor[1].GetGlobalItem<ItemUpdate>());
                    else
                        ItemTreeUi.visible = false;
                }
            }
            else if (ItemTreeUi.visible)
            {
                ItemTreeUi.visible = false;
            }
        }

        if (AnotherRpgModExpanded.LegsItemTreeHotKey.JustPressed && Config.GpConfig.ItemTree)
        {
            if (ItemUpdate.NeedSavingStatic(Player.armor[2]))
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ItemTreeUi.visible = !ItemTreeUi.visible;
                if (ItemTreeUi.visible)
                {
                    if (ItemUpdate.HaveTree(Player.armor[2]))
                        ItemTreeUi.Instance.Open(Player.armor[2].GetGlobalItem<ItemUpdate>());
                    else
                        ItemTreeUi.visible = false;
                }
            }
            else if (ItemTreeUi.visible)
            {
                ItemTreeUi.visible = false;
            }
        }
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        HandleTrigger();
    }

    public float GetBonusHeal()
    {
        if (Config.GpConfig.RpgPlayer)
            return GetHealthPerHeart() / 20;
        return 1;
    }

    public float GetBonusHealMana()
    {
        if (Config.GpConfig.RpgPlayer)
            return (float)Math.Sqrt(GetManaPerStar() / 20);
        return 1;
    }

    public float GetHealthRegen()
    {
        var regenMultiplier = 1f;
        if (GetSkillTree.HavePerk(Perk.Chlorophyll))
            regenMultiplier = 0.25f + 0.25f * GetSkillTree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel;

        return (GetStatImproved(Stat.Vit) + GetStatImproved(Stat.Cons)) * 0.02f * StatMultiplier * regenMultiplier;
    }

    public float GetManaRegen()
    {
        var regenMultiplier = 1f;
        if (GetSkillTree.HavePerk(Perk.Chlorophyll))
            regenMultiplier = 0.25f + 0.25f * GetSkillTree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel;

        if (Player.manaRegenDelay > 0)
            regenMultiplier *= 0.5f;

        return (GetStatImproved(Stat.Int) + GetStatImproved(Stat.Spr)) * 0.02f * StatMultiplier * regenMultiplier;
    }

    public bool HaveRangedWeapon()
    {
        if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
        {
            var isHunterDamage = AnotherRpgModExpanded.LoadedMods[SupportedMod.Metroid] &&
                                 HaveHunterWeapon(Player.HeldItem.DamageType);

            return isHunterDamage || Player.HeldItem.DamageType == DamageClass.Ranged;
        }

        return false;
    }

    [JITWhenModsEnabled("MetroidMod")]
    public static bool HaveHunterWeapon(DamageClass damageClass)
    {
        return damageClass.GetType() == typeof(HunterDamageClass);
    }

    public bool HaveBow()
    {
        if (HaveRangedWeapon())
            if (Player.HeldItem.useAmmo == 40)
                return true;
        return false;
    }

    public float GetHealthMultiplier()
    {
        float mult = 1;
        if (GetSkillTree.ActiveClass != null)
            mult += JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).Health;
        return mult;
    }

    public float GetArmorMultiplier()
    {
        float mult = 1;
        if (GetSkillTree.ActiveClass != null)
            mult += JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).Armor;
        return mult;
    }

    public float GetDamageMultiplier(DamageType type, int skill = 0)
    {
        if (skill == 0)
            switch (type)
            {
                case DamageType.Magic:
                    return (GetStatImproved(Stat.Int) * MainStatsMultiplier + GetStatImproved(Stat.Spr) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                case DamageType.Ranged:
                    return (GetStatImproved(Stat.Agi) * MainStatsMultiplier + GetStatImproved(Stat.Dex) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                case DamageType.Summon:
                    return (GetStatImproved(Stat.Spr) * MainStatsMultiplier + GetStatImproved(Stat.Foc) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                case DamageType.Throw:
                    return (GetStatImproved(Stat.Dex) * MainStatsMultiplier + GetStatImproved(Stat.Str) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                case DamageType.Symphonic:
                    return (GetStatImproved(Stat.Agi) * SecondaryStatsMultiplier +
                            GetStatImproved(Stat.Foc) * SecondaryStatsMultiplier) * StatMultiplier + 0.8f;
                case DamageType.Radiant:
                    return (GetStatImproved(Stat.Int) * SecondaryStatsMultiplier +
                            GetStatImproved(Stat.Spr) * SecondaryStatsMultiplier) * StatMultiplier + 0.8f;
                case DamageType.Ki:
                    return (GetStatImproved(Stat.Spr) * MainStatsMultiplier + GetStatImproved(Stat.Str) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                case DamageType.Hunter:
                    return (GetStatImproved(Stat.Agi)  * 0.5f * MainStatsMultiplier + GetStatImproved(Stat.Dex) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
                default:
                    return (GetStatImproved(Stat.Str) * MainStatsMultiplier + GetStatImproved(Stat.Agi) * SecondaryStatsMultiplier) *
                        StatMultiplier + 0.8f;
            }

        if (skill == 2)
            return GetSkillTree.GetDamageMult(type) * StatMultiplier * GetDamageMultiplier(type);
        return GetSkillTree.GetDamageMult(type);
    }

    public void CheckExp()
    {
        var actualLevelGained = 0;
        while (_exp >= XpToNextLevel())
        {
            actualLevelGained++;
            _exp -= XpToNextLevel();
            LevelUp();
            if (actualLevelGained > 5) _exp = 0;
        }
    }

    private int ReduceExp(int xp, int level)
    {
        var exp = xp;

        if (level <= this._level - Config.GpConfig.XpReductionDelta)
        {
            var expMult = 1 - (this._level - level) * 0.1f;
            exp = (int)(exp * expMult);
        }

        if (exp < 1)
            exp = 1;

        return exp;
    }

    public void AddXp(int exp, int level)
    {
        if (Config.GpConfig.RpgPlayer)
        {
            exp = (int)(exp * GetXpMult());

            if (Config.GpConfig.XpReduction) exp = ReduceExp(exp, level);

            if (this._level >= 1000 && !GetSkillTree.IsLimitBreak())
            {
                if (!_xpLimitMessage)
                {
                    _xpLimitMessage = true;
                    Main.NewText(
                        "You character has Reached the limit of his mortal body and is unable to gain more power !");
                }

                exp = 0;
            }

            if (exp >= XpToNextLevel() * 0.1f)
                CombatText.NewText(Player.getRect(), new Color(50, 26, 255), exp + " XP !!");
            else
                CombatText.NewText(Player.getRect(), new Color(127, 159, 255), exp + " XP");

            _exp += exp;
            CheckExp();
        }
    }

    public void CommandLevelup()
    {
        LevelUp(true);
    }

    public void ResetLevel()
    {
        ResetSkillTree();
        TotalPoints = 0;
        FreePoints = 0;
        GetSkillPoints = 0;
        _level = 1;
        _stats.Reset(1);
    }

    public void RecalculateStat()
    {
        var level = this._level;
        this._level = 0;
        TotalPoints = 0;
        FreePoints = 0;
        _stats = new RPGStats();
        for (var i = 0; i < level; i++) LevelUp(true);
    }

    private float GetLifeLeech(int damage)
    {
        float value = 0;
        if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
        {
            var Item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            if (Item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
            {
                if (Config.GpConfig.ItemTree)
                    value = Item.Leech;
                else
                    value = Item.GetLifeLeech * 0.01f;
            }
        }

        if (Config.GpConfig.RpgPlayer)
        {
            if (GetSkillTree.HavePerk(Perk.Vampire) && !Main.dayTime)
                switch (GetSkillTree.nodeList.GetPerk(Perk.Vampire).GetLevel)
                {
                    case 1:
                        value += 0.005f;
                        break;
                    case 2:
                        value += 0.0075f;
                        break;
                    case 3:
                        value += 0.01f;
                        break;
                }

            value += GetSkillTree.GetLeech(LeechType.Life) + GetSkillTree.GetLeech(LeechType.Both);
        }

        value *= Player.statLifeMax2;
        return value;
    }

    private float GetManaLeech()
    {
        float value = 0;
        if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
        {
            var item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            if (item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
                if (!Config.GpConfig.ItemTree)
                    value = item.GetManaLeech * 0.01f;
        }

        if (Config.GpConfig.RpgPlayer)
            value += GetSkillTree.GetLeech(LeechType.Magic) + GetSkillTree.GetLeech(LeechType.Both);
        return value;
    }

    private void LevelUpMessage(int pointsToGain)
    {
        CombatText.NewText(Player.getRect(), new Color(255, 25, 100),
            Language.GetTextValue("Mods.AnotherRpgModExpanded.RPGPlayer.LEVELUP"));
        CombatText.NewText(Player.getRect(), new Color(255, 125, 255),
            Language.GetTextValue("Mods.AnotherRpgModExpanded.RPGPlayer.SKILLPOINTS"), true);
        CombatText.NewText(Player.getRect(), new Color(150, 100, 200),
            "+" + pointsToGain + Language.GetTextValue("Mods.AnotherRpgModExpanded.RPGPlayer.Abilitypoints"), true);
        Main.NewText(
            Player.name + Language.GetTextValue("Mods.AnotherRpgModExpanded.RPGPlayer.Isnowlevel") + _level +
            " .Congratulation !", 255, 223, 63);
    }

    private void LevelUp(bool silent = false)
    {
        var pointsToGain = 5 + Mathf.FloorInt(Mathf.Pow(_level, 0.5f));
        TotalPoints += pointsToGain;
        FreePoints += pointsToGain;
        GetSkillPoints++;
        _stats.OnLevelUp();
        _level++;
        if (!silent)
            LevelUpMessage(pointsToGain);

        if (Main.netMode == NetmodeID.MultiplayerClient)
            SendClientChanges(this);
        else
            WorldManager.PlayerLevel = _level;
    }

    public int XpToNextLevel()
    {
        return 15 * _level + 5 * Mathf.CeilInt(Mathf.Pow(_level, 1.8f)) + 40;
    }

    public int[] ConvertStatToInt()
    {
        var convertedStats = new int[8];
        for (var i = 0; i < 8; i++) convertedStats[i] = GetAddStat((Stat)i);
        return convertedStats;
    }

    public int[] ConvertStatXpToInt()
    {
        var convertedStats = new int[8];
        for (var i = 0; i < 8; i++) convertedStats[i] = GetStatXp((Stat)i);
        return convertedStats;
    }

    private void LoadStats(int[] level, int[] xp)
    {
        if (xp.Length != 8) //if save is not correct , will try to port
        {
            RecalculateStat();
            if (level.Length != 8) //if port don't work
                return;
            for (var i = 0; i < 8; i++) _stats.UpgradeStat((Stat)i, level[i]);
        }
        else
        {
            for (var i = 0; i < 8; i++) _stats.SetStats((Stat)i, this._level + 3, level[i], xp[i]);
        }
    }

    public int[] SaveSkills()
    {
        var skillLevels = new int[GetSkillTree.nodeList.nodeList.Count];

        for (var i = 0; i < skillLevels.Length; i++) skillLevels[i] = GetSkillTree.nodeList.nodeList[i].GetLevel;

        return skillLevels;
    }

    public void ResetSkillTree()
    {
        GetSkillTree = new SkillTree();
        GetSkillPoints = _level - 1;

        SkillTreeUi.Instance.LoadSkillTree();
    }

    private void LoadSkills(int[] skillLevel)
    {
        if (GetSkillTree.nodeList.nodeList.Count < skillLevel.Length)
        {
            AnotherRpgModExpanded.Instance.Logger.Warn("Saved skill tree and Actual skill tree are of diferent size");
            ResetSkillTree();
        }

        for (var i = 0; i < skillLevel.Length; i++)
            if (skillLevel[i] > 0)
            {
                var canUp = GetSkillTree.nodeList.nodeList[i]
                    .CanUpgrade(skillLevel[i] * GetSkillTree.nodeList.nodeList[i].GetCostPerLevel, _level);
                if (!(canUp == Reason.LevelRequirement || canUp == Reason.MaxLevelReach))
                {
                    for (var U = 0; U < skillLevel[i]; U++)
                        if (GetSkillPoints - GetSkillTree.nodeList.nodeList[i].GetCostPerLevel >= 0)
                        {
                            GetSkillPoints -= GetSkillTree.nodeList.nodeList[i].GetCostPerLevel;
                            if (GetSkillTree.nodeList.nodeList[i].GetNodeType != NodeType.Class)
                                GetSkillTree.nodeList.nodeList[i].Upgrade();
                            else
                                GetSkillTree.nodeList.nodeList[i].Upgrade(true);
                        }
                }
                else
                {
                    AnotherRpgModExpanded.Instance.Logger.Warn("Can't level up node at rank : " + i);
                    //ResetSkillTree();
                    return;
                }
            }
    }

    private int GetActiveClassId()
    {
        if (Config.GpConfig.RpgPlayer)
            return -1;

        if (GetSkillTree.ActiveClass == null) return -1;

        if (GetSkillTree.ActiveClass.GetParent == null) return -1;
        return GetSkillTree.ActiveClass.GetParent.ID;
    }

    public override void SaveData(TagCompound tag)
    {
        //AnotherRpgModExpanded.Instance.Logger.Info("Is it saving Player Data ? ....");
        base.SaveData(tag);
        if (_stats == null) _stats = new RPGStats();

        if (GetSkillTree == null)
        {
            AnotherRpgModExpanded.Instance.Logger.Warn("save skillTree reset");
            GetSkillTree = new SkillTree();
        }

        AnotherRpgModExpanded.Instance.Logger.Info("Saving this skilltree");
        AnotherRpgModExpanded.Instance.Logger.Info(SkillTree.SKILLTREEVERSION);
        tag.Add("Exp", _exp);
        tag.Add("level", _level);
        tag.Add("Stats", ConvertStatToInt());
        tag.Add("StatsXP", ConvertStatXpToInt());
        tag.Add("totalPoints", TotalPoints);
        tag.Add("freePoints", FreePoints);
        tag.Add("skillLevel", SaveSkills());
        tag.Add("activeClass", GetActiveClassId());
        tag.Add("AnRPGSkillVersion", SkillTree.SKILLTREEVERSION);
        tag.Add("AnRPGSaveVersion", ActualSaveVersion);
        tag.Add("Migrated", Migrated);
    }

    public override void Initialize()
    {
        if (_stats == null)
            _stats = new RPGStats();
        NodeParent.ResetID();
        if (GetSkillTree == null) GetSkillTree = new SkillTree();
    }

    public override void LoadData(TagCompound tag)
    {
        //base.LoadData(tag);
        //AnotherRpgModExpanded.Instance.Logger.Info(tag);
        //AnotherRpgModExpanded.Instance.Logger.Info("Load Player Data");
        NodeParent.ResetID();
        _damageToApply = 0;
        _exp = tag.GetInt("Exp");
        _level = tag.GetInt("level");

        LoadStats(tag.GetIntArray("Stats"), tag.GetIntArray("StatsXP"));
        TotalPoints = tag.GetInt("totalPoints");

        FreePoints = tag.GetInt("freePoints");
        GetSkillPoints = _level - 1;

        Migrated = tag.GetBool("Migrated");

        if (tag.GetInt("AnRPGSkillVersion") != SkillTree.SKILLTREEVERSION)
        {
            AnotherRpgModExpanded.Instance.Logger.Warn(tag.GetInt("AnRPGSkillVersion"));
            AnotherRpgModExpanded.Instance.Logger.Warn(SkillTree.SKILLTREEVERSION);
            AnotherRpgModExpanded.Instance.Logger.Warn("AnRPG SkillTree Is Outdated, resetting skillTree");
        }
        else
        {
            LoadSkills(tag.GetIntArray("skillLevel"));
            if (tag.GetInt("activeClass") < GetSkillTree.nodeList.nodeList.Count && tag.GetInt("activeClass") > 0)
                GetSkillTree.ActiveClass = (ClassNode)GetSkillTree.nodeList.nodeList[tag.GetInt("activeClass")].GetNode;
        }

        if (GetSkillTree.ActiveClass == null)
            GetSkillTree.ActiveClass = (ClassNode)GetSkillTree.nodeList.nodeList[0].GetNode;

        foreach (var node in GetSkillTree.nodeList.nodeList)
        {
            if (node.GetNodeType == NodeType.Class)
            {
                var classNode = (ClassNode)node.GetNode;
                if (classNode.GetClassType != GetSkillTree.ActiveClass.GetClassType && classNode.GetActivate)
                    classNode.Disable(this);
            }
        }

        NodeParent.ResetID();
        _initiated = true;
    }

    public void MigrateData(RpgPlayer player)
    {
        _exp = player._exp;
        _level = player._level;
        _stats = player._stats;
        TotalPoints = player.TotalPoints;
        FreePoints = player.FreePoints;
        GetSkillPoints = player.GetSkillPoints;
        ResetSkillTree();
        
        Migrated = true;
    }

    private struct ManaShieldInfo(float absorption, float manaPerDamage)
    {
        public readonly float DamageAbsorption = absorption;
        public readonly float ManaPerDamage = manaPerDamage;
    }

    #region WEAPONXP

    public void AddWeaponXp(int damage, Item xItem, float multiplier = 1)
    {
        if (damage < 0)
            damage = -damage;

        if (xItem != null && xItem.damage > 0 && xItem.maxStack <= 1)
        {
            var item = xItem.GetGlobalItem<ItemUpdate>();

            if (item != null && ItemUpdate.NeedSavingStatic(xItem))
                item.AddExp(Mathf.Ceillong(damage * multiplier), Player, xItem);
        }
    }

    public void HealSlow(int lifeHeal)
    {
        Player.GetModPlayer<RpgPlayer>().ApplyReduction(ref lifeHeal);

        if (lifeHeal > 0)
        {
            var duration = lifeHeal / (Player.statLifeMax2 * HealthRegenLeech);
            float buffer = 0;
            float totalHeal = 0;

            if (_lifeLeechDuration < 1)
            {
                buffer = _lifeLeechDuration;
                _lifeLeechDuration = (_lifeLeechDuration + duration).Clamp(_lifeLeechDuration, 1);
                buffer = _lifeLeechDuration - buffer;
                totalHeal = lifeHeal * buffer;
                duration -= buffer;
            }

            if (duration > 0)
            {
                duration *= 0.2f;

                buffer = _lifeLeechDuration;

                _lifeLeechDuration = (_lifeLeechDuration + duration).Clamp(_lifeLeechDuration, LifeLeechMaxDuration);
                buffer = _lifeLeechDuration - buffer;
                totalHeal += lifeHeal * buffer;
            }

            if (totalHeal < 1)
                totalHeal = 1;
            CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + (int)totalHeal);
        }
    }

    public void Leech(int damage)
    {
        var cd = TimeSpan.FromSeconds(Config.GpConfig.LifeLeechCd);

        if (_lastLeech + cd < DateTime.Now)
        {
            _lastLeech = DateTime.Now;
            HealSlow(Mathf.CeilInt(GetLifeLeech(damage)));
        }

        var manaHeal = (int)(Player.statManaMax2 * GetManaLeech());

        if (manaHeal > 0) Player.statMana = (Player.statMana + manaHeal).Clamp(0, Player.statManaMax2);
    }

    private float _bufferLife;

    private void CustomPostUpdates()
    {
        if (_lifeLeechDuration > 0 && Player.statLife < Player.statLifeMax2)
        {
            _lifeLeechDuration -= 1f / 60f;

            var newLife = _bufferLife + Player.statLife + Player.statLifeMax2 * HealthRegenLeech / 60;
            var lifeGain = Mathf.FloorInt(newLife).Clamp(Player.statLife, Player.statLifeMax2);
            _bufferLife = newLife - lifeGain;
            Player.statLife = lifeGain;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI);
        }
        else
        {
            _bufferLife = 0;
            _lifeLeechDuration = 0;
        }
    }

    private float GetHealthRatio()
    {
        float hp = Player.statLife;
        float maxHp = Player.statLifeMax2;

        return hp / maxHp;
    }

    private float GetManaRatio()
    {
        float mp = Player.statMana;
        float maxmp = Player.statManaMax2;

        return mp / maxmp;
    }

    private int GetBuffAmount(int[] buffTime)
    {
        var count = 0;
        for (var i = 0; i < buffTime.Length; i++)
            if (buffTime[i] > 0)
                count++;

        return count;
    }

    private float GetXpMult()
    {
        float value = 1;
        for (var i = 0; i <= Player.armor.Length; i++)
        {
            Item item;
            if (i == Player.armor.Length)
                item = Player.HeldItem;
            else
                item = Player.armor[i];

            if (item.netID > 0)
            {
                var instance = item.GetGlobalItem<ItemUpdate>();

                if (ModifierManager.HaveModifier(Modifier.Smart, instance.Modifier) && !Main.dayTime)
                    value += ModifierManager.GetModifierBonus(Modifier.Smart, instance) * 0.01f;
            }
        }

        return value;
    }

    private void UpdateModifier()
    {
        var maxslot = Player.armor.Length;

        if (!Config.GpConfig.VanityGiveStat) maxslot = 9;

        for (var i = 0; i <= maxslot; i++)
        {
            Item item;
            if (i == maxslot)
                item = Player.HeldItem;
            else
                item = Player.armor[i];

            if (item.TryGetGlobalItem(out ItemUpdate instance)) instance.EquippedUpdateModifier(item, Player);
        }
    }

    private float DamageMultiplierFromModifier(NPC target, int damage)
    {
        float value = 1;

        for (var i = 0; i <= Player.armor.Length; i++)
        {
            Item item;
            if (i == Player.armor.Length)
                item = Player.HeldItem;
            else
                item = Player.armor[i];

            if (item.netID > 0)
            {
                var instance = item.GetGlobalItem<ItemUpdate>();

                if (ModifierManager.HaveModifier(Modifier.MoonLight, instance.Modifier) && !Main.dayTime)
                    value += ModifierManager.GetModifierBonus(Modifier.MoonLight, instance) * 0.01f;

                if (ModifierManager.HaveModifier(Modifier.SunLight, instance.Modifier) && Main.dayTime)
                    value += ModifierManager.GetModifierBonus(Modifier.SunLight, instance) * 0.01f;

                if (ModifierManager.HaveModifier(Modifier.Berserker, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.Berserker, instance) * (1 - GetHealthRatio());

                if (ModifierManager.HaveModifier(Modifier.MagicConnection, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.MagicConnection, instance) *
                             (1 - GetManaRatio());

                if (ModifierManager.HaveModifier(Modifier.Sniper, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.Sniper, instance) * 0.01f *
                             Vector2.Distance(Player.position, target.position);

                if (ModifierManager.HaveModifier(Modifier.Brawler, instance.Modifier) &&
                    Vector2.Distance(Player.position, target.position) < 130)
                    value += ModifierManager.GetModifierBonus(Modifier.Brawler, instance) * 0.01f;

                if (ModifierManager.HaveModifier(Modifier.Executor, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.Executor, instance) *
                             (1 - target.life / target.lifeMax);

                if (ModifierManager.HaveModifier(Modifier.Venom, instance.Modifier) && target.HasBuff(20))
                    value += ModifierManager.GetModifierBonus(Modifier.Venom, instance) * 0.01f;

                if (ModifierManager.HaveModifier(Modifier.Chaotic, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f *
                             GetBuffAmount(Player.buffTime);

                if (ModifierManager.HaveModifier(Modifier.Cunning, instance.Modifier))
                    value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f *
                             GetBuffAmount(target.buffTime);

                //APPLY DEBUFF

                if (ModifierManager.HaveModifier(Modifier.Poisones, instance.Modifier))
                    if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Poisones, instance))
                        target.AddBuff(BuffID.Venom, 360);

                if (ModifierManager.HaveModifier(Modifier.Confusion, instance.Modifier))
                    if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Confusion, instance))
                        target.AddBuff(BuffID.Confused, 360);

                if (ModifierManager.HaveModifier(Modifier.Cleave, instance.Modifier))
                {
                    var damageToApply = (int)(ModifierManager.GetModifierBonus(Modifier.Cleave, instance) * damage *
                                              value * 0.01f);
                    for (var j = 0; j < Main.npc.Length; j++)
                        if (Vector2.Distance(Main.npc[j].position, target.position) < 200 && !Main.npc[j].townNPC)
                            Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyCleave(Main.npc[j], damageToApply);
                }

                if (ModifierManager.HaveModifier(Modifier.BloodSeeker, instance.Modifier))
                    if (ModifierManager.GetModifierBonus(Modifier.BloodSeeker, instance) >
                        target.life / (float)target.lifeMax * 100)
                    {
                        var heal = (int)(damage * value * 0.01f *
                                         ModifierManager.GetModifierBonusAlt(Modifier.BloodSeeker, instance));
                        Player.GetModPlayer<RpgPlayer>().ApplyReduction(ref heal);
                        HealSlow(heal);

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI);
                    }
            }
        }

        return value;
    }

    private int GetMaxMinion()
    {
        var value = 0;
        for (var i = 0; i < Player.inventory.Length; i++)
        {
            var item = Player.inventory[i];

            if (item.netID > 0)
            {
                var instance = item.GetGlobalItem<ItemUpdate>();
                if (item.DamageType == DamageClass.Summon && instance.Ascension > value)
                    value = instance.Ascension;
            }
        }

        return value;
    }

    public int ApplyPiercing(float piercing, int def, int damage)
    {
        if (damage < def)
            damage = Mathf.RoundInt(def + damage * piercing);
        else if (damage > def + damage * piercing) damage += Mathf.RoundInt(def * piercing);

        return damage;
    }

    public override void
        ModifyHitNPCWithItem(Item item, NPC target,
            ref NPC.HitModifiers modifiers) /* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
    {
        ModifyHitGeneralModifiers(item, target, ref modifiers, 1);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

        var damage = hit.Damage;

        if (Config.GpConfig.RpgPlayer)
        {
            //CupidonPerk
            float cupidOnChance = 0;
            if (GetSkillTree.HavePerk(Perk.Cupidon))
                cupidOnChance = GetSkillTree.nodeList.GetPerk(Perk.Cupidon).GetLevel * 0.05f;

            if (Mathf.Random(0, 1) < cupidOnChance)
                Item.NewItem(Player.GetSource_OnHit(target), target.getRect(), ItemID.Heart);
            //StarGatherer Perk
            float starChance = 0;
            if (GetSkillTree.HavePerk(Perk.StarGatherer))
                starChance = GetSkillTree.nodeList.GetPerk(Perk.StarGatherer).GetLevel * 0.05f;

            if (Mathf.Random(0, 1) < starChance)
                Item.NewItem(Player.GetSource_OnHit(target), target.getRect(), ItemID.Star);
        }

        if (target.type != NPCID.TargetDummy)
            AddWeaponXp(damage, Player.HeldItem);

        Leech(damage);
    }

    private void ModifyHitGeneralModifiers(Item item, NPC target, ref NPC.HitModifiers modifiers, int pen)
    {
        if (Config.GpConfig.RpgPlayer) modifiers.CritDamage *= GetCriticalDamage() * 0.5f;

        modifiers.FinalDamage *= DamageMultiplierFromModifier(target,
            Mathf.RoundInt(modifiers.FinalDamage.Additive * modifiers.FinalDamage.Multiplicative));

        if (ModifierManager.HaveModifier(Modifier.Piercing, Player.HeldItem.GetGlobalItem<ItemUpdate>().Modifier))
            modifiers.ArmorPenetration +=
                ModifierManager.GetModifierBonus(Modifier.Piercing, Player.HeldItem.GetGlobalItem<ItemUpdate>());

        MPPacketHandler.SendNpcUpdate(Mod, target);
    }

    public override void
        ModifyHitNPCWithProj(Projectile proj, NPC target,
            ref NPC.HitModifiers modifiers) /* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
    {
        var pen = proj.penetrate;
        if (pen == 0) pen++;
        ModifyHitGeneralModifiers(Player.HeldItem, target, ref modifiers, pen);
    }

    #endregion

    #region ARMORXP

    private void AddArmorXp(int damage, float multiplier = 1)
    {
        damage = (damage + Player.statDefense).Clamp(0, Player.statLifeMax2);
        Item armorItem;
        for (var i = 0; i < 3; i++)
        {
            armorItem = Player.armor[i];
            if (armorItem.Name != "")
                armorItem.GetGlobalItem<ItemUpdate>().AddExp(Mathf.Ceillong(damage * multiplier), Player, armorItem);
        }
    }

    private void RpgDodge(ref Player.HurtModifiers modifiers)
    {
        if (GetSkillTree.ActiveClass != null)
        {
            var rand = Mathf.Random(0, 1);
            if (rand < JsonCharacterClass.GetJsonCharList.GetClass(GetSkillTree.ActiveClass.GetClassType).Dodge)
            {
                CombatText.NewText(Player.getRect(), new Color(64, 255, 150), "Dodged");
                Player.ShadowDodge();
                //qint heal = damage;
                //Player.GetModPlayer<RPGPlayer>().ApplyReduction(ref damage);
                //Player.statLife = Mathf.Clamp(Player.statLife + damage, 0, Player.statLifeMax2);
                modifiers.FinalDamage *= 0;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI);
        }
    }

    private void ModifyHitByGeneral(ref Player.HurtModifiers modifiers)
    {
        if (Config.GpConfig.RpgPlayer)
        {
            var damage = Mathf.RoundInt(modifiers.FinalDamage.Additive * modifiers.FinalDamage.Multiplicative);

            RpgDodge(ref modifiers);
            if (damage == 0)
                return;

            if (GetSkillTree.HavePerk(Perk.TheGambler))
            {
                if (Mathf.Random(0, 1) < 0.5f)
                {
                    CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + damage);
                    Player.statLife = (Player.statLife + damage).Clamp(0, Player.statLifeMax2);
                    damage = 0;
                    return;
                }

                damage *= 2;
            }

            ApplyReduction(ref damage);
            damage = ManaShieldReduction(damage);
        }
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        ModifyHitByGeneral(ref modifiers);
        base.ModifyHitByNPC(npc, ref modifiers);
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        ModifyHitByGeneral(ref modifiers);
        base.ModifyHitByProjectile(proj, ref modifiers);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        var damage = hurtInfo.Damage;

        AddArmorXp(damage);

        if (damage > Player.statLife)
            if (IsSaved())
                Player.statLife = Player.statLifeMax2;

        base.OnHitByNPC(npc, hurtInfo);
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        AddArmorXp(hurtInfo.Damage);

        if (hurtInfo.Damage > Player.statLife)
            if (IsSaved())
            {
                hurtInfo.Damage *= 0;
                Player.statLife = Player.statLifeMax2;
            }

        base.OnHitByProjectile(proj, hurtInfo);
    }

    #endregion
}