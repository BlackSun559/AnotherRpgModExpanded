using System;
using System.Collections.Generic;
using System.IO;
using AnotherRpgModExpanded.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace AnotherRpgModExpanded.Utils;

internal class WorldManager : ModSystem
{
    private static readonly Dictionary<Message, List<WorldDataTag>> WorldDataTag = new()
    {
        {
            Message.SyncWorld, new List<WorldDataTag>
            {
                Utils.WorldDataTag.Day, Utils.WorldDataTag.FirstBossDefeated, Utils.WorldDataTag.BossDefeated, Utils.WorldDataTag.LastDayTime,
                Utils.WorldDataTag.Ascended, Utils.WorldDataTag.AscendedLevel, Utils.WorldDataTag.PlayerLevel
            }
        }
    };

    public static int BossDefeated;
    public static int FirstBossDefeated;
    public static int Day = 1;
    private static bool _lastDayTime = true;

    public static bool Ascended;
    public static int AscendedLevelBonus;

    public static int PlayerLevel = 1;

    private static List<int> _bossDefeatedList;

    public static WorldManager Instance;
    
    public static bool Migrated { get; private set; }

    public static void OnBossDefeated(NPC npc)
    {
        if (BossDefeated < FirstBossDefeated)
            BossDefeated = FirstBossDefeated;

        BossDefeated++;

        if (_bossDefeatedList.Exists(x => x == npc.type)) return;

        _bossDefeatedList.Add(npc.type);
        FirstBossDefeated++;
        Main.NewText("The world grow stronger..", 144, 32, 185);
    }

    public override void PostWorldGen()
    {
        Instance = this;
        BossDefeated = 0;
        _bossDefeatedList = new List<int>();
    }

    public static int GetWorldLevelMultiplier(int level)
    {
        var baseLevelMult = 0.6f;

        if (Main.hardMode)
            baseLevelMult = 1;
        baseLevelMult += Day * 0.05f;

        if (!Main.expertMode) baseLevelMult.Clamp(0, 1);

        return Mathf.FloorInt(baseLevelMult * level) + AscendedLevelBonus;
    }

    public static int GetMaximumAscend()
    {
        if (!Config.GpConfig.AscendLimit) return 999;

        float limit = 0;


        if (Config.NpcConfig.BossKillLevelIncrease)
            limit = BossDefeated * Config.GpConfig.AscendLimitPerBoss;

        else
            limit = FirstBossDefeated * Config.GpConfig.AscendLimitPerBoss;


        if (Main.hardMode && limit < 5)
        {
            limit = 5;

            if (NPC.downedPlantBoss && limit < 15)
                limit = 15;

            if (NPC.downedMoonlord)
                return 999;
        }

        return Mathf.FloorInt(limit);
    }

    public static float GetHealthMultiplierAscendCap()
    {
        float limit = 0;

        if (Config.NpcConfig.BossKillLevelIncrease)
            limit = BossDefeated * Config.GpConfig.AscendLimitPerBoss;

        else
            limit = FirstBossDefeated * Config.GpConfig.AscendLimitPerBoss;

        return 1 + Mathf.FloorInt(limit) * 0.5f;
    }

    public override void PostUpdateTime()
    {
        if (Main.dayTime != _lastDayTime)
        {
            _lastDayTime = Main.dayTime;

            if (Main.dayTime) Day++;
        }

        base.PostUpdateTime();
    }

    public static int GetWorldAdditionalLevel()
    {
        var bonuslevel = 0;

        if (Config.NpcConfig.BossKillLevelIncrease)
            bonuslevel = BossDefeated * Config.NpcConfig.NpcGrowthValue;

        else
            bonuslevel = FirstBossDefeated * Config.NpcConfig.NpcGrowthValue;


        if (Main.hardMode)
        {
            bonuslevel = Mathf.CeilInt(bonuslevel * Config.NpcConfig.NpcGrowthHardModePercent);
            bonuslevel += Config.NpcConfig.NpcGrowthHardMode;
        }

        return bonuslevel;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        //AnotherRpgModExpanded.Instance.Logger.Info("Is it saving World Data ? ....");
        base.SaveWorldData(tag);

        if (_bossDefeatedList == null) _bossDefeatedList = new List<int>();

        tag.Add("BossDefeated", BossDefeated);
        tag.Add("FirstBossDefeated", FirstBossDefeated);
        tag.Add("BossDefeatedList", ConvertToInt(_bossDefeatedList));
        tag.Add("day", Day);
        tag.Add("ascended", Ascended);
        tag.Add("ascendedLevel", AscendedLevelBonus);
        tag.Add("PlayerLevel", PlayerLevel);
        tag.Add("migrated", Migrated);
    }

