﻿using System;
using System.Collections.Generic;
using System.Linq;
using AnotherRpgModExpanded.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AnotherRpgModExpanded.RPGModule.Entities;

internal class NPCUtils
{
    public static Dictionary<NPCModifier, int> modifierWeight = new()
    {
        { NPCModifier.None, 200 },
        { NPCModifier.Cluster, 10 },
        { NPCModifier.Size, 200 },
        { NPCModifier.Berserker, 100 },
        { NPCModifier.Golden, 10 },
        { NPCModifier.Vampire, 50 },
        { NPCModifier.ArmorBreaker, 50 },
        { NPCModifier.Dancer, 25 }
    };

    public static Dictionary<NPCRank, float[]> NPCRankStats = new()
    {
        //Rank                                  HPMult,   DMGMult,    DefMult
        { NPCRank.Weak, new float[3] { 0.8f, 0.9f, 0.5f } },
        { NPCRank.Normal, new float[3] { 1, 1, 1 } },
        { NPCRank.Alpha, new float[3] { 1.2f, 1.05f, 1.1f } },
        { NPCRank.Elite, new float[3] { 1.4f, 1.10f, 1.2f } },
        { NPCRank.Legendary, new float[3] { 2, 1.2f, 1.4f } },
        { NPCRank.Mythical, new float[3] { 3.5f, 1.3f, 1.6f } },
        { NPCRank.Godly, new float[3] { 5, 1.4f, 1.8f } },
        { NPCRank.DIO, new float[3] { 10, 1.5f, 2f } },

        //ASCENDED WORLD :
        { NPCRank.LimitBreaked, new float[3] { 5, 2f, 3f } },
        { NPCRank.Raised, new float[3] { 10, 3.5f, 4f } },
        { NPCRank.Ascended, new float[3] { 20, 5f, 5f } },
        { NPCRank.HighAscended, new float[3] { 35, 6f, 6f } },
        { NPCRank.PeakAscended, new float[3] { 50, 7f, 7f } },
        { NPCRank.Transcendental, new float[3] { 200, 8.5f, 8.5f } },
        { NPCRank.TransDimensional, new float[3] { 1000f, 10f, 10f } },

        {
            NPCRank.DioAboveHeaven, new float[3] { 9999999999999f, 10, 0f }
        } //int max health, 10X damage, no defense, Good luck
    };

    public static Dictionary<string, float[]> NPCSizeStats = new()
    {
        //Rank                              HPMult,     DMGMult,    DefMult     Size
        { "Mini", new float[4] { 0.8f, 0.9f, 0.7f, 0.6f } },
        { "Giant", new float[4] { 1.8f, 1.05f, 1.05f, 1.2f } },
        { "Colossus", new float[4] { 2.2f, 1.10f, 1.10f, 1.5f } },
        { "Titan", new float[4] { 2.5f, 1.15f, 1.2f, 1.8f } }
    };

    public static float DELTATIME = 1f / 60f;

    public static int GetExp(NPC npc)
    {
        if (!WorldManager.Ascended)
            return Mathf.CeilInt(Math.Pow(npc.lifeMax / 8 + npc.damage * 1.2 + npc.defense * 1.7f, 0.9f));
        return Mathf.CeilInt(Math.Pow(npc.lifeMax / 5 + npc.damage * 1.3 + npc.defense * 1.9f, 1.05f) * 1.5f);
    }

    private static NPCModifier AddRandomModifier(List<NPCModifier> pool)
    {
        var totalWeight = 0;
        for (var i = 0; i < pool.Count; i++)
            totalWeight += modifierWeight[pool[i]];

        var rn = Mathf.RandomInt(0, totalWeight);
        var checkingWeight = 0;
        for (var i = 0; i < pool.Count; i++)
        {
            if (rn < checkingWeight + modifierWeight[pool[i]])
                return pool[i];
            checkingWeight += modifierWeight[pool[i]];
        }

        return pool[pool.Count - 1];
    }

