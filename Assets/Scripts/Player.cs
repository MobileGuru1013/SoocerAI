using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerRoles { Attacker, Defender, GoalKeeper} 

public class Player : MonoBehaviour
{

    public PlayerRoles Role = PlayerRoles.Attacker;

    //The Base Position for the player
    [HideInInspector]
    public Vector2 HomePosition;
    [HideInInspector]
    public Vector2 AttackingPosition;
    [HideInInspector]
    public Vector2 DefendingPosition;

    public GameObject AttackPoint;

    //Player Constants
    public float PassingForce = 5.0f;
    public float MaxShootingForce = 5.0f;
    public float DribbleForce = 1.0f;
    public float TurningForce = 0.5f;
    public float MinPassDistance = 1.0f;

    public float KickDelay = 0.5f;
    private float NextKick = 0f;
    public float KickingDistance = 0.2f;

    public float ShootingConfidence = 0.8f;
    public float ShootingAccuracy = 0.8f;
    public float ShootingRange = 2.0f;

    //will be one or minus one // randomised on start
    public int PreferedTurnDir = 1;

    State CurrentState;
    State PreviousState;
    State GlobalState;

    Team PlayersTeam = null;

    public GameObject Ball;
    public GameObject OpponentsGoal;
    SteeringController SteerController;

    bool ClosestPlayer = false;

    public float TextOffSet = 0.5f;
    public GameObject StateTextObject;
    public bool DebugOn = false;

    public Team GetTeam()
    {
        return PlayersTeam;
    }
    public void SetTeam(Team team)
    {
        PlayersTeam = team;
    }
    public SteeringController GetSteeringController()
    {
        return SteerController;
    }

    // Use this for initialization
    void Start()
    {
        if (Random.Range(0, 1) == 0) //randomise the turn Direction
        {
            PreferedTurnDir = -1;
        }

        SteerController = GetComponent<SteeringController>();

        HomePosition = transform.position;
        DefendingPosition = transform.position;
        AttackingPosition = AttackPoint.transform.position;

        CurrentState = ReturnToHomeRegion.Instance();
        PreviousState = ReturnToHomeRegion.Instance();
        GlobalState = GlobalPlayerState.Instance();

        CurrentState.Enter(gameObject);

        UpdateStateText();

        NextKick = Time.time + KickDelay;
    }
    // Update is called once per frame
    void Update ()
    {
        CurrentState.Excute(gameObject);

        StateTextObject.transform.position = transform.position + new Vector3(0, TextOffSet,0);

    }

    public void FindSupport()
    {

        if (PlayersTeam.SupportingPlayer == null)
        {
            Player Guy = PlayersTeam.DetermineBestSupportingAttacker();

            PlayersTeam.SupportingPlayer = Guy;

            Dispatcher.Instance().DispatchMessage(0, gameObject, Guy.gameObject, PlayerMessages.SupportAttacker);
        }
    }

    public bool InShootingRange()
    {

        float Dist = Vector3.Distance(OpponentsGoal.transform.position, gameObject.transform.position);

        if (Dist < ShootingRange)
        {
            return true;
        }

        return false;
    }


    public void SetDefaultHomeRegion()
    {
        HomePosition = DefendingPosition;
    }


    public bool IsInState(State CheckingState)
    {
        if (CurrentState == CheckingState)
        {
            return true;
        }
        return false;
    }

    public bool BallInKickingRange()
    {
        float Dist = Vector3.Distance(Ball.transform.position, gameObject.transform.position);

        if (KickingDistance > Dist)
        {
            return true;
        }

        return false;
    }

    public bool IsClosestTeamMemberToBall()
    {
       return ClosestPlayer;
    }

    public void ChangeState(GameObject CallingObject, State NewState)
    {
        PreviousState = CurrentState;

        CurrentState.Exit(gameObject);

        CurrentState = NewState;

        CurrentState.Enter(gameObject);

        UpdateStateText();
    }

    public bool HandleMessage(Message Telegram)
    {
        if (GlobalState != null)
        {
            if (GlobalState.OnMessage(gameObject, Telegram))
            {
                return true;
            }
        }

        return false;
    }

    public void TrackBall()
    {
        //transform.rotation = Quaternion.LookRotation(Vector3.right, Ball.transform.position - transform.position);

        Vector2 Dir = Ball.transform.position - transform.position;

        float Angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
    }

    public bool AheadOfAttacker()
    {
        float GoalXPos = OpponentsGoal.transform.position.x;
        float AttackerX = PlayersTeam.ControllingPlayer.transform.position.x;
        float PlayersX = gameObject.transform.position.x;

        float PlayersDistToGoal = Vector2.Distance(OpponentsGoal.transform.position, gameObject.transform.position);
        float AttackersDistToGoal = Vector2.Distance(OpponentsGoal.transform.position, PlayersTeam.ControllingPlayer.transform.position);


        if (PlayersDistToGoal > AttackersDistToGoal)
        {
            return true;
        }

        return false;
    }

    public bool IsReadyForNextKick()
    {
        if (NextKick < Time.time)
        {
            NextKick = Time.time + KickDelay;
            return true;
        }
        return false;
    }

    //public Vector2 GetShotTarget()
    //{
    //    Vector2 Shot = OpponentsGoal.transform.position;

    //    Shot += Random.insideUnitCircle * (1.0f - ShootingAccuracy);

    //    return Shot;
    //}

    public Vector2 AddNoiseToTarget(Vector2 Target)
    {
        Vector2 Shot = Target;

        Shot += Random.insideUnitCircle * (1.0f - ShootingAccuracy);

        return Shot;
    }

    public bool IsOppenentWithinRadius()
    {
        foreach (Player Guy in PlayersTeam.Opponents.Players)
        {

            float Dist = 1f;
            if (Dist > Vector2.Distance(Guy.gameObject.transform.position, gameObject.transform.position))
            {
                return true;
            }

        }
        return false;
    }

    public bool BallInReceivingRange()
    {
        float Dist = Vector3.Distance(Ball.transform.position, gameObject.transform.position);

        if (KickingDistance > Dist)
        {
            return true;
        }

        return false;
    }

    public bool IsThreatened()
    {
        if (IsOppenentWithinRadius())
        {
            return true;

        }
        return false;
    }

    public void SetClosestTeamMemberToBall(bool IsClosest)
    {
        ClosestPlayer = IsClosest;
    }

    public PlayerRoles GetRole()
    {
        return Role;
    }

    public float GetMaxSpeed()
    {
        return gameObject.GetComponent<Steer2D.SteeringAgent>().MaxVelocity;
    }

    public bool InHomePosition()
    {
        float Dist = Vector3.Distance(HomePosition, gameObject.transform.position);

        if (0.5f > Dist)
        {
            return true;
        }

        return false;
    }

    void UpdateStateText()
    {
        StateTextObject.GetComponent<TextMesh>().text = CurrentState.GetType().ToString();
    }
}
