using System;
using AnotherRpgModExpanded.RPGModule;
using AnotherRpgModExpanded.RPGModule.Entities;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnotherRpgModExpanded.UI;

internal class OpenStButton : UIState
{
    public static bool Visible = true;
    private Texture2D _button;
    private bool _hidden;
    private UIElement _openStPanel;
    private float _scale = Config.VConfig.HealthBarScale;
    private float _yOffSet = Config.VConfig.HealthBarYOffSet / 100f;

    private void Erase()
    {
        RemoveAllChildren();
    }

    public override void Update(GameTime gameTime)
    {
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

        base.Update(gameTime);
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    public void LoadTexture()
    {
        _button = ModContent
            .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/skill_tree", AssetRequestMode.ImmediateLoad).Value;
    }

    private float CalcUiPosition()
    {
        return Main.screenTarget.Height - Main.screenTarget.Height * _yOffSet;
    }

    public void Reset()
    {
        Erase();
        _yOffSet = Config.VConfig.HealthBarYOffSet;
        _scale = Config.VConfig.HealthBarScale;

        _openStPanel = new UIElement();
        _openStPanel.SetPadding(0);
        _openStPanel.Left.Set(90 * _scale, 0f);
        _openStPanel.Top.Set(CalcUiPosition(), 0f);
        _openStPanel.Width.Set(32 * _scale, 0f);
        _openStPanel.Height.Set(64 * _scale, 0f);
        _openStPanel.HAlign = 0;
        _openStPanel.VAlign = 0;


        var openButton = new OpenStatButton(_button);
        openButton.Left.Set(0, 0f);
        openButton.Top.Set(0, 0f);
        openButton.ImageScale = _scale;
        openButton.Width.Set(32 * _scale, 0f);
        openButton.Height.Set(64 * _scale, 0f);
        openButton.OnLeftClick += OpenStMenu;
        openButton.HAlign = 0;
        openButton.VAlign = 0;
        _openStPanel.Append(openButton);
        Recalculate();
        Append(_openStPanel);
    }

    private static void OpenStMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Config.GpConfig.RpgPlayer)
            return;

        SoundEngine.PlaySound(SoundID.MenuOpen);

        SkillTreeUi.visible = !SkillTreeUi.visible;

        if (SkillTreeUi.visible)
            SkillTreeUi.Instance.LoadSkillTree();
    }
}

internal class OpenStatsButton : UIState
{
    public static bool Visible = true;
    private Texture2D _button;
    private bool _hidden;
    private UIElement _openStatsPanel;
    private float _scale = Config.VConfig.HealthBarScale;
    private float _yOffSet = Config.VConfig.HealthBarYOffSet / 100f;

    private void Erase()
    {
        RemoveAllChildren();
    }

    public override void Update(GameTime gameTime)
    {
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

        base.Update(gameTime);
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    private void LoadTexture()
    {
        _button = ModContent
            .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/character", AssetRequestMode.ImmediateLoad).Value;
    }

    private float CalcUiPosition()
    {
        return Main.screenTarget.Height - Main.screenTarget.Height * _yOffSet;
    }

    public void Reset()
    {
        Erase();
        _yOffSet = Config.VConfig.HealthBarYOffSet;
        _scale = Config.VConfig.HealthBarScale;

        _openStatsPanel = new UIElement();
        _openStatsPanel.SetPadding(0);
        _openStatsPanel.Left.Set(57 * _scale, 0f);
        _openStatsPanel.Top.Set(CalcUiPosition(), 0f);
        _openStatsPanel.Width.Set(32 * _scale, 0f);
        _openStatsPanel.Height.Set(64 * _scale, 0f);
        _openStatsPanel.HAlign = 0;
        _openStatsPanel.VAlign = 0;

        var openButton = new OpenStatButton(_button);
        openButton.Left.Set(0, 0f);
        openButton.Top.Set(0, 0f);
        openButton.ImageScale = _scale;
        openButton.Width.Set(32 * _scale, 0f);
        openButton.Height.Set(64 * _scale, 0f);
        openButton.OnLeftClick += OpenStatMenu;
        openButton.HAlign = 0;
        openButton.VAlign = 0;
        _openStatsPanel.Append(openButton);
        Recalculate();
        Append(_openStatsPanel);
    }

    private static void OpenStatMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Config.GpConfig.RpgPlayer)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Stats.Instance.LoadChar();
        Stats.Visible = !Stats.Visible;
    }
}

