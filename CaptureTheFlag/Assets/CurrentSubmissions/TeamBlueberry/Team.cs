using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamBlueberry
{
    public class Team : IEnumerable
    {
        private List<TeamRoleBase> members = new List<TeamRoleBase>();

        public IEnumerator GetEnumerator()
        {
            return members.GetEnumerator();
        }

        private Route route = new Route();
        public Route Route { get { return route; } }

        public Cell MyFlagPosition { get; set; }
        public Cell EnemyFlagPosition { get; set; }

        public string Name { get; set; }

        public int MemberCount { get { return members.Count; } }

        public Team()
        {
        }

        public void Reset()
        {
            route.Clear();
            teamStartFinished = false;
            teamUpdateFinished = false;
        }

        public void AddMember(TeamRoleBase member)
        {
            if (!members.Contains(member))
                members.Add(member);
        }

        public void RemoveMember(TeamRoleBase member)
        {
            members.Remove(member);
        }

        public void SendMessage(TeamRoleBase sender, TeamMessage message)
        {
            members.ForEach(r => {
                if (r != sender)
                    r.ReceiveMessage(message);
            });
        }


        private bool teamStartFinished;
        public void TeamStart()
        {
            if (teamStartFinished)
                return;

            teamStartFinished = true;
        }


        private bool teamUpdateFinished;
        public void TeamUpdate()
        {
            if (teamUpdateFinished)
                return;

            members.ForEach(m => m.ClearReportedEnemyPositions());

            members.ForEach(m => {
                m.UpdateCell();
                m.GetEnemiesInSight();
                m.ReportEnemyInSightPositions();
            });

            teamUpdateFinished = true;
        }

        public void ResetTeamUpdate()
        {
            teamUpdateFinished = false;
        }

    }
}
