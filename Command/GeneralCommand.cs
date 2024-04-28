using System.Collections.Generic;
using System.Reflection;
using AnotherRpgModExpanded.Items;
using AnotherRpgModExpanded.RPGModule.Entities;
using AnotherRpgModExpanded.RPGModule.Items;
using AnotherRpgModExpanded.Utils;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace AnotherRpgModExpanded.Command;

public class SetLevel : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "setlevel";

    public override string Usage => "/setlevel <level>";

    public override string Description => "Sets your character level to the chosen value";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var character = caller.Player.GetModPlayer<RpgPlayer>();
        var level = int.Parse(args[0]) - 1;
        level = level.Clamp(0, 9999);

        character.ResetLevel();
        WorldManager.PlayerLevel = level;

        for (var i = 0; i < level; i++) character.CommandLevelup();
    }
}

public class Level : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "level";

    public override string Usage => "/level <level>";

    public override string Description => "Levelup X time";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var character = caller.Player.GetModPlayer<RpgPlayer>();
        var level = int.Parse(args[0]);
        level = level.Clamp(0, 9999);

        character.ResetSkillTree();

        for (var i = 0; i < level; i++) character.CommandLevelup();
    }
}

public class ResetCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "reset";

    public override string Usage => "/reset ";

    public override string Description => "Reset your points";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var character = caller.Player.GetModPlayer<RpgPlayer>();
        character.RecalculateStat();
    }
}

public class RarityReroll : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "rarity";

    public override string Usage => "/rarity";

    public override string Description => "Reroll the held item rarity taking as much coins as the goblin tinkerer";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var player = caller.Player;

        var item = player.HeldItem.GetGlobalItem<ItemUpdate>();
        float itemvalue = player.HeldItem.value;
        var cost = Mathf.RoundInt(itemvalue * 0.33333f);

        if (player.CanAfford(cost))
        {
            player.BuyItem(cost);
            var plat = 0;
            var gold = 0;
            var silv = 0;
            var copp = 0;

            var costbuffer = cost;

            if (costbuffer >= 1000000)
            {
                plat = costbuffer / 1000000;
                costbuffer = -plat * 1000000;
            }

            if (costbuffer > 10000)
            {
                gold = costbuffer / 10000;
                costbuffer = -gold * 10000;
            }

            if (costbuffer > 100)
            {
                silv = costbuffer / 100;
                costbuffer = -silv * 100;
            }

            if (costbuffer > 1)
                copp = costbuffer;

            var coststring = "";

            if (plat > 0) coststring += " " + plat + " " + Lang.inter[15].Value;

            if (gold > 0) coststring += " " + gold + " " + Lang.inter[16].Value;

            if (silv > 0) coststring += " " + silv + " " + Lang.inter[17].Value;

            if (copp > 0) coststring += " " + copp + " " + Lang.inter[18].Value;

            coststring += " used to reroll your item rarity";
            Main.NewText(coststring);

            item.Roll(caller.Player.HeldItem);
        }

        else
        {
            var plat = 0;
            var gold = 0;
            var silv = 0;
            var copp = 0;

            var costbuffer = cost;

            if (costbuffer >= 1000000)
            {
                plat = costbuffer / 1000000;
                costbuffer = -plat * 1000000;
            }

            if (costbuffer > 10000)
            {
                gold = costbuffer / 10000;
                costbuffer = -gold * 10000;
            }

            if (costbuffer > 100)
            {
                silv = costbuffer / 100;
                costbuffer = -silv * 100;
            }

            if (costbuffer > 1)
                copp = costbuffer;

            var coststring = "need";

            if (plat > 0) coststring += " " + plat + " " + Lang.inter[15].Value;

            if (gold > 0) coststring += " " + gold + " " + Lang.inter[16].Value;

            if (silv > 0) coststring += " " + silv + " " + Lang.inter[17].Value;

            if (copp > 0) coststring += " " + copp + " " + Lang.inter[18].Value;

            coststring += " to reroll your item rarity";
            Main.NewText(coststring);
        }
    }
}

