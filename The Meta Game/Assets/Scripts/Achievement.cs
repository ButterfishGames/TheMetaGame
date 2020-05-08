using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string name;
    [TextArea(3,10)] public string desc;
    public Sprite img;
    public bool unlocked;
    public bool secret;

    public Achievement(string aName, string aDesc, Sprite aImg, bool aSecret)
    {
        name = aName;
        desc = aDesc;
        img = aImg;
        unlocked = false;
        secret = aSecret;
    }

    public void Unlock()
    {
        unlocked = true;
        // TODO: Steam Achievement unlock call
    }
}
