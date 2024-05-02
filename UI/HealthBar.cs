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
    Hp,
    Leech,
    Mana,
    Xp,
    Weapon,
    Breath
}

internal class BuffIcon : UIElement
{
    private Texture2D _texture;
    public Color Color;
    private const float ImageScale = 1f;

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
            Color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
    }
}

internal class ResourceInfo
{
    private readonly Vector2 _baseSize;
    public Vector2 Position;
    public Vector2 Size;
    public readonly Texture2D Texture;

    public ResourceInfo(Texture2D texture, Vector2 position, float scale = 1f)
    {
        Texture = texture;
        Position = position;
        _baseSize = texture.Size();
        Size = _baseSize * scale;
    }

    public void ChangeSize(float scale)
    {
        Size = _baseSize * scale;
    }
}

internal class HealthBar : UIState
{
    public static bool Visible = false;
    private const float BaseUiHeight = 393f;

    private ResourceBreath _breath;

    private UIElement _buffPanel;
    private UIElement _buffTtPanel;

    private UIText _health;
    private bool _hidden;
    private UIText _level;

    private readonly UIElement[] _mainPanel = new UIElement[7];
    private UIText _manaText;
    private UiOverlay _overlay;

    private Player _player;

    private readonly Resource[] _resourceBar = new Resource[5];

    private Dictionary<Mode, ResourceInfo> _resourceTexture;

    private float _scale = Config.VConfig.HealthBarScale;
    private UIText _xpText;
    private float _yDefaultOffSet = Config.VConfig.HealthBarYOffSet / 100f;
    
    public override void Update(GameTime gameTime)
    {
        UpdateBuffList();

        if (!Config.GpConfig.RpgPlayer)
        {
            RemoveAllChildren();
            _hidden = true;
            return;
        }

        if (_hidden)
        {
            _hidden = false;
            OnInitialize();
        }

        _buffPanel.Top.Set(CalcUiPosition() - 300 * _scale, 0f);
        var player = Main.player[Main.myPlayer]; //Get Player

        if (player.GetModPlayer<RpgPlayer>().MVirtualRes > 0)
        {
            _health.Left.Set(450 * _scale, 0f);
            _health.SetText("" + player.statLife + " | " + player.statLifeMax2 + " (" +
                            player.statLifeMax2 / (1 - player.GetModPlayer<RpgPlayer>().MVirtualRes) + ")"); //Set Life
        }

        else
        {
            _health.SetText("" + player.statLife + " | " + player.statLifeMax2); //Set Life
        }

        _manaText.SetText("" + player.statMana + " | " + player.statManaMax2); //Set Mana
        _xpText.SetText("" + (float)player.GetModPlayer<RpgPlayer>().GetExp() + " | " +
                        (float)player.GetModPlayer<RpgPlayer>().XpToNextLevel()); //Set Mana
        _level.SetText("Lvl. " + (float)player.GetModPlayer<RpgPlayer>().GetLevel());

        Recalculate();

        base.Update(gameTime);
    }


    private void UpdateBuffList()
    {
        _buffPanel.RemoveAllChildren();
        _buffTtPanel.RemoveAllChildren();
        var rowLimit = 11;
        for (var i = 0; i < Player.MaxBuffs; i++)
            if (Main.player[Main.myPlayer].buffType[i] > 0)
            {
                var buffType = Main.player[Main.myPlayer].buffType[i];
                var xPosition = 32 + i * 38;
                var yPosiiton = 76;

                if (i >= rowLimit)
                {
                    yPosiiton -= 50;
                    xPosition = 32 + (i - rowLimit) * 38;
                }

                DrawBuff(buffType, i, xPosition, yPosiiton);
            }
    }

    private void DrawBuff(int type, int i, int x, int y)
    {
        var buffTexture = TextureAssets.Buff[type].Value;
        var buffIcon = new BuffIcon(buffTexture);
        buffIcon.Color = new Color(0.4f, 0.4f, 0.4f, 0.4f);

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
            var uIText = new UIText(text, _scale);
            uIText.Top.Set(buffTexture.Height, 0);
            buffIcon.Append(uIText);

            //buffIcon.MouseOver() += draw
        }

