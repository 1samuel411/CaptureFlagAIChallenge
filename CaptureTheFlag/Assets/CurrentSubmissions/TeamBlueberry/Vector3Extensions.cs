using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{
    public static class Vector3Extensions
    {
        public static Cell ToCell(this Vector3 position)
        {
            return GridManagerExt.Instance.FindClosestCell(position);
        }

        public static Vector3 GetPositionWithSmallestDiffAngle(List<Vector3> enemiesPosition, SoldierTeamBlueberry soldier)
        {
            Vector3 myForward = soldier.transform.forward;
            Vector3 myPosition = soldier.soldier.GetLocation();
            float smallestAngle = float.MaxValue;
            Vector3 position = Vector3.down;

            for (int idx = 0; idx < enemiesPosition.Count; idx++)
            {
                Vector3 dir = enemiesPosition[idx] - myPosition;
                float angle = Vector3.Angle(dir, myForward);
                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    position = enemiesPosition[idx];
                }
            }
            return position;
        }

        public static Vector3 GetPositionWithSmallestDiffAngle(List<Cell> cellPositions, SoldierTeamBlueberry soldier)
        {
            List<Vector3> positions = cellPositions.ConvertAll(c => new Vector3(c.GetPosition().x, c.GetPosition().y, c.GetPosition().z));
            return Vector3Extensions.GetPositionWithSmallestDiffAngle(positions, soldier);
        }

        public static Vector3 GetPositionWithSmallestDiffAngle(List<IAgent> enemies, SoldierTeamBlueberry soldier)
        {
            List<Vector3> positions = enemies.ConvertAll(e => new Vector3(e.GetLocation().x, e.GetLocation().y, e.GetLocation().z));
            return Vector3Extensions.GetPositionWithSmallestDiffAngle(positions, soldier);
        }
    }
}