internal class Stats : UIState
{
    public static Stats Instance;
    public static bool Visible;
    private float _baseXOffset = 100;
    private float _baseYOffset = 100;
    private RpgPlayer _char;
    private bool _dragging;
    private UIText _infoStat;

    private readonly Color _mainColor = new(75, 75, 255);

    private Vector2 _offset;

    private UIText _pointsLeft = new("");

    private readonly StatProgress[] _progressStatsBar = new StatProgress[8];
    private readonly ProgressBG[] _progressStatsBarBg = new ProgressBG[8];

    private UIText _resetText;
    private readonly Color _secondaryColor = new(150, 150, 255);
    private float _sizeMultiplier = 1;
    private readonly UIText[] _statProgress = new UIText[8];
    private UIPanel _statsPanel;
    private readonly UIText[] _upgradeStatDetails = new UIText[12];
    private readonly UIText[] _upgradeStatOver = new UIText[12];

    private readonly UIText[] _upgradeStatText = new UIText[8];
    private float _xOffset = 120;
    private float _yOffset = 35;

    public void LoadChar()
    {
        _char = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();
    }

    private void ResetTextHover(UIMouseEvent evt, UIElement listeningElement)
    {
        _resetText.TextColor = Color.White;
    }

    private void ResetTextOut(UIMouseEvent evt, UIElement listeningElement)
    {
        _resetText.TextColor = Color.Gray;
    }

