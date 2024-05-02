using System;
using System.Collections.Generic;
using System.IO;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule.Entities;
using AnotherRpgModExpanded.UI;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnotherRpgModExpanded.RPGModule.Items;

internal class ItemUpdate : GlobalItem
{
    public static bool HaveTree(Item item)
    {
        if (item.GetGlobalItem<ItemUpdate>() != null && NeedSavingStatic(item) && !item.accessory) return true;

        return false;
    }

    private void InitItem(Item item)
    {
        _mWeaponType = ModifierManager.GetWeaponType(item);

        if (_init)
            return;

        _init = true;

        if (NeedSavingStatic(item))
        {
            _baseMana = item.mana;
            _baseAutoReuse = item.autoReuse;
            _baseUseTime = item.useTime;
            _baseName = item.Name;
            _baseValue = item.value;
            GetItemType = GetItemTypeCustom(item);
            _baseArmor = item.defense;
            BaseDamage = item.damage;

            if (WorldManager.Ascended)
            {
                _bAscendWorldDrop = true;
                _ascendWorldDropLevel = WorldManager.AscendedLevelBonus;
            }

            Roll(item);

            BaseCap = GetCapLevel();

            if (HaveTree(item) && (_mItemTree == null || _mItemTree.GetSize == 0))
            {
                _mItemTree = new ItemSkillTree();
                _mItemTree.Init(this);
            }

            if (WorldManager.Ascended)
            {
                var ascendLevel = 1 + Mathf.CeilInt(WorldManager.AscendedLevelBonus / 250);
                float baseLevel = Mathf.CeilInt(WorldManager.AscendedLevelBonus / 2500);
                if (baseLevel > 0.4f) baseLevel = 0.4f;
                baseLevel += 0.1f;
                Ascension = ascendLevel;
                Level = Mathf.CeilInt(BaseCap * baseLevel * ascendLevel);
                
                if (HaveTree(item))
                {
                    _mItemTree.MaxAscendPoints = Ascension;
                    _mItemTree.AscendPoints = Ascension;
                    _mItemTree.MaxEvolutionPoints = Level;
                    _mItemTree.EvolutionPoints = Level;
                    _mItemTree.ExtendTree(Mathf.CeilInt(Mathf.Pow(BaseCap / 3f, 0.95)).Clamp(5, 99) * Ascension);
                }
            }
        }

        if (item.healLife > 0)
            _baseHealLife = item.healLife;

        if (item.healMana > 0)
            _baseHealMana = item.healMana;
        if (_stats.Stats == null)
            _stats.Stats = new List<ItemStat>();
    }

    #region Variables

    private static readonly string[] AscendName =
    [
        "",
        "Limit Broken ",
        "Mortal ",
        "Raised ",
        "Unleashed ",
        "Immortal ",
        "Ascended ",
        "High Ascended ",
        "Peak Ascended ",
        "Transcendental ",
        "Lower Divine Collection ",
        "Divine Collection ",
        "Higher Divine Collection ",
        "Heavenly Divine Collection ",
        "Heavenly Divine Collection ",
        "Heavenly Div1ne Collection ",
        "Heavenly D1vine Collection ",
        "HeAvenly D1v1ne CollEction ",
        "He@v3nly D1B1n3 C01leCti0n ",
        "C8@V3€4y D3?5n C01;eCti0, ",
        "C0rpt3d d30n Collection ",
        "Corrupted Demon Collection ",
        "Corrupted Arch-Demon Collection ",
        "Corrupted Devil Collection ",
        "Devil Horror Collection ",
        "Cosmic Horror Collection ",
        "Eldritch Horror Collection ",
        "........ ",
        "...H... ",
        "..Hello. ",
        "..Why are you keep using it ?.. ",
        "..This is beyond any limit... ",
        "..What are you trying to achieve like this ?.. ",
        "..Do you want Power ? You have it, wealth ? You have it too, WHAT IS THE POINTS OF THIS ?.. ",
        "..Oh you do it for the Text ?.. ",
        "..Do you realy think I want to speak to your ?.. ",
        "..Ah, you can stop here, there won't be more, honestly, I'm surprised you didn't get an int overflow yet.. ",
        "..Good bye.. ",
        "........ ",
        "........ ",
        "........ ",
        "........ ",
        "....LIFE ANSWER..... ",
        "...What are you doing ?! Did I fix this stupid overflow bug... ",
        "...Well, here something for you, Item node rarity increase as you go deep into the tree... ",
        "...So some impossible to get node will appear often at deep layer, but I guess you've seen it... ",
        "...Good Luck... ",
        "... ",
        ".. ",
        ". ",
        "",
        "One Punch Man ",
        "...Well, speaking of One Punch Man, maybe I should make a list of suggestion for manga to read ? ",
        "If you like cool fight, I really suggest to read Kengan Asura (and Kengan Omega) ",
        "One of the popular manga out there is actually a manhua titled 'Solo Leveling', you are playing a rpg mod, so you should definitely like it ",
        "Another manhua similar to solo leveling but taking a different story is 'I Am the sorcerer King' ",
        "Totally different genre, but you should also read Dr.Stone, especially if you like science ",
        "hmm... You really like those text ? Well ... ",
        "Player - Is this a discution ? ",
        "Discution - Ascension Text ",
        "Yea I mess around, but guy, if you reach this far you most likely used an exploit, and are actively looking for this ! ",
        "So I'm doing this : NIGERUNDAYO ........ ",
        "... ",
        "Yea that jojo reference, Watch jojo you boi ! ",
        "You next sentence will be : 'Does this wall of text ever end ?' ",
        "Does this wall of text ever end ? ",
        "Hue ! How do you know ? ",
        "Simple ! I am the one writing these text ",
        "well it was fun, I'll find more stuff to talk about ",
        "69 LOL",
        "oh yea, do you want some cool music suggestion ? ",
        "then you should definitely look after 'M2U' really original stuff ",
        "Shiro Sagisu is also nice to listen to, he composed ost of Evangelion ",
        "Look after Keichii Okabe, he made the ost of Nier, Drakengard and a few other game ! ",
        "ElectroSwing is also way to go, Parov stellar and Caravane Palace being classic ",
        "... ",
        "Do you know I'm actually working for a game named 'QANGA' made by the IolaCorp ",
        "Sort of survival game in a post-apo sci-fi game where you play as human trapped in robot ",
        "Yea, that the reason, update are so scarce, that and MAINLY my laziness, but please let me find some excuses ",
        "... ",
        "Another funFact, I've made a mod on dota 2 back in 2015, it was my first mod ever, It actually was REALLY successful ",
        "The name was 'Epic Boss Fight' and got more than 5 million subscriber, and even snatched most played mod from valve custom mod back then ",
        "now it's totally broken and unplayable hahahaha.... volvo broke it I promise ! ",
        "The TRUE last text for ascended is 'Infinity +1', no lie ! ",
        "I will also randomly add new text with update without any changelog, there is nothing to gain, you ... well you will just make me laugh to see you looking for all that ",
        "I mean, only because it's not that easy to find doesn't mean it's interesting, of you should read it, I even made some advertisement for a game I work on as employee Hahaha ",
        "... ",
        "Have you Player CivIdle ? That a game I work on my free time When I'm not lazy, Ready manhua, Working on AnRPG or other random stuff, You can easily find the link on discord ",
        "What is it ? A simply Idle uncomplete Game where you lead a civilization ! ",
        "The link ? SERIOUSLY ?!!! CHECK ON DISCORD DAMN IT ! ",
        "... ",
        "Yes... lower ascending name are inspired from chinese ArtMartial Manhua ",
        "... ",
        "... ",
        "...here a new text for 1.5, never knew I would come back at this, but here we goes... ",
        "I've finally added some Player ascend ",
        "Once you reach level 1000 you can obtain the Limit Break skill tree node ",
        "Allowing exp gain after level 1000 and unlocking ascended class ",
        "Will this Terraria 1.4 patch finally come ?!",
        "Duh, of course, this version is on 1.4... if I ever published it... ",
        "Well if you read this, congratulation you are in the good universe !",
        "Do you remember the game Qanga I've spoken about ? there has been lot of change about this since",
        "The game now have the entire earth planet to explore, 1-1 scale, you can also travel between earth and moon...",
        "And mars, and Venus, and some other stuff",
        "It use a plugin I've spent a year working on, WorldScape, allowing for realistic Planet generation in 3D :)",
        "Yes it was another advertisement, No one expected it !",
        "Also recently I've spent a lot of time working on a Vampire Survivor type of game, I'm still looking for a definitive name but for now it's Idle-wave",
        "Yes the name is quite weird as there is no idle game part... yet, maybe I'll use the idle part for persistent progressions",
        "Infinity +1 "
    ];