    public static NPCModifier GetModifier(NPCRank rank, NPC npc)
    {
        if (npc.dontCountMe)
            return NPCModifier.None;

        if (!Config.NPCConfig.NPCModifier)
            return NPCModifier.None;

        if (npc.boss && !Config.NPCConfig.BossModifier)
            return NPCModifier.None;

        var maxModifier = 1;

        switch (rank)
        {
            case NPCRank.Weak:
                return 0;
            case NPCRank.Normal:
            case NPCRank.LimitBreaked:
                maxModifier = Mathf.Random(0, 3) < 1 ? 0 : 1;
                break;
            case NPCRank.Alpha:
            case NPCRank.Raised:
                maxModifier = 1;
                break;
            case NPCRank.Elite:

            case NPCRank.Ascended:
                maxModifier = Mathf.Random(0, 3) < 1 ? 1 : 2;
                break;
            case NPCRank.Legendary:
            case NPCRank.HighAscended:
                maxModifier = 2;
                break;
            case NPCRank.Mythical:
            case NPCRank.PeakAscended:
                maxModifier = 3;
                break;
            case NPCRank.Godly:
            case NPCRank.Transcendental:
                maxModifier = 4;
                break;
            case NPCRank.TransDimensional:
                maxModifier = 5;
                break;
            case NPCRank.DIO:
            case NPCRank.DioAboveHeaven:
                maxModifier = (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length;
                break;
        }

        NPCModifier modifiers = 0;
        //if npc.aiStyle == 3

        var modifiersPool = (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).ToList();

        if (npc.boss)
        {
            modifiersPool.Remove(NPCModifier.Size);
            if (maxModifier == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                maxModifier -= 1;
            if (!Config.NPCConfig.BossClustered)
            {
                modifiersPool.Remove(NPCModifier.Cluster);
                if (maxModifier + 1 == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                    maxModifier -= 1;
            }
        }

        else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody ||
                 npc.type == NPCID.EaterofWorldsTail)
        {
            modifiersPool.Remove(NPCModifier.Cluster);
            modifiersPool.Remove(NPCModifier.Size);
            if (maxModifier == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                maxModifier -= 2;
        }

        for (var i = 0; i < maxModifier; i++)
        {
            var modid = AddRandomModifier(modifiersPool);
            modifiers = modifiers | modid;
            modifiersPool.Remove(modid);
        }

        return modifiers;
    }

    public static string GetNpcNameChange(NPC npc, int tier, int level, NPCRank rank)
    {
        var name = npc.GivenOrTypeName;
        if (npc.townNPC)
            return name;
        if (name == "")
            name = npc.GivenName;
        if (name == "")
            name = npc.TypeName;
        if (name == "")
            return name;


        var sufix = " the ";
        var Prefix = "";
        /*
    if (Config.gpConfig.NPCProgress)
        Prefix+= "Lvl. " + (tier + level) + " ";
    if (WorldManager.GetWorldAdditionalLevel() > 0)
        Prefix += "(+" + GetWorldTier(npc,level) + ") ";
        */
        switch (rank)
        {
            case NPCRank.Weak:
                Prefix += "Weak ";
                break;
            case NPCRank.Alpha:
                Prefix += "Alpha ";
                break;
            case NPCRank.Elite:
                Prefix += "Elite ";
                break;
            case NPCRank.Legendary:
                Prefix += "Legendary ";
                break;
            case NPCRank.Mythical:
                Prefix += "Mythical ";
                break;
            case NPCRank.Godly:
                Prefix += "Godly ";
                break;
            case NPCRank.DIO:
                Prefix += "Kono Dio Da ";
                break;
            case NPCRank.Raised:
                Prefix += "Raised ";
                break;
            case NPCRank.LimitBreaked:
                Prefix += "Limits Breaked ";
                break;
            case NPCRank.Ascended:
                Prefix += "Ascended ";
                break;
            case NPCRank.HighAscended:
                Prefix += "High Ascent ";
                break;
            case NPCRank.PeakAscended:
                Prefix += "Peak Ascent ";
                break;
            case NPCRank.Transcendental:
                Prefix += "Transcendental ";
                break;
            case NPCRank.TransDimensional:
                Prefix += "TransDimensional ";
                break;
            case NPCRank.DioAboveHeaven:
                Prefix += "Dio Above Heaven ";
                break;
        }

        var anpc = npc.GetGlobalNPC<ARPGGlobalNPC>();
        if (anpc.HaveModifier(NPCModifier.Dancer))
            sufix += "Dancing ";

        if (anpc.HaveModifier(NPCModifier.Cluster))
            sufix += "Clustered ";

        if (anpc.HaveModifier(NPCModifier.Golden))
            sufix += "Golden ";

        if (anpc.HaveModifier(NPCModifier.ArmorBreaker))
            sufix += "Armor Breaker ";

        if (anpc.HaveModifier(NPCModifier.Berserker))
            sufix += "Berserker ";

        if (anpc.HaveModifier(NPCModifier.Vampire))
            sufix += "Vampire ";

        if (anpc.HaveModifier(NPCModifier.Size))
        {
            var size = anpc.GetBufferProperty("size");
            Prefix += size + " ";
        }

        if (sufix == " the ")
            sufix = "";
        return Prefix + name + sufix;
    }

    public static NPC SetRankStat(NPC npc, NPCRank rank)
    {
        if (rank == NPCRank.Normal)
            return npc;

        if (rank == NPCRank.Weak)
        {
            npc.lifeMax = Mathf.CeilInt(npc.lifeMax * NPCRankStats[rank][0]);
            if (npc.damage > 0)
                npc.damage = Mathf.CeilInt(npc.damage * NPCRankStats[rank][1]);
            if (npc.defense > 0)
                npc.defense = Mathf.CeilInt(npc.defense * NPCRankStats[rank][2]);
        }
        else
        {
            npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCRankStats[rank][0]), npc.lifeMax);
            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCRankStats[rank][1]), npc.damage);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCRankStats[rank][2]), npc.defense);
        }

        npc.life = npc.lifeMax;

        return npc;
    }

    public static NPC SetSizeStat(NPC npc, string size)
    {
        if (size == "Growther")
            size = npc.GetGlobalNPC<ARPGGlobalNPC>().GetBufferProperty("GrowtherStep");

        if (size == "Normal")
            return npc;

        if (size == "Mini")
        {
            npc.lifeMax = Mathf.CeilInt(npc.lifeMax * NPCSizeStats[size][0]);
            if (npc.damage > 0)
                npc.damage = Mathf.CeilInt(npc.damage * NPCSizeStats[size][1]);
            if (npc.defense > 0)
                npc.defense = Mathf.CeilInt(npc.defense * NPCSizeStats[size][2]);
        }
        else
        {
            npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCSizeStats[size][0]), npc.lifeMax);
            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCSizeStats[size][1]), npc.damage);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCSizeStats[size][2]), npc.defense);
        }

        npc.scale *= NPCSizeStats[size][3];

        npc.life = npc.lifeMax;

        return npc;
    }

    public static NPC SizeShiftMult(NPC npc, string size)
    {
        if (size == "Mini")
        {
            npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax / NPCSizeStats["Mini"][0]), npc.lifeMax);
            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage / NPCSizeStats["Mini"][1]), npc.damage);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense / NPCSizeStats["Mini"][2]), npc.defense);

            npc.scale /= NPCSizeStats["Mini"][3];
        }
        else if (size == "Normal")
        {
            npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCSizeStats["Giant"][0]), npc.lifeMax);
            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCSizeStats["Giant"][1]), npc.damage);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCSizeStats["Giant"][2]), npc.defense);

            npc.scale *= NPCSizeStats["Giant"][3];
        }
        else if (size == "Giant")
        {
            npc.lifeMax =
                Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax / NPCSizeStats["Giant"][0] * NPCSizeStats["Colossus"][0]),
                    npc.lifeMax);
            if (npc.damage > 0)
                npc.damage =
                    Mathf.HugeCalc(Mathf.CeilInt(npc.damage / NPCSizeStats["Giant"][1] * NPCSizeStats["Colossus"][1]),
                        npc.damage);
            if (npc.defense > 0)
                npc.defense =
                    Mathf.HugeCalc(Mathf.CeilInt(npc.defense / NPCSizeStats["Giant"][2] * NPCSizeStats["Colossus"][2]),
                        npc.defense);

            npc.scale *= NPCSizeStats["Colossus"][3] / NPCSizeStats["Giant"][3];
        }
        else if (size == "Colossus")
        {
            npc.lifeMax =
                Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax / NPCSizeStats["Colossus"][0] * NPCSizeStats["Titan"][0]),
                    npc.lifeMax);
            if (npc.damage > 0)
                npc.damage =
                    Mathf.HugeCalc(Mathf.CeilInt(npc.damage / NPCSizeStats["Colossus"][1] * NPCSizeStats["Titan"][1]),
                        npc.damage);
            if (npc.defense > 0)
                npc.defense =
                    Mathf.HugeCalc(Mathf.CeilInt(npc.defense / NPCSizeStats["Colossus"][2] * NPCSizeStats["Titan"][2]),
                        npc.defense);

            npc.scale *= NPCSizeStats["Titan"][3] / NPCSizeStats["Colossus"][3];
        }

        return npc;
    }

    public static NPC SetModifierStat(NPC npc)
    {
        var ArNpc = npc.GetGlobalNPC<ARPGGlobalNPC>();
        if (ArNpc.HaveModifier(NPCModifier.Golden))
        {
            npc.lifeMax = Mathf.HugeCalc((int)(npc.lifeMax * 3f), npc.lifeMax);

            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc((int)(npc.damage * 1.5f), npc.damage);

            npc.value += Item.buyPrice(0, 1, 50);
            npc.value *= 2 * ArNpc.getRank;
        }

        if (ArNpc.HaveModifier(NPCModifier.Size))
            npc = SetSizeStat(npc, ArNpc.GetBufferProperty("size"));

        if (ArNpc.HaveModifier(NPCModifier.Vampire))
            npc.lifeMax = Mathf.HugeCalc((int)(npc.lifeMax * 1.5f), npc.lifeMax);

        if (npc.damage > 0)
            npc.damage = Mathf.HugeCalc((int)(npc.damage * 1.3f), npc.damage);

        if (ArNpc.HaveModifier(NPCModifier.Berserker))
            npc.color = Color.Lerp(npc.color, new Color(1.0f, 0.0f, 0.0f), 0.3f);

        if (ArNpc.HaveModifier(NPCModifier.Golden))
            npc.color = Color.Lerp(npc.color, new Color(1.0f, 0.8f, 0.5f), 0.8f);

        npc.life = npc.lifeMax;

        return npc;
    }

    public static void SpawnSized(NPC npc, IEntitySource source, Dictionary<string, string> buffer)
    {
        var n = NPC.NewNPC(source,
            (int)npc.position.X,
            (int)npc.position.Y,
            npc.type
        );
        Main.npc[n].SetDefaults(Main.npc[n].netID);
        Main.npc[n].velocity.X = Mathf.RandomInt(-8, 8);
        Main.npc[n].velocity.Y = Mathf.RandomInt(-20, -2);
        Main.npc[n].GetGlobalNPC<ARPGGlobalNPC>().SetBufferProperty("clustered", "true");
    }

    public static float UpdateDOT(NPC npc)
    {
        float DoTDamage = 0;
        var life = npc.life;

        if (life > 1)
        {
            if (npc.HasBuff(BuffID.OnFire))
                DoTDamage +=
                    (Mathf.Logx(npc.lifeMax, 1.01f) * 0.25f * DELTATIME).Clamp(0, npc.lifeMax * 0.003f * DELTATIME);

            if (npc.HasBuff(BuffID.Burning))
                DoTDamage +=
                    (Mathf.Logx(npc.lifeMax, 1.01f) * 0.05f * DELTATIME).Clamp(0, npc.lifeMax * 0.002f * DELTATIME);

            if (npc.HasBuff(BuffID.Frostburn))
                DoTDamage +=
                    (Mathf.Logx(npc.lifeMax, 1.01f) * 0.25f * DELTATIME).Clamp(0, npc.lifeMax * 0.004f * DELTATIME);

            if (npc.HasBuff(BuffID.Venom))
                DoTDamage +=
                    (Mathf.Logx(npc.lifeMax, 1.01f) * 0.25f * DELTATIME).Clamp(0, npc.lifeMax * 0.005f * DELTATIME);
        }

        return DoTDamage;
    }

    public static NPC SetNPCStats(NPC npc, int level, int tier, NPCRank rank)
    {
        if (npc == null)
            return npc;

        if (!Config.NPCConfig.NPCProgress)
        {
            npc = SetRankStat(npc, rank);
            npc = SetModifierStat(npc);
            return npc;
        }

        if (npc.townNPC || npc.damage == 0)
        {
            npc.lifeMax = Mathf.HugeCalc(Mathf.FloorInt(npc.lifeMax * (0.5f + (tier + level) * 0.1f)), npc.lifeMax);
            if (npc.damage > 0)
                npc.damage = Mathf.HugeCalc(Mathf.FloorInt(npc.damage * (0.75f + level * 0.03f + tier * 0.06f)),
                    npc.damage);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.FloorInt(npc.defense * (0.8f + level * 0.012f + tier * 0.02f)),
                    npc.defense);

            if (npc.defense < 0)
                npc.defense = 0;
            if (npc.damage < 0)
                npc.damage = 0;

            npc.life = npc.lifeMax;

            return npc;
        }

        var power = 1.0f;
        if (Main.hardMode)
            power = 1.15f;

        if (Main.masterMode)
        {
            if (npc.boss)
            {
                if (npc.damage > 0)
                {
                    if (Mathf.HugeCalc(
                            Mathf.FloorInt(npc.damage * (0.35f + level * 0.015f + tier * 0.025f) *
                                           Config.NPCConfig.NpcDamageMultiplier * 0.75f), 1) < 250000)
                        npc.damage =
                            Mathf.HugeCalc(
                                Mathf.FloorInt(npc.damage * (0.35f + level * 0.015f + tier * 0.025f) *
                                               Config.NPCConfig.NpcDamageMultiplier * 0.75f), 1);
                    else
                        npc.damage = Mathf.FloorInt(250000 * Mathf.Logx(1 + level * 0.025f + tier * 0.05f, 7.5f) *
                                                    Config.NPCConfig.NpcDamageMultiplier);
                }

                npc.lifeMax =
                    Mathf.HugeCalc(
                        Mathf.FloorInt(Mathf.Pow(npc.lifeMax * (level * 0.025f + tier * 0.035f), 1.05f) *
                                       Config.NPCConfig.BossHealthMultiplier * Config.NPCConfig.NpcHealthMultiplier),
                        1);
            }
            else
            {
                if (npc.damage > 0)
                    npc.damage =
                        Mathf.HugeCalc(
                            Mathf.FloorInt(npc.damage * (0.75f + level * 0.015f + tier * 0.025f) *
                                           Config.NPCConfig.NpcDamageMultiplier), 1);
                npc.lifeMax =
                    Mathf.HugeCalc(
                        Mathf.FloorInt(Mathf.Pow(npc.lifeMax * (level * 0.10f + tier * 0.15f), power) *
                                       Config.NPCConfig.NpcHealthMultiplier), 1);
            }

            npc.value = npc.value * (1 + (level + tier) * 0.001f) * (1 + (int)rank * 0.1f);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.FloorInt(npc.defense * (1 + level * 0.005f + tier * 0.01f)),
                    npc.defense);
            if (npc.defense > 5)
                npc.defense -= 5;
            else
                npc.defense = 0;

            npc.life = npc.lifeMax;
        }
        else
        {
            if (npc.boss)
            {
                if (npc.damage > 0)
                {
                    if (Mathf.HugeCalc(
                            Mathf.FloorInt(npc.damage * (0.35f + level * 0.04f + tier * 0.06f) *
                                           Config.NPCConfig.NpcDamageMultiplier * 0.75f), 1) < 250000)
                        npc.damage =
                            Mathf.HugeCalc(
                                Mathf.FloorInt(npc.damage * (0.35f + level * 0.04f + tier * 0.06f) *
                                               Config.NPCConfig.NpcDamageMultiplier * 0.75f), 1);
                    else
                        npc.damage = Mathf.FloorInt(250000 * Mathf.Logx(1 + level * 0.10f + tier * 0.30f, 7.5f) *
                                                    Config.NPCConfig.NpcDamageMultiplier);
                }

                npc.lifeMax =
                    Mathf.HugeCalc(
                        Mathf.FloorInt(Mathf.Pow(npc.lifeMax * (level * 0.05f + tier * 0.070), 1.08f) *
                                       Config.NPCConfig.BossHealthMultiplier * Config.NPCConfig.NpcHealthMultiplier),
                        1);
            }
            else
            {
                if (npc.damage > 0)
                    npc.damage =
                        Mathf.HugeCalc(
                            Mathf.FloorInt(npc.damage * (0.75f + level * 0.035f + tier * 0.05f) *
                                           Config.NPCConfig.NpcDamageMultiplier), 1);
                npc.lifeMax =
                    Mathf.HugeCalc(
                        Mathf.FloorInt(Mathf.Pow(npc.lifeMax * (level * 0.20f + tier * 0.30f), power) *
                                       Config.NPCConfig.NpcHealthMultiplier), 1);
            }

            npc.value = npc.value * (1 + (level + tier) * 0.001f) * (1 + (int)rank * 0.1f);
            if (npc.defense > 0)
                npc.defense = Mathf.HugeCalc(Mathf.FloorInt(npc.defense * (1 + level * 0.01f + tier * 0.02f)),
                    npc.defense);
            if (npc.defense > 5)
                npc.defense -= 5;
            else
                npc.defense = 0;

            npc.life = npc.lifeMax;
        }


        npc = SetRankStat(npc, rank);
        npc = SetModifierStat(npc);
        return npc;
    }

    //Generate/Get level and tier or NPC

    #region LevelGen

    //Get Base Level Based on Stat of npc
    public static int GetBaseLevel(NPC npc)
    {
        if (npc.type == NPCID.DungeonGuardian || (npc.type == NPCID.SpikeBall) | (npc.type == NPCID.BlazingWheel))
            return 1;
        var maxLevel = GetMaxLevel();

        float health = npc.lifeMax;

        var damage = npc.damage;
        var def = npc.defense;

        if (npc.damage < 0)
            damage = 1;

        if (npc.defense < 0)
            def = 1;

        if (npc.defense > npc.damage)
            def = npc.damage / 2;

        var baselevel =
            Mathf.HugeCalc(
                (int)(Mathf.Pow(npc.lifeMax / 20, 1.1f) + Mathf.Pow(damage * 0.4f, 1.2f) + Mathf.Pow(def, 1.4f)), -1);

        if (npc.boss)
        {
            if (Main.masterMode)
                baselevel = Mathf.HugeCalc(
                    (int)(health / 150 + Mathf.Pow(damage * 0.30f, 1.09f) + Mathf.Pow(def * 0.8f, 1.15f)), -1);

            else if (Main.expertMode)
                baselevel = Mathf.HugeCalc(
                    (int)(health / 100 + Mathf.Pow(damage * 0.325f, 1.09f) + Mathf.Pow(def * 0.85f, 1.15f)), -1);

            else
                baselevel = Mathf.HugeCalc(
                    (int)(health / 80 + Mathf.Pow(damage * 0.35f, 1.09f) + Mathf.Pow(def * 0.9f, 1.15f)), -1);

            if ((npc.aiStyle == 6 || npc.aiStyle == 37) && npc.type > NPCID.SeekerTail)
            {
                if (Main.expertMode)
                    baselevel = Mathf.HugeCalc(
                        (int)(Mathf.Pow(health / 650, 0.5f) + Mathf.Pow(damage * 0.31f, 1.05f) +
                              Mathf.Pow(def * 0.8f, 1.07f)), -1);

                else
                    baselevel = Mathf.HugeCalc(
                        (int)(Mathf.Pow(health / 500, 0.5f) + Mathf.Pow(damage * 0.31f, 1.05f) +
                              Mathf.Pow(def * 0.8f, 1.07f)), -1);
            }
        }

        if (Main.masterMode)
        {
            baselevel = (int)(baselevel * 0.35f);

            if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Calamity])
            {
                if (baselevel > 15)
                {
                    baselevel -= 15;
                    baselevel.Clamp(5, int.MaxValue);
                }

                baselevel = (int)(baselevel * 0.35f);
            }
        }
        else if (Main.expertMode)
        {
            baselevel = (int)(baselevel * 0.5f);

            if (AnotherRpgModExpanded.LoadedMods[SupportedMod.Calamity])
            {
                if (baselevel > 15)
                {
                    baselevel -= 15;
                    baselevel.Clamp(5, int.MaxValue);
                }

                baselevel = (int)(baselevel * 0.5f);
            }
        }

        baselevel -= 10;

        baselevel = WorldManager.GetWorldLevelMultiplier(baselevel);

        if (baselevel < -1)
            return 1;

        if (Config.NPCConfig.LimitNPCGrowth) baselevel = baselevel.Clamp(10, maxLevel);


        baselevel = baselevel.Clamp(10, int.MaxValue);
        return baselevel;
    }

    public static int GetMaxLevel()
    {
        var maxLevel = Mathf.CeilInt(WorldManager.PlayerLevel + Config.NPCConfig.LimitNPCGrowthValue +
                                     WorldManager.PlayerLevel * Config.NPCConfig.LimitNPCGrowthPercent * 0.01f);

        if (maxLevel < 1)
            maxLevel = 1;
        return maxLevel;
    }

    //Get Tier bonus from world
    public static int GetWorldTier(NPC npc, int baselevel)
    {
        var BonusLevel = WorldManager.GetWorldAdditionalLevel();
        var maxLevel = GetMaxLevel();

        if (BonusLevel + baselevel > maxLevel) return maxLevel - baselevel;
        return 0;
    }

    //Tier are bonus level either random or from world
    public static int GetTier(NPC npc, int baselevel)
    {
        if (baselevel < 0)
            baselevel = 0;
        var rand = Mathf.RandomInt(0, 4 + Mathf.CeilInt(baselevel * 0.1f));
        return Mathf.HugeCalc(rand + GetWorldTier(npc, baselevel + rand), -1);
    }

    public static int GetTierAlly(NPC npc, int baselevel)
    {
        return WorldManager.GetWorldAdditionalLevel();
    }

    //get actual rank of the monster
    public static NPCRank GetRank(int level, bool boss = false)
    {
        if (!WorldManager.Ascended)
        {
            if (!Config.NPCConfig.NPCRarity)
                return NPCRank.Normal;

            if (boss && !Config.NPCConfig.BossRarity)
                return NPCRank.Normal;

            if (level < 1)
                level = 1;
            var rarityBooster = (float)Math.Log(level + 1) + 1;
            var rn = Mathf.RandomInt(0, 1500 / (level / 50 + 1));

            if (rn <= 1)
                return NPCRank.DIO;

            if (rn <= 3)
                return NPCRank.Godly;

            if (rn <= 8)
                return NPCRank.Mythical;

            if (rn < 15)
                return NPCRank.Legendary;

            if (rn < 150)
                return NPCRank.Elite;

            if (rn < 350)
                return NPCRank.Alpha;

            if (rn < 1050)
                return NPCRank.Normal;
            return NPCRank.Weak;
        }
        else
        {
            if (!Config.NPCConfig.NPCRarity)
                return NPCRank.Raised;

            if (level < 1)
                level = 1;
            var rn = Mathf.RandomInt(0, 4000 / (level / 1000 + 1));

            if (rn <= 1)
                return NPCRank.DioAboveHeaven;

            if (rn <= 5)
                return NPCRank.TransDimensional;

            if (rn <= 15)
                return NPCRank.Transcendental;

            if (rn < 35)
                return NPCRank.PeakAscended;

            if (rn < 150)
                return NPCRank.HighAscended;

            if (rn < 500)
                return NPCRank.Ascended;

            if (rn < 2000)
                return NPCRank.Raised;
            return NPCRank.LimitBreaked;
        }
    }

    #endregion
}