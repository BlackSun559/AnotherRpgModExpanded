using System;
using System.Collections.Generic;
using System.IO;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.UI;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnotherRpgModExpanded;

public enum DamageType : byte
{
    Melee,
    Ranged,
    Throw,
    Magic,
    Summon,
    Symphonic, //thorium
    Radiant, //thorium
    Ki,
    Hunter
}

public enum Message : byte
{
    AddXp,
    SyncLevel,
    SyncPlayerHealth,
    SyncNpcSpawn,
    SyncNpcUpdate,
    SyncWeapon,
    AskNpc,
    Log,
    SyncWorld
}

internal enum SupportedMod
{
    Thorium,
    Calamity,
    Dbzmod,
    Metroid
}

internal class AnotherRpgModExpanded : Mod
{
    public static AnotherRpgModExpanded Instance;

    public static ModKeybind StatsHotKey;
    public static ModKeybind SkillTreeHotKey;
    public static ModKeybind ItemTreeHotKey;

    public static ModKeybind HelmetItemTreeHotKey;
    public static ModKeybind ChestItemTreeHotKey;
    public static ModKeybind LegsItemTreeHotKey;

    internal static GamePlayConfig GpConfig;
    internal static NPCConfig NpcConfig;
    internal static VisualConfig VisualConfig;

    public static ItemUpdate Source;
    public static ItemUpdate Transfer;
    public static float XpTvalueA;
    public static float XpTvalueB;

    public static Vector2 ZoomValue = new(1, 1);

    public static Dictionary<SupportedMod, bool> LoadedMods = new()
    {
        { SupportedMod.Thorium, false },
        { SupportedMod.Calamity, false },
        { SupportedMod.Dbzmod, false },
        { SupportedMod.Metroid, false }
        //{SupportedMod.Spirit,false }
    };

    public UserInterface CustomItemTree;

    public UserInterface CustomNpcInfo;
    public UserInterface CustomNpcName;
    public UserInterface CustomOpenSt;
    public UserInterface CustomOpenstats;
    public UserInterface CustomResources;
    public UserInterface CustomSkillTree;
    public UserInterface Customstats;
    public HealthBar HealthBar;
    public ItemTreeUi ItemTreeUi;

    public float LastUpdateScreenScale = Main.screenHeight;

    public ReworkMouseOver NpcInfo;
    public NPCNameUI NpcName;
    public OpenSTButton OpenSt;

    public OpenStatsButton OpenStatMenu;

