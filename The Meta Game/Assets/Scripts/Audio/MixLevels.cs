using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetSfxLvl(float sfxLvl)
    {

        masterMixer.SetFloat("SFX_Volume", sfxLvl);
        AkSoundEngine.SetRTPCValue("SFX_Volume", sfxLvl);
  
        }
    

    public void SetMxLvl(float mxLvl)
    {
        masterMixer.SetFloat("MX_Volume", mxLvl);
        AkSoundEngine.SetRTPCValue("MX_Volume", mxLvl);
    }

}
