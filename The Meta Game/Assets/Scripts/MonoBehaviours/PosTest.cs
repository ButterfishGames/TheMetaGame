using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PosTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log(GetComponentInChildren<TextMeshProUGUI>().text + ": " + GetComponent<RectTransform>().localPosition.y);
    }
}
