using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour {


    Quaternion StartingRot;
	// Use this for initialization
	void Start ()
    {
        StartingRot = transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {

        // rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
        //transform.parent.transform.rotation;
      
        transform.localRotation = Quaternion.Inverse(transform.parent.transform.rotation);

    }
}