    public override void OnInitialize()
    {
        _sizeMultiplier = Main.screenHeight / 1080f;
        _baseYOffset *= _sizeMultiplier;
        _baseXOffset *= _sizeMultiplier;
        _yOffset *= _sizeMultiplier;
        _xOffset *= _sizeMultiplier;


        Instance = this;
        _statsPanel = new UIPanel();
        _statsPanel.SetPadding(0);
        _statsPanel.Left.Set(400f * _sizeMultiplier, 0f);
        _statsPanel.Top.Set(100f * _sizeMultiplier, 0f);
        _statsPanel.Width.Set(1000 * _sizeMultiplier, 0f);
        _statsPanel.Height.Set(600 * _sizeMultiplier, 0f);
        _statsPanel.BackgroundColor = new Color(73, 94, 171, 150);

        _statsPanel.OnLeftMouseDown += DragStart;
        _statsPanel.OnLeftMouseUp += DragEnd;

        _pointsLeft = new UIText("Points : 0 / 0", _sizeMultiplier);
        _pointsLeft.Left.Set(250 * _sizeMultiplier, 0f);
        _pointsLeft.Top.Set(20 * _sizeMultiplier, 0f);
        _pointsLeft.Width.Set(0, 0f);
        _pointsLeft.Height.Set(0, 0f);
        _statsPanel.Append(_pointsLeft);


        _resetText = new UIText("RESET", _sizeMultiplier, true)
        {
            TextColor = Color.Gray
        };
        _resetText.Left.Set(50 * _sizeMultiplier, 0f);
        _resetText.Top.Set(20 * _sizeMultiplier, 0f);
        _resetText.Width.Set(0, 0f);
        _resetText.Height.Set(0, 0f);
        _resetText.OnLeftClick += ResetStats;
        _resetText.OnMouseOver += ResetTextHover;
        _resetText.OnMouseOut += ResetTextOut;
        _statsPanel.Append(_resetText);

        var button = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay", AssetRequestMode.ImmediateLoad);
        for (var i = 0; i < 12; i++)
        {
            if (i < 8)
            {
                var upgradeStatButton = new UIImageButton(button);

                upgradeStatButton.Left.Set(_baseXOffset + _xOffset * 2, 0f);
                upgradeStatButton.Top.Set(_baseYOffset + _yOffset * i, 0f);
                upgradeStatButton.Width.Set(22 * _sizeMultiplier, 0f);
                upgradeStatButton.Height.Set(22 * _sizeMultiplier, 0f);
                var statused = (Stat)i;
                upgradeStatButton.OnMouseOver +=
                    (uiMouseEvent, uiElement) => UpdateStat(uiMouseEvent, uiElement, statused);
                upgradeStatButton.OnMouseOut += ResetOver;
                upgradeStatButton.OnLeftClick += (uiMouseEvent, uiElement) =>
                    UpgradeStat(uiMouseEvent, uiElement, statused, 1);
                upgradeStatButton.OnRightClick += (uiMouseEvent, uiElement) =>
                    UpgradeStat(uiMouseEvent, uiElement, statused, 5);
                upgradeStatButton.OnMiddleClick += (uiMouseEvent, uiElement) =>
                    UpgradeStat(uiMouseEvent, uiElement, statused, 25);
                upgradeStatButton.OnScrollWheel += (uiMouseEvent, uiElement) =>
                    UpgradeStatWheel(uiMouseEvent, uiElement, statused);
                _statsPanel.Append(upgradeStatButton);


                _progressStatsBar[i] = new StatProgress((Stat)i,
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank",
                        AssetRequestMode.ImmediateLoad).Value);
                _progressStatsBar[i].Left.Set(_baseXOffset + _xOffset * 1.0f, 0f);
                _progressStatsBar[i].Top.Set(_baseYOffset + _yOffset * i + 6, 0f);
                _progressStatsBar[i].Width.Set(105, 0);
                _progressStatsBar[i].HAlign = 0;
                _progressStatsBar[i].Height.Set(10, 0);
                _progressStatsBar[i].width = 105;
                _progressStatsBar[i].left = _baseYOffset + _yOffset * i;
                _statsPanel.Append(_progressStatsBar[i]);

                _progressStatsBarBg[i] = new ProgressBG(ModContent
                    .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank", AssetRequestMode.ImmediateLoad)
                    .Value);
                _progressStatsBarBg[i].Left.Set(_baseXOffset + _xOffset * 1.0f, 0f);
                _progressStatsBarBg[i].Top.Set(_baseYOffset + _yOffset * i + 6, 0f);
                _progressStatsBarBg[i].Width.Set(105, 0);
                _progressStatsBarBg[i].HAlign = 0;
                _progressStatsBarBg[i].Height.Set(10, 0);
                _progressStatsBarBg[i].Color = new Color(10, 0, 0, 128);
                _progressStatsBar[i].left = _baseYOffset + _yOffset * i;

                _statsPanel.Append(_progressStatsBarBg[i]);

                _statProgress[i] = new UIText("0", _sizeMultiplier);
                _statProgress[i].SetText("0/2");
                _statProgress[i].Left.Set(_baseXOffset + _xOffset * 2.3f, 0f);
                _statProgress[i].Top.Set(_baseYOffset + _yOffset * i, 0f);
                _statProgress[i].HAlign = 0f;
                _statProgress[i].VAlign = 0f;
                _statProgress[i].MinWidth.Set(150 * _sizeMultiplier, 0);
                _statProgress[i].MaxWidth.Set(150 * _sizeMultiplier, 0);

                _statsPanel.Append(_statProgress[i]);


                _upgradeStatText[i] = new UIText("0", _sizeMultiplier);
                _upgradeStatText[i].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Mana"));
                _upgradeStatText[i].Left.Set(_baseXOffset - 75, 0f);
                _upgradeStatText[i].Top.Set(_baseYOffset + _yOffset * i, 0f);
                _upgradeStatText[i].HAlign = 0f;
                _upgradeStatText[i].VAlign = 0f;
                _upgradeStatText[i].MinWidth.Set(150 * _sizeMultiplier, 0);
                _upgradeStatText[i].MaxWidth.Set(150 * _sizeMultiplier, 0);
                _statsPanel.Append(_upgradeStatText[i]);
            }

            _infoStat = new UIText("0", _sizeMultiplier);
            _infoStat.SetText("");
            _infoStat.Left.Set(_baseXOffset - 75 * _sizeMultiplier, 0f);
            _infoStat.Top.Set(_baseYOffset + 300 * _sizeMultiplier, 0f);
            _infoStat.HAlign = 0f;
            _infoStat.VAlign = 0f;
            _infoStat.MinWidth.Set(150 * _sizeMultiplier, 0);
            _infoStat.MaxWidth.Set(150 * _sizeMultiplier, 0);
            _statsPanel.Append(_infoStat);

            _upgradeStatDetails[i] = new UIText("", _sizeMultiplier);

            if (i < 3 || i > 7)
                _upgradeStatDetails[i].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Health"));
            _upgradeStatDetails[i]
                .SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.MeleeDamageMultiplier"));
            _upgradeStatDetails[i].Left.Set(_baseXOffset + _xOffset * 2.9f, 0f);
            _upgradeStatDetails[i].Top.Set(_baseYOffset + _yOffset * i, 0f);
            _upgradeStatDetails[i].HAlign = 0f;
            _upgradeStatDetails[i].VAlign = 0f;
            _upgradeStatDetails[i].MinWidth.Set(300f * _sizeMultiplier, 0);
            _upgradeStatDetails[i].MaxWidth.Set(300f * _sizeMultiplier, 0);
            _statsPanel.Append(_upgradeStatDetails[i]);

            _upgradeStatOver[i] = new UIText("", _sizeMultiplier)
            {
                TextColor = Color.Aqua
            };
            _upgradeStatOver[i].SetText("");
            _upgradeStatOver[i].Left.Set(_baseXOffset + _xOffset * 6.7f, 0f);
            _upgradeStatOver[i].Top.Set(_baseYOffset + _yOffset * i, 0f);
            _upgradeStatOver[i].HAlign = 0f;
            _upgradeStatOver[i].VAlign = 0f;
            _upgradeStatOver[i].MinWidth.Set(20f * _sizeMultiplier, 0);
            _upgradeStatOver[i].MaxWidth.Set(20f * _sizeMultiplier, 0);
            _statsPanel.Append(_upgradeStatOver[i]);
        }

