using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryUpdater : MonoBehaviour
{
    public static Sprite lockFrame;
    public Sprite nsLockFrame;
    public Sprite art;

    private Image artImg;
    private int artInd;
    private bool unlocked;

    // Start is called before the first frame update
    void Start()
    {
        if (nsLockFrame != null)
        {
            lockFrame = nsLockFrame;
        }

        artImg = GetComponent<Image>();

        artInd = int.Parse(transform.parent.parent.name.Substring(7, 1));
    }

    public void SetUnlocked (bool val)
    {
        unlocked = val;
        if (unlocked)
        {
            artImg.sprite = art;
        }
        else
        {
            artImg.sprite = lockFrame;
        }
    }

    public int GetInd()
    {
        return artInd;
    }
}
