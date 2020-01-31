using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintDisp : MonoBehaviour
{
    public void SetDisplay(bool val)
    {
        GetComponent<SpriteRenderer>().enabled = val;
    }
}
