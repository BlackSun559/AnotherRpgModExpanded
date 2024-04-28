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

internal class OpenSTButton : UIState
{
    public static bool visible = true;
    private Texture2D Button;
    public bool hiden;
    public UIElement OpenSTPanel;
    public float scale = Config.vConfig.HealthBarScale;
    public float yOffSet = Config.vConfig.HealthBarYoffSet;

    public void Erase()
    {
        RemoveAllChildren();
    }

    public override void Update(GameTime gameTime)
    {
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

        base.Update(gameTime);
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    public void LoadTexture()
    {
        Button = ModContent
            .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/skill_tree", AssetRequestMode.ImmediateLoad).Value;
    }

    public void Reset()
    {
        Erase();
        yOffSet = Config.vConfig.HealthBarYoffSet;
        scale = Config.vConfig.HealthBarScale;

        OpenSTPanel = new UIElement();
        OpenSTPanel.SetPadding(0);
        OpenSTPanel.Left.Set(90 * scale, 0f);
        OpenSTPanel.Top.Set(Main.screenHeight - 175 * scale - yOffSet, 0f);
        OpenSTPanel.Width.Set(32 * scale, 0f);
        OpenSTPanel.Height.Set(64 * scale, 0f);
        OpenSTPanel.HAlign = 0;
        OpenSTPanel.VAlign = 0;


        var OpenButton = new OpenStatButton(Button);
        OpenButton.Left.Set(0, 0f);
        OpenButton.Top.Set(0, 0f);
        OpenButton.ImageScale = scale;
        OpenButton.Width.Set(32 * scale, 0f);
        OpenButton.Height.Set(64 * scale, 0f);
        OpenButton.OnLeftClick += OpenSTMenu;
        OpenButton.HAlign = 0;
        OpenButton.VAlign = 0;
        OpenSTPanel.Append(OpenButton);
        Recalculate();
        Append(OpenSTPanel);
    }

    public void OpenSTMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Config.gpConfig.RPGPlayer)
            return;

        SoundEngine.PlaySound(SoundID.MenuOpen);

        SkillTreeUi.visible = !SkillTreeUi.visible;

        if (SkillTreeUi.visible)
            SkillTreeUi.Instance.LoadSkillTree();
    }
}

internal class OpenStatsButton : UIState
{
    public static bool visible = true;
    private Texture2D Button;
    public bool hiden;
    public UIElement OpenStatsPanel;
    public float scale = Config.vConfig.HealthBarScale;
    public float yOffSet = Config.vConfig.HealthBarYoffSet;


    public void Erase()
    {
        RemoveAllChildren();
    }

    public override void Update(GameTime gameTime)
    {
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

        base.Update(gameTime);
    }

    public override void OnInitialize()
    {
        LoadTexture();
        Reset();
    }

    public void LoadTexture()
    {
        Button = ModContent
            .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/character", AssetRequestMode.ImmediateLoad).Value;
    }

    public void Reset()
    {
        Erase();
        yOffSet = Config.vConfig.HealthBarYoffSet;
        scale = Config.vConfig.HealthBarScale;

        OpenStatsPanel = new UIElement();
        OpenStatsPanel.SetPadding(0);
        OpenStatsPanel.Left.Set(57 * scale, 0f);
        OpenStatsPanel.Top.Set(Main.screenHeight - 175 * scale - yOffSet, 0f);
        OpenStatsPanel.Width.Set(32 * scale, 0f);
        OpenStatsPanel.Height.Set(64 * scale, 0f);
        OpenStatsPanel.HAlign = 0;
        OpenStatsPanel.VAlign = 0;

        var OpenButton = new OpenStatButton(Button);
        OpenButton.Left.Set(0, 0f);
        OpenButton.Top.Set(0, 0f);
        OpenButton.ImageScale = scale;
        OpenButton.Width.Set(32 * scale, 0f);
        OpenButton.Height.Set(64 * scale, 0f);
        OpenButton.OnLeftClick += OpenStatMenu;
        OpenButton.HAlign = 0;
        OpenButton.VAlign = 0;
        OpenStatsPanel.Append(OpenButton);
        Recalculate();
        Append(OpenStatsPanel);
    }

    public void OpenStatMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!Config.gpConfig.RPGPlayer)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Stats.Instance.LoadChar();
        Stats.visible = !Stats.visible;
    }
}

