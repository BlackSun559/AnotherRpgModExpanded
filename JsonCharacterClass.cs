using System;
using System.IO;
using AnotherRpgModExpanded.RPGModule;
using Newtonsoft.Json;
using Terraria;

namespace AnotherRpgModExpanded;

/*DamageType _damageType
 * bool _flat
 * NodeType _type,
 * bool _unlocked
 * float _value
 * int _levelrequirement
 * int _maxLevel
 * int _pointsPerLevel
 */

public class JsonChrClassList
{
    public JsonChrClass[] jsonList =
    {
        //Jack of all trade
        new(
            "Tourist",
            new float[7] { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0, 0 },
            0, 0, 0
        ),
        new(
            "Apprentice",
            new float[7] { 0.125f, 0.125f, 0.125f, 0.125f, 0.125f, 0, 0 },
            0,
            0.05f, 0
        ),
        new(
            "Regular",
            new float[7] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0, 0 },
            0,
            0.15f, 0.1f
        ),
        new(
            "Expert",
            new float[7] { 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0, 0 },
            0,
            0.25f, 0.15f, 0, -0.05f, 0.1f, 0.0001f, 0.1f
        ),
        new(
            "Master",
            new float[7] { 0.7f, 0.7f, 0.7f, 0.7f, 0.7f, 0, 0 },
            0,
            0.4f, 0.2f, 1, -0.1f, 0.15f, 0.00025f, 0.25f
        ),
        new(
            "PerfectBeing",
            new float[7] { 1, 1, 1, 1, 1, 0, 0 },
            0,
            1f, 0.25f, 1, -0.25f, 0.25f, 0.0005f, 0.5f
        ),
        //T6 
        new(
            "Ascended",
            new float[7] { 4, 4, 4, 4, 4, 0, 0 },
            0,
            1.5f, 0.5f, 2, -0.75f, 0.75f, 0.002f, 1f
        ),

