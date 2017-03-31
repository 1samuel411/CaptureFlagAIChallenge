using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBlueberry
{
    public class TeamRoleSniper : TeamRoleBase
    {
        Cell targetPos;
        Cell lookAtPos;

        public TeamRoleSniper(SoldierTeamBlueberry mySoldier, Team myTeam) : base(mySoldier, myTeam)
        {
            MemberIndex = myTeam.MemberCount;
            mySoldier.gameObject.name += "_Sniper" + myTeam.Name + "_" + MemberIndex;
        }

        public override void StartRole()
        {
            base.StartRole();

            InitializeTargetPositions();

            navAgent.targetCell = targetPos;
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
            else if (reportedEnemyPositions.Count > 0)
            {
                mySoldier.LookAt(Vector3Extensions.GetPositionWithSmallestDiffAngle(reportedEnemyPositions, mySoldier));
            }
            else
            {
                if (navAgent.pathGenerated.Count > 0)
                {
                    navAgent.moveToTarget = true;
                    mySoldier.MoveTowards(navAgent.pathGenerated[0]);
                }
                if (backToHome && (navAgent.pathGenerated.Count > 0))
                    mySoldier.LookAt(navAgent.pathGenerated[0]);
                else
                {
                    Vector3 dir = MyCell.GetPosition() + lookAtPos.GetPosition();
                    mySoldier.LookAt(dir);
                }
            }
        }

        public override void ReceiveMessage(TeamMessage message)
        {
            if ((message.messageType == TeamMessageTypes.EnemyLocated) && (message.content != null))
            {
                MessageContentEnemyLocated content = (MessageContentEnemyLocated)message.content;
                content.positions.ForEach(v => {
                    Cell cell = v.ToCell();
                    AddReportedEnemyPosition(cell);
                });
            }
            else if ((message.messageType == TeamMessageTypes.UnderFire) && (message.content != null))
            {
                MessageContentUnderFire content = (MessageContentUnderFire)message.content;
                underFireCell = content.position;
            }
        }

        private void InitializeTargetPositions()
        {
            if (myTeam.MyFlagPosition.x < 0)
            {
                if (myTeam.Name == SNIPER_TEAM1_NAME)
                {
                    targetPos = new Cell(-15, -1, 0);
                    if (MemberIndex == 1)
                        lookAtPos = new Cell(10, 0, 0);
                    else if (MemberIndex == 2)
                        lookAtPos = new Cell(0, -10, 0);
                    else
                        lookAtPos = new Cell(-10, 0, 0);
                }
                else
                {
                    targetPos = new Cell(-10, -11, 0);
                    targetPos = new Cell(-4, -17, 0);
                    if (MemberIndex == 1)
                        lookAtPos = new Cell(-10, 0, 0);
                    else if (MemberIndex == 2)
                        lookAtPos = new Cell(10, -10, 0);
                    else
                        lookAtPos = new Cell(0, 10, 0);
                }
            }
            else
            {
                if (myTeam.Name == SNIPER_TEAM1_NAME)
                {
                    targetPos = new Cell(3, 17, 0);
                    if (MemberIndex == 1)
                        lookAtPos = new Cell(-10, 10, 0);
                    else if (MemberIndex == 2)
                        lookAtPos = new Cell(10, 10, 0);
                    else
                        lookAtPos = new Cell(10, -10, 0);
                }
                else
                {
                    targetPos = new Cell(15, 1, 0);
                    if (MemberIndex == 1)
                        lookAtPos = new Cell(-10, 0, 0);
                    else if (MemberIndex == 2)
                        lookAtPos = new Cell(0, 10, 0);
                    else
                        lookAtPos = new Cell(10, 0, 0);
                }
            }
        }
    }
}