internal class Stats : UIState
{
    public static Stats Instance;
    public static bool visible;
    private float baseXOffset = 100;
    private float baseYOffset = 100;
    private RpgPlayer Char;
    public bool dragging;
    private UIText InfoStat;

    private readonly Color MainColor = new(75, 75, 255);

    private Vector2 offset;

    private UIText PointsLeft = new("");

    public StatProgress[] progressStatsBar = new StatProgress[8];
    public ProgressBG[] progressStatsBarBG = new ProgressBG[8];

    private UIText ResetText;
    private readonly Color SecondaryColor = new(150, 150, 255);
    private float SizeMultiplier = 1;
    private readonly UIText[] StatProgress = new UIText[8];
    public UIPanel statsPanel;
    private readonly UIText[] UpgradeStatDetails = new UIText[12];
    private readonly UIText[] UpgradeStatOver = new UIText[12];

    private readonly UIText[] UpgradeStatText = new UIText[8];
    private float XOffset = 120;
    private float YOffset = 35;

    public void LoadChar()
    {
        Char = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();
    }

    private void ResetTextHover(UIMouseEvent evt, UIElement listeningElement)
    {
        ResetText.TextColor = Color.White;
    }

    private void ResetTextOut(UIMouseEvent evt, UIElement listeningElement)
    {
        ResetText.TextColor = Color.Gray;
    }

