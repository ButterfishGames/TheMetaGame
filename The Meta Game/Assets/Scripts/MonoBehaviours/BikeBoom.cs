using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeBoom : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(BikeFall());
    }

    private IEnumerator BikeFall()
    {
        gameObject.AddComponent<Rigidbody2D>();

        GetComponent<SpriteRenderer>().enabled = true;
        float t = 0;

        while (Mathf.Round(transform.rotation.eulerAngles.z) != 0)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, transform.rotation.y, t));
            yield return new WaitForEndOfFrame();
        }

        GetComponent<Animator>().enabled = true;
    }
}
