using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Vector2 spawnBox;

    public Flag flag;

    void OnDrawGizmos()
    {
        if(flag.teamFlag == Team.Type.A)
            Gizmos.color = Color.red;
        else if(flag.teamFlag == Team.Type.B)
            Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(transform.position, new Vector3(spawnBox.x, 0.1f, spawnBox.y));
    }

    public Vector3 GetPosition()
    {
        Vector3 position = transform.position;
        position.x += Random.Range(-spawnBox.x / 2, spawnBox.x / 2);
        position.z += Random.Range(-spawnBox.y / 2, spawnBox.y / 2);
        return position;
    }

    public bool InSpawn(Vector3 position)
    {
        bool inSpawn = Mathf.Abs(position.x - transform.position.x) <= spawnBox.x/2 &&
                       Mathf.Abs(position.z - transform.position.z) <= spawnBox.y/2;
        return inSpawn;
    }
}
