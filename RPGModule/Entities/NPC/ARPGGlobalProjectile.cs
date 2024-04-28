using AnotherRpgModExpanded.Utils;
using Terraria;
using Terraria.ModLoader;

namespace AnotherRpgModExpanded.RPGModule.Entities;

internal class ARPGGlobalProjectile : GlobalProjectile
{
    private bool init;

    public Item itemOrigin;

    public override bool InstancePerEntity => true;


    public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
    {
        var projectilelevel =
            (int)((WorldManager.GetWorldLevelMultiplier(Config.NPCConfig.NPCProjectileDamageLevel) +
                   WorldManager.GetWorldAdditionalLevel()) * Config.NPCConfig.NpclevelMultiplier);


        /*debug

        Main.NewText("projectile base level : " + Config.NPCConfig.NPCProjectileDamageLevel);
        Main.NewText("World Day : " + WorldManager.Day);
        Main.NewText("projectile day level : " + WorldManager.GetWorldLevelMultiplier(Config.NPCConfig.NPCProjectileDamageLevel));
        Main.NewText("projectile World level : " + WorldManager.GetWorldAdditionalLevel());
        Main.NewText("npc level multiplier : " + Config.NPCConfig.NpclevelMultiplier);

        Main.NewText("projectile level : " + (int)(WorldManager.GetWorldLevelMultiplier(Config.NPCConfig.NPCProjectileDamageLevel) * Config.NPCConfig.NpclevelMultiplier));
        Main.NewText("projectile base damage : " + projectile.damage);
        Main.NewText("projectile damage multiplier : " + Mathf.Pow(1 + projectilelevel * 0.02f, 0.95f) * Config.NPCConfig.NpcDamageMultiplier);
        */

        projectile.damage =
            Mathf.HugeCalc(
                Mathf.FloorInt(projectile.damage * (1 + projectilelevel * 0.05f) *
                               Config.NPCConfig.NpcDamageMultiplier), projectile.damage);
    }


    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (projectile.npcProj)
        {
            var projectilelevel =
                (int)(WorldManager.GetWorldLevelMultiplier(Config.NPCConfig.NPCProjectileDamageLevel) *
                      Config.NPCConfig.NpclevelMultiplier);

            projectile.damage =
                Mathf.HugeCalc(
                    Mathf.FloorInt(projectile.damage * Mathf.Pow(1 + projectilelevel * 0.02f, 0.95f) *
                                   Config.NPCConfig.NpcDamageMultiplier), projectile.damage);
        }

        base.ModifyHitNPC(projectile, target, ref modifiers);
    }

    /*
    public override void SetDefaults(Projectile projectile)
    {
        if (projectile.npcProj)
        {
            int projectilelevel = WorldManager.GetWorldLevelMultiplier(1);

            projectile.damage = Mathf.HugeCalc(Mathf.FloorInt(projectile.damage * Mathf.Pow(1 + projectilelevel * 0.02f, 0.95f)), projectile.damage);
        }

        base.SetDefaults(projectile);
    }
    */

    public override bool PreAI(Projectile projectile)
    {
        if (init)
            return base.PreAI(projectile);

        if (projectile.friendly)
            return base.PreAI(projectile);

        if (!projectile.npcProj && projectile.minion)
        {
            var p = Main.player[projectile.owner];
            itemOrigin = p.HeldItem;
        }


        init = true;


        return base.PreAI(projectile);
    }
}