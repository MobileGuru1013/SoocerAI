using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
/*
 *  ******************************************** Global State ********************** 
 */

public class GlobalPlayerState : State
{
    static GlobalPlayerState instance;

    public static GlobalPlayerState Instance()
    {
        if (instance == null)
        {
            instance = new GlobalPlayerState();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        //Empty
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        //TODO
    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        //Empty
    }

    public override bool OnMessage(GameObject CallingObject, Message Msg)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();


        switch (Msg.Msg)
        {
            case PlayerMessages.ReceiveBall:
                {
                    //GCHandle Handle = (GCHandle)Msg.ExtraInfo;
                    //Vector2 Pos = (Vector2)Handle.Target;

                    Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");
                    Arr.TargetPoint = Msg.ExtraInfo;
                    
                    PlayerScript.ChangeState(CallingObject, ReceiveBall.Instance());
                    return true;
                }
            case PlayerMessages.SupportAttacker:
                {
                    if(PlayerScript.IsInState(SupportAttacker.Instance()))
                    {
                        return true;
                    }

                    //TODO Behaviour
                    PlayerScript.ChangeState(CallingObject, SupportAttacker.Instance());
                    return true;
                }
               

            case PlayerMessages.Wait:
                {
                    PlayerScript.ChangeState(CallingObject, Wait.Instance());
                    return true;
                }             

            case PlayerMessages.PassToMe:
                {
                    GameObject Reciever = Msg.Sender;

                    //If there is already a recieving player or the ball isnt in range then cant pass pall to requesting player
                    if (PlayerScript.GetTeam().RecievingPlayer != null || !PlayerScript.BallInKickingRange())
                    {
                        //Debug.Log("Can't make requested Pass");
                        return true;
                    }

                    //TODO double check this

                    Vector2 PassTarget = PlayerScript.AddNoiseToTarget(Reciever.gameObject.transform.position);

                    //Get the direction of the shot
                    Vector2 KickDir = (PassTarget - (Vector2)PlayerScript.Ball.transform.position).normalized;

                    float dot = Vector3.Dot(CallingObject.transform.right, (PlayerScript.Ball.transform.position - CallingObject.transform.position).normalized);
                    float KickPower = PlayerScript.PassingForce * dot;

                    //Add force to the ball

                    if (!PlayerScript.IsReadyForNextKick())
                    {

                        return true;
                    }

                    PlayerScript.Ball.GetComponent<Football>().AddForce(KickDir * KickPower, PassTarget, "Passing To Reqested Pass");



                    //GCHandle Handle = GCHandle.Alloc(Reciever.transform.position);
                    //System.IntPtr PositionPtr = (System.IntPtr)Handle;

                    Dispatcher.Instance().DispatchMessage(0, CallingObject, Reciever, PlayerMessages.ReceiveBall, PassTarget);

                    PlayerScript.ChangeState(CallingObject, Wait.Instance());

                    PlayerScript.FindSupport();

                    return true;
                }

            case PlayerMessages.GoHome:
                {
                    PlayerScript.SetDefaultHomeRegion();
                    PlayerScript.ChangeState(CallingObject, ReturnToHomeRegion.Instance());
                    return true;
                }
                
        }


        return false;
    }
}

/*
 *  ******************************************** ChaseBall State ********************** 
 */

public class ChaseBall : State
{
    static ChaseBall instance;

    public static ChaseBall Instance()
    {
        if (instance == null)
        {
            instance = new ChaseBall();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOn(Behaviour.Seek);

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Player Chase Ball State");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.BallInKickingRange())
        {
            PlayerScript.ChangeState(CallingObject, KickBall.Instance());

            return;
        }


        if (PlayerScript.IsClosestTeamMemberToBall())
        {
            Steer2D.Seek S = (Steer2D.Seek)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Seek");

            if (S)
            {
                S.TargetPoint = PlayerScript.Ball.transform.position;
            }

            return;
        }

        PlayerScript.ChangeState(CallingObject, ReturnToHomeRegion.Instance());
    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOff(Behaviour.Seek);

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting ChaseBall State");
        }
    }
}

/*
 *  ******************************************** Dribble State ********************** 
 */

public class Dribble : State
{
    static Dribble instance;

    public static Dribble Instance()
    {
        if (instance == null)
        {
            instance = new Dribble();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetTeam().SetControllingPlayer(PlayerScript);

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Player Dribble State");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        float dot = Vector3.Dot(CallingObject.transform.right, (PlayerScript.OpponentsGoal.transform.position - CallingObject.transform.position).normalized);

        if (dot > 0.7f) //Facing the goal
        {
            //Dribble towards goal
            Vector2 Target = PlayerScript.transform.position + (CallingObject.transform.right * PlayerScript.DribbleForce);
            PlayerScript.Ball.GetComponent<Football>().AddForce(CallingObject.transform.right * PlayerScript.DribbleForce, Target, "Dribbling Towards Goal");
        }
        else
        {
            //Kick to your prefered turn dir
            //Debug.Log("Turning With Ball");
            Vector2 Target = PlayerScript.transform.position + (CallingObject.transform.right * PlayerScript.DribbleForce);

            PlayerScript.Ball.GetComponent<Football>().AddForce(CallingObject.transform.up * PlayerScript.TurningForce * PlayerScript.PreferedTurnDir, Target, "Turning With Ball");
        }

        PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        //Empty
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting Player Dribble State");
        }
    }
}
/*
 *  ******************************************** ReturnToHomeRegion State ********************** 
 */

