using System;
using System.Collections.Generic;
using AnotherRpgModExpanded.RPGModule.Entities;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnotherRpgModExpanded.UI;

internal enum Mode
{
    HP,
    Leech,
    MANA,
    XP,
    Weapon,
    Breath
}

internal class BuffIcon : UIElement
{
    private Texture2D _texture;
    public Color color;
    public float ImageScale = 1f;

    public BuffIcon(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        VAlign = 0;
        HAlign = 0;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();

        spriteBatch.Draw(_texture, dimensions.Position() + new Vector2(0, _texture.Size().Y) * (1f - ImageScale), null,
            color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
    }
}

internal class RessourceInfo
{
    private readonly Vector2 baseSize;
    public Vector2 position;
    public Vector2 size;
    public Texture2D texture;

    public RessourceInfo(Texture2D _texture, Vector2 _position, float scale = 1f)
    {
        texture = _texture;
        position = _position;
        baseSize = _texture.Size();
        size = baseSize * scale;
    }

    public void ChangeSize(float scale)
    {
        size = baseSize * scale;
    }
}

internal class HealthBar : UIState
{
    public static bool visible = false;
    private readonly float baseUiHeight = 393f;

    private RessourceBreath breath;

    public UIElement buffPanel;
    public UIElement buffTTPanel;

    private UIText health;
    public bool hiden;
    private UIText Level;


    public UIElement[] MainPanel = new UIElement[7];
    private UIText manatext;
    private UIOverlay Overlay;


    private Player player;

    private readonly Ressource[] ressourcebar = new Ressource[5];

    private Dictionary<Mode, RessourceInfo> RessourceTexture;

    private float scale = Config.vConfig.HealthBarScale;
    private UIText xptext;
    private float YDefaultOffSet = -Config.vConfig.HealthBarYoffSet;

    public override void Update(GameTime gameTime)
    {
        UpdateBuffList();

        if (!Config.gpConfig.RPGPlayer)
        {
            RemoveAllChildren();
            hiden = true;
            return;
        }

        if (hiden)
        {
            hiden = false;
            OnInitialize();
        }

        buffPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
        var player = Main.player[Main.myPlayer]; //Get Player

        if (player.GetModPlayer<RpgPlayer>().MVirtualRes > 0)
        {
            health.Left.Set(450 * scale, 0f);
            health.SetText("" + player.statLife + " | " + player.statLifeMax2 + " (" +
                           player.statLifeMax2 / (1 - player.GetModPlayer<RpgPlayer>().MVirtualRes) + ")"); //Set Life
        }

        else
        {
            health.SetText("" + player.statLife + " | " + player.statLifeMax2); //Set Life
        }


        manatext.SetText("" + player.statMana + " | " + player.statManaMax2); //Set Mana
        xptext.SetText("" + (float)player.GetModPlayer<RpgPlayer>().GetExp() + " | " +
                       (float)player.GetModPlayer<RpgPlayer>().XpToNextLevel()); //Set Mana
        Level.SetText("Lvl. " + (float)player.GetModPlayer<RpgPlayer>().GetLevel());


        Recalculate();

        base.Update(gameTime);
    }


    private void UpdateBuffList()
    {
        buffPanel.RemoveAllChildren();
        buffTTPanel.RemoveAllChildren();
        var rowLimit = 11;
        for (var i = 0; i < Player.MaxBuffs; i++)
            if (Main.player[Main.myPlayer].buffType[i] > 0)
            {
                var buffType = Main.player[Main.myPlayer].buffType[i];
                var x_pos = 32 + i * 38;
                var y_pos = 76;

                if (i >= rowLimit)
                {
                    y_pos -= 50;
                    x_pos = 32 + (i - rowLimit) * 38;
                }

                DrawBuff(buffType, i, x_pos, y_pos);
            }
    }

    private void DrawBuff(int type, int i, int x, int y)
    {
        var buffTexture = TextureAssets.Buff[type].Value;
        var buffIcon = new BuffIcon(buffTexture);
        buffIcon.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);

        buffIcon.Left.Set(x, 0f);
        buffIcon.Top.Set(y, 0f);

