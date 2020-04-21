using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public bool constMove = false;
    public bool parallaxY = true;

    public float moveRateMult;

    public void UpdatePos(float xDiff, float yDiff)
    {
        float x = constMove ? xDiff - 1 : xDiff;
        x = x * moveRateMult;

        float y = parallaxY ? yDiff * moveRateMult : yDiff;

        transform.position += new Vector3(x, y, 0);
    }
}
