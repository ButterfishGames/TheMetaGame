using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log(GetComponent<RectTransform>().localPosition.y);
    }
}