        if (Main.mouseX - _buffPanel.Left.Pixels < x + buffTexture.Width &&
            Main.mouseY - _buffPanel.Top.Pixels < y + buffTexture.Height && Main.mouseX - _buffPanel.Left.Pixels > x &&
            Main.mouseY - _buffPanel.Top.Pixels > y)
        {
            DrawBuffToolTip(type, buffIcon);

            if (Main.mouseRight && Main.mouseRightRelease) RemoveBuff(i, type);
        }

        _buffPanel.Append(buffIcon);
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
        var mouseY = Main.lastMouseY - (int)_buffPanel.Top.Pixels;
        var mouseX = Main.lastMouseX - (int)_buffPanel.Left.Pixels;
        icon.Color = new Color(1, 1, 1, 1f);
        var buffDesc = Lang.GetBuffDescription(id);

        if (id == 26 && Main.expertMode) buffDesc = Language.GetTextValue("BuffDescription.WellFed_Expert");

        if (id == 94)
        {
            var percentManaSick = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
            buffDesc = Main.buffString + percentManaSick + "%";
        }

        var buffName = Lang.GetBuffName(id);
        var text = new UIText(buffName, _scale, true);
        text.Top.Set(mouseY - 60, 0);
        text.Left.Set(mouseX + 20, 0);
        var descText = new UIText(buffDesc, _scale);
        descText.Top.Set(mouseY - 30, 0);
        descText.Left.Set(mouseX + 20, 0);

        _buffTtPanel.Append(text);
        _buffTtPanel.Append(descText);

