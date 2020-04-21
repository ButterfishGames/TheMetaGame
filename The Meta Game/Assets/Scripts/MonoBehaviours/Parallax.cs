using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public bool constMove = false;
    public bool parallaxY = true;

    public float moveRateMult;
    public float moveSpeed = 0;

    public void UpdatePos(float xDiff, float yDiff)
    {
        float x = xDiff * moveRateMult;
        x = constMove ? x - moveSpeed : x;

        float y = parallaxY ? yDiff * moveRateMult : yDiff;

        transform.position += new Vector3(x, y, 0);
    }
}
