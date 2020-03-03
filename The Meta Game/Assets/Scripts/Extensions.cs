using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string HexEmbed(this string str, string hex)
    {
        string res = "<color=#" + hex + ">" + str + "</color>";
        return res;
    }

    public static string HexEmbed(this string str, Color color)
    {
        string hexR = Mathf.RoundToInt(color.r * 255).ToString("X");
        string hexG = Mathf.RoundToInt(color.g * 255).ToString("X");
        string hexB = Mathf.RoundToInt(color.b * 255).ToString("X");
        string hex = hexR + hexG + hexB;
        string res = "<color=#" + hex + ">" + str + "</color>";
        return res;
    }

    public static string HexEmbed(this string str, int dec)
    {
        string res = "<color=#" + dec.ToString("X") + ">" + str + "</color>";
        return res;
    }

    public static string HexEmbed(this string str, float r, float g, float b)
    {
        Color color = new Color(r, g, b);
        string res = str.HexEmbed(color);
        return res;
    }

    public static string HexEmbed(this string str, int r, int g, int b)
    {
        string hexR = r.ToString("X");
        string hexG = g.ToString("X");
        string hexB = b.ToString("X");
        string hex = hexR + hexG + hexB;
        string res = "<color=#" + hex + ">" + str + "</color>";
        return res;
    }
}
