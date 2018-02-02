using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Behaviour { Arrive, Evade, Flee, Flock, FollowPath, Pursue, Seek};

public class SteeringController : MonoBehaviour
{
    List<Steer2D.SteeringBehaviour> Behaviours = new List<Steer2D.SteeringBehaviour>();


    void Awake()
    {
        Behaviours = GetComponents<Steer2D.SteeringBehaviour>().ToList();

        foreach (Steer2D.SteeringBehaviour b in Behaviours)
        {
           // print(b.GetType().ToString());
        }
    }

    public void TurnOn(Behaviour B)
    {
        switch (B)
        {
            case Behaviour.Arrive:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Arrive");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.Evade:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Evade");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.Flee:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Flee");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.Flock:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Flock");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.FollowPath:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.FollowPath");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.Pursue:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Pursue");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;
            case Behaviour.Seek:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Seek");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = true;
                    }
                }
                break;

        }
    }

    public void TurnOff(Behaviour B)
    {
        switch (B)
        {
            case Behaviour.Arrive:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Arrive");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.Evade:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Evade");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.Flee:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Flee");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.Flock:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Flock");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.FollowPath:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.FollowPath");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.Pursue:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Pursue");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;
            case Behaviour.Seek:
                {
                    Steer2D.SteeringBehaviour SteeringBeh = GetBehaviourByTypeName("Steer2D.Seek");

                    if (SteeringBeh)
                    {
                        SteeringBeh.enabled = false;
                    }
                }
                break;

        }
    }

    public Steer2D.SteeringBehaviour GetBehaviourByTypeName(string Name)
    {

        foreach (Steer2D.SteeringBehaviour B in Behaviours)
        {
            if (B.GetType().ToString() == Name)
            {
                return B;
            }

            int testser = 0;
        }

        print(Name + ": Behaviour not found when turning on/off");
        return null;
    }

}
