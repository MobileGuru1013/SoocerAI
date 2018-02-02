using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour {

    public Region Pitch;

    public TeamEnums OpposingTeamEnum;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            Pitch.GoalScored(OpposingTeamEnum);
            Debug.Log("Collision Detected in goal");
        }

        
    }
}
