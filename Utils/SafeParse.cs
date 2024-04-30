using System;
using System.Globalization;

namespace AnotherRpgModExpanded.Utils;

public static class FloatExtention
{
    public static float SafeFloatParse(this string input)
    {
        if (string.IsNullOrEmpty(input)) throw new ArgumentNullException("input");

        input = input.Replace(',', '.');
        float res;

        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out res)) return res;

        if (float.TryParse(input, out res)) return res;

        throw new ArgumentException("Fail To Parse");
    }
}