        if (!Main.vanityPet[type] &&
            !Main.lightPet[type] &&
            !Main.buffNoTimeDisplay[type] &&
            (!Main.player[Main.myPlayer].honeyWet || type != 48) &&
            (!Main.player[Main.myPlayer].wet || !Main.expertMode || type != 46) &&
            Main.player[Main.myPlayer].buffTime[i] > 2)
        {
            var text = Lang.LocalizedDuration(new TimeSpan(0, 0, Main.player[Main.myPlayer].buffTime[i] / 60), true,
                false);
            var uIText = new UIText(text, scale);
            uIText.Top.Set(buffTexture.Height, 0);
            buffIcon.Append(uIText);

            //buffIcon.MouseOver() += draw
        }

        if (Main.mouseX - buffPanel.Left.Pixels < x + buffTexture.Width &&
            Main.mouseY - buffPanel.Top.Pixels < y + buffTexture.Height && Main.mouseX - buffPanel.Left.Pixels > x &&
            Main.mouseY - buffPanel.Top.Pixels > y)
        {
            DrawBuffToolTip(type, buffIcon);

            if (Main.mouseRight && Main.mouseRightRelease) RemoveBuff(i, type);
        }

        buffPanel.Append(buffIcon);
    }

    private void RemoveBuff(int id, int type)
    {
        //AnotherRpgModExpanded.Instance.Logger.Info("Remove buff");
        var flag = false;

        if (!Main.debuff[type] && type != 60 && type != 151)
        {
            if (Main.player[Main.myPlayer].mount.Active && Main.player[Main.myPlayer].mount.CheckBuff(type))
            {
                Main.player[Main.myPlayer].mount.Dismount(Main.player[Main.myPlayer]);
                flag = true;
            }

            if (Main.player[Main.myPlayer].miscEquips[0].buffType == type && !Main.player[Main.myPlayer].hideMisc[0])
                Main.player[Main.myPlayer].hideMisc[0] = true;

            if (Main.player[Main.myPlayer].miscEquips[1].buffType == type && !Main.player[Main.myPlayer].hideMisc[1])
                Main.player[Main.myPlayer].hideMisc[1] = true;

            SoundEngine.PlaySound(SoundID.MenuTick);

            if (!flag) Main.player[Main.myPlayer].DelBuff(id);
        }
    }

    private void DrawBuffToolTip(int id, BuffIcon icon)
    {
        var mouseY = Main.lastMouseY - (int)buffPanel.Top.Pixels;
        var mouseX = Main.lastMouseX - (int)buffPanel.Left.Pixels;
        icon.color = new Color(1, 1, 1, 1f);
        var buffDesc = Lang.GetBuffDescription(id);

        if (id == 26 && Main.expertMode) buffDesc = Language.GetTextValue("BuffDescription.WellFed_Expert");

        if (id == 94)
        {
            var percentManaSick = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
            buffDesc = Main.buffString + percentManaSick + "%";
        }

        var buffName = Lang.GetBuffName(id);
        var TText = new UIText(buffName, scale, true);
        TText.Top.Set(mouseY - 60, 0);
        TText.Left.Set(mouseX + 20, 0);
        var DescText = new UIText(buffDesc, scale);
        DescText.Top.Set(mouseY - 30, 0);
        DescText.Left.Set(mouseX + 20, 0);

        buffTTPanel.Append(TText);
        buffTTPanel.Append(DescText);

        if (id == 147)
        {
            var bannerTT = "";
            for (var l = 0; l < NPCLoader.NPCCount; l++)
                if (Item.BannerToNPC(l) != 0 && Main.SceneMetrics.NPCBannerBuff[l])
                    bannerTT += "\n" + Lang.GetNPCNameValue(Item.BannerToNPC(l));

            var BText = new UIText(bannerTT, scale);
            BText.Top.Set(mouseY - 20, 0);
            BText.Left.Set(mouseX + 20, 0);
            BText.TextColor = Color.Green;
            buffTTPanel.Append(BText);
        }
    }

