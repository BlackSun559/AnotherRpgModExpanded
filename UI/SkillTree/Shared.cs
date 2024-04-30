using System;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnotherRpgModExpanded.UI;

internal class Skill : UIElement
{
    private readonly Texture2D _texture;
    public Color color = Color.White;

    public Skill(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();

        spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero,
            SkillTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
    }
}

internal class ItemSkill : UIElement
{
    private Texture2D _texture;

    public Color color = Color.White;
    public bool Hidden = false;

    public ItemSkill(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
    }

    public Texture2D SetTexture
    {
        set => _texture = value;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Hidden)
            return;
        var dimensions = GetDimensions();

        spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero,
            ItemTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
    }
}

internal class SkillText : UIText
{
    public NodeParent node;

    public SkillText(string text, NodeParent node, float textScale = 1, bool large = false) : base(text, textScale,
        large)
    {
        this.node = node;
    }
}

internal class ItemSkillText : UIText
{
    public ItemNode node;

    public ItemSkillText(string text, ItemNode node, float textScale = 1, bool large = false) : base(text, textScale,
        large)
    {
        this.node = node;
    }
}

internal class ItemSkillPanel : UIPanel
{
    private readonly Texture2D _texture;
    public Vector2 basePos;
    public Color color = Color.White;
    public bool Hidden = false;
    public ItemNode node;
    public ItemSkill skill;

    public ItemSkillPanel(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width * ItemTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(_texture.Height * ItemTreeUi.Instance.sizeMultplier, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();

        if (Hidden)
            return;
        spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero,
            ItemTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
    }
}

internal class SkillPanel : UIPanel
{
    private readonly Texture2D _texture;
    public Vector2 basePos;
    public Color color = Color.White;
    public NodeParent node;
    public Skill skill;

    public SkillPanel(Texture2D texture)
    {
        _texture = texture;
        Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();

        spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero,
            SkillTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
    }
}

internal class ItemConnection : UIElement
{
    public Vector2 basePos;
    public bool bg = false;
    public Color color;
    public bool Hidden = false;
    public float m_rotation;
    public int neighboorID;
    public int nodeID;

    private readonly Texture2D texture = ModContent
        .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank", AssetRequestMode.ImmediateLoad).Value;

    public ItemConnection(float rotation, float distance, float height)
    {
        Width.Set(distance * ItemTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(height * ItemTreeUi.Instance.sizeMultplier, 0f);
        m_rotation = rotation;
        color = Color.White;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Hidden)
            return;
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(texture, dimensions.Position(), new Rectangle(point1.X, point1.Y, width, height), color,
            m_rotation, bg ? new Vector2(0, 5) : new Vector2(0, 3), 1, SpriteEffects.None, 0f);
    }
}

internal class Connection : UIElement
{
    public Vector2 basePos;
    public bool bg = false;
    public Color color;
    public float m_rotation;
    public NodeParent neighboor;
    public NodeParent node;

    private readonly Texture2D texture = ModContent
        .Request<Texture2D>("AnotherRpgModExpanded/Textures/UI/Blank", AssetRequestMode.ImmediateLoad).Value;

    public Connection(float rotation, float distance, float height)
    {
        Width.Set(distance * SkillTreeUi.Instance.sizeMultplier, 0f);
        Height.Set(height * SkillTreeUi.Instance.sizeMultplier, 0f);
        m_rotation = rotation;
        color = Color.White;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point1 = new Point((int)dimensions.X, (int)dimensions.Y);
        var width = (int)Math.Ceiling(dimensions.Width);
        var height = (int)Math.Ceiling(dimensions.Height);

        spriteBatch.Draw(texture, dimensions.Position(), new Rectangle(point1.X, point1.Y, width, height), color,
            m_rotation, bg ? new Vector2(0, 5) : new Vector2(0, 3), 1, SpriteEffects.None, 0f);
    }
}