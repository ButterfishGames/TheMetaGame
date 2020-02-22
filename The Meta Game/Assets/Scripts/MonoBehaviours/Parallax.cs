using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float moveRateMult;

    public void UpdatePos(float xDiff, float yDiff)
    {
        transform.position += new Vector3(xDiff * moveRateMult, yDiff * moveRateMult, 0);
    }
}