    public string ItemName;

    private ItemSkillTree _mItemTree;

    public void SetItemTree(ItemSkillTree newItemTree)
    {
        _mItemTree = newItemTree;
    }

    public int GetEvolutionPoints => _mItemTree.EvolutionPoints;
    public int GetAscendPoints => _mItemTree.AscendPoints;
    public int GetMaxEvolutionPoints => _mItemTree.MaxEvolutionPoints;
    public int GetMaxAscendPoints => _mItemTree.MaxAscendPoints;

    public Modifier Modifier { get; set; } = Modifier.None;
    public Rarity Rarity = Rarity.NONE;
    private ItemStats _stats = new() { Stats = new List<ItemStat>() };

    public ItemType GetItemType { get; private set; } = ItemType.Other;

    private bool _init;
    private bool _ascendLimit;
    private bool _bAscendWorldDrop;
    private int _ascendWorldDropLevel;
    private int _baseHealLife;
    private int _baseHealMana;
    private int _baseValue;
    private float _defenceBuffer;
    private float _shootTimeBuffer;

    public float DamageBuffer;
    public float DamageFlatBuffer;
    public float DefenceFlatBuffer;
    public float UseTimeBuffer;
    public float Leech;
    public float BonusXp;

    public int BaseCap = 5;

    private WeaponType _mWeaponType;
    public WeaponType GetWeaponType => _mWeaponType;

    public int Level { get; private set; }

    public int Ascension { get; private set; }

    private List<TooltipLine> _ascendToolTip = new();

    public long GetXp { get; private set; }

    public long GetMaxXp => GetExpToNextLevel(Level, Ascension);

    private const int BaseShootTime = 1;
    private int BaseDamage { get; set; }

    private int _baseArmor;
    private int _baseUseTime = 1;
    private int _baseMana;
    private bool _baseAutoReuse;
    private string _baseName = "";
    public float GetLifeLeech { get; private set; }

    public float GetManaLeech { get; private set; }

    public bool Migrated { get; set; } = false;

    public static readonly Dictionary<Message, List<ItemDataTag>> ItemDataTags = new()
    {
        {
            Message.SyncWeapon, new List<ItemDataTag>
            {
                ItemDataTag.init, ItemDataTag.baseDamage, ItemDataTag.baseArmor, ItemDataTag.baseAutoReuse,
                ItemDataTag.baseName, ItemDataTag.baseUseTime, ItemDataTag.baseMana, ItemDataTag.level, ItemDataTag.xp,
                ItemDataTag.ascendedlevel, ItemDataTag.modifier, ItemDataTag.rarity, ItemDataTag.statst1,
                ItemDataTag.stat1, ItemDataTag.statst2, ItemDataTag.stat2, ItemDataTag.statst3, ItemDataTag.stat3,
                ItemDataTag.statst4, ItemDataTag.stat4, ItemDataTag.statst5, ItemDataTag.stat5, ItemDataTag.statst6,
                ItemDataTag.stat6, ItemDataTag.itemTree, ItemDataTag.bWorldAscendDrop, ItemDataTag.WorldAscendDropLevel,
                ItemDataTag.migrated
            }
        }
    };

    #endregion

    #region Information

    private bool IsEquiped(Item item)
    {
        if (item.headSlot > 0 || item.legSlot > 0 || item.bodySlot > 0 || item.accessory)
            return true;
        return false;
    }

