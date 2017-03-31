using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{
    public enum TeamMessageTypes
    {
        EnemyLocated,
        GoToPosition,
        SoldierDied,
        UnderFire
    }

    public struct MessageContentEnemyLocated
    {
        public MessageContentEnemyLocated(List<Vector3> positions)
        {
            this.positions = positions;
        }

        public List<Vector3> positions;
    }

    public struct MessageContentUnderFire
    {
        public MessageContentUnderFire(Cell position)
        {
            this.position = position;
        }

        public Cell position;
    }

    public struct MessageContentGoToPosition
    {
        public MessageContentGoToPosition(Cell position)
        {
            this.position = position;
        }

        public Cell position;
    }

    public struct TeamMessage 
    {
        public TeamMessage(TeamMessageTypes messageType, object content)
        {
            this.messageType = messageType;
            this.content = content;
        }

        public TeamMessageTypes messageType; 
        public object content;
    }
}