using System;
using UnityEngine;

class Colors
{
    public static Color HslToColor(int h, float s, float l)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            float v1, v2;
            float hue = (float)h / 360;

            v2 = (l < 0.5) ? (l * (1 + s)) : ((l + s) - (l * s));
            v1 = 2 * l - v2;

            r = HueToRbg(v1, v2, hue + (1.0f / 3));
            g = HueToRbg(v1, v2, hue);
            b = HueToRbg(v1, v2, hue - (1.0f / 3));
        }

        return new Color(r, g, b);
    }

    private static float HueToRbg(float v1, float v2, float vH)
    {
        if (vH < 0)
            vH += 1;

        if (vH > 1)
            vH -= 1;

        if ((6 * vH) < 1)
            return (v1 + (v2 - v1) * 6 * vH);

        if ((2 * vH) < 1)
            return v2;

        if ((3 * vH) < 2)
            return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

        return v1;
    }

    public static bool EnoughDistance(int[] hues)
    {
        int min = 80;
        for (var i = 0; i < hues.Length; i++)
        {
            for (var j = i + 1; j < hues.Length; j++)
            {
                if (Math.Abs(hues[i] - hues[j]) < min) return false;
                if (Math.Abs(Math.Abs(hues[i] - hues[j]) - 360) < min) return false;
            }
        }
        return true;
    }

    public static string HueToColorName(int hue)
    {
        if (hue > 0 && hue <= 30) return "red";
        if (hue > 30 && hue <= 70) return "yellow";
        if (hue > 70 && hue <= 150) return "green";
        if (hue > 150 && hue <= 200) return "cyan";
        if (hue > 200 && hue <= 270) return "blue";
        if (hue > 270 && hue <= 330) return "magenta";
        if (hue > 330 && hue <= 360) return "red";
        return "";
    }
}