    public override bool OnPickup(Item item, Player Player)
    {
        InitItem(item);
        return base.OnPickup(item, Player);
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
    {
        InitItem(item);
        base.Update(item, ref gravity, ref maxFallSpeed);
    }

    public static bool NeedSavingStatic(Item item)
    {
        return item.maxStack == 1 && (item.damage > 0 || item.headSlot > 0 || item.legSlot > 0 || item.bodySlot > 0 ||
                                      item.accessory);
    }

    public ItemType GetItemTypeCustom(Item item)
    {
        if (item.maxStack > 1)
        {
            if (item.healLife > 0 || item.healMana > 0)
                return ItemType.Healing;
            return ItemType.Other;
        }

        if (item.accessory)
            return ItemType.Accessory;

        if (item.bodySlot > 0 || item.legSlot > 0 || item.headSlot > 0)
            return ItemType.Armor;

        if (item.damage > 0)
            return ItemType.Weapon;
        return ItemType.Other;
    }

    public float GetStatSlot(int slot)
    {
        return StatLevel(_stats.Stats[slot].value);
    }

    public float GetStat(Stat stat)
    {
        float value = 0;
        for (var i = 0; i < _stats.Stats.Count; i++)
            if (_stats.Stats[i].stat == stat)
                value += _stats.Stats[i].value;
        return value;
    }

    public void UpdatePassive(Item item, Player player)
    {
        Leech = 0;
        BonusXp = 0;

        if (_mItemTree == null)
        {
            _mItemTree = new ItemSkillTree();
            _mItemTree.Init(this);
        }

        _mItemTree.ApplyFlatPassives(item);
        _mItemTree.ApplyMultiplierPassives(item);
        _mItemTree.ApplyOtherPassives(item);
        _mItemTree.ApplyPlayerPassive(item, player);
    }

    public int GetDamage(Item item)
    {
        var damage = BaseDamage;
        DamageFlatBuffer = damage;

        if (Config.GpConfig.ItemTree)
        {
            if (_mItemTree == null || item == null)
                return damage;
            _mItemTree.ApplyFlatPassives(item);
            DamageBuffer = DamageFlatBuffer;
            _mItemTree.ApplyMultiplierPassives(item);
            _mItemTree.ApplyOtherPassives(item);
            damage = (int)(DamageBuffer * (1 + ModifierManager.GetRarityDamageBoost(Rarity) * 0.01f));
        }
        else
        {
            damage = Mathf.CeilInt(
                damage * (1f + Ascension * 0.1f + Level / 100f)
                        * (1 + ModifierManager.GetRarityDamageBoost(Rarity) * 0.01f));
        }

        if (_bAscendWorldDrop)
            damage = Mathf.CeilInt(damage * 1.5f);

        return damage;
    }

    private int GetUse(Item item)
    {
        var use = _baseUseTime;
        UseTimeBuffer = use;
        if (Config.GpConfig.ItemTree)
        {
            if (_mItemTree == null || item == null)
                return use;
            _mItemTree.ApplyMultiplierPassives(item);
            _mItemTree.ApplyOtherPassives(item);
            use = (int)UseTimeBuffer;
        }
        else
        {
            var useReduction = Mathf.Pow(0.95, Ascension) * Mathf.Pow(0.995, Level);
            use = Mathf.FloorInt(use * useReduction);
        }

        return use;
    }

    public int GetShoot(Item item)
    {
        var use = BaseShootTime;
        _shootTimeBuffer = use;
        if (Config.GpConfig.ItemTree)
        {
            if (_mItemTree == null || item == null)
                return use;
            _mItemTree.ApplyMultiplierPassives(item);
            _mItemTree.ApplyOtherPassives(item);
            use = (int)_shootTimeBuffer;
        }
        else
        {
            var useReduction = Mathf.Pow(0.95, Ascension) * Mathf.Pow(0.995, Level);
            use = Mathf.FloorInt(use * useReduction);
        }

        return use;
    }

    public int GetDefense(Item item)
    {
        var defense = _baseArmor;
        _defenceBuffer = defense;
        DefenceFlatBuffer = defense;
        if (Config.GpConfig.ItemTree)
        {
            if (_mItemTree == null || item == null)
                return defense;
            _mItemTree.ApplyFlatPassives(item);
            _defenceBuffer = DefenceFlatBuffer;
            _mItemTree.ApplyMultiplierPassives(item);
            _mItemTree.ApplyOtherPassives(item);
            defense = (int)_defenceBuffer;
        }
        else
        {
            defense = Mathf.CeilInt(defense * (1 + Ascension * 0.25f) + Level * 0.25f);
        }

        if (_bAscendWorldDrop)
            defense = Mathf.CeilInt(defense * 1.25f);

        return defense;
    }

    public long GetExpToNextLevel(int level, int ascendedLevel)
    {
        if (Level == GetCapLevel() * (Ascension + 1))
            return Mathf.Floorlong((level + 1) * 10000 + Mathf.Pow(level, 4.5f));

        return Mathf.Floorlong((level + 1) * 1000 + Mathf.Pow(level, 4));


        /*
        if (itemType == ItemType.Weapon)
        {
            if (_level <= 11)
                return Mathf.Floorlong((_level + 1) * 50 + Mathf.Pow(_level * (baseDamage * 0.3f) * (1 + 15 / baseUseTime), 2.0f) * Mathf.Pow(1.5f, _ascendedLevel));
            else
                return Mathf.Floorlong((_level + 1) * 50 + Mathf.Pow(_level * (baseDamage * 0.3f) * (1 + 15 / baseUseTime), 2.05f) * Mathf.Pow(1.5f, _ascendedLevel)) * Mathf.Floorlong(1 + _level / 10);
        }
        else
            return Mathf.Floorlong((_level + 1) * 50 + Mathf.Pow(baseArmor * 10 * _level, 2.0f) * Mathf.Pow(1.5f, _ascendedLevel));
        */
    }

    private float StatLevel(float statsValue)
    {
        statsValue = (statsValue + (1 + Level) * statsValue * (1f / 40f)) * (1 + Ascension * .05f);
        return statsValue;
    }

    public override bool InstancePerEntity => true;

    public ItemSkillTree GetItemTree => _mItemTree;

    public void SpendPoints(int points, bool ascend = false)
    {
        if (ascend)
            _mItemTree.AscendPoints -= points;
        else
            _mItemTree.EvolutionPoints -= points;
    }

    #endregion

    #region OverrideFunction

    public override void NetSend(Item item, BinaryWriter writer)
    {
        if (NeedSavingStatic(item))
        {
            writer.Write((byte)Message.SyncWeapon);
            writer.Write(_init);

            writer.Write(BaseDamage);
            writer.Write(_baseArmor);
            writer.Write(_baseAutoReuse);
            writer.Write(_baseName);
            writer.Write(_baseUseTime);
            writer.Write(_baseMana);

            writer.Write(Level);
            writer.Write(GetXp);
            writer.Write(Ascension);
            writer.Write((int)Modifier);
            writer.Write((int)Rarity);

            if (_stats.Stats == null)
                _stats.Stats = new List<ItemStat>();

            for (var i = 0; i < 6; i++)
                if (i < _stats.Stats.Count)
                {
                    writer.Write((sbyte)_stats.Stats[i].stat);
                    writer.Write((int)(_stats.Stats[i].value * 100));
                }
                else
                {
                    writer.Write((sbyte)-1);
                    writer.Write(0);
                }

            var itemTree = "";
            if (HaveTree(item) && _mItemTree != null && _mItemTree.GetSize > 0)
            {
                itemTree = ItemSkillTree.ConvertToString(_mItemTree);
            }
            else
            {
                _mItemTree = new ItemSkillTree();
                _mItemTree.Init(this);
                itemTree = ItemSkillTree.ConvertToString(_mItemTree);
            }

            writer.Write(itemTree);

            writer.Write(_bAscendWorldDrop);
            writer.Write(_ascendWorldDropLevel);
            writer.Write(Migrated);
        }
        
        base.NetSend(item, writer);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        if (NeedSavingStatic(item))
        {
            var msg = (Message)reader.ReadByte();
            var tags = new Dictionary<ItemDataTag, object>();
            foreach (var tag in ItemDataTags[msg]) tags.Add(tag, tag.read(reader));
            switch (msg)
            {
                case Message.SyncWeapon:
                    _baseName = (string)tags[ItemDataTag.baseName];
                    _init = (bool)tags[ItemDataTag.init];

                    BaseDamage = (int)tags[ItemDataTag.baseDamage];
                    _baseArmor = (int)tags[ItemDataTag.baseArmor];
                    _baseAutoReuse = (bool)tags[ItemDataTag.baseAutoReuse];
                    _baseUseTime = (int)tags[ItemDataTag.baseUseTime];
                    _baseMana = (int)tags[ItemDataTag.baseMana];

                    GetItemType = GetItemTypeCustom(item);
                    if (GetItemType == ItemType.Armor)
                    {
                        item.defense = GetDefense(item);
                    }

                    else if (GetItemType == ItemType.Weapon)
                    {
                        if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                        {
                            item.useTime = GetUse(item);
                            item.useAnimation = item.useTime;
                        }
                        else
                        {
                            item.damage = GetDamage(item);
                        }
                    }

                    Level = (int)tags[ItemDataTag.level];
                    GetXp = (long)tags[ItemDataTag.xp];
                    Ascension = (int)tags[ItemDataTag.ascendedlevel];
                    Modifier = (Modifier)tags[ItemDataTag.modifier];
                    Rarity = (Rarity)tags[ItemDataTag.rarity];

                    _stats = new ItemStats
                    {
                        Stats = new List<ItemStat>()
                    };

                    if ((sbyte)tags[ItemDataTag.statst1] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst1], (int)tags[ItemDataTag.stat1]));

                    if ((sbyte)tags[ItemDataTag.statst2] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst2], (int)tags[ItemDataTag.stat2]));