public class EvolutionReroll : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "itemtree";

    public override string Usage => "/itemtree";

    public override string Description => "Reroll the held item Evolution Tree for it's entire value of coins";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var player = caller.Player;

        var item = player.HeldItem.GetGlobalItem<ItemUpdate>();
        var cost = player.HeldItem.value;

        if (player.CanAfford(cost))
        {
            player.BuyItem(cost);

            var plat = 0;
            var gold = 0;
            var silv = 0;
            var copp = 0;

            var costbuffer = cost;

            if (costbuffer >= 1000000)
            {
                plat = costbuffer / 1000000;
                costbuffer = -plat * 1000000;
            }

            if (costbuffer > 10000)
            {
                gold = costbuffer / 10000;
                costbuffer = -gold * 10000;
            }

            if (costbuffer > 100)
            {
                silv = costbuffer / 100;
                costbuffer = -silv * 100;
            }

            if (costbuffer > 1)
                copp = costbuffer;

            var coststring = "";

            if (plat > 0) coststring += " " + plat + " " + Lang.inter[15].Value;

            if (gold > 0) coststring += " " + gold + " " + Lang.inter[16].Value;

            if (silv > 0) coststring += " " + silv + " " + Lang.inter[17].Value;

            if (copp > 0) coststring += " " + copp + " " + Lang.inter[18].Value;

            coststring += " used to reroll your item evolution tree";
            Main.NewText(coststring);

            item.CompleteReset();
        }

        else
        {
            var plat = 0;
            var gold = 0;
            var silv = 0;
            var copp = 0;

            var costbuffer = cost;

            if (costbuffer >= 1000000)
            {
                plat = costbuffer / 1000000;
                costbuffer = -plat * 1000000;
            }

            if (costbuffer > 10000)
            {
                gold = costbuffer / 10000;
                costbuffer = -gold * 10000;
            }

            if (costbuffer > 100)
            {
                silv = costbuffer / 100;
                costbuffer = -silv * 100;
            }

            if (costbuffer > 1)
                copp = costbuffer;

            var coststring = "need";

            if (plat > 0) coststring += " " + plat + " " + Lang.inter[15].Value;

            if (gold > 0) coststring += " " + gold + " " + Lang.inter[16].Value;

            if (silv > 0) coststring += " " + silv + " " + Lang.inter[17].Value;

            if (copp > 0) coststring += " " + copp + " " + Lang.inter[18].Value;

            coststring += " to reroll your item evolution tree";
            Main.NewText(coststring);
        }
    }
}

public class ItemName : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "iname";

    public override string Usage => "/iname <slot>";

    public override string Description => "Get Item Name from slot (to confirm XP transfer)";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var player = caller.Player;

        if (args.Length == 0)
        {
            Main.NewText(Description);
            return;
        }

        if (int.TryParse(args[0], out var slot) == false)
        {
            Main.NewText("Slot Number invalid");
            return;
        }

        Main.NewText(player.inventory[slot].Name);
    }
}

public class XpTransfer : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "xpt";

    public override string Usage => "/xpt <slot>";

    public override string Description => "eXPerience Transfer from the slot to the held item, have 75% loss";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var player = caller.Player;

        if (args.Length == 0)
        {
            Main.NewText(Description);
            return;
        }

        if (int.TryParse(args[0], out var slot) == false)
        {
            Main.NewText("Slot Number invalid");
            return;
        }

        var item = player.HeldItem.GetGlobalItem<ItemUpdate>();
        var source = player.inventory[slot].GetGlobalItem<ItemUpdate>();

        if (item == source)
        {
            Main.NewText("Slot number And Held Items are the same");
            return;
        }

        AnotherRpgModExpanded.Source = source;
        AnotherRpgModExpanded.Transfer = item;
        AnotherRpgModExpanded.XpTvalueA = ItemExtraction.GetTotalEarnedXp(source);
        AnotherRpgModExpanded.XpTvalueB = ItemExtraction.GetTotalEarnedXp(item);

        var xp = ItemExtraction.GetExtractedXp(false, source);

        Main.NewText("Transfering " + xp + " exp from " + player.inventory[slot].Name + " to " +
                     player.HeldItem.Name);

        source.ResetLevelXp();
        item.XpTransfer(xp, player, player.HeldItem);
    }
}

