using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAFew : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAFewRtn());
    }

    private IEnumerator WaitAFewRtn()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
}
