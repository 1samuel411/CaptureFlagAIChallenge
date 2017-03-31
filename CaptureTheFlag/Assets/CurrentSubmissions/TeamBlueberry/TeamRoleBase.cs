using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TeamBlueberry
{
    public abstract class TeamRoleBase
    {
        public const string SNEAKER_TEAM1_NAME = "SneakerTeam1";
        public const string SNEAKER_TEAM2_NAME = "SneakerTeam2";
        public const string SNIPER_TEAM1_NAME = "SniperTeam1";
        public const string SNIPER_TEAM2_NAME = "SniperTeam2";

        protected SoldierTeamBlueberry mySoldier;
        protected Team myTeam;
        public Team Team { get { return myTeam; } private set { myTeam = value; } }

        public int MemberIndex { get; protected set; }

        protected bool backToHome;

        protected NavAgentTeamBlueberry navAgent;

        public Cell MyCell { get; private set; }
        protected Cell underFireCell = null;

        protected List<Cell> reportedEnemyPositions = new List<Cell>();
        protected void AddReportedEnemyPosition(Cell pos)
        {
            if (!reportedEnemyPositions.Exists(c => CellExtentions.IsCellEqual(c, pos)))
            {
                reportedEnemyPositions.Add(pos);
                reportedEnemyPositions.Sort(new CellComparerDistanceToMe() { MyCell = MyCell });
            }
        }

        protected List<IAgent> enemiesInSight = new List<IAgent>();

        public TeamRoleBase(SoldierTeamBlueberry mySoldier, Team myTeam)
        {
            this.mySoldier = mySoldier;
            this.myTeam = myTeam;
            myTeam.AddMember(this);

            NavAgentExample navAgentEx = mySoldier.GetComponent<NavAgentExample>();
            if (navAgentEx)
                GameObject.Destroy(navAgentEx);

            navAgent = mySoldier.GetComponent<NavAgentTeamBlueberry>();
            if (!navAgent)
            {
                navAgent = mySoldier.gameObject.AddComponent<NavAgentTeamBlueberry>();
                navAgent.distanceNeeded = 0.05f;
            }
        }

        public virtual void StartRole()
        {
            UpdateCell();
            myTeam.TeamStart();
        }

        public virtual void UpdateRole()
        {
            myTeam.TeamUpdate();
        }

        public virtual void LateUpdateRole()
        {
            myTeam.ResetTeamUpdate();
        }

        public virtual void OnDisableRole()
        {
            myTeam.RemoveMember(this);
        }

        public abstract void ReceiveMessage(TeamMessage message);

        public void GetEnemiesInSight()
        {
            enemiesInSight.Clear();
            enemiesInSight.AddRange(mySoldier.soldiersInSight.FindAll(s => !s.IsDead() && (s.GetTeam() != mySoldier.soldier.GetTeam())));
        }

        public void ClearReportedEnemyPositions()
        {
            reportedEnemyPositions.Clear();
        }

        public void UpdateCell()
        {
            MyCell = mySoldier.soldier.GetLocation().ToCell();
        }

        public void ReportEnemyInSightPositions()
        {
            List<Vector3> positions = enemiesInSight.ConvertAll(e => new Vector3(e.GetLocation().x, e.GetLocation().y, e.GetLocation().z));
            if (underFireCell != null)
                positions.Add(underFireCell.GetPosition());
            myTeam.SendMessage(this, new TeamMessage(TeamMessageTypes.EnemyLocated, new MessageContentEnemyLocated(positions)));
        }

        public virtual void CreateRouteToEnemyFlag()
        {
            backToHome = false;
            myTeam.Route.Clear();
            myTeam.Route.AddCell(myTeam.EnemyFlagPosition);
            navAgent.targetCell = myTeam.Route.GetNextWaypoint();
        }

        public virtual void CreateRouteToHomeBase()
        {
            backToHome = true;
            myTeam.Route.Clear();
            myTeam.Route.AddCell(myTeam.MyFlagPosition);
            navAgent.targetCell = myTeam.Route.GetNextWaypoint();
        }
    }
}