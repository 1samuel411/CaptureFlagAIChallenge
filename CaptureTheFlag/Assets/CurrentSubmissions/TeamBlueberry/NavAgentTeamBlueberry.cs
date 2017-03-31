using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamBlueberry;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentTeamBlueberry : MonoBehaviour
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

    public Cell currentCell
    {
        get
        {
            return GridManagerExt.Instance.FindClosestCell(new Vector2(transform.position.x, transform.position.z), true);
        }
    }

    public List<Vector3> pathGenerated = new List<Vector3>();
    public float distanceNeeded = 0.1f;

    public float offset;

    public bool moveToTarget = true;

    void Start()
    {
        //targetCell = currentCell;
    }

    void Update()
    {
        CheckTarget();
    }

    void CheckTarget()
    {
        if (!moveToTarget)
            return;

        if (CellExtentions.IsCellEqual(transform.position.ToCell(), targetCell))
            pathGenerated.Clear();

        if (pathGenerated.Count <= 0)
            return;

        if ((transform.position - pathGenerated[0]).magnitude <= distanceNeeded)
        {
            pathGenerated.RemoveAt(0);
            return;
        }
    }

    public void GeneratePath(Cell targetCell)
    {
        if (targetCell == null)
            return;

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetCell.GetPosition(), NavMesh.AllAreas, path);
        pathGenerated = path.corners.ToList();
        if (pathGenerated.Count > 0)
            pathGenerated.RemoveAt(0);
    }

    void OnDrawGizmosSelected()
    {
        Color oldColor = Gizmos.color;
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

        Gizmos.color = oldColor;
    }
}
