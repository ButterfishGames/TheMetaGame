using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{ 

    public void SetSfxLvl(float sfxLvl)
    {
        AkSoundEngine.SetRTPCValue("SFX_Volume", sfxLvl);
  
        }
    

    public void SetMxLvl(float mxLvl)
    {
        AkSoundEngine.SetRTPCValue("MX_Volume", mxLvl);
    }

}
