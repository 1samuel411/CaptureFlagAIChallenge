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

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
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
}