public class UndoXpTransfer : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "undoxpt";

    public override string Usage => "/undoxpt";

    public override string Description => "Undo The last Exp Transfer (I know you naughty boi would make a mistake ;))";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (AnotherRpgModExpanded.Source == null)
            return;

        if (AnotherRpgModExpanded.Transfer == null)
            return;

        AnotherRpgModExpanded.Source.ResetLevelXp(false);
        AnotherRpgModExpanded.Transfer.ResetLevelXp(false);
        AnotherRpgModExpanded.Source.SilentXpTransfer(AnotherRpgModExpanded.XpTvalueA);
        AnotherRpgModExpanded.Transfer.SilentXpTransfer(AnotherRpgModExpanded.XpTvalueB);

        AnotherRpgModExpanded.Source = null;
        AnotherRpgModExpanded.Transfer = null;
        AnotherRpgModExpanded.XpTvalueA = 0;
        AnotherRpgModExpanded.XpTvalueB = 0;
    }
}

public class WorldLevel : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "WorldRPGInfo";

    public override string Usage => "/WorldRPGInfo";

    public override string Description => "Debug print the world info";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Main.NewText("is world ascended : " + WorldManager.ascended);

        if (WorldManager.ascended)
            Main.NewText("world ascend level : " + WorldManager.ascendedLevelBonus);

        Main.NewText("Number of boss defeated : " + WorldManager.BossDefeated);
        Main.NewText("Number of boss defeated for the first time : " + WorldManager.FirstBossDefeated);
        Main.NewText("Is hardmode ? : " + Main.hardMode);
        Main.NewText("Day : " + WorldManager.Day);
        Main.NewText("Player Level : " + WorldManager.PlayerLevel);
        Main.NewText("Additional NPC Level : " + WorldManager.GetWorldAdditionalLevel());
    }
}

public class AscentWorld : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "Ascend";

    public override string Usage => "/Ascend <level>";

    public override string Description => "Ascend world, and add X level to the world";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        WorldManager.ascended = true;
        var level = int.Parse(args[0]);

        if (level < 250) level = 250;
        WorldManager.ascendedLevelBonus = level;
    }
}

public class MigrateLevels : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "migrate";

    public override string Usage => "/migrate";

    public override string Description =>
        "Migrates levels from existing items to the new mod. Use on an existing character that had previously played with AnRPG mod.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var player = caller.Player;

        var unloadedPlayer = player.GetModPlayer<UnloadedPlayer>();

        if (unloadedPlayer.GetType()
                .GetField("data", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(unloadedPlayer) is List<TagCompound> { Count: > 0 } savedPlayers)
        {
            var original = player.GetModPlayer<RpgPlayer>();
            if (!original.Migrated)
            {
                foreach (var tag in savedPlayers)
                {
                    if (tag.GetString("mod") == "AnotherRpgMod")
                    {
                        var rpgPlayer = new RpgPlayer();
                        rpgPlayer.Initialize();
                        rpgPlayer.LoadData(tag.Get<TagCompound>("data"));
                        original.MigrateData(rpgPlayer);
                    }
                }
            }
        }

        var fullInventory = new Item[player.inventory.Length + player.armor.Length + player.bank.item.Length +
                                     player.bank2.item.Length + player.bank3.item.Length + player.bank4.item.Length];
        var count = 0;
        
        player.inventory.CopyTo(fullInventory, count);
        count += player.inventory.Length;
        player.armor.CopyTo(fullInventory, count);
        count += player.armor.Length; 
        player.bank.item.CopyTo(fullInventory, count);
        count += player.bank.item.Length;
        player.bank2.item.CopyTo(fullInventory, count);
        count += player.bank2.item.Length;
        player.bank3.item.CopyTo(fullInventory, count);
        count += player.bank3.item.Length; 
        player.bank4.item.CopyTo(fullInventory, count);

        foreach (var item in fullInventory)
            if (!item.IsAir)
            {
                var unloaded = item.GetGlobalItem<UnloadedGlobalItem>();
                try
                {
                    var original = item.GetGlobalItem<ItemUpdate>();
                    if (!original.Migrated)
                        if (unloaded.GetType()
                                .GetField("data", BindingFlags.Instance | BindingFlags.NonPublic)
                                ?.GetValue(unloaded) is List<TagCompound> { Count: > 0 } savedTags)
                            foreach (var tag in savedTags)
                                if (tag.GetString("mod") == "AnotherRpgMod")
                                {
                                    var update = new ItemUpdate();
                                    update.LoadData(item, tag.Get<TagCompound>("data"));
                                    original.MigrateData(update);
                                }
                }
                catch
                {
                    // ignored
                }
            }
    }
}