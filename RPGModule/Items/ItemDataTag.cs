using System;
using System.IO;

namespace AnotherRpgModExpanded.RPGModule.Items;

public class ItemDataTag
{
    public static ItemDataTag level = new(reader => reader.ReadInt32());
    public static ItemDataTag xp = new(reader => reader.ReadInt64());
    public static ItemDataTag ascendedlevel = new(reader => reader.ReadInt32());
    public static ItemDataTag modifier = new(reader => reader.ReadInt32());

    public static ItemDataTag init = new(reader => reader.ReadBoolean());

    public static ItemDataTag rarity = new(reader => reader.ReadInt32());

    public static ItemDataTag statsamm = new(reader => reader.ReadInt32());

    public static ItemDataTag statst1 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat1 = new(reader => reader.ReadInt32());

    public static ItemDataTag statst2 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat2 = new(reader => reader.ReadInt32());

    public static ItemDataTag statst3 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat3 = new(reader => reader.ReadInt32());

    public static ItemDataTag statst4 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat4 = new(reader => reader.ReadInt32());

    public static ItemDataTag statst5 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat5 = new(reader => reader.ReadInt32());

    public static ItemDataTag statst6 = new(reader => reader.ReadSByte());
    public static ItemDataTag stat6 = new(reader => reader.ReadInt32());

    public static ItemDataTag baseDamage = new(reader => reader.ReadInt32());
    public static ItemDataTag baseArmor = new(reader => reader.ReadInt32());
    public static ItemDataTag baseAutoReuse = new(reader => reader.ReadBoolean());
    public static ItemDataTag baseName = new(reader => reader.ReadString());
    public static ItemDataTag baseUseTime = new(reader => reader.ReadInt32());
    public static ItemDataTag baseMana = new(reader => reader.ReadInt32());

    public static ItemDataTag itemTree = new(reader => reader.ReadString());
    public static ItemDataTag bWorldAscendDrop = new(reader => reader.ReadBoolean());
    public static ItemDataTag WorldAscendDropLevel = new(reader => reader.ReadInt32());

    public static ItemDataTag migrated = new(reader => reader.ReadBoolean());

    public Func<BinaryReader, object> read;

    public ItemDataTag(Func<BinaryReader, object> read)
    {
        this.read = read;
    }
}