    public void Erase()
    {
        RemoveAllChildren();
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    public void LoadTexture()
    {
        float[] baseUiOffset =
        {
            105 * scale,
            69 * scale,
            46 * scale,
            33 * scale,
            357 * scale
        };

        RessourceTexture = new Dictionary<Mode, RessourceInfo>
        {
            {
                Mode.Leech,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/LeechBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(14 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[0]), scale)
            },
            {
                Mode.HP,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/HealthBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(14 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[0]), scale)
            },
            {
                Mode.MANA,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/ManaBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(31 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[1]), scale)
            },
            {
                Mode.XP,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/XPBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(44 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[2]), scale)
            },
            {
                Mode.Weapon,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/WeaponBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(50 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[3]), scale)
            },
            {
                Mode.Breath,
                new RessourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/BreathBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(5 * scale, Main.screenHeight + YDefaultOffSet - baseUiOffset[4]), scale)
            }
        };

        Overlay = new UIOverlay(ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/OverlayHealthBar",
            AssetRequestMode.ImmediateLoad).Value);
    }

    public void Reset()
    {
        Erase();

        buffTTPanel = new UIElement();
        buffTTPanel.Left.Set(10 * scale, 0f);
        buffTTPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
        buffTTPanel.Width.Set(0, 0f);
        buffTTPanel.Height.Set(0, 0f);

        buffPanel = new UIElement();
        buffPanel.Left.Set(10 * scale, 0f);
        buffPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
        buffPanel.Width.Set(1000, 0f);
        buffPanel.Height.Set(400, 0f);
        YDefaultOffSet = -Config.vConfig.HealthBarYoffSet;
        scale = Config.vConfig.HealthBarScale;

        float[] baseUiOffset =
        {
            105 * scale,
            69 * scale,
            46 * scale,
            33 * scale,
            357 * scale
        };

        RessourceTexture[Mode.Leech].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[0];
        RessourceTexture[Mode.HP].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[0];
        RessourceTexture[Mode.MANA].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[1];
        RessourceTexture[Mode.XP].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[2];
        RessourceTexture[Mode.Weapon].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[3];
        RessourceTexture[Mode.Breath].position.Y = Main.screenHeight + YDefaultOffSet - baseUiOffset[4];


        player = Main.player[Main.myPlayer];


        MainPanel[0] = new UIElement();
        MainPanel[0].SetPadding(0);
        MainPanel[0].Width.Set(840f, 0f);
        MainPanel[0].Height.Set(baseUiHeight, 0f);
        MainPanel[0].HAlign = 0;
        MainPanel[0].VAlign = 0;
        MainPanel[0].Left.Set(0, 0f);
        MainPanel[0].Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet, 0f);


        Overlay.ImageScale = scale;
        Overlay.HAlign = 0;
        Overlay.VAlign = 0;
        MainPanel[0].Append(Overlay);

        for (var i = 0; i < 6; i++)
        {
            MainPanel[i + 1] = new PanelBar((Mode)i, RessourceTexture[(Mode)i].texture);

            if ((Mode)i == Mode.Breath)
                breath = new RessourceBreath((Mode)i, RessourceTexture[(Mode)i].texture);
            else
                ressourcebar[i] = new Ressource((Mode)i, RessourceTexture[(Mode)i].texture, Color.White);
            MainPanel[i + 1].HAlign = 0;
            MainPanel[i + 1].VAlign = 0;
            MainPanel[i + 1].SetPadding(0);

            MainPanel[i + 1].Width.Set(RessourceTexture[(Mode)i].size.X, 0f);
            MainPanel[i + 1].Height.Set(RessourceTexture[(Mode)i].size.Y, 0f);
            MainPanel[i + 1].Left.Set(RessourceTexture[(Mode)i].position.X, 0f);
            MainPanel[i + 1].Top.Set(RessourceTexture[(Mode)i].position.Y, 0f);

            if ((Mode)i == Mode.Breath)
            {
                breath.ImageScale = scale;
                MainPanel[i + 1].Append(breath);
            }
            else
            {
                ressourcebar[i].ImageScale = scale;

                if (i == 0)
                    ressourcebar[i].color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                MainPanel[i + 1].Append(ressourcebar[i]);
            }

            Append(MainPanel[i + 1]);
        }

        ;

        Append(MainPanel[0]);
        Append(buffPanel);
        Append(buffTTPanel);


        health = new UIText("0|0", 1.3f * scale);
        manatext = new UIText("0|0", scale);
        xptext = new UIText("0|0", scale);
        Level = new UIText("Lvl. 1", 0.7f * scale, true);


        health.Left.Set(500 * scale, 0f);
        health.Top.Set(MainPanel[0].Height.Pixels - 99 * scale, 0f);
        manatext.Left.Set(420 * scale, 0f);
        manatext.Top.Set(MainPanel[0].Height.Pixels - 69 * scale, 0f);
        xptext.Left.Set(420 * scale, 0f);
        xptext.Top.Set(MainPanel[0].Height.Pixels - 47 * scale, 0f);

        Level.Left.Set(135 * scale, 0f);
        Level.HAlign = 0;
        Level.Top.Set(MainPanel[0].Height.Pixels - 136 * scale, 0f);

        MainPanel[0].Append(health);
        MainPanel[0].Append(manatext);
        MainPanel[0].Append(xptext);
        MainPanel[0].Append(Level);

        Recalculate();
        //Texture2D OverlayTexture = ModLoader.Request<Texture2D>("AnotherRpgModExpanded/Assets/UI/OverlayHealthBar").Value;
    }
}

