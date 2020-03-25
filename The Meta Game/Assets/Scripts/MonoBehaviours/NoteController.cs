using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public Sprite[] arrowSprites, xboxSprites, ps4Sprites, switchSprites;

    public void SetDir(int dir)
    {
        if (dir < 0 || dir > 3)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = arrowSprites[dir];
    }
}
