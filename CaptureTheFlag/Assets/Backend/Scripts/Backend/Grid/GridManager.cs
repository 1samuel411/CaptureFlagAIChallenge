using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public static GridManager instance;

    public Transform groundTransform;

    public Vector2 gridSize;

    public Dictionary<Vector2, Cell> grid = new Dictionary<Vector2, Cell>();

    public LayerMask groundMask;
    public LayerMask obstacleMask;

    void Awake()
    {
        instance = this;
        GenerateGrid();
    }

    /// <summary>
    /// Generates the grid
    /// </summary>
    public void GenerateGrid()
    {
        // Create the Mesh


        // Generate the Cells
        gridSize = new Vector2(groundTransform.localScale.x, groundTransform.localScale.z);

        for (int x = (int)(-groundTransform.localScale.x / 2); x < (int)(groundTransform.localScale.x / 2) + 1; x++)
        {
            for (int y = (int)(-groundTransform.localScale.z / 2); y < (int)(groundTransform.localScale.z / 2) + 1; y++)
            {
                grid.Add(new Vector2(x, y), new Cell(x, y, GetHeight(x, y)));
            }
        }
    }

    /// <summary>
    /// Returns the closest cell based on the position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="availableOnly"></param>
    /// <returns></returns>
    public Cell FindClosestCell(Vector2 position, bool availableOnly = false)
    {

        position.x = (int)Mathf.Clamp(position.x, -gridSize.x / 2, gridSize.x / 2);
        position.y = (int)Mathf.Clamp(position.y, -gridSize.y / 2, gridSize.y / 2);

        Cell closestCell = grid[position];

        if (availableOnly)
        {
            while (closestCell.isAvailable == false)
            {
                position.x-= 1;
                position.x = (int)Mathf.Clamp(position.x, -gridSize.x / 2, gridSize.x / 2);
                position.y = (int)Mathf.Clamp(position.y, -gridSize.y / 2, gridSize.y / 2);

                closestCell = grid[position];
            }
        }

        if (closestCell == null)
        {
            closestCell = grid[new Vector2(0, 0)];
        }

        return closestCell;
    }

    /// <summary>
    /// Gets the closest Cell
    /// </summary>
    /// <param name="position"></param>
    /// <param name="availableOnly"></param>
    /// <returns></returns>
    public Cell FindClosestCell(Vector3 position, bool availableOnly = false)
    {
        return FindClosestCell(new Vector2(position.x, position.z), availableOnly);
    }

    private void DrawCube(Vector3 position)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
    }

    /// <summary>
    /// Checks if a cell is available
    /// </summary>
    /// <param name="position"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    public bool IsAvailable(Vector2 position, Vector2 region)
    {
        List<Cell> newCells = FindCellsInRegion(position, region);
        for (int i = 0; i < newCells.Count; i++)
        {
            if (!grid[new Vector2(newCells[i].x, newCells[i].z)].isAvailable)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns cells in a region excpet for the initial cell
    /// </summary>
    /// <param name="position"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    public List<Cell> FindCellsInRegion(Vector2 position, Vector2 region)
    {
        Cell cellInPosition = FindClosestCell(position);
        List<Cell> newCells = new List<Cell>();
        region -= Vector2.one;
        for (int i = (int)-region.x / 2; i <= region.x / 2; i++)
        {
            for (int x = (int)-region.y / 2; x <= region.y / 2; x++)
            {
                newCells.Add(FindClosestCell(new Vector2(position.x + i, position.y + x)));
            }
        }
        return newCells;
    }

    /// <summary>
    /// Gets the height of a cell
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetHeight(int x, int y)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x, 150, y), Vector3.down, out hit, 10000, groundMask))
        {
            return (hit.point.y);
        }
        return -1;
    }

    /// <summary>
    /// Checks again to see if a cell is available
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool CheckAvailability(Cell cell)
    {
        return !(Physics.Raycast(new Vector3(cell.x, 150, cell.z), Vector3.down, 10000, obstacleMask));
    }

    /// <summary>
    /// Returns cells in a region excpet for the initial cell
    /// </summary>
    /// <param name="position"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    public List<Cell> FindCellsAroundRegion(Vector2 position, Vector2 region)
    {
        List<Cell> cellsInRegion = FindCellsInRegion(position, region + (Vector2.one * 2));
        return cellsInRegion.Except(FindCellsInRegion(position, region)).ToList();
    }

    /// <summary>
    /// Returns 8 cells around the cell provided
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="availableOnly">Should only available cells be returned?</param>
    /// <returns></returns>
    public List<Cell> FindCellsAround(Cell cell, bool availableOnly = true)
    {
        List<Cell> listToReturn = new List<Cell>();
        for (int i = cell.x - 1; i <= cell.x + 1; i++)
        {
            for (int y = cell.z - 1; y <= cell.z + 1; y++)
            {
                if (i == cell.x && y == cell.z)
                    continue;

                Cell currentCell = grid[new Vector2(i, y)];

                if (availableOnly && !currentCell.isAvailable)
                    continue;

                listToReturn.Add(currentCell);
            }
        }
        return listToReturn;
    }

    /// <summary>
    /// Returns 4 adjacent cells around the cell provided
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="availableOnly">Should only available cells be returned?</param>
    /// <returns></returns>
    public List<Cell> FindCellsAdjacent(Cell cell, bool availableOnly = true)
    {
        List<Cell> listToReturn = new List<Cell>();
        for (int i = cell.x - 1; i <= cell.x + 1; i++)
        {
            if (i == cell.x)
                continue;

            Cell currentCell = grid[new Vector2(i, cell.z)];

            if (availableOnly && !currentCell.isAvailable)
                continue;

            listToReturn.Add(currentCell);
        }
        for (int y = cell.z - 1; y <= cell.z + 1; y++)
        {
            if (y == cell.z)
                continue;

            Cell currentCell = grid[new Vector2(cell.x, y)];

            if (availableOnly && !currentCell.isAvailable)
                continue;

            listToReturn.Add(currentCell);
        }
        return listToReturn;
    }

    /// <summary>
    /// Gets the distance between two cells
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float GetDistance(Cell a, Cell b)
    {
        return (new Vector2(a.x, a.z) - new Vector2(b.x, b.z)).magnitude;
    }

    /// <summary>
    /// Gets the distance between a position and Cell
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float GetDistance(Vector3 a, Cell b)
    {
        return (new Vector2(a.x, a.z) - new Vector2(b.x, b.z)).magnitude;
    }

    public static List<Cell> SetParent(List<Cell> list, Cell parent)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].parentCell = parent;
        }

        return list;
    }

    /// <summary>
    /// Sets the costs for a cell based on inputs
    /// </summary>
    /// <param name="openList"></param>
    /// <param name="targetCell"></param>
    /// <param name="startCell">s</param>
    /// <returns></returns>
    public static List<Cell> SetCosts(List<Cell> openList, Cell targetCell, Cell startCell)
    {
        for (int i = 0; i < openList.Count; i++)
        {
            openList[i].gCost = GridManager.GetDistance(openList[i], startCell);
            openList[i].hCost = GridManager.GetDistance(openList[i], targetCell);
        }

        return openList;
    }
    
    /// <summary>
    /// Returns the lowest cost in a list
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Cell FindLowestCostCell(List<Cell> list)
    {
        float minCost = list.Min(x => x.fCost);
        return list.FirstOrDefault(x => x.fCost.Equals(minCost));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < grid.Count; i++)
        {
            Cell curCell = grid.ElementAt(i).Value;

            if (!curCell.isAvailable)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector3(curCell.x, curCell.yPos + 1, curCell.z), Vector3.one);
            }
        }
    }
}

[System.Serializable]
public class Cell
{
    public int x;
    public int z;
    public float yPos;

	// Ignore this
    public bool isVisible;
    public bool isExplored;
	// Ignore ^
	
    public bool isAvailable = true;

    public Cell parentCell;

    public float gCost;
    public float hCost;
    public float fCost { get { return gCost + hCost; } }

    public Cell(int x, int z, float yPos)
    {
        this.x = x;
        this.z = z;
        
        this.yPos = yPos;

        isAvailable = GridManager.instance.CheckAvailability(this);
    }

    public Cell(int x, int z, float yPos, Cell parentCell)
    {
        this.x = x;
        this.z = z;
        this.yPos = yPos;

        this.parentCell = parentCell;

        isAvailable = true;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(x, yPos, z);
    }
}