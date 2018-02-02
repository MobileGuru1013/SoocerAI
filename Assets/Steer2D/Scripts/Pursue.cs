using System;
using UnityEngine;

namespace Steer2D
{
    public class Pursue : SteeringBehaviour
    {
        //public SteeringAgent TargetAgent;

        //public override Vector2 GetVelocity()
        //{
        //    float t = Vector3.Distance(transform.position, TargetAgent.transform.position) / TargetAgent.MaxVelocity;
        //    Vector2 targetPoint = (Vector2)TargetAgent.transform.position + TargetAgent.CurrentVelocity * t;

        //    return ((targetPoint - (Vector2)transform.position).normalized * agent.MaxVelocity) - agent.CurrentVelocity;
        //}

        public GameObject TargetAgent;

        public override Vector2 GetVelocity()
        {
            float Speed = TargetAgent.GetComponent<Rigidbody2D>().velocity.magnitude;

            Vector2 vel =  TargetAgent.GetComponent<Rigidbody2D>().velocity;

            float t = Vector3.Distance(transform.position, TargetAgent.transform.position) / Speed;
            Vector2 targetPoint = (Vector2)TargetAgent.transform.position + TargetAgent.GetComponent<Rigidbody2D>().velocity * t;

            return ((targetPoint - (Vector2)transform.position).normalized * agent.MaxVelocity) - agent.CurrentVelocity;
        }
    }
}
