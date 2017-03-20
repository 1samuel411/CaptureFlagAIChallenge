using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            return curCell;
        }
    }

    public List<Cell> pathGenerated = new List<Cell>();
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

        if (pathGenerated.Count <= 0 || pathGenerated[0] == null || !pathGenerated[0].isAvailable)
            return;

        if (GridManager.GetDistance(transform.position, pathGenerated[0]) <= distanceNeeded)
        {
            pathGenerated.RemoveAt(0);
            return;
        }
    }

    // Simple A* Algorithim
    public void GeneratePath(Cell targetCel)
    {
        pathGenerated.Clear();

        List<Cell> openList = GridManager.instance.FindCellsAdjacent(currentCell);
        GridManager.SetParent(openList, currentCell);
        List<Cell> closedList = new List<Cell>();
        Cell finalCell = new Cell(0, 0, 0);

        // Continue till the open list is not empty
        while (openList.Count > 0)
        {
            // get costs
            openList = GridManager.SetCosts(openList, targetCell, currentCell);
            
            // find lowest costing cell
            Cell cheapestCell = GridManager.FindLowestCostCell(openList);

            if (cheapestCell.x == targetCell.x && cheapestCell.z == targetCell.z)
            {
                pathGenerated = FollowTheParent(cheapestCell, currentCell);
                pathGenerated.Add(targetCell);
                return;
            }
            else
            {
                // move cheapest node to the closed list
                openList.Remove(cheapestCell);
                closedList.Add(cheapestCell);

                // Examine each node around the cheapest node
                List<Cell> cheapestNeighbors = GridManager.instance.FindCellsAdjacent(cheapestCell);

                for (int i = 0; i < cheapestNeighbors.Count; i++)
                {
                    if (!openList.Contains(cheapestNeighbors[i]) && !closedList.Contains(cheapestNeighbors[i]) && cheapestNeighbors[i].isAvailable)
                    {
                        cheapestNeighbors[i].parentCell = cheapestCell;
                        openList.Add(cheapestNeighbors[i]);    
                    }
                }
            }
        }
    }

    private List<Cell> FollowTheParent(Cell cell, Cell beginningCell)
    {
        List<Cell> returnList = new List<Cell>();
        while (cell != beginningCell)
        {
            cell = cell.parentCell;
            returnList.Add(cell);
        }
        returnList.Reverse();
        return returnList;
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