    public SkillTreeUi SkillTreeUi;
    public Stats StatMenu;

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MPPacketHandler.HandlePacket(reader, whoAmI);
    }

    private void Player_Update(ILContext il)
    {
        try
        {
            var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.Before,
                    i => i.MatchLdfld("Terraria.Player", "statManaMax2"),
                    i => i.MatchLdcI4(400)))
            {
                Logger.Error("Can't find this damn mana instruction D:");
                return;
            }

            if (cursor.Next != null) cursor.Next.Next.Operand = 100000;
        }
        catch
        {
            Logger.Error("another mod is editiong TerrariaPlayer StatmanaMax 2, can't edit mana cap");
        }
    }

    public override void Unload()
    {
        base.Unload();

        IL_Player.Update -= Player_Update;
        JsonSkillTree.Unload();
        JsonCharacterClass.Unload();

        StatsHotKey = null;
        SkillTreeHotKey = null;
        ItemTreeHotKey = null;

        HelmetItemTreeHotKey = null;
        ChestItemTreeHotKey = null;
        LegsItemTreeHotKey = null;

        if (!Main.dedServ)
        {
            ItemTreeUi.Instance = null;
            SkillTreeUi.Instance = null;
            Stats.Instance = null;

            VisualConfig = null;
            NpcConfig = null;
            GpConfig = null;
            LoadedMods.Clear();
            LoadedMods = null;
            NPCNameUI.Instance = null;
            ReworkMouseOver.Instance = null;

            Source = null;
            Transfer = null;

            CustomNpcInfo = null;
            NpcInfo = null;
            CustomNpcName = null;
            NpcName = null;
            CustomResources = null;
            HealthBar = null;
            Customstats = null;
            StatMenu = null;
            CustomOpenstats = null;
            OpenStatMenu = null;
            CustomOpenSt = null;
            OpenSt = null;
            CustomSkillTree = null;
            SkillTreeUi = null;
            CustomItemTree = null;
            ItemTreeUi = null;
        }
        //Instance.Logger.Info("Another Rpg Mod " + Version + " Correctly Unloaded");

        Instance = null;
    }

    public override void Load()
    {
        IL_Player.Update += Player_Update;

        Instance = this;
        Instance.Logger.Info("Another Rpg Mod " + Version + " Correctly loaded");
        JsonSkillTree.Init();
        JsonCharacterClass.Init();
        LoadedMods[SupportedMod.Thorium] = ModLoader.HasMod("ThoriumMod");
        // LoadedMods[SupportedMod.Calamity] = ModLoader.HasMod("CalamityMod");
        LoadedMods[SupportedMod.Dbzmod] = ModLoader.HasMod("DBZMOD");
        LoadedMods[SupportedMod.Metroid] = ModLoader.HasMod("MetroidMod");

        StatsHotKey = KeybindLoader.RegisterKeybind(this, "Open Stats Menu", "C");
        SkillTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open SkillTree", "X");
        ItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Item Tree", "V");
        HelmetItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Helmet Item Tree", "NumPad1");
        ChestItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Chest Item Tree", "NumPad2");
        LegsItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Legs Item Tree", "NumPad3");

        if (!Main.dedServ)
        {
            CustomNpcInfo = new UserInterface();
            NpcInfo = new ReworkMouseOver();
            ReworkMouseOver.visible = true;
            CustomNpcInfo.SetState(NpcInfo);

            CustomNpcName = new UserInterface();
            NpcName = new NPCNameUI();
            NPCNameUI.visible = true;
            CustomNpcName.SetState(NpcName);

            CustomResources = new UserInterface();
            HealthBar = new HealthBar();
            HealthBar.visible = true;
            CustomResources.SetState(HealthBar);

            Customstats = new UserInterface();
            StatMenu = new Stats();
            Stats.visible = false;
            Customstats.SetState(StatMenu);

            CustomOpenstats = new UserInterface();
            OpenStatMenu = new OpenStatsButton();
            OpenStatsButton.visible = true;
            CustomOpenstats.SetState(OpenStatMenu);

            CustomOpenSt = new UserInterface();
            OpenSt = new OpenSTButton();
            OpenSTButton.visible = true;
            CustomOpenSt.SetState(OpenSt);

            CustomSkillTree = new UserInterface();
            SkillTreeUi = new SkillTreeUi();
            OpenStatsButton.visible = true;
            CustomSkillTree.SetState(SkillTreeUi);

            CustomItemTree = new UserInterface();
            ItemTreeUi = new ItemTreeUi();
            ItemTreeUi.visible = false;
            CustomItemTree.SetState(ItemTreeUi);

            /*

            statMenu = new Stats();
            Stats.visible = true;
            customstats.SetState(statMenu);
            */
        }
    }

    private void DrawInterface_Resources_ClearBuffs()
    {
        Main.buffString = "";
        Main.bannerMouseOver = false;

        if (!Main.recBigList) Main.recStart = 0;
    }

    public void DrawInterface_Resources_Buffs()
    {
        Main.recBigList = false;
        var buffId = -1;
        var num1 = 11;
        for (var buffSlot = 0; buffSlot < Player.MaxBuffs; buffSlot++)
            if (Main.player[Main.myPlayer].buffType[buffSlot] <= 0)
            {
                Main.buffAlpha[buffSlot] = 0.4f;
            }
            else
            {
                var x = 32 + buffSlot * 38;
                var y = 76;

                if (buffSlot >= num1)
                {
                    x = 32 + Math.Abs(buffSlot % 11) * 38;
                    y = y + 50 * (buffSlot / 11);
                }

                buffId = Main.DrawBuffIcon(buffId, buffSlot, x, y);
            }

        if (buffId >= 0)
        {
            var num5 = Main.player[Main.myPlayer].buffType[buffId];

            if (num5 > 0)
            {
                var buffName = Lang.GetBuffName(num5);
                var buffTooltip = Main.GetBuffTooltip(Main.player[Main.myPlayer], num5);

                if (num5 == 147) Main.bannerMouseOver = true;
                var rarity = 0;

                if (Main.meleeBuff[num5]) rarity = -10;
                Main.instance.MouseText(buffName, buffTooltip, rarity);
            }
        }
    }

    public static int DrawBuffIcon(int drawBuffText, int buffSlotOnPlayer, int x, int y)
    {
        int num1;
        var buffId = Main.player[Main.myPlayer].buffType[buffSlotOnPlayer];

        if (buffId != 0)
        {
            var color = new Color(Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer],
                Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer]);
            var spriteBatch = Main.spriteBatch;
            var value = TextureAssets.Buff[buffId].Value;
            var position = new Vector2(x, y);
            var nullable = new Rectangle(0, 0, TextureAssets.Buff[buffId].Width(), TextureAssets.Buff[buffId].Height());
            var origin = new Vector2();
            spriteBatch.Draw(value, position, nullable, color, 0f, origin, 1f, 0, 0f);

            if (Main.TryGetBuffTime(buffSlotOnPlayer, out var buffTimeValue) && buffTimeValue > 2)
            {
                var str = Lang.LocalizedDuration(new TimeSpan(0, 0, buffTimeValue / 60), true, false);
                var buffTimerBatch = Main.spriteBatch;
                var dynamicSpriteFont = FontAssets.ItemStack.Value;
                var timerPosition = new Vector2(x, y + TextureAssets.Buff[buffId].Height());
                origin = new Vector2();
                buffTimerBatch.DrawString(dynamicSpriteFont, str, timerPosition, color, 0f, origin, 0.8f, 0, 0f);
            }

            if (Main.mouseX >= x + TextureAssets.Buff[buffId].Width() ||
                Main.mouseY >= y + TextureAssets.Buff[buffId].Height() || Main.mouseX <= x || Main.mouseY <= y)
            {
                Main.buffAlpha[buffSlotOnPlayer] -= 0.05f;
            }
            else
            {
                drawBuffText = buffSlotOnPlayer;
                Main.buffAlpha[buffSlotOnPlayer] += 0.1f;
                var flag = Main.mouseRight && Main.mouseRightRelease;

                if (!PlayerInput.UsingGamepad)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                }
                else
                {
                    flag = Main.mouseLeft && Main.mouseLeftRelease && Main.playerInventory;

                    if (Main.playerInventory) Main.player[Main.myPlayer].mouseInterface = true;
                }

                if (flag) Main.TryRemovingBuff(buffSlotOnPlayer, buffId);
            }

            if (Main.buffAlpha[buffSlotOnPlayer] > 1f)
                Main.buffAlpha[buffSlotOnPlayer] = 1f;
            else if (Main.buffAlpha[buffSlotOnPlayer] < 0.4) Main.buffAlpha[buffSlotOnPlayer] = 0.4f;

            if (PlayerInput.UsingGamepad && !Main.playerInventory) drawBuffText = -1;

            num1 = drawBuffText;
        }
        else
        {
            num1 = drawBuffText;
        }

        return num1;
    }
}