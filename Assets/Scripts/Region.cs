using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TeamEnums { RedTeam, BlueTeam};

public class SupportPosition 
{
    public Vector2 Position;
    public float Weighting;
    public GameObject obj;
}

public class Region : MonoBehaviour {

    /**
    *   Support Position
    *
    *   This is posiiton on the pitch that players can use to calculate a score on
    *   whether it is a worthwhile position moving to, to supoprt the controlling
    *   player
    */
  

    /*
    * List Of support Positions in the Level     
    */
    public List<SupportPosition> SupportPositions = new List<SupportPosition>();

    /**
    *   Divisons 
    *
    *   Number of Divisions/Supporting Positions there will be on the pitch
    *   Cant Be less than zero
    */
    [SerializeField]
    private int WidthDivisions = 1;
    [SerializeField]
    private int HeightDivisions = 1;

    /**
    *   Default Weigthing
    *
    *   This is the Base score for each positons
    *   However It acts as a visual represntaiton as well for Debuging
    *   (The Radius of the debug circle) 
    */
    [SerializeField]
    private float DefaultWeighting = 0.1f;
    [SerializeField]
    private float SafePassScore = 0.1f;
    [SerializeField]
    private float ShootingChanceScore = 0.2f;


    private SpriteRenderer Rend;

    /**
    *   Bottom Left And Top Right Vectors of the pitch
    */
    private Vector2 BottomLeftVec;
    private Vector2 TopRightVec;

    /**
    *   Players/Agents to be added to each Team
    */
    public List<GameObject> RedTeamPlayers = new List<GameObject>();
    public List<GameObject> BlueTeamPlayers = new List<GameObject>();

    public Team RedTeam;  //AddComponent<Team>();
    public Team BlueTeam; //new Team();

    /**
    *   Bool monitoring whether the Game is play or not
    */
    public bool GameInPlay = false;

    public GameObject RedTeamStateUI;
    public GameObject BlueTeamStateUI;

    public GameObject BlueTeamScoreText;
    public GameObject RedTeamScoreText;

    private int RedScore = 0;
    private int BlueScore = 0;



    void Start ()
    {
        //RedTeam = gameObject.AddComponent<Team>();
       // BlueTeam = gameObject.AddComponent<Team>();


        //Get up the Dimensions of the Pitch
        Rend = GetComponent<SpriteRenderer>();
        BottomLeftVec = Rend.bounds.min;
        TopRightVec = Rend.bounds.max;

        //Make Sure that we won't be dividing by Zero
        if ((WidthDivisions <= 0) || (HeightDivisions <= 0))
        {
            Debug.LogError("Divisions in the Region cannot be less than or equal to 0");
            return;
        }

        //Set up the Support positions Regions
        SetUpSupportRegions();

        //Set up the teams
        RedTeam.Init(this, RedTeamPlayers);
        BlueTeam.Init(this, BlueTeamPlayers);


    }

    // Update is called once per frame
    void Update ()
    {
        //counter++;
        //RedTeam.DetermineBestSupportingPosition();

        if (GameInPlay)
        {
            if (RedTeam.Ball.transform.position.x < -2)
            {
                RedTeam.SetHomeRegions(HomeRegions.Defending);
                BlueTeam.SetHomeRegions(HomeRegions.Attacking);
                RedTeam.UpdateTargetsOfWaitingPlayers();
                BlueTeam.UpdateTargetsOfWaitingPlayers();

            }

            if (RedTeam.Ball.transform.position.x > 2)
            {
                BlueTeam.SetHomeRegions(HomeRegions.Defending);
                RedTeam.SetHomeRegions(HomeRegions.Attacking);
                RedTeam.UpdateTargetsOfWaitingPlayers();
                BlueTeam.UpdateTargetsOfWaitingPlayers();

            }
        }

        if (RedTeamStateUI && BlueTeamStateUI)
        {

            RedTeamStateUI.GetComponent<TextMesh>().text = RedTeam.GetState().GetType().ToString();
            BlueTeamStateUI.GetComponent<TextMesh>().text = BlueTeam.GetState().GetType().ToString();
        }

    }



    void SetUpSupportRegions()
    {
        //Calculate the offsets for each axis
        float XOffSet = (Mathf.Abs((TopRightVec.x - BottomLeftVec.x))/ (WidthDivisions+1));
        float YOffSet = (Mathf.Abs((TopRightVec.y - BottomLeftVec.y)) / (HeightDivisions+1));

      //starting point is bootom left of the pitch
        Vector2 CurrentPos = BottomLeftVec;

        //Loop for each Division on Each Axis 
        for (int x = 1; x <= WidthDivisions; x++)
        {    
            for (int y = 1; y <= HeightDivisions; y++)
            {
                //Add the offsets to the current position
                CurrentPos = new Vector2(BottomLeftVec.x+(XOffSet * x), BottomLeftVec.y + (YOffSet *y));

                //Create a Support position at that location
                SupportPosition NewPosition = new SupportPosition();
                NewPosition.Position = CurrentPos;
                NewPosition.Weighting = DefaultWeighting;
                NewPosition.obj = new GameObject();
                NewPosition.obj.transform.position = CurrentPos;


                //Add to the support posiiton list
                SupportPositions.Add(NewPosition);
            }             
        }
    }

    /**
     *  Getters and setters  
     */
    public float GetDefaultWeighting()
    {
        return DefaultWeighting;
    }
    public float GetSafePassScore()
    {
        return SafePassScore;
    }
    public float GetShootingChanceScore()
    {
        return ShootingChanceScore;
    }

    void OnDrawGizmos()
    {
        //Draw the Support Possitions based on their current Weightings
        foreach (SupportPosition SP in SupportPositions)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(SP.Position, SP.Weighting);
        }    
    }

    public void SetGameInPlay(bool InPlayOrNot)
    {
        GameInPlay = InPlayOrNot;
    }

    public bool GetGameInPlay()
    {
        return GameInPlay;
    }


    public bool GoalKeeperHasBall()
    {
        //TODO
        return false;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void GoalScored(TeamEnums WhoScored)
    {
        switch (WhoScored)
        {
            case TeamEnums.BlueTeam:
                BlueScore++;
                BlueTeamScoreText.GetComponent<TextMesh>().text = BlueScore.ToString();
               
                break;
            case TeamEnums.RedTeam:
                RedScore++;
                RedTeamScoreText.GetComponent<TextMesh>().text = RedScore.ToString();

                break;
        }
        RedTeam.ChangeState(RedTeam.gameObject, PrepareForKickoff.Instance());
        BlueTeam.ChangeState(BlueTeam.gameObject, PrepareForKickoff.Instance());

        RedTeam.Ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        RedTeam.Ball.transform.position = new Vector2(0,0);
        GameInPlay = false;
        CheckForWinners();
    }

    void CheckForWinners()
    {
        if (BlueScore >= 5)
        {
            SceneManager.LoadScene(1);
        }

        if (RedScore >= 5)
        {
            SceneManager.LoadScene(2);
        }
    }
}