        if (id == 147)
        {
            var bannerText = "";
            for (var l = 0; l < NPCLoader.NPCCount; l++)
                if (Item.BannerToNPC(l) != 0 && Main.SceneMetrics.NPCBannerBuff[l])
                    bannerText += "\n" + Lang.GetNPCNameValue(Item.BannerToNPC(l));

            var bText = new UIText(bannerText, _scale);
            bText.Top.Set(mouseY - 20, 0);
            bText.Left.Set(mouseX + 20, 0);
            bText.TextColor = Color.Green;
            _buffTtPanel.Append(bText);
        }
    }

    private void Erase()
    {
        RemoveAllChildren();
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    private float CalcUiPosition()
    {
        return Main.screenTarget.Height - Main.screenTarget.Height * _yDefaultOffSet;
    }

    private void LoadTexture()
    {
        float[] baseUiOffset =
        {
            105 * _scale,
            69 * _scale,
            46 * _scale,
            33 * _scale,
            357 * _scale
        };

        _resourceTexture = new Dictionary<Mode, ResourceInfo>
        {
            {
                Mode.Leech,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/LeechBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(14 * _scale, CalcUiPosition() - baseUiOffset[0]), _scale)
            },
            {
                Mode.Hp,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/HealthBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(14 * _scale, CalcUiPosition() - baseUiOffset[0]), _scale)
            },
            {
                Mode.Mana,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/ManaBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(31 * _scale, CalcUiPosition() - baseUiOffset[1]), _scale)
            },
            {
                Mode.Xp,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/XPBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(44 * _scale, CalcUiPosition() - baseUiOffset[2]), _scale)
            },
            {
                Mode.Weapon,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/WeaponBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(50 * _scale, CalcUiPosition() - baseUiOffset[3]), _scale)
            },
            {
                Mode.Breath,
                new ResourceInfo(
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/BreathBar",
                        AssetRequestMode.ImmediateLoad).Value,
                    new Vector2(5 * _scale, CalcUiPosition() - baseUiOffset[4]), _scale)
            }
        };

        _overlay = new UiOverlay(ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/OverlayHealthBar",
            AssetRequestMode.ImmediateLoad).Value);
    }

    public void Reset()
    {
        Erase();
        AnotherRpgModExpanded.Instance.Logger.Debug("reset");
        
        _yDefaultOffSet = Config.VConfig.HealthBarYOffSet / 100f;
        _scale = Config.VConfig.HealthBarScale;

        _buffTtPanel = new UIElement();
        _buffTtPanel.Left.Set(10 * _scale, 0f);
        _buffTtPanel.Top.Set(CalcUiPosition() + 185 * _scale, 0f);
        _buffTtPanel.Width.Set(0, 0f);
        _buffTtPanel.Height.Set(0, 0f);

        _buffPanel = new UIElement();
        _buffPanel.Left.Set(10 * _scale, 0f);
        _buffPanel.Top.Set(CalcUiPosition() + 185 * _scale, 0f);
        _buffPanel.Width.Set(1000, 0f);
        _buffPanel.Height.Set(400, 0f);

        float[] baseUiOffset =
        {
            105 * _scale,
            69 * _scale,
            46 * _scale,
            33 * _scale,
            357 * _scale
        };

        _resourceTexture[Mode.Leech].Position.Y = CalcUiPosition() - baseUiOffset[0];
        _resourceTexture[Mode.Hp].Position.Y = CalcUiPosition() - baseUiOffset[0];
        _resourceTexture[Mode.Mana].Position.Y = CalcUiPosition() - baseUiOffset[1];
        _resourceTexture[Mode.Xp].Position.Y = CalcUiPosition() - baseUiOffset[2];
        _resourceTexture[Mode.Weapon].Position.Y = CalcUiPosition() - baseUiOffset[3];
        _resourceTexture[Mode.Breath].Position.Y = CalcUiPosition() - baseUiOffset[4];

        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition());
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[0]);
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[0]);
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[1]);
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[2]);
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[3]);
        AnotherRpgModExpanded.Instance.Logger.Debug(CalcUiPosition() - baseUiOffset[4]);

        _player = Main.player[Main.myPlayer];

        _mainPanel[0] = new UIElement();
        _mainPanel[0].SetPadding(0);
        _mainPanel[0].Width.Set(840f, 0f);
        _mainPanel[0].Height.Set(BaseUiHeight, 0f);
        _mainPanel[0].HAlign = 0;
        _mainPanel[0].VAlign = 0;
        _mainPanel[0].Left.Set(0, 0f);
        _mainPanel[0].Top.Set(CalcUiPosition() - BaseUiHeight, 0f);

        _overlay.ImageScale = _scale;
        _overlay.HAlign = 0;
        _overlay.VAlign = 0;
        _mainPanel[0].Append(_overlay);

        for (var i = 0; i < 6; i++)
        {
            _mainPanel[i + 1] = new PanelBar((Mode)i, _resourceTexture[(Mode)i].Texture);

            if ((Mode)i == Mode.Breath)
                _breath = new ResourceBreath((Mode)i, _resourceTexture[(Mode)i].Texture);
            else
                _resourceBar[i] = new Resource((Mode)i, _resourceTexture[(Mode)i].Texture, Color.White);
            _mainPanel[i + 1].HAlign = 0;
            _mainPanel[i + 1].VAlign = 0;
            _mainPanel[i + 1].SetPadding(0);

            _mainPanel[i + 1].Width.Set(_resourceTexture[(Mode)i].Size.X, 0f);
            _mainPanel[i + 1].Height.Set(_resourceTexture[(Mode)i].Size.Y, 0f);
            _mainPanel[i + 1].Left.Set(_resourceTexture[(Mode)i].Position.X, 0f);
            _mainPanel[i + 1].Top.Set(_resourceTexture[(Mode)i].Position.Y, 0f);
            
            if ((Mode)i == Mode.Breath)
            {
                _breath.ImageScale = _scale;
                _mainPanel[i + 1].Append(_breath);
            }
            else
            {
                _resourceBar[i].ImageScale = _scale;

                if (i == 0)
                    _resourceBar[i].Color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                _mainPanel[i + 1].Append(_resourceBar[i]);
            }

            Append(_mainPanel[i + 1]);
        }

        Append(_mainPanel[0]);
        Append(_buffPanel);
        Append(_buffTtPanel);

        _health = new UIText("0|0", 1.3f * _scale);
        _manaText = new UIText("0|0", _scale);
        _xpText = new UIText("0|0", _scale);
        _level = new UIText("Lvl. 1", 0.7f * _scale, true);

        _health.Left.Set(500 * _scale, 0f);
        _health.Top.Set(_mainPanel[0].Height.Pixels - 99 * _scale, 0f);
        _manaText.Left.Set(420 * _scale, 0f);
        _manaText.Top.Set(_mainPanel[0].Height.Pixels - 69 * _scale, 0f);
        _xpText.Left.Set(420 * _scale, 0f);
        _xpText.Top.Set(_mainPanel[0].Height.Pixels - 47 * _scale, 0f);

        _level.Left.Set(135 * _scale, 0f);
        _level.HAlign = 0;
        _level.Top.Set(_mainPanel[0].Height.Pixels - 136 * _scale, 0f);

        _mainPanel[0].Append(_health);
        _mainPanel[0].Append(_manaText);
        _mainPanel[0].Append(_xpText);
        _mainPanel[0].Append(_level);

        Recalculate();
        //Texture2D OverlayTexture = ModLoader.Request<Texture2D>("AnotherRpgModExpanded/Assets/UI/OverlayHealthBar").Value;
    }
}

