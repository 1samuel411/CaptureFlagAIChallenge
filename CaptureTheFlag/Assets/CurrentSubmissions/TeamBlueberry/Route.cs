using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamBlueberry
{
    public class Route
    {
        List<Cell> cells = new List<Cell>();

        public void Clear()
        {
            cells.Clear();
        }

        public void AddCell(Cell cell)
        {
            cells.Add(cell);
        }

        public bool IsEmpty()
        {
            return cells.Count == 0;
        }

        public Cell GetNextWaypoint()
        {
            if (cells.Count > 0)
                return cells[0];

            return null;
        }

        public void RemoveNextWaypoint()
        {
            if (cells.Count > 0)
                cells.RemoveAt(0);
        }

    }
}