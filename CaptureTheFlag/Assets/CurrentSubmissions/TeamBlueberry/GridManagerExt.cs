using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{

    public class GridManagerExt
    {
        private static GridManagerExt instance;
        public static GridManagerExt Instance
        {
            get
            {
                if (instance == null)
                    instance = new GridManagerExt();

                return instance;
            }

            private set { instance = value; }
        }

        private GridManagerExt() { }

        public Cell FindClosestCell(Vector3 position, bool availableOnly = false)
        {
            Vector2 posVec2 = new Vector2(position.x, position.z);
            return FindClosestCell(posVec2, availableOnly);
        }

        public Cell FindClosestCell(Vector2 position, bool availableOnly = false)
        {
            position.x = Mathf.RoundToInt(position.x);
            position.y = Mathf.RoundToInt(position.y);

            return GridManager.instance.FindClosestCell(position, availableOnly);
        }
    }
}
