using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{
    public class CellComparerDistanceToMe : IComparer<Cell>
    {
        public Cell MyCell { get; set; }

        public int Compare(Cell x, Cell y)
        {
            Vector2 toX = CellExtentions.Direction(x, MyCell);
            Vector2 toY = CellExtentions.Direction(y, MyCell);

            return toX.sqrMagnitude.CompareTo(toY.sqrMagnitude);
        }
    }

    public static class CellExtentions
    {
        public static bool IsCellEqual(Cell rhs, Cell lhs)
        {
            if (rhs == null || lhs == null)
                return false;
            return (rhs.x == lhs.x) && (rhs.z == lhs.z);
        }

        public static Vector2 Direction(Cell destination, Cell start)
        {
            return new Vector2(destination.x - start.x, destination.z - start.z);
        }

        public static bool IsCellAngleSmaller(Cell destination, float angle, SoldierTeamBlueberry soldier)
        {
            Vector3 myForward = soldier.transform.forward;
            Vector3 myPosition = soldier.soldier.GetLocation();

            Vector3 dir = destination.GetPosition() - myPosition;
            return Vector3.Angle(dir, myForward) < angle;
        }

    }
}