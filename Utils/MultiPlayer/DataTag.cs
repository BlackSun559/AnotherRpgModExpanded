using System;
using System.IO;

namespace AnotherRpgModExpanded.Utils;

public class DataTag
{
    public static DataTag amount = new(reader => reader.ReadInt32());

    public static DataTag amount_single = new(reader => reader.ReadSingle());
    public static DataTag PlayerId = new(reader => reader.ReadByte());
    public static DataTag npcId = new(reader => reader.ReadByte());
    public static DataTag itemId = new(reader => reader.ReadInt32());


    public static DataTag level = new(reader => reader.ReadInt32());
    public static DataTag tier = new(reader => reader.ReadInt32());
    public static DataTag WorldTier = new(reader => reader.ReadInt32());
    public static DataTag rank = new(reader => reader.ReadInt32());

    public static DataTag modifiers = new(reader => reader.ReadInt32());
    public static DataTag buffer = new(reader => reader.ReadString());


    public static DataTag damage = new(reader => reader.ReadInt32());
    public static DataTag life = new(reader => reader.ReadInt32());
    public static DataTag maxLife = new(reader => reader.ReadInt32());


    public static DataTag GPFlag = new(reader => reader.ReadInt32());

    public static DataTag XPReductionDelta = new(reader => reader.ReadInt32());

    public static DataTag XpMultiplier = new(reader => reader.ReadSingle());
    public static DataTag NpclevelMultiplier = new(reader => reader.ReadSingle());
    public static DataTag ItemXpMultiplier = new(reader => reader.ReadSingle());

    public static DataTag Seed = new(reader => reader.ReadInt32());


    public Func<BinaryReader, object> read;

    public DataTag(Func<BinaryReader, object> read)
    {
        this.read = read;
    }
}