using System;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace AnotherRpgModExpanded.Items;

internal class AdditionalProjectile : ItemNodeAdvanced
{
    protected new string m_Desc = "+ X Projectile";
    protected new bool m_isAscend = true;

    protected new string m_Name = "(Ascended) Multiple Projectile";
    protected new NodeCategory m_NodeCategory = NodeCategory.Other;


    public int ProjectileAmmount;

    public override bool IsAscend => m_isAscend;

    public override string GetName => m_Name;

    public override string GetDesc => "+ " + ProjectileAmmount * GetLevel.Clamp(1, GetMaxLevel) + " Projectile";

    public override void OnShoot(EntitySource_ItemUse_WithAmmo source, Item item, Player Player, ref Vector2 position,
        ref Vector2 Velocity, ref int type, ref int damage, ref float knockBack)
    {
        for (var i = 0; i < ProjectileAmmount * GetLevel; i++)
        {
            var spread = 10 * 0.0174f; //20 degree cone
            var baseSpeed = Velocity.Length() * (0.9f + 0.2f * Main.rand.NextFloat());
            var baseAngle = MathF.Atan2(Velocity.X, Velocity.Y);
            var randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
            var newVelocity = baseSpeed * new Vector2(MathF.Sin(randomAngle), MathF.Cos(randomAngle));

            var projnum =
                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockBack, Player.whoAmI);
            Main.projectile[projnum].friendly = true;
            Main.projectile[projnum].hostile = false;
            Main.projectile[projnum].originalDamage = damage;
        }
    }

    public override void SetPower(float value)
    {
        ProjectileAmmount = Mathf.FloorInt(Mathf.Pow(value, 0.25f));
        power = value;
    }

    public override void LoadValue(string saveValue)
    {
        power = saveValue.SafeFloatParse();
        SetPower(power);
    }

    public override string GetSaveValue()
    {
        return power.ToString();
    }
}