        Append(_statsPanel);
    }

    private float GetCritImprov()
    {
        return Math.Abs(Mathf.Round(
            Mathf.Pow(_char.GetStatImproved(Stat.Agi) + _char.GetStatImproved(Stat.Str) + 1, 0.8f) * 0.005f -
            _char.GetCriticalDamage(), 2));
    }

    private float GetCritChanceImprov()
    {
        return Math.Abs(Mathf.Round(
            Mathf.Pow(_char.GetStatImproved(Stat.Foc) + _char.GetStatImproved(Stat.Dex) + 1, 0.8f) * 0.05f -
            _char.GetCriticalChanceBonus(), 3));
    }

    public void UpdateStat(UIMouseEvent evt, UIElement listeningElement, Stat stat)
    {
        Recalculate();

        if (_char == null) LoadChar();

        for (var i = 0; i < 12; i++) _upgradeStatOver[i].TextColor = _secondaryColor;
        switch (stat)
        {
            case Stat.Vit:
                _upgradeStatOver[0]
                    .SetText("+ " + _char.Player.statLifeMax / 20f * 0.65f * _char.StatMultiplier + " HP");
                _upgradeStatOver[0].TextColor = _mainColor;
                _upgradeStatOver[2].SetText("+ " + _char.BaseArmor * 0.0025f * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armor"));
                _upgradeStatOver[2].TextColor = _secondaryColor;
                _upgradeStatOver[10].SetText("+ " + 0.02f * _char.StatMultiplier + " HP/Sec");
                break;
            case Stat.Foc:
                _upgradeStatOver[1]
                    .SetText("+ " + _char.Player.statManaMax / 20f * 0.02f * _char.StatMultiplier + " Mana");
                _upgradeStatOver[1].TextColor = _mainColor;
                _upgradeStatOver[7].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[7].TextColor = _secondaryColor;
                _upgradeStatOver[8].SetText("+ " + GetCritChanceImprov() + " %");

                break;
            case Stat.Cons:
                _upgradeStatOver[0]
                    .SetText("+ " + _char.Player.statLifeMax / 20f * 0.325f * _char.StatMultiplier + " HP");
                _upgradeStatOver[0].TextColor = _secondaryColor;
                _upgradeStatOver[2].SetText("+ " + _char.BaseArmor * 0.006f * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armor"));
                _upgradeStatOver[2].TextColor = _mainColor;
                _upgradeStatOver[10].SetText("+ " + 0.02f * _char.StatMultiplier + " HP/Sec");
                break;
            case Stat.Str:
                _upgradeStatOver[3].SetText("+ " + RpgPlayer.MainStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[3].TextColor = _mainColor;
                _upgradeStatOver[5].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[5].TextColor = _secondaryColor;
                _upgradeStatOver[9].SetText("+ " + GetCritImprov() * 0.01f + " %");
                break;
            case Stat.Agi:
                _upgradeStatOver[4].SetText("+ " + RpgPlayer.MainStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[4].TextColor = _mainColor;
                _upgradeStatOver[3].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[3].TextColor = _secondaryColor;
                _upgradeStatOver[9].SetText("+ " + GetCritImprov() * 0.01f + " %");
                break;
            case Stat.Dex:
                _upgradeStatOver[5].SetText("+ " + RpgPlayer.MainStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[5].TextColor = _mainColor;
                _upgradeStatOver[4].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[4].TextColor = _secondaryColor;
                _upgradeStatOver[8].SetText("+ " + GetCritChanceImprov() + " %");
                break;
            case Stat.Int:
                _upgradeStatOver[6].SetText("+ " + RpgPlayer.MainStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[6].TextColor = _mainColor;
                _upgradeStatOver[1]
                    .SetText("+ " + _char.Player.statManaMax / 20f * 0.05f * _char.StatMultiplier + " Mana");
                _upgradeStatOver[1].TextColor = _secondaryColor;
                _upgradeStatOver[11].SetText("+ " + 0.02f * _char.StatMultiplier + " MP/Sec");
                break;
            case Stat.Spr:
                _upgradeStatOver[7].SetText("+ " + RpgPlayer.MainStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[7].TextColor = _mainColor;
                _upgradeStatOver[6].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * _char.StatMultiplier +
                                            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                _upgradeStatOver[6].TextColor = _secondaryColor;
                _upgradeStatOver[11].SetText("+ " + 0.02f * _char.StatMultiplier + " MP/Sec");
                break;
        }

        _infoStat.SetText(AdditionalInfo.GetAdditionalStatInfo(stat));
    }

    private void ResetOver(UIMouseEvent evt, UIElement listeningElement)
    {
        for (var i = 0; i < 12; i++) _upgradeStatOver[i].SetText("");
        _infoStat.SetText("");
    }

    private void ResetStats(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Visible)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        _char.ResetStats();
    }

    private void UpgradeStat(UIMouseEvent evt, UIElement listeningElement, Stat stat, int amount)
    {
        if (!Visible)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);

        if (Main.keyState.PressingShift())
        {
            if (Main.keyState.IsKeyDown(Keys.LeftControl))
            {
                while (_char.FreePoints > 0)
                    _char.SpendPoints(stat, _char.GetStatXpMax(stat) - _char.GetStatXp(stat));
                return;
            }

            for (var i = 0; i < amount; i++)
                _char.SpendPoints(stat, _char.GetStatXpMax(stat) - _char.GetStatXp(stat));
            return;
        }

        _char.SpendPoints(stat, amount);
    }

    private void UpgradeStatWheel(UIScrollWheelEvent evt, UIElement listeningElement, Stat stat)
    {
        if (!Visible)
            return;
        var amount = 0;

        if (evt.ScrollWheelValue > 0)
            amount = 20;

        if (evt.ScrollWheelValue < 0)
            amount = 150;
        SoundEngine.PlaySound(SoundID.MenuOpen);

        if (Main.keyState.PressingShift())
        {
            if (Main.keyState.IsKeyDown(Keys.LeftControl))
            {
                while (_char.FreePoints > 0)
                    _char.SpendPoints(stat, _char.GetStatXpMax(stat) - _char.GetStatXp(stat));
                return;
            }

            for (var i = 0; i < amount; i++)
                _char.SpendPoints(stat, _char.GetStatXpMax(stat) - _char.GetStatXp(stat));
            return;
        }

        _char.SpendPoints(stat, amount);
    }

    public override void Update(GameTime gameTime)
    {
        if (Visible)
            UpdateStats();
        Recalculate();
        base.Update(gameTime);
    }

    private void UpdateStats()
    {
        for (var i = 0; i < 8; i++)
        {
            var pre = "";
            if (_char.GetStatImproved((Stat)i) > _char.GetStat((Stat)i))
                pre = "+";
            _upgradeStatText[i].SetText((Stat)i + " : " + _char.GetNaturalStat((Stat)i) + " + " +
                                        _char.GetAddStat((Stat)i) + " (" + pre +
                                        (_char.GetStatImproved((Stat)i) - _char.GetStat((Stat)i)) + ")");
            var statProgressColor = (float)_char.GetStatXp((Stat)i) / _char.GetStatXpMax((Stat)i);
            _statProgress[i].TextColor = new Color(127, (int)(280 * statProgressColor), (int)(243 * statProgressColor));
            _statProgress[i].SetText(_char.GetStatXp((Stat)i) + " / " + _char.GetStatXpMax((Stat)i));
            _progressStatsBar[i].Color = new Color((int)(200 * (1 - statProgressColor)), (int)(280 * statProgressColor),
                (int)(130 * statProgressColor) + 50, 1);
        }

        for (var i = 0; i < 5; i++)
        {
            _upgradeStatDetails[i + 3].SetText((DamageType)i +
                                               Language.GetTextValue(
                                                   "Mods.AnotherRpgModExpanded.Stats.DamageMultiplier") +
                                               Math.Round(_char.GetDamageMultiplier((DamageType)i), 2) + " x " +
                                               Math.Round(_char.GetDamageMultiplier((DamageType)i, 1), 2) + " = " +
                                               Math.Round(_char.GetDamageMultiplier((DamageType)i, 2) * 100, 2) + " %");
        }
        
        _upgradeStatDetails[0].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Health1") +
                                       _char.Player.statLifeMax2 + " ( " + _char.Player.statLifeMax / 20 +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Heartx") +
                                       Math.Round(_char.GetHealthMultiplier(), 2) + " x " +
                                       Math.Round(_char.GetHealthPerHeart(), 2) +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.HealthPerHeart"));
        _upgradeStatDetails[1].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Mana1") +
                                       _char.Player.statManaMax2 + " ( " + _char.Player.statManaMax / 20 +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Crystalx") +
                                       Math.Round(_char.GetManaPerStar(), 2) +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Manapercrystal"));
        _upgradeStatDetails[2].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Defense") +
                                       _char.Player.statDefense + " ( " + _char.BaseArmor +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armorx") +
                                       Math.Round(_char.GetDefenceMult(), 2) + " x " +
                                       Math.Round(_char.GetArmorMultiplier(), 2) +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.DefensePerArmor"));

        _upgradeStatDetails[8].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.CritChance") +
                                       Math.Round(_char.GetCriticalChanceBonus(), 2) + "%");
        _upgradeStatDetails[9].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.CritDamage") +
                                       Math.Round(_char.GetCriticalDamage() * 100, 2) + "%");
        _upgradeStatDetails[10].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.HealthRegen") +
                                        Math.Round((double)_char.Player.lifeRegen, 2) +
                                        Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.PerSec"));

        _upgradeStatDetails[11].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.ManaRegen") +
                                        Math.Round(_char.Player.manaRegen + _char.GetManaRegen(), 2) +
                                        Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.PerSec"));
        _pointsLeft.SetText(
            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Points") + _char.FreePoints + " / " +
            _char.TotalPoints,
            1, true);
    }

    private void DragStart(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Visible)
            return;
        _offset = new Vector2(evt.MousePosition.X - _statsPanel.Left.Pixels,
            evt.MousePosition.Y - _statsPanel.Top.Pixels);
        _dragging = true;
    }

    private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Visible)
            return;
        var end = evt.MousePosition;
        _dragging = false;

        _statsPanel.Left.Set(end.X - _offset.X, 0f);
        _statsPanel.Top.Set(end.Y - _offset.Y, 0f);

        Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var mousePosition = new Vector2(Main.mouseX, Main.mouseY);
        if (_statsPanel.ContainsPoint(mousePosition)) Main.LocalPlayer.mouseInterface = true;

        if (_dragging)
        {
            _statsPanel.Left.Set(mousePosition.X - _offset.X, 0f);
            _statsPanel.Top.Set(mousePosition.Y - _offset.Y, 0f);
            Recalculate();
        }
    }
}

