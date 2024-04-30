using System;
using System.Collections.Generic;
using System.IO;
using AnotherRpgModExpanded.RPGModule.Entities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnotherRpgModExpanded.Utils;

internal class MPPacketHandler
{
    public static Dictionary<Message, List<DataTag>> dataTags = new()
    {
        { Message.AddXp, new List<DataTag> { DataTag.amount, DataTag.level } },
        {
            Message.SyncLevel,
            new List<DataTag> { DataTag.PlayerId, DataTag.amount, DataTag.buffer, DataTag.life, DataTag.maxLife }
        },
        { Message.SyncPlayerHealth, new List<DataTag> { DataTag.PlayerId, DataTag.life, DataTag.maxLife } },
        {
            Message.SyncNpcSpawn,
            new List<DataTag>
            {
                DataTag.PlayerId, DataTag.npcId, DataTag.level, DataTag.tier, DataTag.rank, DataTag.modifiers,
                DataTag.buffer, DataTag.WorldTier
            }
        },
        {
            Message.SyncNpcUpdate,
            new List<DataTag> { DataTag.PlayerId, DataTag.npcId, DataTag.life, DataTag.maxLife, DataTag.damage }
        },
        { Message.SyncWeapon, new List<DataTag> {}},
        { Message.AskNpc, new List<DataTag> { DataTag.PlayerId, DataTag.npcId } },
        { Message.Log, new List<DataTag> { DataTag.buffer } },
        { Message.SyncWorld, new List<DataTag> {}}
    };

