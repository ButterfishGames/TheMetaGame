using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGDreadKnight : EnemyBehaviour
{
    #region variables

    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            GameController.singleton.Battle();
        }
    }
}