internal class ResourceBreath : UIElement
{
    private Texture2D _texture;
    private Color _color;
    private readonly float _height;
    public float ImageScale = 1f;

    private Mode _stat;

    public ResourceBreath(Mode stat, Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        _height = _texture.Height;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        _color = Color.White;
        _stat = stat;
        VAlign = 0;
        HAlign = 0;
    }

    public ResourceBreath(float height)
    {
        _height = height;
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

        var quotient =
            //Calculate quotient
            player.breath / (float)player.breathMax;

        Height.Set(quotient * _height * ImageScale, 0f);
        Top.Set((1 - quotient) * _height * ImageScale, 0);
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
            _color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
    }
}

internal class Resource : UIElement
{
    private Texture2D _texture;
    public Color Color;
    public float ImageScale = 1f;

    private readonly Mode _stat;
    private readonly float _width;

    public Resource(Mode stat, Texture2D texture, Color col)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        _width = _texture.Width;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        Color = col;
        _stat = stat;
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
        switch (_stat)
        {
            case Mode.Hp:
                quotient = player.statLife / (float)player.statLifeMax2;
                break;
            case Mode.Leech:
                quotient = Mathf.Clamp(player.statLife + player.GetModPlayer<RpgPlayer>().GetLifeLeechLeft, 0,
                    player.statLifeMax2) / player.statLifeMax2;
                break;

            case Mode.Mana:
                quotient = player.statMana / (float)player.statManaMax2;
                break;
            case Mode.Xp:
                quotient = player.GetModPlayer<RpgPlayer>().GetExp() /
                           (float)player.GetModPlayer<RpgPlayer>().XpToNextLevel();
                break;
            case Mode.Weapon:
                quotient = player.GetModPlayer<RpgPlayer>().EquippedItemXp /
                           (float)player.GetModPlayer<RpgPlayer>().EquippedItemMaxXp;
                break;
        }

        quotient = Mathf.Clamp(quotient, 0, 1);
        Left.Set(-(1 - quotient) * _width * ImageScale, 0f);
        Recalculate(); // recalculate the position and size

        base.Draw(spriteBatch);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        spriteBatch.Draw(_texture, dimensions.Position(), null, Color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
            0f);
    }
}

internal class UiOverlay : UIElement
{
    private Texture2D _texture;
    public float ImageScale = 1f;

    public UiOverlay(Texture2D texture)
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
    private Mode _stat;
    private float _width;


    public PanelBar(Mode stat, Texture2D texture)
    {
        _stat = stat;
        _width = texture.Width;
        VAlign = 0;
        HAlign = 0;
    }
}