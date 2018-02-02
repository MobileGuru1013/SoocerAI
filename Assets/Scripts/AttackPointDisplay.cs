using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointDisplay : MonoBehaviour {

    public bool Blue;

    void OnDrawGizmos()
    {
        if (Blue)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.3f);
        
    }
}