                    if ((sbyte)tags[ItemDataTag.statst3] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst3], (int)tags[ItemDataTag.stat3]));

                    if ((sbyte)tags[ItemDataTag.statst4] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst4], (int)tags[ItemDataTag.stat4]));

                    if ((sbyte)tags[ItemDataTag.statst5] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst5], (int)tags[ItemDataTag.stat5]));

                    if ((sbyte)tags[ItemDataTag.statst6] >= 0)
                        _stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst6], (int)tags[ItemDataTag.stat6]));

                    if ((string)tags[ItemDataTag.itemTree] != "")
                        _mItemTree = ItemSkillTree.ConvertToTree((string)tags[ItemDataTag.itemTree], this, Level,
                            Ascension);

                    _bAscendWorldDrop = (bool)tags[ItemDataTag.bWorldAscendDrop];
                    _ascendWorldDropLevel = (int)tags[ItemDataTag.WorldAscendDropLevel];
                    Migrated = (bool)tags[ItemDataTag.migrated];
                    _init = true;
                    break;
            }
        }

        base.NetReceive(item, reader);
    }

    public override GlobalItem Clone(Item item, Item itemClone)
    {
        var clonedItem = (ItemUpdate)base.Clone(item, itemClone);
        clonedItem.SetItemTree(_mItemTree);
        clonedItem._ascendToolTip = _ascendToolTip;
        return clonedItem;
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position,
        Vector2 velocity, int type, int damage, float knockback)
    {
        if (_mItemTree != null)
        {
            if (ModifierManager.HaveModifier(Modifier.Random, Modifier))
            {
                var projectileId = Mathf.RandomInt(1, 500);
                var spread = 10 * 0.0174f; //20 degree cone
                var baseSpeed = velocity.Length() * (0.9f + 0.2f * Main.rand.NextFloat());
                var baseAngle = MathF.Atan2(velocity.X, velocity.Y);
                var randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                var newVelocity = baseSpeed * new Vector2(MathF.Sin(randomAngle), MathF.Cos(randomAngle));
                var projectileNumber = Projectile.NewProjectile(source, position, newVelocity, projectileId, damage, knockback,
                    player.whoAmI);
                Main.projectile[projectileNumber].friendly = true;
                Main.projectile[projectileNumber].hostile = false;
                Main.projectile[projectileNumber].originalDamage = damage;
                return false;
            }

            _mItemTree.OnShoot(source, item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }

    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        var rpgPlayer = player.GetModPlayer<RpgPlayer>();

        modifiers.CritDamage *= 0.5f * rpgPlayer.GetCriticalDamage();

        if (target.type != NPCID.TargetDummy)
            rpgPlayer.AddWeaponXp(Mathf.RoundInt(modifiers.FinalDamage.Additive * modifiers.FinalDamage.Multiplicative),
                item);
        //rpgPlayer.Leech(damage);
        base.ModifyHitNPC(item, player, target, ref modifiers);
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        //base.LoadData(item, tag);
        //AnotherRpgModExpanded.Instance.Logger.Info(tag);
        //AnotherRpgModExpanded.Instance.Logger.Info("Load Item Data");
        if (_init)
            return;
        //item.r
        if (!NeedSavingStatic(item))
            return;

        ItemName = item.Name;
        GetXp = tag.GetAsLong("Exp");
        Level = tag.GetInt("level");
        Ascension = tag.GetInt("ascendedLevel");

        Rarity = (Rarity)tag.GetInt("rarity");
        Modifier = (Modifier)tag.GetInt("modifier");
        _bAscendWorldDrop = tag.GetBool("bAscendWorldDrop");
        _ascendWorldDropLevel = tag.GetInt("AscendWorldDropLevel");
        Migrated = tag.GetBool("Migrated");

        if (_mItemTree == null)
            _mItemTree = new ItemSkillTree();

        var itemTreeSave = tag.Get<string>("tree");

        if (itemTreeSave != "")
        {
            _mItemTree = ItemSkillTree.ConvertToTree(itemTreeSave, this, 0, 0);
        }
        else
        {
            _mItemTree = new ItemSkillTree();
            _mItemTree.Init(this);
        }

        if (tag.GetIntArray("evolutionInfo") != null && tag.GetIntArray("evolutionInfo").Length == 4)
        {
            _mItemTree.EvolutionPoints = tag.GetIntArray("evolutionInfo")[0];
            _mItemTree.MaxEvolutionPoints = tag.GetIntArray("evolutionInfo")[1];
            _mItemTree.AscendPoints = tag.GetIntArray("evolutionInfo")[2];
            _mItemTree.MaxAscendPoints = tag.GetIntArray("evolutionInfo")[3];
        }
        else
        {
            Level = 0;
            Ascension = 0;
            GetXp = 0;
        }

        var itemStatsList = new List<ItemStat>();

        for (var i = 0; i < tag.GetIntArray("stats").Length * 0.5f; i++)
            itemStatsList.Add(new ItemStat((Stat)tag.GetIntArray("stats")[i * 2],
                tag.GetIntArray("stats")[i * 2 + 1] * 0.01f));
        _stats = new ItemStats
        {
            Stats = itemStatsList
        };

        BaseDamage = item.damage;
        _baseArmor = item.defense;
        _baseAutoReuse = item.autoReuse;
        _baseName = item.Name;
        _baseUseTime = item.useTime;

        GetItemType = GetItemTypeCustom(item);
        if (GetItemType == ItemType.Armor)
        {
            item.defense = GetDefense(item);
        }
        else if (GetItemType == ItemType.Weapon)
        {
            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
            {
                item.useTime = GetUse(item);
                item.useAnimation = item.useTime;
            }
            else
            {
                item.damage = GetDamage(item);
            }
        }

        _baseValue = item.value;
        _baseMana = item.mana;

        if (item.healLife > 0)
            _baseHealLife = item.healLife;
        if (item.healMana > 0)
            _baseHealMana = item.healMana;

        if (Rarity == Rarity.NONE) Roll(item);
        
        BaseCap = GetCapLevel();
        
        if (_stats.Stats == null)
            _stats.Stats = new List<ItemStat>();
        _init = true;

        item.SetNameOverride(SetName(item));
    }

    public void MigrateData(ItemUpdate updated)
    {
        GetXp = updated.GetXp;
        Level = updated.Level;
        Ascension = updated.Ascension;
        Rarity = updated.Rarity;
        Modifier = updated.Modifier;
        _stats = updated._stats;
        _mItemTree = updated._mItemTree;
        _bAscendWorldDrop = updated._bAscendWorldDrop;
        _ascendWorldDropLevel = updated._ascendWorldDropLevel;
        Migrated = true;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.healLife > 0)
        {
            var heal = tooltips.Find(x => x.Name == "HealLife");
            if (heal != null)
            {
                var iheal = tooltips.FindIndex(x => x.Name == "HealLife");
                heal = new TooltipLine(Mod, "HealLife", heal.Text + "( " + _baseHealLife + " )");
                tooltips[iheal] = heal;
            }
        }

        if (item.healMana > 0)
        {
            var heal = tooltips.Find(x => x.Name == "HealMana");
            if (heal != null)
            {
                var iheal = tooltips.FindIndex(x => x.Name == "HealMana");
                heal = new TooltipLine(Mod, "HealMana", heal.Text + "( " + _baseHealMana + " )");
                tooltips[iheal] = heal;
            }
        }

        if (NeedSavingStatic(item))
        {
            if (GetItemType == ItemType.Accessory || GetItemType == ItemType.Armor || GetItemType == ItemType.Weapon)
            {
                /*
                if (itemType == ItemType.Weapon) {
                    TooltipLine ITT = new TooltipLine(mod, "WeaponType",ItemUtils.GetWeaponType(item).ToString());
                    tooltips.Add(ITT);
                }*/

                if (_bAscendWorldDrop)
                {
                    var awtt = new TooltipLine(Mod, "AscendedInfo",
                        "This item is from an Ascended World : +50% Damage, +25% Defence")
                    {
                        OverrideColor = Color.Azure
                    };

                    tooltips.Add(awtt);
                }

                if (Config.GpConfig.ItemRarity)
                {
                    TooltipLine rtt;
                    if (Rarity != Rarity.NONE)
                        rtt = new TooltipLine(Mod, "Rarity", Enum.GetName(typeof(Rarity), Rarity))
                        {
                            OverrideColor = ModifierManager.GetRarityColor(Rarity)
                        };
                    else
                        rtt = new TooltipLine(Mod, "Rarity", "???")
                        {
                            OverrideColor = Color.Pink
                        };
                    tooltips.Add(rtt);
                }

                if ((GetItemType == ItemType.Accessory || GetItemType == ItemType.Armor) &&
                    Config.GpConfig.ItemRarity)
                    if (_stats.Stats.Count > 0)
                        for (var i = 0; i < _stats.Stats.Count; i++)
                        {
                            TooltipLine stt;
                            var p = Main.LocalPlayer.GetModPlayer<RpgPlayer>();
                            if (GetStatSlot(i) > 0)
                                stt = new TooltipLine(Mod, "statsInfo" + i,
                                    Enum.GetName(typeof(Stat), _stats.Stats[i].stat) + " +" +
                                    Mathf.Round(GetStatSlot(i), 2) + " % ( +" +
                                    Mathf.Round(p.GetStat(_stats.Stats[i].stat) * GetStatSlot(i) * 0.01f) + " )")
                                {
                                    OverrideColor = Color.LimeGreen
                                };
                            else
                                stt = new TooltipLine(Mod, "statsInfo" + i,
                                    Enum.GetName(typeof(Stat), _stats.Stats[i].stat) + " -" +
                                    Math.Abs(Mathf.Round(GetStatSlot(i), 2)) + " % ( " +
                                    Mathf.Round(p.GetStat(_stats.Stats[i].stat) * GetStatSlot(i) * 0.01f) + " )")
                                {
                                    OverrideColor = Color.PaleVioletRed
                                };

                            tooltips.Add(stt);
                        }

                if (GetItemType == ItemType.Weapon && Config.GpConfig.ItemRarity)
                {
                    TooltipLine stt;
                    var p = Main.LocalPlayer.GetModPlayer<RpgPlayer>();
                    if (ModifierManager.GetRarityDamageBoost(Rarity) > 0)
                        stt = new TooltipLine(Mod, "statsInfo",
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Raritybonus") + ModifierManager.GetRarityDamageBoost(Rarity) +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Damage"))
                        {
                            OverrideColor = ModifierManager.GetRarityColor(Rarity)
                        };
                    else
                        stt = new TooltipLine(Mod, "statsInfo",
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Raritybonus") + ModifierManager.GetRarityDamageBoost(Rarity) +
                            Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Damage"))
                        {
                            OverrideColor = ModifierManager.GetRarityColor(Rarity)
                        };
                    tooltips.Add(stt);
                }

                if (Config.GpConfig.ItemModifier)
                {
                    var sList = ModifierManager.GetModifierDesc(this);

                    for (var i = 0; i < sList.Length; i++)
                    {
                        var mtt = new TooltipLine(Mod, "Modifier" + i, sList[i])
                        {
                            OverrideColor = ModifierManager.GetRarityColor(Rarity)
                        };
                        tooltips.Add(mtt);
                    }
                }
            }

            //BASE EXP BONUS FROM ITEM
            if (!item.accessory && GetItemType != ItemType.Healing)
            {
                if (GetItemType == ItemType.Weapon)
                {
                    var bt = new TooltipLine(Mod, "BaseDamage",
                        "" + BaseDamage + Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.BaseDamage"));
                    tooltips.Add(bt);
                }
                else if (GetItemType == ItemType.Armor)
                {
                    var bt = new TooltipLine(Mod, "BaseDefense",
                        "" + _baseArmor + Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.BaseDamage"));
                    tooltips.Add(bt);
                }

                if (GetItemType == ItemType.Weapon || GetItemType == ItemType.Armor)
                {
                    var ltt = new TooltipLine(Mod, "LevelInfo",
                        "Level : " + Level + " / " + GetCapLevel() * (Ascension + 1))
                    {
                        IsModifier = true
                    };
                    tooltips.Add(ltt);
                    var xptt = new TooltipLine(Mod, "Experience", "Xp : " + GetXp + "/" + GetMaxXp)
                    {
                        IsModifier = true
                    };
                    tooltips.Add(xptt);
                }

                if (Level >= 0)
                {
                    if (Config.GpConfig.ItemTree && _mItemTree != null)
                    {
                        if (_mItemTree.EvolutionPoints > 0)
                        {
                            var infott = new TooltipLine(Mod, "pointsInfo",
                                _mItemTree.EvolutionPoints + " Evolution Points left !")
                            {
                                IsModifier = true,
                                OverrideColor = Color.Orange
                            };
                            tooltips.Add(infott);
                        }

                        if (_mItemTree.AscendPoints > 0)
                        {
                            var infotta = new TooltipLine(Mod, "pointsAscendInfo",
                                _mItemTree.AscendPoints + " Ascend Points left !!!")
                            {
                                IsModifier = true,
                                OverrideColor = Color.Red
                            };
                            tooltips.Add(infotta);
                        }
                    }
                    else
                    {
                        if (GetItemType == ItemType.Weapon)
                        {
                            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                            {
                                var stt = new TooltipLine(Mod, "BonusSpeedstuff",
                                    "+" + Math.Round(
                                        (Mathf.Pow(1.005, Level) - 1) * 100 + (Mathf.Pow(1.05, Ascension) - 1) * 100,
                                        1) + "% Speed")
                                {
                                    IsModifier = true
                                };
                                tooltips.Add(stt);
                            }
                            else
                            {
                                var tt = new TooltipLine(Mod, "PrefixDamage",
                                    "+" + (Level * 1f + Ascension * 10f) + "% Damage")
                                {
                                    IsModifier = true
                                };
                                tooltips.Add(tt);
                            }
                        }

                        if (GetItemType == ItemType.Armor)
                        {
                            var tt = new TooltipLine(Mod, "PrefixDefense",
                                "+" + Mathf.CeilInt(Level * 0.25f + _baseArmor * Ascension * 0.25f) + " Defense")
                            {
                                IsModifier = true
                            };
                            tooltips.Add(tt);
                        }
                    }

                    foreach (var t in _ascendToolTip)
                        tooltips.Add(t);
                }
            }
        }
    }

    public static string TreeToString(ItemSkillTree itemTree)
    {
        if (itemTree != null)
            return ItemSkillTree.ConvertToString(itemTree);
        return "";
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        //AnotherRpgModExpanded.Instance.Logger.Info("Is it saving Item Data ? ....");
        base.SaveData(item, tag);
        var statsArray = new int[0];
        if (_stats.Stats != null && _stats.Stats.Count > 0)
        {
            statsArray = new int[_stats.Stats.Count * 2];
            for (var i = 0; i < _stats.Stats.Count; i++)
            {
                statsArray[i * 2] = (int)_stats.Stats[i].stat;
                statsArray[i * 2 + 1] = (int)(_stats.Stats[i].value * 100);
            }
        }
        //AnotherRpgModExpanded.Instance.Logger.Info(item.Name);
        //AnotherRpgModExpanded.Instance.Logger.Info(treeToString(m_itemTree));

        var treeToStringVal = "";
        var evInfo = new int[4] { 0, 0, 0, 0 };

        if (_mItemTree != null)
        {
            treeToStringVal = TreeToString(_mItemTree);
            if (_mItemTree.MaxEvolutionPoints >= Level - 1)
                evInfo = new int[4] { GetEvolutionPoints, GetMaxEvolutionPoints, GetAscendPoints, GetMaxAscendPoints };
            else
                evInfo = new int[4]
                    { Level - 1 - _mItemTree.GetUsedPoints, Level - 1, _mItemTree.AscendPoints, Ascension };
        }

        tag.Add("Exp", GetXp);
        tag.Add("level", Level);
        tag.Add("ascendedLevel", Ascension);
        tag.Add("rarity", (int)Rarity);
        tag.Add("modifier", (int)Modifier);
        tag.Add("stats", statsArray);
        tag.Add("tree", treeToStringVal);
        tag.Add("evolutionInfo", evInfo);
        //tag.Add("bAscendWorldDrop", _bAscendWorldDrop);
        tag.Add("bAscendWorldDrop", false);
        tag.Add("AscendWorldDropLevel", _ascendWorldDropLevel);
        tag.Add("Migrated", Migrated);
    }

    public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
    {
        if (Ascension > 0)
            return Main.rand.NextFloat() >= .5f;
        return base.CanConsumeAmmo(weapon, ammo, player);
    }

    public override void UpdateEquip(Item item, Player player)
    {
        if (NeedSavingStatic(item))
            if (Rarity == Rarity.NONE)
                Roll(item);

        var character = player.GetModPlayer<RpgPlayer>();
        if (character != null)
        {
            _ascendToolTip = new List<TooltipLine>();

            if (NeedSavingStatic(item) && (Level > 0 || Ascension > 0))
            {
                _baseValue = (int)(item.value * (1 + Mathf.Pow(Ascension, 2f) + Level * 0.1f));
                if (GetItemType == ItemType.Armor)
                {
                    UpdatePassive(item, player);
                    item.defense = GetDefense(item);
                    if (Ascension > 0)
                    {
                        var maxascend = Ascension.Clamp(0, AscendName.Length - 1);
                        _ascendToolTip.Add(new TooltipLine(Mod, "Ascending",
                            "Ascending Tier " + Ascension + " : " + AscendName[maxascend]) { IsModifier = true });
                    }
                }
            }
        }

        InitItem(item);
    }

    public override void UpdateInventory(Item item, Player player)
    {
        var character = player.GetModPlayer<RpgPlayer>();
        if (NeedSavingStatic(item)) item.SetNameOverride(SetName(item));

        if (character == null) return;
        _ascendToolTip = new List<TooltipLine>();

        InitItem(item);

        if (NeedSavingStatic(item) && (Level > 0 || Ascension > 0))
        {
            UpdatePassive(item, player);

            _baseValue = (int)(item.value * (1 + Mathf.Pow(Ascension, 1.5f) + Level * 0.1f));
            if (GetItemType == ItemType.Armor)
            {
                item.defense = GetDefense(item);
                if (Ascension > 0)
                {
                    var maxascend = Ascension.Clamp(0, AscendName.Length - 1);
                    _ascendToolTip.Add(new TooltipLine(Mod, "Ascending",
                        "Ascending Tier " + Ascension + " : " + AscendName[maxascend]) { IsModifier = true });
                }
            }

            if (GetItemType == ItemType.Weapon)
            {
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                {
                    item.useTime = GetUse(item);
                    item.useAnimation = item.useTime;
                }
                else
                {
                    item.damage = GetDamage(item);
                }

                if (Ascension > 0)
                {
                    var maxascend = Ascension.Clamp(0, AscendName.Length - 1);
                    _ascendToolTip.Add(new TooltipLine(Mod, "Ascending",
                        "Ascending Tier " + Ascension + " : " + AscendName[maxascend]) { IsModifier = true });

                    if (!Config.GpConfig.ItemTree)
                    {
                        if (!_baseAutoReuse && item.DamageType != DamageClass.Magic &&
                            item.DamageType != DamageClass.Summon)
                        {
                            _ascendToolTip.Add(new TooltipLine(Mod, "AscdAutoSwing", "Have AutoUse")
                                { IsModifier = true });
                            item.autoReuse = true;
                        }

                        if (item.DamageType == DamageClass.Ranged)
                            _ascendToolTip.Add(new TooltipLine(Mod, "ascdProjectile", "+ " + Ascension + " Projectiles")
                                { IsModifier = true });

                        if (_baseAutoReuse || Ascension > 1)
                        {
                            _ascendToolTip.Add(new TooltipLine(Mod, "AscdAutoSwingBonus", "+ 40% attack speed")
                                { IsModifier = true });
                            item.useTime = Mathf.CeilInt(_baseUseTime * 0.6f);
                            item.useAnimation = item.useTime;
                        }

                        if (_baseMana > 0)
                        {
                            _ascendToolTip.Add(new TooltipLine(Mod, "AscdAManaUse", "50% Mana Reduction")
                                { IsModifier = true });
                            item.mana = Mathf.CeilInt(_baseMana * 0.5f);
                        }

                        if (item.DamageType == DamageClass.Summon)
                            _ascendToolTip.Add(new TooltipLine(Mod, "AscdMaxMinion", "Max minion +" + Ascension)
                                { IsModifier = true });
                        /*
                            if (Player.HeldItem == item)
                                Player.maxMinions+=ascendedLevel;
                                */
                        if (Ascension > 1)
                        {
                            if (item.DamageType == DamageClass.Melee)
                            {
                                for (var i = 0; i < Ascension; i++) GetLifeLeech = i * 0.5f;
                                _ascendToolTip.Add(
                                    new TooltipLine(Mod, "AscdLifeLeech", "+ " + GetLifeLeech + "% LifeLeech")
                                        { IsModifier = true });
                            }

                            if (item.DamageType == DamageClass.Magic)
                            {
                                for (var i = 0; i < Ascension; i++) GetManaLeech = i;
                                _ascendToolTip.Add(
                                    new TooltipLine(Mod, "AscdManaLeech", "+ " + GetManaLeech + "% ManaLeech")
                                        { IsModifier = true });
                            }

                            if (item.DamageType == DamageClass.Summon)
                            {
                                _ascendToolTip.Add(new TooltipLine(Mod, "AscdMinionDamage", "+ 50% Minion Damage")
                                    { IsModifier = true });
                                player.GetDamage(DamageClass.Summon) *= 1.5f;
                            }

                            if (item.useAmmo > 0)
                                _ascendToolTip.Add(new TooltipLine(Mod, "AscdMinionDamage", "50% Chance Not to use ammo")
                                    { IsModifier = true });
                        }
                    }
                }

                var manacostGain = 0;
                if (Config.GpConfig.RpgPlayer && character.GetSkillTree.HavePerk(Perk.ManaOverBurst) &&
                    item.DamageType == DamageClass.Magic && item.mana > 0)
                    manacostGain = Mathf.CeilInt(player.statMana *
                        (0.1f + ((float)character.GetSkillTree.nodeList.GetPerk(Perk.ManaOverBurst).GetLevel - 1) *
                            0.15f) / (float)Math.Sqrt(character.GetDamageMultiplier(DamageType.Magic, 2)));
                item.mana = _baseMana + manacostGain;
            }
        }
    }

    public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
    {
        healValue = (int)(healValue * player.GetModPlayer<RpgPlayer>().GetBonusHeal());
        base.GetHealLife(item, player, quickHeal, ref healValue);
    }

    public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue)
    {
        healValue = (int)(healValue * player.GetModPlayer<RpgPlayer>().GetBonusHealMana());
        base.GetHealMana(item, player, quickHeal, ref healValue);
    }

    #endregion

    #region CustomFunction

    private int GetTier(float powerLevel)
    {
        if (powerLevel < 50) return 1;

        if (powerLevel < 90) return 2;

        if (powerLevel < 100) return 3;

        if (powerLevel < 140) return 4;

        if (powerLevel < 200) return 5;

        if (powerLevel < 300) return 6;

        if (powerLevel < 400) return 8;

        if (powerLevel < 700) return 10;

        if (powerLevel < 1000) return 15;

        if (powerLevel < 1500) return 20;

        if (powerLevel < 2500) return 30;
        return 40;
    }

    public int GetCapLevel()
    {
        var powerLevel = BaseDamage * Mathf.Pow((60 / _baseUseTime.Min(1)).Clamp(2, 30), 0.5f) +
                         Mathf.Pow(_baseArmor, 1.45f) * 4;

        if (_mWeaponType == WeaponType.Stab)
            powerLevel *= 0.5f;

        if (_baseAutoReuse)
            powerLevel *= 1.2f;
        if (Config.GpConfig.ItemRarity)
            powerLevel *= 1 - Mathf.Log2((float)Rarity) * 0.05f;
        return 5 * GetTier(powerLevel);
    }

    public void ResetTree()
    {
        _mItemTree.Reset(false);
    }

    public void CompleteReset()
    {
        _mItemTree.MaxEvolutionPoints = Level;
        _mItemTree.MaxAscendPoints = Ascension;
        _mItemTree.Reset(true);

        if (Ascension > 0)
            _mItemTree.ExtendTree(Mathf.CeilInt(Mathf.Pow(BaseCap / 3f, 0.95)).Clamp(5, 99) * Ascension);

        AnotherRpgModExpanded.Instance.ItemTreeUi.Open(this);
    }

    public void ResetLevelXp(bool ascend = true)
    {
        Level = 0;
        GetXp = 0;
        if (ascend)
            Ascension = 0;
        _mItemTree.Reset(false);
        _mItemTree.AscendPoints = 0;
        _mItemTree.MaxAscendPoints = 0;
        _mItemTree.EvolutionPoints = 0;
        _mItemTree.MaxEvolutionPoints = 0;
    }

    public void Ascend()
    {
        if (Ascension >= WorldManager.GetMaximumAscend())
        {
            if (!_ascendLimit)
            {
                _ascendLimit = true;
                Main.NewText("Your weapon have reached it's maximum power, the world can't handle anymore", 144, 32,
                    185);
            }

            return;
        }

        Ascension++;
        _mItemTree.MaxAscendPoints++;
        _mItemTree.AscendPoints++;
        _mItemTree.ExtendTree(Mathf.CeilInt(Mathf.Pow(BaseCap / 3f, 0.95)).Clamp(5, 99));

        if (ItemTreeUi.visible) AnotherRpgModExpanded.Instance.ItemTreeUi.Init();

        //First Ascension = LIMIT BREAK
        if (Ascension == 0)
        {
            Main.NewText("WEAPON LIMIT BREAK!!!", Color.OrangeRed);
            _mItemTree.MaxAscendPoints = 0;
            _mItemTree.MaxEvolutionPoints = 0;
            _mItemTree.Reset(false);
            Level = 0;
        }
        else
        {
            Main.NewText("weapon ascended !");
        }
    }

    public void Roll(Item item)
    {
        if (Config.GpConfig.ItemRarity || Config.GpConfig.ItemModifier)
        {
            var info = ModifierManager.RollItem(this, item, _bAscendWorldDrop, _ascendWorldDropLevel);
            
            if (Config.GpConfig.ItemRarity)
            {
                Rarity = info.rarity;
                _stats = info.stats;
            }

            if (Config.GpConfig.ItemModifier) Modifier = info.modifier;
        }
    }

    private static ItemStat ReceivedStat(sbyte stat, int value)
    {
        return new ItemStat((Stat)stat, value * 0.01f);
    }

    private string SetName(Item item)
    {
        var maxAscend = Ascension.Clamp(0, AscendName.Length - 1);
        var prefix = "";
        if (Rarity != Rarity.NONE && Config.GpConfig.ItemRarity)
            prefix += Enum.GetName(typeof(Rarity), Rarity) + " ";
        prefix += AscendName[maxAscend];
        var sufix = "";
        if (Level > 0)
            sufix = " +" + Level;

        if (_baseName == "") _baseName = item.Name;

        return prefix + _baseName + sufix;
    }

    private void SilentLevelUp()
    {
        if (Level >= GetCapLevel() * (Ascension + 1) && Ascension >= WorldManager.GetMaximumAscend()) return;

        GetXp -= GetExpToNextLevel(Level, Ascension);
        Level++;
        _mItemTree.MaxEvolutionPoints++;
        _mItemTree.EvolutionPoints++;

        if (Level == GetCapLevel() && Ascension == 0)
            Main.NewText("Your item has reached its limit", Color.Red);
        if (Level > GetCapLevel() * (Ascension + 1)) Ascend();
    }

    private void LevelUp(Player player, Item item)
    {
        if (_mItemTree == null)
        {
            _mItemTree = new ItemSkillTree();
            _mItemTree.Init(this);
        }

        if (Level >= GetCapLevel() * (Ascension + 1) && Ascension >= WorldManager.GetMaximumAscend()) return;

        GetXp -= GetExpToNextLevel(Level, Ascension);
        if (GetItemType == ItemType.Armor)
            CombatText.NewText(player.getRect(), new Color(255, 26, 255),
                Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Armorupgrade"), true);
        if (GetItemType == ItemType.Weapon)
            CombatText.NewText(player.getRect(), new Color(255, 26, 255),
                Language.GetTextValue("Mods.AnotherRpgModExpanded.ItemUpdate.Weaponupgrade"), true);
        Level++;
        _mItemTree.MaxEvolutionPoints++;
        _mItemTree.EvolutionPoints++;

        if (Level == GetCapLevel() && Ascension == 0)
            Main.NewText("Your item has reached its limit", Color.Red);
        if (Level > GetCapLevel() * (Ascension + 1)) Ascend();

        item.SetNameOverride(SetName(item));
    }

    public void AddExp(long xp, Player player, Item item)
    {
        if (ModifierManager.HaveModifier(Modifier.SelfLearning, Modifier) && !Main.dayTime)
        {
            xp *= 1 + (long)ModifierManager.GetModifierBonus(Modifier.SelfLearning, this);
            xp *= 1 + (long)BonusXp;
        }

        if (!CanLevelUpMore())
            GetXp = (long)(GetXp + xp * Config.GpConfig.ItemXpMultiplier).Clamp(0,
                GetExpToNextLevel(Level, Ascension));
        else
            GetXp += (long)(xp * Config.GpConfig.ItemXpMultiplier).Clamp(0, long.MaxValue);

        while (GetXp >= GetExpToNextLevel(Level, Ascension) && CanLevelUpMore())
        {
            LevelUp(player, item);
            if (!CanLevelUpMore()) GetXp = GetXp.Clamp(0, GetExpToNextLevel(Level, Ascension));
        }
    }

    public bool CanLevelUpMore()
    {
        if (Level < GetCapLevel() * (Ascension + 1))
            return true;
        return Ascension < WorldManager.GetMaximumAscend();
    }

    public void XpTransfer(float xp, Player player, Item item)
    {
        GetXp += (long)xp.Clamp(0, long.MaxValue - GetXp);

        while (GetXp >= GetExpToNextLevel(Level, Ascension) && CanLevelUpMore()) LevelUp(player, item);
    }

    public void SilentXpTransfer(float xp, bool silent = true)
    {
        GetXp += (long)xp.Clamp(0, long.MaxValue - GetXp);

        while (GetXp >= GetExpToNextLevel(Level, Ascension) && CanLevelUpMore()) SilentLevelUp();
    }

    public void EquippedUpdateModifier(Item item, Player player)
    {
        if (ModifierManager.HaveModifier(Modifier.Thorny, Modifier))
            player.thorns += ModifierManager.GetModifierBonus(Modifier.Thorny, this) * 0.01f;

        if (ModifierManager.HaveModifier(Modifier.VampiricAura, Modifier))
            for (var j = 0; j < Main.npc.Length; j++)
            {
                var damageToApply = ModifierManager.GetModifierBonus(Modifier.VampiricAura, this) * (1f / 60f);
                if (Vector2.Distance(Main.npc[j].position, player.position) < 1000 && !Main.npc[j].townNPC &&
                    Main.npc[j].damage > 1)
                {
                    var heal = Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyVampricAura(Main.npc[j], damageToApply);
                    player.GetModPlayer<RpgPlayer>().ApplyReduction(ref heal);
                    player.GetModPlayer<RpgPlayer>().HealSlow(heal);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, j);
                }
            }

        if (ModifierManager.HaveModifier(Modifier.FireLord, Modifier))
            for (var j = 0; j < Main.npc.Length; j++)
                if (!Main.npc[j].townNPC)
                    if (Vector2.Distance(Main.npc[j].position, player.position) < ModifierManager.GetModifierBonus(Modifier.FireLord, this))
                        Main.npc[j].AddBuff(BuffID.OnFire, 15);
    }

    #endregion
}