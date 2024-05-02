using System;
using System.IO;
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
public class JsonNodeList
{
    public JsonNode[] jsonList =
    {
        // BASE TYPE , POSX, POSY, NEIGHTBOORLIST, SPECIFIC TYPE, ISFLATDAMAGE?, VALUEPERLEVEL, LEVEL REQUIREMENT, MAXLEVEL , POINTS PER LEVEL
        new("Class", 0, 0, true, new int[6] { 1, 2, 3, 4, 5, 6 }, "Tourist", false, 0, 0, 1, 0), //0
        new("Class", 0, 250, false, new int[1] { 0 }, "Apprentice", false, 0.1f, 10, 1, 1), //1

        new("Damage", -100, 75, false, new int[4] { 0, 3, 7, 8 }, "Melee", false, 0.01f, 1, 20, 1), //2
        new("Damage", 100, 75, false, new int[5] { 0, 2, 4, 9, 10 }, "Magic", false, 0.01f, 1, 20, 1), //3
        new("Damage", 100, -50, false, new int[4] { 0, 3, 5, 11 }, "Summon", false, 0.01f, 1, 20, 1), //4
        new("Damage", 0, -111, false, new int[5] { 0, 5, 6, 12, 13 }, "Ranged", false, 0.01f, 1, 20, 1), //5
        new("Damage", -100, -50, false, new int[4] { 0, 5, 2, 14 }, "Throw", false, 0.01f, 1, 20, 1), //6

        new("Class", -200, 200, false, new int[2] { 2, 8 }, "Cavalier", false, 0.03f, 10, 1, 1), //7
        new("Class", -300, 100, false, new int[2] { 2, 7 }, "SwordMan", false, 0.03f, 10, 1, 1), //8
        new("Class", 300, 100, false, new int[2] { 3, 10 }, "Mage", false, 0.03f, 10, 1, 1), //9
        new("Class", 200, 300, false, new int[3] { 3, 9, 1 }, "Acolyte", false, 0.03f, 10, 1, 1), //10
        new("Class", 200, -200, false, new int[1] { 4 }, "Spiritualist", false, 0.03f, 10, 1, 1), //11
        new("Class", -75, -250, false, new int[2] { 5, 13 }, "Archer", false, 0.03f, 10, 1, 1), //12
        new("Class", 75, -250, false, new int[2] { 5, 12 }, "Gunner", false, 0.03f, 10, 1, 1), //13
        new("Class", -200, -150, false, new int[1] { 6 }, "Ninja", false, 0.03f, 10, 1, 1), //14

        new("Leech", 0, 350, false, new int[2] { 1, 22 }, "Life", false, 0.001f, 25, 5, 5), //15

        new("Class", 0, 450, false, new int[1] { 15 }, "Regular", false, 0.1f, 50, 1, 1), //16

        new("Stats", -300, 300, false, new int[2] { 7, 8 }, "Str", false, 2, 20, 15, 2), //17
        new("Stats", 350, 250, false, new int[2] { 9, 10 }, "Int", false, 2, 25, 15, 2), //18
        new("Stats", 300, -50, false, new int[2] { 9, 11 }, "Spr", false, 2, 25, 15, 2), //19
        new("Stats", 0, -325, false, new int[2] { 12, 13 }, "Agi", false, 2, 25, 15, 2), //20
        new("Stats", -200, -250, false, new int[2] { 12, 14 }, "Dex", false, 2, 25, 15, 2), //21

        new("Class", -200, 400, false, new int[2] { 7, 17 }, "Knight", false, 0.05f, 50, 1, 5), //22
        new("Class", -400, 200, false, new int[2] { 8, 17 }, "Mercenary", false, 0.05f, 50, 1, 5), //23
        new("Class", 400, 100, false, new int[2] { 9, 18 }, "ArchMage", false, 0.05f, 50, 1, 5), //24
        new("Class", 200, 400, false, new int[2] { 10, 18 }, "Monk", false, 0.05f, 50, 1, 5), //25
        new("Class", 400, -200, false, new int[2] { 11, 19 }, "Invoker", false, 0.05f, 50, 1, 5), //26
        new("Class", -75, -400, false, new int[2] { 12, 20 }, "Hunter", false, 0.05f, 50, 1, 5), //27
        new("Class", 75, -400, false, new int[2] { 13, 20 }, "Gunslinger", false, 0.05f, 50, 1, 5), //28
        new("Class", -325, -250, false, new int[2] { 14, 21 }, "Shinobi", false, 0.05f, 50, 1, 5), //29

        new("Damage", 400, 400, false, new int[2] { 24, 25 }, "Magic", false, 0.02f, 75, 50, 4), //30
        new("Damage", 450, -50, false, new int[2] { 24, 26 }, "Summon", false, 0.02f, 75, 50, 4), //31
        new("Damage", 0, -475, false, new int[2] { 27, 28 }, "Ranged", false, 0.02f, 75, 50, 4), //32
        new("Damage", -500, -400, false, new int[2] { 27, 29 }, "Throw", false, 0.02f, 75, 50, 4), //33
        new("Damage", -400, 400, false, new int[2] { 22, 23 }, "Melee", false, 0.02f, 75, 50, 4), //34
        new("Leech", 0, 550, false, new int[1] { 16 }, "Both", false, 0.001f, 100, 10, 10), //35

        new("Stats", 550, 550, false, new int[1] { 30 }, "Int", false, 10, 90, 10, 5), //36
        new("Stats", 600, -50, false, new int[1] { 31 }, "Spr", false, 10, 90, 10, 5), //37
        new("Stats", 100, -575, false, new int[1] { 32 }, "Agi", false, 10, 90, 10, 5), //38
        new("Stats", -625, -400, false, new int[1] { 33 }, "Dex", false, 10, 90, 10, 5), //39
        new("Stats", -550, 550, false, new int[1] { 34 }, "Str", false, 10, 90, 10, 5), //40


        //EXPERT CLASS
        new("Class", 0, 650, false, new int[1] { 35 }, "Expert", false, 0.1f, 100, 1, 3), //41

        new("Stats", -200, 750, false, new int[1] { 41 }, "Int", false, 50, 150, 1, 10), //42
        new("Stats", -100, 750, false, new int[1] { 41 }, "Spr", false, 50, 150, 1, 10), //43
        new("Stats", 0, 750, false, new int[1] { 41 }, "Agi", false, 50, 150, 1, 10), //44
        new("Stats", 100, 750, false, new int[1] { 41 }, "Dex", false, 50, 150, 1, 10), //45
        new("Stats", 200, 750, false, new int[1] { 41 }, "Str", false, 50, 150, 1, 10), //46

        new("Class", 0, 850, false, new int[5] { 42, 43, 44, 45, 46 }, "Master", false, 0.1f, 150, 1, 3), //47

        new("Damage", -200, 950, false, new int[1] { 47 }, "Magic", false, 0.2f, 200, 1, 20), //48
        new("Damage", -100, 950, false, new int[1] { 47 }, "Summon", false, 0.2f, 200, 1, 20), //49
        new("Damage", 0, 950, false, new int[1] { 47 }, "Ranged", false, 0.2f, 200, 1, 20), //50
        new("Damage", 100, 950, false, new int[1] { 47 }, "Throw", false, 0.2f, 200, 1, 20), //51
        new("Damage", 200, 950, false, new int[1] { 47 }, "Melee", false, 0.2f, 200, 1, 20), //52

        new("Stats", -200, 1050, false, new int[1] { 48 }, "Int", false, 200, 225, 1, 20), //53
        new("Stats", -100, 1050, false, new int[1] { 49 }, "Spr", false, 200, 225, 1, 20), //54
        new("Stats", 0, 1050, false, new int[1] { 50 }, "Agi", false, 200, 225, 1, 20), //55
        new("Stats", 100, 1050, false, new int[1] { 51 }, "Dex", false, 200, 225, 1, 20), //56
        new("Stats", 200, 1050, false, new int[1] { 52 }, "Str", false, 200, 225, 1, 20), //57

        new("Class", 0, 1200, false, new int[5] { 53, 54, 55, 56, 57 }, "PerfectBeing", false, 0.1f, 250, 1, 5), //58

        new("LimitBreak", 0, 1300, false, new int[1] { 58 }, "ASCEND", false, 1000, 1000, 1, 900), //59

        new("Class", 0, 1400, false, new int[1] { 59 }, "Ascended", false, 0.1f, 1001, 1, 1, true), //60

        new("Perk", -200, 550, false, new int[1] { 22 }, "Survivalist", false, 0.1f, 75, 5, 10), //61


        new("Class", -550, 700, false, new int[1] { 40 }, "IronKnight", false, 0.05f, 100, 1, 10), //62
        new("Class", -700, 550, false, new int[1] { 40 }, "SwordMaster", false, 0.05f, 100, 1, 10), //63
        new("Class", 700, 550, false, new int[1] { 36 }, "Arcanist", false, 0.05f, 100, 1, 10), //64
        new("Class", 550, 700, false, new int[1] { 36 }, "Templar", false, 0.05f, 100, 1, 10), //65
        new("Class", 700, -50, false, new int[1] { 37 }, "Summoner", false, 0.05f, 100, 1, 10), //66
        new("Class", 250, -575, false, new int[1] { 38 }, "Ranger", false, 0.05f, 100, 1, 10), //67
        new("Class", 100, -725, false, new int[1] { 38 }, "Spitfire", false, 0.05f, 100, 1, 10), //68
        new("Class", -625, -550, false, new int[1] { 39 }, "Rogue", false, 0.05f, 100, 1, 10), //69

        new("Stats", -450, 700, false, new int[1] { 62 }, "Vit", false, 10, 100, 10, 10), //70
        new("Stats", 450, 700, false, new int[1] { 65 }, "Vit", false, 10, 100, 10, 10), //71

        new("Perk", 200, 550, false, new int[1] { 25 }, "DemonEater", false, 0.1f, 75, 4, 10), //72

        new("Stats", -550, 800, false, new int[1] { 62 }, "Cons", false, 10, 100, 10, 10), //73
        new("Stats", 550, 800, false, new int[1] { 65 }, "Cons", false, 10, 100, 10, 10), //74
        new("Stats", -700, 450, false, new int[1] { 63 }, "Dex", false, 10, 100, 10, 10), //75
        new("Stats", -800, 550, false, new int[1] { 63 }, "Agi", false, 10, 100, 10, 10), //76

        new("Stats", -650, 800, false, new int[1] { 73 }, "Str", false, 15, 120, 4, 10), //77
        new("Stats", -900, 550, false, new int[1] { 76 }, "Str", false, 15, 120, 4, 10), //78

        new("Class", -750, 800, false, new int[1] { 77 }, "Mountain", false, 0.05f, 150, 1, 20), //79

        new("Stats", -750, 900, false, new int[1] { 79 }, "Vit", false, 30, 150, 3, 20), //80
        new("Damage", -750, 1000, false, new int[1] { 80 }, "Melee", false, 0.1f, 170, 3, 10), //81
        new("Stats", -650, 1000, false, new int[1] { 81 }, "Str", false, 25, 190, 5, 15), //82
        new("Stats", -650, 1100, false, new int[1] { 82 }, "Cons", false, 10, 220, 8, 5), //83

        new("Class", -750, 1100, false, new int[1] { 83 }, "Fortress", false, 0.05f, 250, 1, 50), //84

        new("Leech", -1000, 500, false, new int[1] { 78 }, "Life", false, 0.002f, 100, 3, 10), //85
        new("Stats", -1000, 600, false, new int[1] { 78 }, "Str", false, 10, 120, 5, 5), //86

        new("Class", -1100, 550, false, new int[2] { 85, 86 }, "Champion", false, 0.05f, 150, 1, 20), //87

        new("Leech", -1200, 650, false, new int[1] { 87 }, "Life", false, 0.002f, 160, 5, 20), //88
        new("Stats", -1100, 750, false, new int[1] { 88 }, "Str", false, 25, 190, 4, 15), //89
        new("Stats", -1200, 850, false, new int[1] { 89 }, "Agi", false, 10, 220, 5, 10), //90


        new("Class", -1100, 950, false, new int[1] { 90 }, "SwordSaint", false, 0.05f, 250, 1, 50), //91

        new("Stats", 650, 900, false, new int[1] { 74 }, "Foc", false, 10, 120, 10, 10), //92
        new("Stats", 550, 1000, false, new int[1] { 92 }, "Int", false, 10, 120, 10, 10), //93
        new("Class", 650, 1100, false, new int[1] { 93 }, "Paladin", false, 0.05f, 150, 1, 20), //94

        new("Damage", 750, 1150, false, new int[1] { 94 }, "Melee", false, 0.1f, 160, 8, 10), //95
        new("Damage", 700, 1250, false, new int[1] { 95 }, "Magic", false, 0.2f, 190, 4, 20), //96
        new("Stats", 650, 1350, false, new int[1] { 96 }, "Cons", false, 30, 220, 5, 16), //97
        new("Stats", 750, 1350, false, new int[1] { 96 }, "Vit", false, 15, 220, 10, 8), //98
        new("Class", 700, 1450, false, new int[2] { 98, 97 }, "Deity", false, 0.05f, 250, 1, 50), //99


        new("Stats", 800, 500, false, new int[1] { 64 }, "Foc", false, 10, 120, 10, 10), //100
        new("Stats", 800, 600, false, new int[1] { 100 }, "Int", false, 10, 120, 10, 10), //101
        new("Class", 900, 550, false, new int[1] { 101 }, "Warlock", false, 0.05f, 150, 1, 20), //102

        new("Damage", 900, 700, false, new int[1] { 102 }, "Magic", false, 0.1f, 160, 8, 10), //103
        new("Stats", 1000, 700, false, new int[1] { 103 }, "Int", false, 20, 190, 4, 20), //104
        new("Stats", 900, 800, false, new int[1] { 103 }, "Spr", false, 30, 220, 5, 16), //105
        new("Stats", 1000, 800, false, new int[2] { 104, 105 }, "Foc", false, 15, 220, 10, 8), //106
        new("Class", 1100, 900, false, new int[1] { 106 }, "Mystic", false, 0.05f, 250, 1, 50), //107


        new("Stats", 800, -150, false, new int[1] { 66 }, "Foc", false, 10, 110, 10, 10), //108
        new("Damage", 900, -250, false, new int[1] { 108 }, "Summon", false, 0.1f, 120, 8, 10), //109
        new("Stats", 900, -150, false, new int[1] { 108 }, "Spr", false, 10, 135, 10, 10), //110
        new("Class", 1000, -250, false, new int[2] { 109, 110 }, "SoulBinder", false, 0.05f, 150, 1, 20), //111

        new("Damage", 1100, -150, false, new int[1] { 111 }, "Summon", false, 0.1f, 160, 8, 10), //112
        new("Stats", 1100, 50, false, new int[1] { 112 }, "Spr", false, 20, 190, 4, 20), //113
        new("Stats", 950, -50, false, new int[1] { 113 }, "Spr", false, 30, 220, 5, 16), //114
        new("Stats", 800, 50, false, new int[1] { 114 }, "Foc", false, 15, 235, 10, 8), //115
        new("Class", 700, 50, false, new int[1] { 115 }, "SoulLord", false, 0.05f, 250, 1, 50), //116


        new("Stats", 250, -650, false, new int[1] { 67 }, "Dex", false, 10, 110, 10, 10), //117
        new("Damage", 250, -500, false, new int[1] { 67 }, "Ranged", false, 0.1f, 120, 8, 10), //118
        new("Stats", 325, -575, false, new int[2] { 117, 118 }, "Agi", false, 10, 135, 10, 10), //119
        new("Class", 400, -575, false, new int[1] { 119 }, "Marksman", false, 0.05f, 150, 1, 20), //120

        new("Damage", 750, -575, false, new int[1] { 120 }, "Ranged", false, 0.1f, 160, 8, 10), //121
        new("Stats", 825, -575, false, new int[1] { 121 }, "Vit", false, 10, 190, 10, 5), //122
        new("Stats", 825, -650, false, new int[1] { 122 }, "Agi", false, 30, 220, 8, 15), //123
        new("Stats", 825, -500, false, new int[1] { 122 }, "Dex", false, 20, 220, 8, 10), //124
        new("Class", 925, -575, false, new int[2] { 123, 124 }, "WindWalker", false, 0.05f, 250, 1, 50), //125


        new("Stats", 25, -725, false, new int[1] { 68 }, "Agi", false, 10, 110, 10, 10), //126
        new("Stats", 175, -725, false, new int[1] { 68 }, "Cons", false, 10, 110, 10, 10), //127

        new("Damage", 25, -825, false, new int[1] { 126 }, "Ranged", false, 0.1f, 120, 8, 10), //128
        new("Stats", 25, -900, false, new int[1] { 128 }, "Dex", false, 15, 135, 8, 15), //129

        new("Stats", 175, -825, false, new int[1] { 127 }, "Cons", false, 5, 120, 10, 8), //130
        new("Stats", 175, -900, false, new int[1] { 130 }, "Vit", false, 15, 135, 5, 12), //131

        new("Class", 100, -900, false, new int[2] { 129, 131 }, "Sniper", false, 0.05f, 150, 1, 20), //132


        new("Damage", 175, -975, false, new int[1] { 132 }, "Ranged", false, 0.15f, 160, 10, 12), //133
        new("Stats", 175, -1100, false, new int[1] { 133 }, "Dex", false, 10, 190, 10, 5), //134
        new("Stats", 150, -1175, false, new int[1] { 134 }, "Agi", false, 10, 220, 10, 5), //135

        new("Stats", 25, -975, false, new int[1] { 132 }, "Dex", false, 30, 160, 8, 15), //136
        new("Stats", 25, -1100, false, new int[1] { 136 }, "Cons", false, 20, 190, 8, 10), //137
        new("Stats", 50, -1175, false, new int[1] { 137 }, "Vit", false, 20, 220, 8, 10), //138

        new("Class", 100, -1250, false, new int[2] { 135, 138 }, "Hitman", false, 0.05f, 250, 1, 50), //139


        new("Stats", -625, -650, false, new int[1] { 69 }, "Dex", false, 10, 110, 10, 10), //140
        new("Damage", -725, -650, false, new int[1] { 140 }, "Throw", false, 0.1f, 120, 8, 10), //141
        new("Stats", -725, -550, false, new int[1] { 141 }, "Str", false, 10, 135, 10, 10), //142
        new("Class", -725, -450, false, new int[1] { 142 }, "Assassin", false, 0.05f, 150, 1, 20), //143

        new("Damage", -825, -450, false, new int[1] { 143 }, "Throw", false, 0.1f, 160, 8, 10), //144
        new("Stats", -925, -450, false, new int[1] { 144 }, "Dex", false, 20, 190, 4, 20), //145
        new("Stats", -925, -350, false, new int[1] { 145 }, "Dex", false, 30, 220, 5, 16), //146
        new("Stats", -825, -350, false, new int[1] { 146 }, "Str", false, 15, 235, 10, 8), //147
        new("Class", -725, -350, false, new int[1] { 147 }, "ShadowDancer", false, 0.05f, 250, 1, 50), //148

        new("Perk", -350, 800, false, new int[1] { 70 }, "Biologist", false, 0.1f, 120, 5, 8), //149
        new("Perk", -1200, 450, false, new int[1] { 87 }, "Vampire", false, 0.1f, 175, 3, 20), //150
        new("Perk", 750, 1000, false, new int[1] { 92 }, "Chlorophyll", false, 0.1f, 135, 3, 15), //151
        new("Perk", 1100, 600, false, new int[1] { 104 }, "BloodMage", false, 0.1f, 225, 3, 30), //152
        new("Perk", 1200, -250, false, new int[1] { 112 }, "Masochist", false, 0.1f, 200, 3, 15), //153
        new("Perk", 100, -975, false, new int[1] { 132 }, "Berserk", false, 0.1f, 175, 3, 20), //154
        new("Perk", -200, 1200, false, new int[1] { 58 }, "TheGambler", false, 0.1f, 300, 1, 50), //155
        //
        new("Perk", -800, -725, false, new int[1] { 141 }, "Cupidon", false, 0, 150, 5, 8), //156
        new("Perk", 500, 100, false, new int[1] { 24 }, "StarGatherer", false, 0, 75, 5, 8), //157
        new("Perk", 800, 700, false, new int[1] { 101 }, "ManaOverBurst", false, 0, 150, 3, 15), //158


        new("Class", -350, 1450, false, new int[1] { 60 }, "AscendedShadowDancer", false, 0.1f, 1500, 1, 200,
            true), //159
        new("Class", -250, 1500, false, new int[1] { 60 }, "AscendedWindWalker", false, 0.1f, 1500, 1, 200, true), //160
        new("Class", -150, 1550, false, new int[1] { 60 }, "AscendedHitman", false, 0.1f, 1500, 1, 200, true), //161
        new("Class", -50, 1600, false, new int[1] { 60 }, "AscendedSoulLord", false, 0.1f, 1500, 1, 200, true), //162
        new("Class", 50, 1600, false, new int[1] { 60 }, "AscendedMystic", false, 0.1f, 1500, 1, 200, true), //163
        new("Class", 150, 1550, false, new int[1] { 60 }, "AscendedDeity", false, 0.1f, 1500, 1, 200, true), //164
        new("Class", 250, 1500, false, new int[1] { 60 }, "AscendedFortress", false, 0.1f, 1500, 1, 200, true), //165
        new("Class", 350, 1450, false, new int[1] { 60 }, "AscendedSwordSaint", false, 0.1f, 1500, 1, 200, true), //166


        new("Stats", -350, 1850, false, new int[1] { 159 }, "Dex", false, 1000, 1750, 100, 50, true), //167
        new("Stats", -250, 1800, false, new int[1] { 160 }, "Agi", false, 1000, 1750, 100, 50, true), //168
        new("Stats", -150, 1750, false, new int[1] { 161 }, "Agi", false, 1000, 1750, 100, 50, true), //169
        new("Stats", -50, 1700, false, new int[1] { 162 }, "Spr", false, 1000, 1750, 100, 50, true), //170
        new("Stats", 50, 1700, false, new int[1] { 163 }, "Int", false, 1000, 1750, 100, 50, true), //171
        new("Stats", 150, 1750, false, new int[1] { 164 }, "Int", false, 1000, 1750, 100, 50, true), //172
        new("Stats", 250, 1800, false, new int[1] { 165 }, "Str", false, 1000, 1750, 100, 50, true), //173
        new("Stats", 350, 1850, false, new int[1] { 166 }, "Str", false, 1000, 1750, 100, 50, true), //174

        new("LimitBreak", 0, 1900, false, new int[8] { 167, 168, 169, 170, 171, 172, 173, 174 }, "TRANSCEND", false,
            9999, 2000, 1, 100), //175

        new("Class", 0, 2000, false, new int[1] { 175 }, "TranscendentalBeing", false, 0.1f, 2001, 1, 400, true) //176
    };
}

