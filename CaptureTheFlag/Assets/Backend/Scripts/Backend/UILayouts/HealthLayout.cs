using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthLayout : MonoBehaviour
{
    public IAgent agent;

    public Image healthImage;

    public float yOffset = 3;

    public void SetUp(IAgent agent)
    {
        this.agent = agent;
    }

    void Update()
    {
        if(agent.IsDead())
            Destroy(gameObject);

        healthImage.fillAmount = (float)agent.GetHealth() / (float)agent.GetMaxHealth();

        transform.position = Camera.main.WorldToScreenPoint(agent.GetLocation() + (Vector3.up * yOffset));
    }

}
