using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentExample : MonoBehaviour
{

    private Cell _targetCell;
    public Cell targetCell
    {
        get { return _targetCell; }
        set
        {
            _targetCell = value;
            GeneratePath(_targetCell);
        }
    }

    private Cell lastCell;
    public Cell currentCell
    {
        get
        {
            Cell curCell = GridManager.instance.FindClosestCell(new Vector2(transform.position.x, transform.position.z), true);
            if (lastCell != curCell && targetCell != null)
            {
                lastCell = curCell;
                GeneratePath(targetCell);
            }
            return curCell;
        }
    }

    public List<Vector3> pathGenerated = new List<Vector3>();
    public float distanceNeeded = 0.01f;

    public float offset;

    public bool moveToTarget = true;

    void Start()
    {
        targetCell = currentCell;
    }

    void Update()
    {
        CheckTarget();
    }

    void CheckTarget()
    {
        if (!moveToTarget)
            return;

        if (pathGenerated.Count <= 0 || pathGenerated[0] == null)
            return;

        if ((transform.position - pathGenerated[0]).magnitude <= distanceNeeded)
        {
            pathGenerated.RemoveAt(0);
            return;
        }
    }

    private NavMeshPath path;
    public void GeneratePath(Cell targetCell)
    {
        if (targetCell == null)
            return;

        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetCell.GetPosition(), NavMesh.AllAreas, path);
        pathGenerated = path.corners.ToList();
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < pathGenerated.Count; i++)
        {
            if(i == 0)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector3(pathGenerated[i].x, 2, pathGenerated[i].z), Vector3.one);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(currentCell.GetPosition() + Vector3.up * 2, Vector3.one);
    }
}
