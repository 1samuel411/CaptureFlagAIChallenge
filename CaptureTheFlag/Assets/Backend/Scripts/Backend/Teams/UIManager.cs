using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Text roundOverText;
    public GameObject roundOverGameObject;

    public Text teamAText;
    public Text teamBText;

    public Transform healthLayoutHolder;
    public GameObject healthLayout;

    public Text timeSpeedText;

    public float curTime = 1;
    public void IncreaseTime()
    {
        curTime += 0.5f;
        curTime = Mathf.Clamp(curTime, 0, 20);
    }

    public void DecreaseTime()
    {
        curTime -= 0.5f;
        curTime = Mathf.Clamp(curTime, 0, 20);
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        Time.timeScale = curTime;
        timeSpeedText.text = curTime + "x";

        Team teamA = TeamManager.instance.GetTeamA();
        Team teamB = TeamManager.instance.GetTeamB();
        teamAText.text = teamA.teamName + "\n" + "Score: " + teamA.teamScore;

        teamBText.text = teamB.teamName + "\n" + "Score: " + teamB.teamScore;
    }

    public void RoundOver(Team winner)
    {
        roundOverGameObject.SetActive(true);
        roundOverText.text = "<color=" + ((winner.teamType == Team.Type.A) ? "red" : "blue") + ">" + winner.teamName + " has won!</color>";
    }

    public void RoundOverComplete()
    {
        roundOverGameObject.SetActive(false);
    }

    public void CreateHealthbars()
    {
        // Clear healthbars
        for (int i = 0; i < healthLayoutHolder.childCount; i++)
        {
            GameObject.Destroy(healthLayoutHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < TeamManager.instance.GetTeamA().soldiers.Count; i++)
        {
            CreateHealthbar(TeamManager.instance.GetTeamA().soldiers[i]);
        }

        for (int i = 0; i < TeamManager.instance.GetTeamB().soldiers.Count; i++)
        {
            CreateHealthbar(TeamManager.instance.GetTeamB().soldiers[i]);
        }
    }

    void CreateHealthbar(IAgent agent)
    {
        GameObject newHealthbar = GameObject.Instantiate(healthLayout, healthLayoutHolder);
        newHealthbar.transform.SetParent(healthLayoutHolder);
        newHealthbar.transform.position = Vector3.zero;

        newHealthbar.GetComponent<HealthLayout>().SetUp(agent);
    }
}