internal class RessourceBreath : UIElement
{
    private Texture2D _texture;
    public Color color;
    private readonly float height;
    public float ImageScale = 1f;

    private Mode stat;

    public RessourceBreath(Mode stat, Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        height = _texture.Height;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        color = Color.White;
        this.stat = stat;
        VAlign = 0;
        HAlign = 0;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var player = Main.player[Main.myPlayer];
        var quotient = 1f;
        //Calculate quotient

        quotient = player.breath / (float)player.breathMax;

        Height.Set(quotient * height * ImageScale, 0f);
        Top.Set((1 - quotient) * height * ImageScale, 0);
        Recalculate(); // recalculate the position and size

        base.Draw(spriteBatch);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Vector2(dimensions.X, dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width * 1 / ImageScale);
        var height = (int)Math.Ceiling(dimensions.Height * 1 / ImageScale);

        spriteBatch.Draw(_texture, dimensions.Position(), new Rectangle((int)point1.X, (int)point1.Y, width, height),
            color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
    }
}

internal class Ressource : UIElement
{
    private Texture2D _texture;
    public Color color;
    public float ImageScale = 1f;

    private readonly Mode stat;
    private readonly float width;

    public Ressource(Mode stat, Texture2D texture, Color col)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        width = _texture.Width;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        color = col;
        this.stat = stat;
        VAlign = 0;
        HAlign = 0;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var player = Main.player[Main.myPlayer];
        var quotient = 1f;
        //Calculate quotient
        switch (stat)
        {
            case Mode.HP:
                quotient = player.statLife / (float)player.statLifeMax2;
                break;
            case Mode.Leech:
                quotient = Mathf.Clamp(player.statLife + player.GetModPlayer<RpgPlayer>().GetLifeLeechLeft, 0,
                    player.statLifeMax2) / player.statLifeMax2;
                break;

            case Mode.MANA:
                quotient = player.statMana / (float)player.statManaMax2;
                break;
            case Mode.XP:
                quotient = player.GetModPlayer<RpgPlayer>().GetExp() /
                           (float)player.GetModPlayer<RpgPlayer>().XpToNextLevel();
                break;
            case Mode.Weapon:
                quotient = player.GetModPlayer<RpgPlayer>().EquippedItemXp /
                           (float)player.GetModPlayer<RpgPlayer>().EquippedItemMaxXp;
                break;
        }

        quotient = Mathf.Clamp(quotient, 0, 1);
        Left.Set(-(1 - quotient) * width * ImageScale, 0f);
        Recalculate(); // recalculate the position and size

        base.Draw(spriteBatch);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();


        spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
            0f);
    }
}

internal class UIOverlay : UIElement
{
    private Texture2D _texture;
    public float ImageScale = 1f;

    public UIOverlay(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        VAlign = 0;
        HAlign = 0;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();

        spriteBatch.Draw(_texture, dimensions.Position() + new Vector2(0, _texture.Size().Y) * (1f - ImageScale), null,
            Color.White, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
    }
}

internal class PanelBar : UIElement
{
    private Mode stat;
    private float width;


    public PanelBar(Mode stat, Texture2D texture)
    {
        this.stat = stat;
        width = texture.Width;
        VAlign = 0;
        HAlign = 0;
    }
}