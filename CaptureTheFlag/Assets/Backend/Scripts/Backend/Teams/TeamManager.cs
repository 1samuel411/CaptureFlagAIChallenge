using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{

    public static TeamManager instance;

    public int teamSize = 2;

    [SerializeField]
    private Team aTeam;
    [SerializeField]
    private Team bTeam;

    public int maxTime;
    public int curTime;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Debug.Log("Starting Match...");
        Debug.Log("Press H to Reset Round...");
        Debug.Log("Press G to Start Over...");

        StartOver();

        InvokeRepeating("ReduceTime", 1, 1);
    }

    void ReduceTime()
    {
        if(curTime > 0)
            curTime--;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Reset();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartOver();
        }

        CheckTimers();

        if (roundOver)
            return;

        CheckDeaths();

        CheckFlags();
    }

    public float roundOverTime;
    private float curRoundOverTime;

    void CheckTimers()
    {
        if (curTime <= 0 && !timeOver)
        {
            RoundOver(null);
            timeOver = true;
        }

        if (Time.time >= curRoundOverTime && roundOver)
        {
            RoundOverComplete();
        }
    }

    private bool timeOver = false;

    // checks if flag is another location
    void CheckFlags()
    {
        // Flag B in Team A's spawn, Team A Wins
        if (aTeam.spawn.InSpawn(bTeam.spawn.flag.GetLocation()))
            RoundOver(aTeam);

        // Flag A in Team B's spawn, Team B Wins
        if (bTeam.spawn.InSpawn(aTeam.spawn.flag.GetLocation()))
            RoundOver(bTeam);
    }

    // Checks if everyone is dead
    void CheckDeaths()
    {
        int deadCount = aTeam.soldiers.Count(x => x.IsDead());

        if (deadCount >= aTeam.soldiers.Count)
            RoundOver(bTeam);

        deadCount = bTeam.soldiers.Count(x => x.IsDead());

        if (deadCount >= bTeam.soldiers.Count)
            RoundOver(aTeam);
    }

    private bool roundOver;
    void RoundOver(Team team)
    {
        UIManager.instance.RoundOver(team);
        if(team != null)
            team.teamScore++;

        roundOver = true;
        curRoundOverTime = Time.time + roundOverTime;
    }

    void RoundOverComplete()
    {
        roundOver = false;
        UIManager.instance.RoundOverComplete();

        Reset();
    }

    void StartOver()
    {
        Debug.Log("Starting over...");

        aTeam.teamScore = 0;
        bTeam.teamScore = 0;

        Reset();
    }

    void Reset()
    {
        Debug.Log("Resetting round...");

        for (int i = 0; i < aTeam.soldiers.Count; i++)
        {
            GameObject.Destroy(aTeam.soldiers[i].gameObject);
        }

        aTeam.soldiers.Clear();

        for (int i = 0; i < bTeam.soldiers.Count; i++)
        {
            GameObject.Destroy(bTeam.soldiers[i].gameObject);
        }

        bTeam.soldiers.Clear();

        for (int i = 0; i < teamSize; i++)
        {
            GameObject aSoldier = GameObject.Instantiate(aTeam.soldier, aTeam.spawn.GetPosition(), Quaternion.identity);

            Soldier newSoldier = aSoldier.GetComponent<Soldier>();
            newSoldier.teamType = Team.Type.A;
            aTeam.soldiers.Add(newSoldier);

            GameObject bSoldier = GameObject.Instantiate(bTeam.soldier, bTeam.spawn.GetPosition(), Quaternion.identity);
            
            newSoldier = bSoldier.GetComponent<Soldier>();
            newSoldier.teamType = Team.Type.B;
            bTeam.soldiers.Add(newSoldier);
        }

        UIManager.instance.CreateHealthbars();

        aTeam.teamName = aTeam.soldiers[0].GetComponent<Soldier>().soldierName;
        bTeam.teamName = bTeam.soldiers[0].GetComponent<Soldier>().soldierName;

        aTeam.spawn.flag.ResetFlag();
        bTeam.spawn.flag.ResetFlag();

        curTime = maxTime;
        timeOver = false;
    }

    public Team GetTeamA()
    {
        return aTeam;
    }

    public Team GetTeamB()
    {
        return bTeam;
    }

    public void SendFlagMessage(Flag flag)
    {
        // Send msg to soldiers
        for (int i = 0; i < aTeam.soldiers.Count; i++)
        {
            if (aTeam.soldiers[i].flagStatusChangedCallback != null)
                aTeam.soldiers[i].flagStatusChangedCallback.Invoke(flag);
        }
        for (int i = 0; i < bTeam.soldiers.Count; i++)
        {
            if (bTeam.soldiers[i].flagStatusChangedCallback != null)
                bTeam.soldiers[i].flagStatusChangedCallback.Invoke(flag);
        }
    }

    public void SendDied(Soldier soldier)
    {
        // Send msg to soldiers
        for (int i = 0; i < aTeam.soldiers.Count; i++)
        {
            if (aTeam.soldiers[i].soldierDiedCallback != null)
                aTeam.soldiers[i].soldierDiedCallback.Invoke(soldier);
        }
        for (int i = 0; i < bTeam.soldiers.Count; i++)
        {
            if (bTeam.soldiers[i].soldierDiedCallback != null)
                bTeam.soldiers[i].soldierDiedCallback.Invoke(soldier);
        }
    }
}

[System.Serializable]
public class Team
{
    public enum Type
    {
        A,
        B
    };

    public string teamName;

    public int teamScore;

    public Spawn spawn;

    public GameObject soldier;

    public Type teamType;

    public List<Soldier> soldiers = new List<Soldier>();
}