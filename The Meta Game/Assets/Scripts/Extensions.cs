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
        int dec = Mathf.FloorToInt(color.r * 255 * 255 * 255) + Mathf.FloorToInt(color.g * 255 * 255) + Mathf.FloorToInt(color.b * 255);
        string hex = dec.ToString("X");
        Debug.Log(hex);
        string res = "<color=#" + color.GetHashCode().ToString("X") + ">" + str + "</color>";
        return res;
    }

    public static string HexEmbed(this string str, int hash)
    {
        string res = "<color=#" + hash.ToString("X") + ">" + str + "</color>";
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
        Color color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        string res = str.HexEmbed(color);
        return res;
    }
}