public class JsonNode
{
    public bool ascended;
    public string baseType = "Damage";
    public bool flatDamage;
    public int levelRequirement;
    public int maxLevel = 5;
    public int[] neigthboorlist = { 1, 2 };
    public int pointsPerLevel = 1;
    public float posX = 50;
    public float posY;
    public string specificType = "Magic";
    public bool unlocked;
    public float valuePerLevel = 0.1f;

    [JsonConstructor]
    public JsonNode(string baseType, float posX, float posY, bool unlocked, int[] neightboorlist, string specificType,
        bool flatDamage, float valuePerLevel, int levelRequirement, int maxLevel, int pointsPerLevel,
        bool ascended = false)
    {
        this.baseType = baseType;
        this.posX = posX;
        this.posY = posY;
        this.unlocked = unlocked;
        neigthboorlist = neightboorlist;
        this.specificType = specificType;
        this.flatDamage = flatDamage;
        this.valuePerLevel = valuePerLevel;
        this.levelRequirement = levelRequirement;
        this.maxLevel = maxLevel;
        this.pointsPerLevel = pointsPerLevel;
        this.ascended = ascended;
    }
}

internal class JsonSkillTree
{
    public static string Name = "skillTree.json";
    public static string Dir = "Mod Configs" + Path.DirectorySeparatorChar + "AnRPG";
    public static string cPath;
    public static JsonNodeList GetJsonNodeList { get; private set; }


    public static void Init()
    {
        try
        {
            cPath = Main.SavePath + Path.DirectorySeparatorChar + Dir + Path.DirectorySeparatorChar + Name;
            GetJsonNodeList = new JsonNodeList();
            Load();
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }

    public static void Load()
    {
        try
        {
            Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + Dir);
            cPath = Main.SavePath + Path.DirectorySeparatorChar + Dir + Path.DirectorySeparatorChar + Name;
            GetJsonNodeList = new JsonNodeList();

            if (File.Exists(cPath))
                using (var reader = new StreamReader(cPath))
                {
                    if (Config.GpConfig.UseCustomSkillTree)
                        GetJsonNodeList = JsonConvert.DeserializeObject<JsonNodeList>(reader.ReadToEnd());
                }

            Save();
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }

    public static void Unload()
    {
        GetJsonNodeList = null;
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + Dir);
            File.WriteAllText(cPath,
                JsonConvert.SerializeObject(GetJsonNodeList, Formatting.Indented).Replace("  ", "\t"));
        }
        catch (SystemException e)
        {
            AnotherRpgModExpanded.Instance.Logger.Error(e.ToString());
        }
    }
}