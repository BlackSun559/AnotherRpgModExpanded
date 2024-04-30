using System;
using Terraria;
using Terraria.Utilities;

//Just Simple utils tool

namespace AnotherRpgModExpanded.Utils;

public static class Mathf // float math
{
    public static int HugeCalc(int val, int original)
    {
        if (val == int.MinValue)
            return int.MaxValue;
        return val;
    }

    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if (val.CompareTo(max) > 0) return max;
        return val;
    }

    public static T Min<T>(this T val, T min) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        return val;
    }

    public static int GenNewSeed()
    {
        var seed = Main.rand.Next();
        Main.rand = new UnifiedRandom(seed);
        return seed;
    }

    public static void NewSeed(int seed)
    {
        Main.rand = new UnifiedRandom(seed);
    }

    public static float Log2(float x)
    {
        return (float)Math.Log(x, 2);
    }

    public static float Logx(float x, float b)
    {
        return (float)Math.Log(x, b);
    }

    public static int RandomInt(int a, int b)
    {
        if (Main.rand == null)
            return a;
        return Main.rand.Next(a, b);
    }

    public static float Random(float a, float b)
    {
        if (Main.rand == null)
            return a;
        return a + (float)Main.rand.NextDouble() * b;
    }

    public static float Pow(double number, double power)
    {
        return (float)Math.Pow(number, power);
    }

    public static float Pow(float number, float power)
    {
        return (float)Math.Pow(number, power);
    }

    public static float Pow(int number, int power)
    {
        return (float)Math.Pow(number, power);
    }

    public static float Pow(float number, int power)
    {
        return (float)Math.Pow(number, power);
    }

    public static float Pow(int number, float power)
    {
        return (float)Math.Pow(number, power);
    }

    public static float Log(double number)
    {
        return (float)Math.Log(number);
    }

    public static float Log(float number)
    {
        return (float)Math.Log(number);
    }

    public static float Log(int number)
    {
        return (float)Math.Log(number);
    }

    public static int RoundInt(double number)
    {
        return (int)Math.Round(number);
    }

    public static int RoundInt(float number)
    {
        return (int)Math.Round(number);
    }

    public static int RoundInt(int number)
    {
        return (int)Math.Round((double)number);
    }

    public static float Round(double number, int dec = 1)
    {
        return (float)Math.Round(number, dec);
    }

    public static float Round(float number, int dec = 1)
    {
        return (float)Math.Round(number, dec);
    }

    public static float Round(int number, int dec = 1)
    {
        return (float)Math.Round((double)number, dec);
    }

    public static float Ceil(double number)
    {
        return (float)Math.Ceiling(number);
    }

    public static float Ceil(float number)
    {
        return (float)Math.Ceiling(number);
    }

    public static float Ceil(int number)
    {
        return (float)Math.Ceiling((double)number);
    }

    public static int CeilInt(double number)
    {
        return (int)Math.Ceiling(number);
    }

    public static int CeilInt(float number)
    {
        return (int)Math.Ceiling(number);
    }

    public static int CeilInt(int number)
    {
        return (int)Math.Ceiling((double)number);
    }

    public static float Floor(double number)
    {
        return (float)Math.Floor(number);
    }

    public static float Floor(float number)
    {
        return (float)Math.Floor(number);
    }

    public static float Floor(int number)
    {
        return (float)Math.Floor((double)number);
    }

    public static int FloorInt(double number)
    {
        return (int)Math.Floor(number);
    }

    public static int FloorInt(float number)
    {
        return (int)Math.Floor(number);
    }

    public static int FloorInt(int number)
    {
        return (int)Math.Floor((double)number);
    }

    public static long Floorlong(double number)
    {
        return (long)Math.Floor(number);
    }

    public static long Floorlong(float number)
    {
        return (long)Math.Floor(number);
    }

    public static long Floorlong(long number)
    {
        return (long)Math.Floor((double)number);
    }

    public static long Ceillong(double number)
    {
        return (long)Math.Ceiling(number);
    }

    public static long Ceillong(float number)
    {
        return (long)Math.Ceiling(number);
    }

    public static long Ceillong(long number)
    {
        return (long)Math.Ceiling((double)number);
    }
}