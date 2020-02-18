using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float moveRateMult;

    public void UpdatePos(float xDiff)
    {
        transform.position += new Vector3(xDiff * moveRateMult, 0, 0);
    }
}