    public override void OnInitialize()
    {
        SizeMultiplier = Main.screenHeight / 1080f;
        baseYOffset *= SizeMultiplier;
        baseXOffset *= SizeMultiplier;
        YOffset *= SizeMultiplier;
        XOffset *= SizeMultiplier;


        Instance = this;
        statsPanel = new UIPanel();
        statsPanel.SetPadding(0);
        statsPanel.Left.Set(400f * SizeMultiplier, 0f);
        statsPanel.Top.Set(100f * SizeMultiplier, 0f);
        statsPanel.Width.Set(1000 * SizeMultiplier, 0f);
        statsPanel.Height.Set(600 * SizeMultiplier, 0f);
        statsPanel.BackgroundColor = new Color(73, 94, 171, 150);

        statsPanel.OnLeftMouseDown += DragStart;
        statsPanel.OnLeftMouseUp += DragEnd;

        PointsLeft = new UIText("Points : 0 / 0", SizeMultiplier);
        PointsLeft.Left.Set(250 * SizeMultiplier, 0f);
        PointsLeft.Top.Set(20 * SizeMultiplier, 0f);
        PointsLeft.Width.Set(0, 0f);
        PointsLeft.Height.Set(0, 0f);
        statsPanel.Append(PointsLeft);


        ResetText = new UIText("RESET", SizeMultiplier, true)
        {
            TextColor = Color.Gray
        };
        ResetText.Left.Set(50 * SizeMultiplier, 0f);
        ResetText.Top.Set(20 * SizeMultiplier, 0f);
        ResetText.Width.Set(0, 0f);
        ResetText.Height.Set(0, 0f);
        ResetText.OnLeftClick += ResetStats;
        ResetText.OnMouseOver += ResetTextHover;
        ResetText.OnMouseOut += ResetTextOut;
        statsPanel.Append(ResetText);

        var Button = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay", AssetRequestMode.ImmediateLoad);
        for (var i = 0; i < 12; i++)
        {
            if (i < 8)
            {
                var UpgradeStatButton = new UIImageButton(Button);

                UpgradeStatButton.Left.Set(baseXOffset + XOffset * 2, 0f);
                UpgradeStatButton.Top.Set(baseYOffset + YOffset * i, 0f);
                UpgradeStatButton.Width.Set(22 * SizeMultiplier, 0f);
                UpgradeStatButton.Height.Set(22 * SizeMultiplier, 0f);
                var Statused = (Stat)i;
                UpgradeStatButton.OnMouseOver +=
                    (UIMouseEvent, UIElement) => UpdateStat(UIMouseEvent, UIElement, Statused);
                UpgradeStatButton.OnMouseOut += ResetOver;
                UpgradeStatButton.OnLeftClick += (UIMouseEvent, UIElement) =>
                    UpgradeStat(UIMouseEvent, UIElement, Statused, 1);
                UpgradeStatButton.OnRightClick += (UIMouseEvent, UIElement) =>
                    UpgradeStat(UIMouseEvent, UIElement, Statused, 5);
                UpgradeStatButton.OnMiddleClick += (UIMouseEvent, UIElement) =>
                    UpgradeStat(UIMouseEvent, UIElement, Statused, 25);
                UpgradeStatButton.OnScrollWheel += (UIMouseEvent, UIElement) =>
                    UpgradeStatWheel(UIMouseEvent, UIElement, Statused);
                statsPanel.Append(UpgradeStatButton);


                progressStatsBar[i] = new StatProgress((Stat)i,
                    ModContent.Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank",
                        AssetRequestMode.ImmediateLoad).Value);
                progressStatsBar[i].Left.Set(baseXOffset + XOffset * 1.0f, 0f);
                progressStatsBar[i].Top.Set(baseYOffset + YOffset * i + 6, 0f);
                progressStatsBar[i].Width.Set(105, 0);
                progressStatsBar[i].HAlign = 0;
                progressStatsBar[i].Height.Set(10, 0);
                progressStatsBar[i].width = 105;
                progressStatsBar[i].left = baseYOffset + YOffset * i;
                statsPanel.Append(progressStatsBar[i]);

                progressStatsBarBG[i] = new ProgressBG(ModContent
                    .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank", AssetRequestMode.ImmediateLoad)
                    .Value);
                progressStatsBarBG[i].Left.Set(baseXOffset + XOffset * 1.0f, 0f);
                progressStatsBarBG[i].Top.Set(baseYOffset + YOffset * i + 6, 0f);
                progressStatsBarBG[i].Width.Set(105, 0);
                progressStatsBarBG[i].HAlign = 0;
                progressStatsBarBG[i].Height.Set(10, 0);
                progressStatsBarBG[i].color = new Color(10, 0, 0, 128);
                progressStatsBar[i].left = baseYOffset + YOffset * i;

                statsPanel.Append(progressStatsBarBG[i]);

                StatProgress[i] = new UIText("0", SizeMultiplier);
                StatProgress[i].SetText("0/2");
                StatProgress[i].Left.Set(baseXOffset + XOffset * 2.3f, 0f);
                StatProgress[i].Top.Set(baseYOffset + YOffset * i, 0f);
                StatProgress[i].HAlign = 0f;
                StatProgress[i].VAlign = 0f;
                StatProgress[i].MinWidth.Set(150 * SizeMultiplier, 0);
                StatProgress[i].MaxWidth.Set(150 * SizeMultiplier, 0);

                statsPanel.Append(StatProgress[i]);


                UpgradeStatText[i] = new UIText("0", SizeMultiplier);
                UpgradeStatText[i].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Mana"));
                UpgradeStatText[i].Left.Set(baseXOffset - 75, 0f);
                UpgradeStatText[i].Top.Set(baseYOffset + YOffset * i, 0f);
                UpgradeStatText[i].HAlign = 0f;
                UpgradeStatText[i].VAlign = 0f;
                UpgradeStatText[i].MinWidth.Set(150 * SizeMultiplier, 0);
                UpgradeStatText[i].MaxWidth.Set(150 * SizeMultiplier, 0);
                statsPanel.Append(UpgradeStatText[i]);
            }

            InfoStat = new UIText("0", SizeMultiplier);
            InfoStat.SetText("");
            InfoStat.Left.Set(baseXOffset - 75 * SizeMultiplier, 0f);
            InfoStat.Top.Set(baseYOffset + 300 * SizeMultiplier, 0f);
            InfoStat.HAlign = 0f;
            InfoStat.VAlign = 0f;
            InfoStat.MinWidth.Set(150 * SizeMultiplier, 0);
            InfoStat.MaxWidth.Set(150 * SizeMultiplier, 0);
            statsPanel.Append(InfoStat);

            UpgradeStatDetails[i] = new UIText("", SizeMultiplier);

            if (i < 3 || i > 7)
                UpgradeStatDetails[i].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Health"));
            UpgradeStatDetails[i]
                .SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.MeleeDamageMultiplier"));
            UpgradeStatDetails[i].Left.Set(baseXOffset + XOffset * 2.9f, 0f);
            UpgradeStatDetails[i].Top.Set(baseYOffset + YOffset * i, 0f);
            UpgradeStatDetails[i].HAlign = 0f;
            UpgradeStatDetails[i].VAlign = 0f;
            UpgradeStatDetails[i].MinWidth.Set(300f * SizeMultiplier, 0);
            UpgradeStatDetails[i].MaxWidth.Set(300f * SizeMultiplier, 0);
            statsPanel.Append(UpgradeStatDetails[i]);

            UpgradeStatOver[i] = new UIText("", SizeMultiplier)
            {
                TextColor = Color.Aqua
            };
            UpgradeStatOver[i].SetText("");
            UpgradeStatOver[i].Left.Set(baseXOffset + XOffset * 6.7f, 0f);
            UpgradeStatOver[i].Top.Set(baseYOffset + YOffset * i, 0f);
            UpgradeStatOver[i].HAlign = 0f;
            UpgradeStatOver[i].VAlign = 0f;
            UpgradeStatOver[i].MinWidth.Set(20f * SizeMultiplier, 0);
            UpgradeStatOver[i].MaxWidth.Set(20f * SizeMultiplier, 0);
            statsPanel.Append(UpgradeStatOver[i]);
        }