internal class StatProgress : UIElement
{
    private Texture2D _texture;
    public Color Color;
    private const float ImageScale = 1f;
    public float left;

    private readonly Stat _stat;
    public float width;

    public StatProgress(Stat stat, Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        width = _texture.Width;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        Color = Color.White;
        this._stat = stat;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var player = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();
        var quotient = 1f;
        //Calculate quotient


        quotient = player.GetStatXp(_stat) / (float)player.GetStatXpMax(_stat);

        Width.Set(quotient * width, 0f);
        //Left.Set((1 - quotient) * width, 0);
        Recalculate(); // recalculate the position and size

        base.Draw(spriteBatch);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var lWidth = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f,
            new Rectangle(point1.X, point1.Y, lWidth, height), Color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
            0f);
    }
}

internal class ProgressBG : UIElement
{
    private readonly Texture2D _texture;
    public Color Color;
    private const float ImageScale = 1f;

    public ProgressBG(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        Color = Color.White;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f,
            new Rectangle(point1.X, point1.Y, width, height), Color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
            0f);
    }
}

internal class OpenStatButton : UIElement
{
    private Texture2D _texture;
    public float ImageScale = 1f;

    public OpenStatButton(Texture2D texture)
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

        spriteBatch.Draw(_texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, ImageScale,
            SpriteEffects.None, 0f);
    }
}