public class ReturnToHomeRegion : State
{
    static ReturnToHomeRegion instance;

    public static ReturnToHomeRegion Instance()
    {
        if (instance == null)
        {
            instance = new ReturnToHomeRegion();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOn(Behaviour.Arrive);

        Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");

        Arr.TargetPoint = PlayerScript.HomePosition;

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Player Return to home state");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.GetTeam().GetPitch().GetGameInPlay())
        {

            //If closest to ball and not in goalkeepers hands... Chase the ball
            if ( PlayerScript.IsClosestTeamMemberToBall() && (!PlayerScript.GetTeam().GetPitch().GoalKeeperHasBall()))
            {
                PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
                return;
            }
        }

        Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");

        if (Arr.AtTarget)
        {
            PlayerScript.ChangeState(CallingObject, Wait.Instance());
        }
    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting Player Return to home state");
        }

    }
}
/*
 *  ******************************************** Wait State ********************** 
 */

public class Wait : State
{
    static Wait instance;

    public static Wait Instance()
    {
        if (instance == null)
        {
            instance = new Wait();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Wait State");
        }

        PlayerScript.GetSteeringController().TurnOn(Behaviour.Arrive);

        //Empty
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.TrackBall();

      

        if (PlayerScript.GetTeam().GetPitch().GetGameInPlay())
        {

            //If closest to ball and not in goalkeepers hands... Chase the ball
            if (PlayerScript.IsClosestTeamMemberToBall() && (!PlayerScript.GetTeam().GetPitch().GoalKeeperHasBall()))
            {
                PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
                return;
            }
        }

        if (PlayerScript.GetTeam().InControl() && (PlayerScript.GetTeam().ControllingPlayer != PlayerScript && PlayerScript.AheadOfAttacker()))
        {
            PlayerScript.GetTeam().RequestPass(CallingObject);
            return;
        }
    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        //Empty
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting Wait State");
        }

        PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);

    }
}
/*
 *  ******************************************** KickBall State ********************** 
 */

public class KickBall : State
{
    static KickBall instance;

    public static KickBall Instance()
    {
        if (instance == null)
        {
            instance = new KickBall();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetTeam().SetControllingPlayer(PlayerScript);

        if (!PlayerScript.IsReadyForNextKick())
        {
            PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
        }

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Kick Ball State");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        float dot = Vector3.Dot(CallingObject.transform.right, (PlayerScript.Ball.transform.position - CallingObject.transform.position).normalized);
        float KickPower;
        //If goal keeper has ball/already a recieving player / ball behind player the chase ball
        if ( PlayerScript.GetTeam().GetPitch().GoalKeeperHasBall() || (dot < 0))
        {
            PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
        }

        //If he can shoot
        Vector2 ShootingTarget = new Vector2();

        if (PlayerScript.InShootingRange())
        {

            if (PlayerScript.GetTeam().CanShoot(CallingObject.transform.position, out ShootingTarget, PlayerScript.MaxShootingForce, PlayerScript.ShootingConfidence))
            {


                //Get the Target the player is aiming at
                Vector2 ShotTarget = PlayerScript.AddNoiseToTarget(ShootingTarget);

                //Get the direction of the shot
                Vector2 KickDir = (ShotTarget - (Vector2)PlayerScript.Ball.transform.position).normalized;

                //Set a kick power based on if the player is facing the ball
                KickPower = PlayerScript.MaxShootingForce * dot;



                //Add force to the ball
                PlayerScript.Ball.GetComponent<Football>().AddForce(KickDir * KickPower, ShotTarget, "Shooting Towards Goal");


                PlayerScript.ChangeState(CallingObject, Wait.Instance());

                PlayerScript.FindSupport();

                return;
            }
        }

        KickPower = PlayerScript.PassingForce * dot;

        Player Reciever = null;
        Vector3 PassingTarget = new Vector3();


    
        //Attempt to pass to player
        if (PlayerScript.IsThreatened() && PlayerScript.GetTeam().FindPass(PlayerScript, out Reciever, out PassingTarget, KickPower))
        {

            float FacingPassingPlayerDot = Vector3.Dot(CallingObject.transform.right, (PassingTarget - CallingObject.transform.position).normalized);

            if (dot < 0.7f) //not facing the player
            {
                PlayerScript.FindSupport();

                PlayerScript.ChangeState(CallingObject, Dribble.Instance());

                return;
            }


            Vector2 PassTarget = PlayerScript.AddNoiseToTarget(PassingTarget); //TODO **** Make sure this works! *****

            //Get the direction of the shot
            Vector2 KickDir = (PassTarget - (Vector2)PlayerScript.Ball.transform.position).normalized;

            //Add force to the ball
            PlayerScript.Ball.GetComponent<Football>().AddForce(KickDir * KickPower, PassTarget,"Passing To Player Indepentently");


           

            //GCHandle Handle = GCHandle.Alloc(PassTarget);
            //System.IntPtr PositionPtr = (System.IntPtr)Handle;

            Dispatcher.Instance().DispatchMessage(0, CallingObject, Reciever.gameObject, PlayerMessages.ReceiveBall, PassTarget);

            PlayerScript.ChangeState(CallingObject, Wait.Instance());

            PlayerScript.FindSupport();

            return;
        }
        else
        {
            PlayerScript.FindSupport();

            PlayerScript.ChangeState(CallingObject, Dribble.Instance());

        }

    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting KickBall State");
        }
    }
}
/*
 *  ******************************************** ReceiveBall State ********************** 
 */