        Append(statsPanel);
    }

    private float GetCritImprov()
    {
        return Math.Abs(Mathf.Round(
            Mathf.Pow(Char.GetStatImproved(Stat.Agi) + Char.GetStatImproved(Stat.Str) + 1, 0.8f) * 0.005f -
            Char.GetCriticalDamage(), 2));
    }

    private float GetCritChanceImprov()
    {
        return Math.Abs(Mathf.Round(
            Mathf.Pow(Char.GetStatImproved(Stat.Foc) + Char.GetStatImproved(Stat.Dex) + 1, 0.8f) * 0.05f -
            Char.GetCriticalChanceBonus(), 3));
    }

    public void UpdateStat(UIMouseEvent evt, UIElement listeningElement, Stat stat)
    {
        Recalculate();

        if (Char == null) LoadChar();

        for (var i = 0; i < 12; i++) UpgradeStatOver[i].TextColor = SecondaryColor;
        switch (stat)
        {
            case Stat.Vit:
                UpgradeStatOver[0].SetText("+ " + Char.Player.statLifeMax / 20f * 0.65f * Char.StatMultiplier + " HP");
                UpgradeStatOver[0].TextColor = MainColor;
                UpgradeStatOver[2].SetText("+ " + Char.BaseArmor * 0.0025f * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armor"));
                UpgradeStatOver[2].TextColor = SecondaryColor;
                UpgradeStatOver[10].SetText("+ " + 0.02f * Char.StatMultiplier + " HP/Sec");
                break;
            case Stat.Foc:
                UpgradeStatOver[1]
                    .SetText("+ " + Char.Player.statManaMax / 20f * 0.02f * Char.StatMultiplier + " Mana");
                UpgradeStatOver[1].TextColor = MainColor;
                UpgradeStatOver[7].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[7].TextColor = SecondaryColor;
                UpgradeStatOver[8].SetText("+ " + GetCritChanceImprov() + " %");

                break;
            case Stat.Cons:
                UpgradeStatOver[0].SetText("+ " + Char.Player.statLifeMax / 20f * 0.325f * Char.StatMultiplier + " HP");
                UpgradeStatOver[0].TextColor = SecondaryColor;
                UpgradeStatOver[2].SetText("+ " + Char.BaseArmor * 0.006f * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armor"));
                UpgradeStatOver[2].TextColor = MainColor;
                UpgradeStatOver[10].SetText("+ " + 0.02f * Char.StatMultiplier + " HP/Sec");
                break;
            case Stat.Str:
                UpgradeStatOver[3].SetText("+ " + RpgPlayer.MainStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[3].TextColor = MainColor;
                UpgradeStatOver[5].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[5].TextColor = SecondaryColor;
                UpgradeStatOver[9].SetText("+ " + GetCritImprov() * 0.01f + " %");
                break;
            case Stat.Agi:
                UpgradeStatOver[4].SetText("+ " + RpgPlayer.MainStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[4].TextColor = MainColor;
                UpgradeStatOver[3].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[3].TextColor = SecondaryColor;
                UpgradeStatOver[9].SetText("+ " + GetCritImprov() * 0.01f + " %");
                break;
            case Stat.Dex:
                UpgradeStatOver[5].SetText("+ " + RpgPlayer.MainStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[5].TextColor = MainColor;
                UpgradeStatOver[4].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[4].TextColor = SecondaryColor;
                UpgradeStatOver[8].SetText("+ " + GetCritChanceImprov() + " %");
                break;
            case Stat.Int:
                UpgradeStatOver[6].SetText("+ " + RpgPlayer.MainStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[6].TextColor = MainColor;
                UpgradeStatOver[1]
                    .SetText("+ " + Char.Player.statManaMax / 20f * 0.05f * Char.StatMultiplier + " Mana");
                UpgradeStatOver[1].TextColor = SecondaryColor;
                UpgradeStatOver[11].SetText("+ " + 0.02f * Char.StatMultiplier + " MP/Sec");
                break;
            case Stat.Spr:
                UpgradeStatOver[7].SetText("+ " + RpgPlayer.MainStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[7].TextColor = MainColor;
                UpgradeStatOver[6].SetText("+ " + RpgPlayer.SecondaryStatsMultiplier * Char.StatMultiplier +
                                           Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Multiplier"));
                UpgradeStatOver[6].TextColor = SecondaryColor;
                UpgradeStatOver[11].SetText("+ " + 0.02f * Char.StatMultiplier + " MP/Sec");
                break;
        }

        InfoStat.SetText(AdditionalInfo.GetAdditionalStatInfo(stat));
    }

    public void ResetOver(UIMouseEvent evt, UIElement listeningElement)
    {
        for (var i = 0; i < 12; i++) UpgradeStatOver[i].SetText("");
        InfoStat.SetText("");
    }

    private void ResetStats(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!visible)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Char.ResetStats();
    }

    private void UpgradeStat(UIMouseEvent evt, UIElement listeningElement, Stat stat, int amount)
    {
        if (!visible)
            return;
        SoundEngine.PlaySound(SoundID.MenuOpen);

        if (Main.keyState.PressingShift())
        {
            if (Main.keyState.IsKeyDown(Keys.LeftControl))
            {
                while (Char.FreePoints > 0)
                    Char.SpendPoints(stat, Char.GetStatXpMax(stat) - Char.GetStatXp(stat));
                return;
            }

            for (var i = 0; i < amount; i++)
                Char.SpendPoints(stat, Char.GetStatXpMax(stat) - Char.GetStatXp(stat));
            return;
        }

        Char.SpendPoints(stat, amount);
    }

    private void UpgradeStatWheel(UIScrollWheelEvent evt, UIElement listeningElement, Stat stat)
    {
        if (!visible)
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
                while (Char.FreePoints > 0)
                    Char.SpendPoints(stat, Char.GetStatXpMax(stat) - Char.GetStatXp(stat));
                return;
            }

            for (var i = 0; i < amount; i++)
                Char.SpendPoints(stat, Char.GetStatXpMax(stat) - Char.GetStatXp(stat));
            return;
        }

        Char.SpendPoints(stat, amount);
    }

    public override void Update(GameTime gameTime)
    {
        if (visible)
            UpdateStats();
        Recalculate();
        base.Update(gameTime);
    }

    private void UpdateStats()
    {
        float statprogresscolor = 0;
        for (var i = 0; i < 8; i++)
        {
            var pre = "";
            if (Char.GetStatImproved((Stat)i) > Char.GetStat((Stat)i))
                pre = "+";
            UpgradeStatText[i].SetText((Stat)i + " : " + Char.GetNaturalStat((Stat)i) + " + " +
                                       Char.GetAddStat((Stat)i) + " (" + pre +
                                       (Char.GetStatImproved((Stat)i) - Char.GetStat((Stat)i)) + ")");
            statprogresscolor = (float)Char.GetStatXp((Stat)i) / Char.GetStatXpMax((Stat)i);
            StatProgress[i].TextColor = new Color(127, (int)(280 * statprogresscolor), (int)(243 * statprogresscolor));
            StatProgress[i].SetText(Char.GetStatXp((Stat)i) + " / " + Char.GetStatXpMax((Stat)i));
            progressStatsBar[i].color = new Color((int)(200 * (1 - statprogresscolor)), (int)(280 * statprogresscolor),
                (int)(130 * statprogresscolor) + 50, 1);
            ;
        }

        for (var i = 0; i < 5; i++)
            UpgradeStatDetails[i + 3].SetText((DamageType)i +
                                              Language.GetTextValue(
                                                  "Mods.AnotherRpgModExpanded.Stats.DamageMultiplier") +
                                              Math.Round(Char.GetDamageMultiplier((DamageType)i), 2) + " x " +
                                              Math.Round(Char.GetDamageMultiplier((DamageType)i, 1), 2) + " = " +
                                              Math.Round(Char.GetDamageMultiplier((DamageType)i, 2) * 100, 2) + " %");
        UpgradeStatDetails[0].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Health1") +
                                      Char.Player.statLifeMax2 + " ( " + Char.Player.statLifeMax / 20 +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Heartx") +
                                      Math.Round(Char.GetHealthMultiplier(), 2) + " x " +
                                      Math.Round(Char.GetHealthPerHeart(), 2) +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.HealthPerHeart"));
        UpgradeStatDetails[1].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Mana1") +
                                      Char.Player.statManaMax2 + " ( " + Char.Player.statManaMax / 20 +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Crystalx") +
                                      Math.Round(Char.GetManaPerStar(), 2) +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Manapercrystal"));
        UpgradeStatDetails[2].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Defense") +
                                      Char.Player.statDefense + " ( " + Char.BaseArmor +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Armorx") +
                                      Math.Round(Char.GetDefenceMult(), 2) + " x " +
                                      Math.Round(Char.GetArmorMultiplier(), 2) +
                                      Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.DefensePerArmor"));

        UpgradeStatDetails[8].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.CritChance") +
                                      Math.Round(Char.GetCriticalChanceBonus(), 2) + "%");
        UpgradeStatDetails[9].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.CritDamage") +
                                      Math.Round(Char.GetCriticalDamage() * 100, 2) + "%");
        UpgradeStatDetails[10].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.HealthRegen") +
                                       Math.Round((double)Char.Player.lifeRegen, 2) +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.PerSec"));

        UpgradeStatDetails[11].SetText(Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.ManaRegen") +
                                       Math.Round(Char.Player.manaRegen + Char.GetManaRegen(), 2) +
                                       Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.PerSec"));
        PointsLeft.SetText(
            Language.GetTextValue("Mods.AnotherRpgModExpanded.Stats.Points") + Char.FreePoints + " / " + Char.TotalPoints,
            1, true);
    }

    private void DragStart(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!visible)
            return;
        offset = new Vector2(evt.MousePosition.X - statsPanel.Left.Pixels, evt.MousePosition.Y - statsPanel.Top.Pixels);
        dragging = true;
    }

    private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!visible)
            return;
        var end = evt.MousePosition;
        dragging = false;

        statsPanel.Left.Set(end.X - offset.X, 0f);
        statsPanel.Top.Set(end.Y - offset.Y, 0f);

        Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
        if (statsPanel.ContainsPoint(MousePosition)) Main.LocalPlayer.mouseInterface = true;

        if (dragging)
        {
            statsPanel.Left.Set(MousePosition.X - offset.X, 0f);
            statsPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
            Recalculate();
        }
    }
}

internal class StatProgress : UIElement
{
    private Texture2D _texture;
    public Color color;
    public float ImageScale = 1f;
    public float left;

    private readonly Stat stat;
    public float width;

    public StatProgress(Stat stat, Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        width = _texture.Width;
        Left.Set(0, 0f);
        Top.Set(0, 0f);
        color = Color.White;
        this.stat = stat;
    }

    public void SetImage(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var Player = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();
        var quotient = 1f;
        //Calculate quotient


        quotient = Player.GetStatXp(stat) / (float)Player.GetStatXpMax(stat);

        Width.Set(quotient * width, 0f);
        //Left.Set((1 - quotient) * width, 0);
        Recalculate(); // recalculate the position and size

        base.Draw(spriteBatch);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f,
            new Rectangle(point1.X, point1.Y, width, height), color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
            0f);
    }
}

internal class ProgressBG : UIElement
{
    private readonly Texture2D _texture;
    public Color color;
    public float ImageScale = 1f;

    public ProgressBG(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width, 0f);
        Height.Set(_texture.Height, 0f);
        color = Color.White;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f,
            new Rectangle(point1.X, point1.Y, width, height), color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None,
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