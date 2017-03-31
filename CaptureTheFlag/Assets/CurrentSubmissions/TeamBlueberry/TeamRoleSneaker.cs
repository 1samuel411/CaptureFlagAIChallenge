using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{
    public class TeamRoleSneaker : TeamRoleBase
    {
        public TeamRoleSneaker(SoldierTeamBlueberry mySoldier, Team myTeam) : base(mySoldier, myTeam)
        {
            mySoldier.gameObject.name += "_Sneaker" + myTeam.Name + "_" + MemberIndex; 
        }

        public override void StartRole()
        {
            base.StartRole();
            Cell target;
            if (myTeam.Name == SNEAKER_TEAM1_NAME)
                target = InitializeTeam1Route();
            else
                target = InitializeTeam2Route();

            navAgent.targetCell = target;
        }

        public override void UpdateRole()
        {
            base.UpdateRole();

            if (enemiesInSight.Count > 0)
            {
                if (mySoldier.LookAt(Vector3Extensions.GetPositionWithSmallestDiffAngle(enemiesInSight, mySoldier)))
                    mySoldier.Shoot();

                navAgent.moveToTarget = false;
                underFireCell = null;
            }
            else if (underFireCell != null)
            {
                mySoldier.LookAt(underFireCell.GetPosition());
                if (CellExtentions.IsCellAngleSmaller(underFireCell, 50, mySoldier))
                    underFireCell = null;
            }
            else
            {
                if (navAgent.pathGenerated.Count > 0)
                {
                    navAgent.moveToTarget = true;
                    mySoldier.MoveTowards(navAgent.pathGenerated[0]);
                    mySoldier.LookAt(navAgent.pathGenerated[0]);
                }
                else
                {
                    myTeam.Route.RemoveNextWaypoint();
                    if (!myTeam.Route.IsEmpty())
                        navAgent.targetCell = myTeam.Route.GetNextWaypoint();
                }
            }
        }

        public override void ReceiveMessage(TeamMessage message)
        {
            if ((message.messageType == TeamMessageTypes.UnderFire) && (message.content != null))
            {
                MessageContentUnderFire content = (MessageContentUnderFire)message.content;
                underFireCell = content.position;
            }
        }

        private Cell InitializeTeam1Route()
        {
            myTeam.Route.Clear();
            if (myTeam.MyFlagPosition.x < 0)
            {
                myTeam.Route.AddCell(new Cell(-19, 1, 0));
                myTeam.Route.AddCell(new Cell(-14, 3, 0));
                myTeam.Route.AddCell(new Cell(-12, 12, 0));
                myTeam.Route.AddCell(new Cell(-3, 20, 0));
            }
            else
            {
                myTeam.Route.AddCell(new Cell(4, 19, 0));
                myTeam.Route.AddCell(new Cell(-3, 19, 0));
                myTeam.Route.AddCell(new Cell(-16, 9, 0));
            }
            myTeam.Route.AddCell(myTeam.EnemyFlagPosition);
            return myTeam.Route.GetNextWaypoint();
        }

        private Cell InitializeTeam2Route()
        {
            myTeam.Route.Clear();
            if (myTeam.MyFlagPosition.x < 0)
            {
                myTeam.Route.AddCell(new Cell(2, -19, 0));
                myTeam.Route.AddCell(new Cell(7, -10, 0));
                myTeam.Route.AddCell(new Cell(11, -4, 0));
                myTeam.Route.AddCell(new Cell(18, -1, 0));
            }
            else
            {
                myTeam.Route.AddCell(new Cell(13, 5, 0));
                myTeam.Route.AddCell(new Cell(11, -8, 0));
                myTeam.Route.AddCell(new Cell(2, -21, 0));
            }
            myTeam.Route.AddCell(myTeam.EnemyFlagPosition);
            return myTeam.Route.GetNextWaypoint();
        }

    }
}