    private int[] ConvertToInt(List<int> list)
    {
        var newList = new int[list.Count];
        for (var i = 0; i < list.Count; i++) newList[i] = list[i];

        return newList;
    }

    private void ConvertToList(int[] list)
    {
        for (var i = 0; i < list.Length; i++) _bossDefeatedList.Add(list[i]);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        Instance = this;
        //AnotherRpgModExpanded.Instance.Logger.Info(tag);
        //AnotherRpgModExpanded.Instance.Logger.Info("Load World Data");
        _bossDefeatedList = new List<int>();
        FirstBossDefeated = tag.GetInt("FirstBossDefeated");
        BossDefeated = tag.GetInt("BossDefeated");
        Day = tag.GetInt("day");
        ConvertToList(tag.GetIntArray("BossDefeatedList"));
        Ascended = tag.GetBool("ascended");
        AscendedLevelBonus = tag.GetInt("ascendedLevel");
        Migrated = tag.GetBool("migrated");

        if (Main.netMode == NetmodeID.SinglePlayer)
            PlayerLevel = tag.GetInt("PlayerLevel");
        else
            PlayerLevel = 0;
    }

    public void MigrateData(TagCompound tag)
    {
        LoadWorldData(tag);
        Migrated = true;
    }

    public void NetUpdateWorld()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)Message.SyncWorld);
            packet.Write(Day);
            packet.Write(FirstBossDefeated);
            packet.Write(BossDefeated);
            packet.Write(_lastDayTime);
            packet.Write(Ascended);
            packet.Write(AscendedLevelBonus);
            packet.Write(PlayerLevel);
            packet.Send();
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((byte)Message.SyncWorld);
        writer.Write(Day);
        writer.Write(FirstBossDefeated);
        writer.Write(BossDefeated);
        writer.Write(_lastDayTime);
        writer.Write(Ascended);
        writer.Write(AscendedLevelBonus);
        writer.Write(PlayerLevel);
        base.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader)
    {
        Message msg;
        msg = (Message)reader.ReadByte();

        if (msg == Message.SyncWorld)
        {
            var tags = new Dictionary<WorldDataTag, object>();
            foreach (var tag in WorldDataTag[msg]) tags.Add(tag, tag.Read(reader));
            Day = (int)tags[Utils.WorldDataTag.Day];
            FirstBossDefeated = (int)tags[Utils.WorldDataTag.FirstBossDefeated];
            BossDefeated = (int)tags[Utils.WorldDataTag.BossDefeated];
            _lastDayTime = (bool)tags[Utils.WorldDataTag.LastDayTime];
            Ascended = (bool)tags[Utils.WorldDataTag.Ascended];
            AscendedLevelBonus = (int)tags[Utils.WorldDataTag.AscendedLevel];
            PlayerLevel = (int)tags[Utils.WorldDataTag.PlayerLevel];
        }

        base.NetReceive(reader);
    }

    public override void PreSaveAndQuit()
    {
        Stats.Visible = false;
        base.PreSaveAndQuit();
    }

    public override void PostUpdateEverything()
    {
        //Update UI when screen Size Change

        if (Main.netMode != NetmodeID.Server)
        {
            if (Math.Abs(AnotherRpgModExpanded.Instance.LastUpdateScreenScale - Main.screenHeight) > 0.01f)
            {
                AnotherRpgModExpanded.Instance.HealthBar.Reset();
                AnotherRpgModExpanded.Instance.OpenSt.Reset();
                AnotherRpgModExpanded.Instance.OpenStatMenu.Reset();
            }

            AnotherRpgModExpanded.Instance.LastUpdateScreenScale = Main.screenHeight;
        }

        base.PostUpdateEverything();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        base.UpdateUI(gameTime);

        if (AnotherRpgModExpanded.Instance.Customstats != null)
            AnotherRpgModExpanded.Instance.Customstats.Update(gameTime);
    }

    public override void ModifyTransformMatrix(ref SpriteViewMatrix transform)
    {
        AnotherRpgModExpanded.ZoomValue = transform.Zoom;
        base.ModifyTransformMatrix(ref transform);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);

        if (Main.netMode == NetmodeID.Server)
            return;


        if (HealthBar.Visible && Config.GpConfig.RpgPlayer && Config.VConfig.HideVanillaHb)
        {
            var ressourceid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            layers.RemoveAt(ressourceid);
        }

        //Vanilla: MouseOver
        var mouseid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));

        if (mouseid != -1)
        {
            layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: NPC Mouse Info",
                delegate
                {
                    AnotherRpgModExpanded.Instance.CustomNpcInfo.Update(Main._drawInterfaceGameTime);
                    AnotherRpgModExpanded.Instance.NpcInfo.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.UI)
            );

            layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: NPC Name Info",
                delegate
                {
                    AnotherRpgModExpanded.Instance.CustomNpcName.Update(Main._drawInterfaceGameTime);
                    AnotherRpgModExpanded.Instance.NpcName.Draw(Main.spriteBatch);
                    return true;
                })
            );
        }

        var skilltreeid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 2"));

        if (skilltreeid != -1)
        {
            //layers.RemoveAt(id);

            //Add you own layer
            layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: StatWindows",
                delegate
                {
                    if (Stats.Visible) AnotherRpgModExpanded.Instance.StatMenu.Draw(Main.spriteBatch);

                    if (OpenStatsButton.Visible)
                    {
                        AnotherRpgModExpanded.Instance.CustomOpenstats.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.OpenStatMenu.Draw(Main.spriteBatch);
                    }

                    return true;
                },
                InterfaceScaleType.UI)
            );

            layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: Skill Tree",
                delegate
                {
                    if (OpenStButton.Visible)
                    {
                        AnotherRpgModExpanded.Instance.CustomOpenSt.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.OpenSt.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.None)
            );

            layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: Skill Tree",
                delegate
                {
                    if (ItemTreeUi.visible)
                    {
                        //Update Item Tree
                        AnotherRpgModExpanded.Instance.CustomItemTree.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.ItemTreeUi.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.None)
            );

            layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: Skill Tree",
                delegate
                {
                    if (SkillTreeUi.visible)
                    {
                        //Update Skill Tree
                        AnotherRpgModExpanded.Instance.CustomSkillTree.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.SkillTreeUi.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.None)
            );
        }

        var id = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        if (id != -1)
            layers.Insert(id, new LegacyGameInterfaceLayer(
                "AnotherRpgMod: Custom Health Bar",
                delegate
                {
                    if (HealthBar.Visible)
                    {
                        //Update CustomBars
                        AnotherRpgModExpanded.Instance.CustomOpenSt.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.CustomOpenstats.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.CustomResources.Update(Main._drawInterfaceGameTime);
                        AnotherRpgModExpanded.Instance.HealthBar.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.None)
            );
    }
}

public class WorldDataTag
{
    public static readonly WorldDataTag Day = new(reader => reader.ReadInt32());
    public static readonly WorldDataTag FirstBossDefeated = new(reader => reader.ReadInt32());
    public static readonly WorldDataTag BossDefeated = new(reader => reader.ReadInt32());
    public static readonly WorldDataTag LastDayTime = new(reader => reader.ReadBoolean());
    public static readonly WorldDataTag Ascended = new(reader => reader.ReadBoolean());
    public static readonly WorldDataTag AscendedLevel = new(reader => reader.ReadInt32());
    public static readonly WorldDataTag PlayerLevel = new(reader => reader.ReadInt32());

    public readonly Func<BinaryReader, object> Read;

    private WorldDataTag(Func<BinaryReader, object> read)
    {
        this.Read = read;
    }
}