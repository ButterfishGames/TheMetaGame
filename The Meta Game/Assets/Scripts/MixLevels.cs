using UnityEngine;
using System.Collections;

public class MixLevels
{ 
    public static void SetSfxLvl(float sfxLvl)
    {
        AkSoundEngine.SetRTPCValue("SFX_Volume", sfxLvl);
    }
    
    public static void SetMxLvl(float mxLvl)
    {
        AkSoundEngine.SetRTPCValue("MX_Volume", mxLvl);
    }
}