        //Specialist - Tier 1
        new(
            "Archer",
            new float[7] { 0, 0.2f, 0, 0, 0, 0.25f, 0 },
            0,
            -0.15f, 0, 0, 0.03f, 0, 0, 0, 0
        ),
        new(
            "Gunner",
            new float[7] { 0, 0.2f, 0, 0, 0, 0, 0.25f },
            0,
            0, 0, 0, 0, 0.2f, 0, 0, 0
        ),
        new(
            "SwordMan",
            new float[7] { 0.6f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.1f, -0.05f, 0, 0.05f, 0, 0, 0, 0
        ),
        new(
            "Spiritualist",
            new float[7] { 0, 0, 0, 0, 0.5f, 0, 0 },
            0,
            0, 0, 1
        ),
        new(
            "Mage",
            new float[7] { 0, 0, 0, 0.4f, 0, 0, 0 },
            0,
            -0.15f, 0, 0, -0.2f
        ),
        new(
            "Ninja",
            new float[7] { 0, 0, 0.4f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.05f, 0.1f, 0, 0, 0
        ),
        new(
            "Acolyte",
            new float[7] { 0, 0, 0, 0.25f, 0, 0, 0 },
            0,
            0.1f, 0, 0, 0, 0.2f, .00025f, 0.1f
        ),
        new(
            "Cavalier",
            new float[7] { 0.25f, 0, 0, 0, 0, 0, 0 },
            0,
            0.25f, 0.1f, 0, 0, 0, 0, 0, 0
        ),

        //Specialist - Tier 2
        new(
            "Hunter",
            new float[7] { 0, 0.4f, 0, 0, 0, 0.4f, 0 },
            0,
            -0.3f, 0, 0, 0.08f, 0, 0, 0, 0
        ),
        new(
            "Gunslinger",
            new float[7] { 0, 0.35f, 0, 0, 0, 0, 0.45f },
            0,
            0, 0, 0, 0, 0.3f, 0, 0, 0
        ),
        new(
            "Mercenary",
            new float[7] { 1f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.2f, -0.1f, 0, 0.1f, 0, 0, 0, 0
        ),
        new(
            "Invoker",
            new float[7] { 0, 0, 0, 0, 0.85f, 0, 0 },
            0,
            -0.1f, 0, 1
        ),
        new(
            "ArchMage",
            new float[7] { 0, 0, 0, .75f, 0, 0, 0 },
            0,
            -0.2f, 0, 0, -0.3f
        ),
        new(
            "Shinobi",
            new float[7] { 0, 0, .75f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.1f, 0.2f, 0, 0, 0
        ),
        new(
            "Monk",
            new float[7] { 0.1f, 0, 0, 0.4f, 0, 0, 0 },
            0,
            0.2f, 0.1f, 0, 0, 0.35f, 0.0005f, 0.2f
        ),
        new(
            "Knight",
            new float[7] { 0.5f, 0, 0, 0, 0, 0, 0 },
            0,
            0.4f, 0.2f, 0, 0, 0, 0, 0, 0
        ),

        //Specialist - Tier 3
        new(
            "Ranger",
            new float[7] { 0, 0.6f, 0, 0, 0, 0.6f, 0 },
            0,
            -0.2f, 0, 0, 0.15f, 0, 0, 0, 0
        ),
        new(
            "Spitfire",
            new float[7] { 0, 0.5f, 0, 0, 0, 0, 0.5f },
            0,
            0, 0, 0, 0, 0.45f, 0, 0, 0
        ),
        new(
            "SwordMaster",
            new float[7] { 1.5f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.3f, -0.15f, 0.1f, 0.2f
        ),
        new(
            "Summoner",
            new float[7] { 0, 0, 0, 0, 1.25f, 0, 0 },
            0,
            -0.15f, 0, 2
        ),
        new(
            "Arcanist",
            new float[7] { 0, 0, 0, 1.25f, 0, 0, 0 },
            0,
            -0.35f, 0, 0, -0.5f
        ),
        new(
            "Rogue",
            new float[7] { 0, 0, 1f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.25f, 0.4f, 0, 0, 0
        ),
        new(
            "Templar",
            new float[7] { 0.3f, 0, 0, 0.6f, 0, 0, 0 },
            0,
            0.3f, 0.15f, 0, 0, 0.5f, 0.00075f, 0.3f
        ),
        new(
            "IronKnight",
            new float[7] { 0.8f, 0, 0, 0, 0, 0, 0 },
            0,
            0.5f, 0.3f, 0, 0, 0, 0, 0, 0
        ),

        //Specialist - Tier 4
        new(
            "Marksman",
            new float[7] { 0, 0.8f, 0, 0, 0, 0.8f, 0 },
            0,
            -0.25f, 0, 0, 0.25f, 0, 0, 0, 0
        ),
        new(
            "Sniper",
            new float[7] { 0, 0.7f, 0, 0, 0, 0, 0.7f },
            0,
            -0.1f, 0, 0, 0.1f, 0.6f, 0, 0, 0
        ),
        new(
            "Champion",
            new float[7] { 2f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.3f, -0.2f, 0.15f, 0.35f
        ),
        new(
            "SoulBinder",
            new float[7] { 0, 0, 0, 0, 1.6f, 0, 0 },
            0,
            -0.2f, 0, 0, 0, 0, 2, -0.10f, 0.1f, 0.00025f, 0.1f
        ),
        new(
            "Warlock",
            new float[7] { 0, 0, 0, 1.5f, 0, 0, 0 },
            0,
            -0.25f, 0, 0, -0.7f, 0.1f, 0.00025f, 0.1f
        ),
        new(
            "Assassin",
            new float[7] { 0, 0, 1.5f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.5f, 0.5f, 0, 0, 0
        ),
        new(
            "Paladin",
            new float[7] { 0.75f, 0, 0, 0.8f, 0, 0, 0 },
            0,
            0.4f, 0.25f, 0, 0, 0.65f, 0.001f, 1
        ),
        new(
            "Mountain",
            new float[7] { 1.25f, 0, 0, 0, 0, 0, 0 },
            0,
            0.6f, 0.4f, -0.05f
        ),

        //Specialist - Tier 5
        new(
            "WindWalker",
            new float[7] { 0, 1.2f, 0, 0, 0, 1.2f, 0 },
            0,
            -0.35f, 0, 0, 0.3f, 0.3f, 0, 0, 0
        ),
        new(
            "Hitman",
            new float[7] { 0, 1.2f, 0, 0, 0, 0, 1.2f },
            0,
            -0.25f, 0, 0, 0.25f, 0.75f, 0, 0, 0
        ),
        new(
            "SwordSaint",
            new float[7] { 3f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.35f, -0.25f, 0.2f, 0.5f
        ),
        new(
            "SoulLord",
            new float[7] { 0, 0, 0, 0, 2.5f, 0, 0 },
            0,
            -0.3f, 0, 0.2f, 0, 0, 2, -0.25f, 0.2f, 0.00035f, 0.15f
        ),
        new(
            "Mystic",
            new float[7] { 0, 0, 0, 2.5f, 0, 0, 0 },
            0,
            -0.25f, 0, 0, -0.9f, 0.25f, 0.0005f, 0.25f
        ),
        new(
            "ShadowDancer",
            new float[7] { 0, 0, 2.5f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.75f, 0.75f, 0, 0, 0
        ),
        new(
            "Deity",
            new float[7] { 1f, 0, 0, 1f, 0, 0, 0 },
            0,
            0.5f, 0.35f, 0, 0, 0.8f, 0.002f, 0.5f
        ),
        new(
            "Fortress",
            new float[7] { 1.75f, 0, 0, 0, 0, 0, 0 },
            0,
            1f, 0.5f, -0.05f
        ),

        //Ascended Specialist / T6
        new(
            "AscendedWindWalker",
            new float[7] { 0, 4f, 0, 0, 0, 4f, 0 },
            0,
            -0.25f, 0, 0, 0.75f, 1f, 0, 0, 0
        ),
        new(
            "AscendedHitman",
            new float[7] { 0, 4f, 0, 0, 0, 0, 4f },
            0,
            0.4f, 0.4f, 0, 0.6f, 1f, 0, 0, 0
        ),
        new(
            "AscendedSwordSaint",
            new float[7] { 19f, 0, 0, 0, 0, 0, 0 },
            0,
            -0.75f, 0, 0.5f, 0.99f
        ),
        new(
            "AscendedSoulLord",
            new float[7] { 0, 0, 0, 0, 9f, 0, 0 },
            0,
            1, 1f, 0.35f, 0, 0, 2, -0.75f, 0.75f, 0.02f, 10
        ),
        new(
            "AscendedMystic",
            new float[7] { 0, 0, 0, 9f, 0, 0, 0 },
            0,
            -0.25f, 1, 0, -1
        ),
        new(
            "AscendedShadowDancer",
            new float[7] { 0, 0, 9f, 0, 0, 0, 0 },
            0,
            0, 0, 0, 0.95f, 1, 0, 0, 0
        ),
        new(
            "AscendedDeity",
            new float[7] { 4f, 0, 0, 4f, 0, 0, 0 },
            0,
            4f, 4f, 0, 0, 1, 0.5f, 100
        ),
        new(
            "AscendedFortress",
            new float[7] { 3f, 0, 0, 0, 0, 0, 0 },
            0,
            14f, 14f, 0, 0.1f, 0, 0, 0, 0
        ),

        //Ascended Specialist / T7... Please balance this ?
        new(
            "TranscendentalBeing",
            new float[7] { 20, 20, 20, 20, 20, 20, 20 },
            0,
            20f, 20f, 0.25f, 0.95f, 0.99f, 5, -0.99999f, 0.9999f, 1f, 100
        )
    };

    public JsonChrClass GetClass(ClassType classType)
    {
        ClassType ClassName;
        for (var i = 0; i < jsonList.Length; i++)
        {
            ClassName = (ClassType)Enum.Parse(typeof(ClassType), jsonList[i].Name);

            if (ClassName == classType) return jsonList[i];
        }

        AnotherRpgModExpanded.Instance.Logger.Warn("Class " + classType + "is missing from class List");
        return jsonList[0];
    }
}

public class JsonChrClass
{
    public float Ammo;
    public float Armor;
    public float[] Damage; //Melee,Ranged,Throw,Magic,Summon,Bow,Gun
    public float Dodge;
    public float Health;
    public float ManaBaseEfficiency;
    public float ManaCost;
    public float ManaEfficiency;
    public float ManaShield;
    public float MovementSpeed;
    public string Name;
    public float Speed; //Melee speed
    public int Summons;

    [JsonConstructor]
    public JsonChrClass(string Name, float[] Damage, float Speed,
        float Health = 0, float Armor = 0, float MovementSpeed = 0, float Dodge = 0, float Ammo = 0,
        int Summons = 0, float ManaCost = 0,
        float ManaShield = 0, float ManaEfficiency = 0, float ManaBaseEfficiency = 0
    )
    {
        this.Name = Name;
        this.Damage = Damage;
        this.Speed = Speed;
        this.Health = Health;
        this.Armor = Armor;
        this.MovementSpeed = MovementSpeed;
        this.Dodge = Dodge;
        this.Ammo = Ammo;
        this.Summons = Summons;
        this.ManaCost = ManaCost;
        this.ManaShield = ManaShield;
        this.ManaEfficiency = ManaEfficiency;
        this.ManaBaseEfficiency = ManaBaseEfficiency;
    }

    public JsonChrClass(string Name, float[] Damage, float Speed, float Health = 0, float Armor = 0,
        int Summons = 0, float ManaCost = 0,
        float ManaShield = 0, float ManaEfficiency = 0, float ManaBaseEfficiency = 0) : this(Name, Damage, Speed,
        Health, Armor, 0, 0, 0, Summons, ManaCost, ManaShield, ManaEfficiency, ManaBaseEfficiency)
    {
    }

    public JsonChrClass(string Name, float[] Damage, float Speed, float Health = 0, float Armor = 0) : this(Name,
        Damage, Speed, Health, Armor, 0, 0, 0, 0, 0, 0)
    {
    }
}

internal class JsonCharacterClass
{
    public static string Name = "skillClass.json";
    public static string Dir = "Mod Configs" + Path.DirectorySeparatorChar + "AnRPG";
    public static string cPath;
    public static JsonChrClassList GetJsonCharList { get; private set; }


    public static void Init()
    {
        try
        {
            cPath = Main.SavePath + Path.DirectorySeparatorChar + Dir + Path.DirectorySeparatorChar + Name;
            GetJsonCharList = new JsonChrClassList();
            Load();
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }

    public static void Unload()
    {
        GetJsonCharList = null;
    }

    public static void Load()
    {
        try
        {
            Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + Dir);
            cPath = Main.SavePath + Path.DirectorySeparatorChar + Dir + Path.DirectorySeparatorChar + Name;
            GetJsonCharList = new JsonChrClassList();

            if (File.Exists(cPath))
                using (var reader = new StreamReader(cPath))
                {
                    if (Config.GpConfig.UseCustomSkillTree)
                        GetJsonCharList = JsonConvert.DeserializeObject<JsonChrClassList>(reader.ReadToEnd());
                }

            Save();
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Main.SavePath);
            File.WriteAllText(cPath,
                JsonConvert.SerializeObject(GetJsonCharList, Formatting.Indented).Replace("  ", "\t"));
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }
}