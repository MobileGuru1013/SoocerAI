using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Football : MonoBehaviour {

    //public Steer2D.Seek[] AgentSeek;
    //public Steer2D.Arrive[] AgentArrive;

    //void Start()
    //{
    //    UpdateAgentsTargets();              
    //}

    //void Update()
    //{
    //    UpdateAgentsTargets();
    //}


    //void UpdateAgentsTargets()
    //{

    //    foreach (Steer2D.Seek Seek in AgentSeek)
    //    {
    //        if (Seek != null)
    //        {
    //            Seek.TargetPoint = transform.position;
    //        }
    //    }

    //    foreach (Steer2D.Arrive Arrive in AgentArrive)
    //    {
    //        if (Arrive != null)
    //        {
    //            Arrive.TargetPoint = transform.position;
    //        }
    //    }
    //}

    public bool DebugOn = false;
    public bool DebugTextOn = false;

    Rigidbody2D RB;

    Color[] DebugColours = {Color.red, Color.blue, Color.yellow, Color.magenta, Color.cyan };
    Color DrawColour;
    int DegbugCount = 0;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        DrawColour = DebugColours[DegbugCount];
    }

    public float CalculateTimeToCoverDistance(Vector3 Target, float Force)
    {
        float TimeToCoverDistance = 0.0f;

       // Vector2 StartingVelocity = RB.velocity;
        //Vector2 StartingForce = (Target - gameObject.transform.position).normalized * Force;
        float Acceleration = Force / RB.mass;
        float Distance = Vector3.Distance(Target, gameObject.transform.position);


        TimeToCoverDistance = Mathf.Sqrt(((2 * Distance) / Acceleration));


        return TimeToCoverDistance;
    }

    public void AddForce(Vector2 ForceVec, Vector2 DebugTarget, string DebugString = "Kicking Ball")
    {
        if (DebugOn)
        {
          

            Debug.DrawLine(transform.position, DebugTarget, DrawColour, 0.5f);
            DrawColour = NextDebugColour();

        }

        if (DebugTextOn)
        {
            Debug.Log(DebugString);
        }

        //print("Kicking Ball");
        RB.velocity = new Vector2(0, 0); //Kicker wont deal with opposing forces already inacting on ball
        RB.AddForce(ForceVec, ForceMode2D.Impulse);
    }

    Color NextDebugColour()
    {
        Color NextCol = new Color();

        DegbugCount++;

        if (DegbugCount >= DebugColours.Length)
        {
            DegbugCount = 0;
        }

        NextCol = DebugColours[DegbugCount];

        return NextCol;

    }
}