public class ReceiveBall : State
{
    static ReceiveBall instance;

    public static ReceiveBall Instance()
    {
        if (instance == null)
        {
            instance = new ReceiveBall();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        //Set as recieving and controlling player
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetTeam().SetControllingPlayer(PlayerScript);
        PlayerScript.GetTeam().RecievingPlayer = PlayerScript;

        //TODO - maybe add more to this if statement
        //If player is close
        if (!PlayerScript.IsOppenentWithinRadius())
        {
            PlayerScript.GetSteeringController().TurnOn(Behaviour.Arrive);
        }
        else
        {
            PlayerScript.GetSteeringController().TurnOn(Behaviour.Pursue);

            Steer2D.Pursue Pur = (Steer2D.Pursue)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Pursue");
            Pur.TargetAgent = PlayerScript.Ball;
        }

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering recieve ball state");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (PlayerScript.BallInReceivingRange() || !PlayerScript.GetTeam().InControl())
        {
            PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());

            return;
        }

        Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");

        if (Arr.AtTarget)
        {
            //PlayerScript.ChangeState(CallingObject, Wait.Instance());
            //PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);        
           // PlayerScript.GetSteeringController().TurnOff(Behaviour.Pursue);
            PlayerScript.TrackBall();

            if (PlayerScript.IsClosestTeamMemberToBall())
            {
                PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());

            }
        }

    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);
        PlayerScript.GetSteeringController().TurnOff(Behaviour.Pursue);

        PlayerScript.GetTeam().RecievingPlayer = null;

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting Receive ball state");
        }
    }
}

/*
 *  ******************************************** SupportAttacker State ********************** 
 */

public class SupportAttacker : State
{
    static SupportAttacker instance;

    public static SupportAttacker Instance()
    {
        if (instance == null)
        {
            instance = new SupportAttacker();
        }
        return instance;
    }

    /**
    *   this will execute when the state is entered
    */
    public override void Enter(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetSteeringController().TurnOn(Behaviour.Arrive);

        Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");

        Arr.TargetPoint = PlayerScript.GetTeam().GetSupportSpot();

        if (PlayerScript.DebugOn)
        {
            Debug.Log("Entering Support Attacker State");
        }
    }

    /**
    *   this is the updated fucntion for the state
    */
    public override void Excute(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        if (!PlayerScript.GetTeam().InControl())
        {
            PlayerScript.ChangeState(CallingObject, ReturnToHomeRegion.Instance());
            return;
        }


        Steer2D.Arrive Arr = (Steer2D.Arrive)PlayerScript.GetSteeringController().GetBehaviourByTypeName("Steer2D.Arrive");

        if (Arr.TargetPoint != PlayerScript.GetTeam().GetSupportSpot())
        {
            PlayerScript.GetSteeringController().TurnOn(Behaviour.Arrive);
            Arr.TargetPoint = PlayerScript.GetTeam().GetSupportSpot();
            
        }

        Vector2 ShootingTarget = new Vector2();
        if (PlayerScript.GetTeam().CanShoot(CallingObject.transform.position, out ShootingTarget, PlayerScript.MaxShootingForce)) //TODO ?Need Confidence here?
        {
            PlayerScript.GetTeam().RequestPass(CallingObject);
        }

        if (Arr.AtTarget)
        {
            //PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);

            PlayerScript.TrackBall();

            if (!PlayerScript.IsThreatened())
            {
                PlayerScript.GetTeam().RequestPass(CallingObject);
            }
        }

        //If closest to ball and not in goalkeepers hands... Chase the ball
        if (PlayerScript.IsClosestTeamMemberToBall() && (!PlayerScript.GetTeam().GetPitch().GoalKeeperHasBall()))
        {
            PlayerScript.ChangeState(CallingObject, ChaseBall.Instance());
            return;
        }

    }

    /**
    *   this will execute when the state is exited
    */
    public override void Exit(GameObject CallingObject)
    {
        FieldPlayer PlayerScript = CallingObject.GetComponent<FieldPlayer>();

        PlayerScript.GetTeam().SupportingPlayer = null;

        PlayerScript.GetSteeringController().TurnOff(Behaviour.Arrive);


        if (PlayerScript.DebugOn)
        {
            Debug.Log("Exiting Support Attacker State");
        }

    }
}