    public static void SendXPPacket(Mod mod, int XPToDrop, int xplevel)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            var packet = mod.GetPacket();
            packet.Write((byte)Message.AddXp);
            packet.Write(XPToDrop);
            packet.Write(xplevel);
            packet.Send();
        }
    }

    public static string ParseBuffer(Dictionary<string, string> buffer)
    {
        var parsed = "";

        if (buffer.Count == 0)
            return parsed;
        foreach (var entry in buffer) parsed += entry.Key + ":" + entry.Value + ",";

        return parsed.Remove(parsed.Length - 1);
    }

    public static Dictionary<string, string> Unparse(string parsed)
    {
        var unparsed = new Dictionary<string, string>();

        if (parsed == "")
            return unparsed;
        var KVString = parsed.Split(',');
        for (var i = 0; i < KVString.Length; i++)
        {
            var KVPair = new KeyValuePair<string, string>(KVString[i].Split(':')[0], KVString[i].Split(':')[1]);
            unparsed.Add(KVPair.Key, KVPair.Value);
        }

        return unparsed;
    }

    public static void SendNpcSpawn(Mod mod, int askingClientId, NPC npc, int Tier, int Level, ARPGGlobalNPC ARPGNPC)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            var packet = mod.GetPacket();

            packet.Write((byte)Message.SyncNpcSpawn);
            packet.Write((byte)askingClientId);
            packet.Write((byte)npc.whoAmI);
            packet.Write(Level);
            packet.Write(Tier);

            packet.Write(ARPGNPC.getRank);
            packet.Write((int)ARPGNPC.modifier);
            packet.Write(ParseBuffer(ARPGNPC.specialBuffer));
            packet.Write(WorldManager.BossDefeated);

            packet.Send(askingClientId);
        }
    }

    public static void SendPlayerHealthSync(Mod mod, int playerIndex, int ignore = -1)
    {
        var packet = mod.GetPacket();
        packet.Write((byte)Message.SyncPlayerHealth);
        packet.Write((byte)playerIndex);
        packet.Write(Main.player[playerIndex].statLife);
        packet.Write(Main.player[playerIndex].statLifeMax);
        packet.Send(ignoreClient: ignore);
    }

    public static void AskNpcInfo(Mod mod, NPC npc, int playerindex, int ignore = -1)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            //AnotherRpgModExpanded.Instance.Logger.Info("ask npc to server");
            var packet = mod.GetPacket();
            packet.Write((byte)Message.AskNpc);
            packet.Write((byte)playerindex);
            packet.Write((byte)npc.whoAmI);
            packet.Send(ignoreClient: ignore);
        }
    }

    public static void SendNpcUpdate(Mod mod, NPC npc, int playerindex = -1, int ignore = -1)
    {
        if (!Main.npc[npc.whoAmI].active)
            return;

        if (Main.netMode == NetmodeID.Server)
        {
            var packet = mod.GetPacket();
            packet.Write((byte)Message.SyncNpcUpdate);
            packet.Write((byte)playerindex);
            packet.Write((byte)npc.whoAmI);
            packet.Write(npc.life);
            packet.Write(npc.lifeMax);
            packet.Write(npc.damage);
            packet.Send(ignoreClient: ignore);
        }

        else if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            AskNpcInfo(mod, npc, playerindex, ignore);
        }
    }

    public static void HandlePacket(BinaryReader reader, int whoAmI)
    {
        var msg = (Message)reader.ReadByte();
        var tags = new Dictionary<DataTag, object>();
        foreach (var tag in dataTags[msg])
            tags.Add(tag, tag.read(reader));
        switch (msg)
        {
            case Message.SyncLevel:
                var playerID = (byte)tags[DataTag.PlayerId];
                var p = Main.player[playerID].GetModPlayer<RpgPlayer>();
                /*

                if (p.baseName == "")
                    p.baseName = Main.player[(int)tags[DataTag.playerId]].name;
                */

                if ((byte)tags[DataTag.PlayerId] != Main.myPlayer && Main.player[playerID] != null)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        p.SyncLevel((int)tags[DataTag.amount]);
                    //Main.player[(int)tags[DataTag.playerId]].name = (string)tags[DataTag.buffer];

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.player[playerID].statLife = (int)tags[DataTag.life];
                        Main.player[playerID].statLifeMax2 = (int)tags[DataTag.maxLife];

                        if (WorldManager.Instance != null) WorldManager.Instance.NetUpdateWorld();
                    }
                }

                WorldManager.PlayerLevel = Math.Max(WorldManager.PlayerLevel, p.GetLevel());

                break;
            case Message.AddXp:
                Main.LocalPlayer.GetModPlayer<RpgPlayer>().AddXp((int)tags[DataTag.amount], (int)tags[DataTag.level]);
                break;
            case Message.SyncNpcSpawn:

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    break;


                var npc = Main.npc[(byte)tags[DataTag.npcId]];

                ARPGGlobalNPC rpgNPC;

                if (npc.TryGetGlobalNPC(out rpgNPC))
                    AnotherRpgModExpanded.Instance.Logger.Info("Sync NPC Spawn | name :" + npc.GivenName);
                else
                    //Request this npc again in a few frame

                if (npc == null || rpgNPC == null)
                    break;

                if (rpgNPC.StatsCreated)
                    break;

                var tier = (int)tags[DataTag.tier];
                var level = (int)tags[DataTag.level];
                var rank = (NPCRank)tags[DataTag.rank];

                var modifiers = (NPCModifier)tags[DataTag.modifiers];

                AnotherRpgModExpanded.Instance.Logger.Info(npc.GivenOrTypeName + "\nTier : " + tier + "   Level : " +
                                                           level + "   rank : " + rank + "   Modifier  : " + modifiers +
                                                           " \n Buffer : " + (string)tags[DataTag.buffer]);

                var bufferStack = Unparse((string)tags[DataTag.buffer]);

                WorldManager.BossDefeated = (int)tags[DataTag.WorldTier];

                npc.GetGlobalNPC<ARPGGlobalNPC>().StatsCreated = true;
                npc.GetGlobalNPC<ARPGGlobalNPC>().modifier = modifiers;
                npc.GetGlobalNPC<ARPGGlobalNPC>().SetLevelTier(level, tier, (byte)rank);
                npc.GetGlobalNPC<ARPGGlobalNPC>().specialBuffer = bufferStack;

                npc.GetGlobalNPC<ARPGGlobalNPC>().SetStats(npc);

                npc.GivenName = NPCUtils.GetNpcNameChange(npc, tier, level, rank);
                npc.life = npc.lifeMax;


                //AnotherRpgModExpanded.Instance.Logger.Info("NPC created with id : " + npc.whoAmI);
                //AnotherRpgModExpanded.Instance.Logger.Info( "Client Side : \n" + npc.GetGivenOrTypeNetName() + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n\n");

                break;

            case Message.SyncNpcUpdate:

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var npcId = (byte)tags[DataTag.npcId];


                    var npcu = Main.npc[npcId];

                    if (!npcu.active)
                        return;

                    if (npcu.lifeMax != (int)tags[DataTag.maxLife])
                        AskNpcInfo(AnotherRpgModExpanded.Instance, npcu, Main.myPlayer);

                    npcu.lifeMax = (int)tags[DataTag.maxLife];
                    npcu.life = (int)tags[DataTag.life];
                    npcu.damage = (int)tags[DataTag.damage];
                }

                break;
            case Message.Log:

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    AnotherRpgModExpanded.Instance.Logger.Info((string)tags[DataTag.buffer]);

                break;
            case Message.AskNpc:

                if (Main.netMode == NetmodeID.Server)
                {
                    int askplayerID = (byte)tags[DataTag.PlayerId];

                    var asknpc = Main.npc[(byte)tags[DataTag.npcId]];

                    ARPGGlobalNPC askrpgnpc;

                    if (!asknpc.TryGetGlobalNPC(out askrpgnpc))
                    {
                        MPDebug.Log(AnotherRpgModExpanded.Instance, "Couldn't find Global NPC of asked NPC");

                        return;
                    }

                    var asktier = askrpgnpc.getTier;
                    var asklevel = askrpgnpc.getLevel;
                    var askrank = askrpgnpc.getRank;
                    Mod mod = AnotherRpgModExpanded.Instance;
                    //MPDebug.Log(mod, "Server Side : \n" + npc.GetGivenOrTypeNetName() + " ID : " + npc.whoAmI + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n");

                    SendNpcSpawn(mod, askplayerID, asknpc, asktier, asklevel, askrpgnpc);
                }

                break;
            case Message.SyncPlayerHealth:


                var pID = (byte)tags[DataTag.PlayerId];

                if (pID == Main.myPlayer && !Main.ServerSideCharacter)
                    break;


                if (Main.netMode == NetmodeID.Server)
                    pID = (byte)whoAmI;

                var player = Main.player[pID];
                player.statLife = (int)tags[DataTag.life];
                player.statLifeMax = (int)tags[DataTag.maxLife];

                if (player.statLifeMax < 100)
                    player.statLifeMax = 100;
                player.dead = player.statLife <= 0;

                if (Main.netMode != NetmodeID.Server)
                    break;

                try
                {
                    SendPlayerHealthSync(AnotherRpgModExpanded.Instance, pID, whoAmI);
                }
                catch (Exception ex)
                {
                }


                break;
            case Message.SyncWorld:
                break;
        }
    }
}