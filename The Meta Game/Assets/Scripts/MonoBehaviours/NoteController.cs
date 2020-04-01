using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public Sprite[] arrowSprites, xboxSprites, ps4Sprites, switchSprites;

    private int dir;

    private RhythmController rCon;
    private StaffController sCon;

    private void Start()
    {
        rCon = FindObjectOfType<RhythmController>();
        sCon = GetComponentInParent<StaffController>();
    }

    private void LateUpdate()
    {
        if (rCon.transform.position.x > transform.position.x + 1)
        {
            rCon.Miss(this);
            Destroy(gameObject);
        }
    }

    public void SetDir(int d)
    {
        if (d < 0 || d > 3)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = arrowSprites[d];
        dir = d;
    }

    public int GetDir()
    {
        return dir;
    }
}
