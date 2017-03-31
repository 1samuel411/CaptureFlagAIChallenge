using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamBlueberry
{
    public class SoldierTeamBlueberry : SoldierWrapper
    {
        public TeamRoleBase MyRole { get; private set; }

        public static Team sneakerTeam1 = new Team() { Name = TeamRoleBase.SNEAKER_TEAM1_NAME };
        public static Team sneakerTeam2 = new Team() { Name = TeamRoleBase.SNEAKER_TEAM2_NAME };
        public static Team sniperTeam1 = new Team() { Name = TeamRoleBase.SNIPER_TEAM1_NAME};
        public static Team sniperTeam2 = new Team() { Name = TeamRoleBase.SNIPER_TEAM2_NAME };

        new void Awake()
        {
            base.Awake();
            SetName("Team Blueberry");

            sneakerTeam1.Reset();
            sneakerTeam2.Reset();
            sniperTeam1.Reset();
            sniperTeam2.Reset();

            Team teamToAdd = sniperTeam1;
            if (sniperTeam1.MemberCount == 3)
                teamToAdd = sniperTeam2;
            if (sniperTeam2.MemberCount == 3)
                teamToAdd = sneakerTeam1;
            if (sneakerTeam1.MemberCount > sneakerTeam2.MemberCount)
                teamToAdd = sneakerTeam2;

            if ((teamToAdd == sneakerTeam1) || (teamToAdd == sneakerTeam2))
                MyRole = new TeamRoleSneaker(this, teamToAdd);
            else 
                MyRole = new TeamRoleSniper(this, teamToAdd);
        }

        new void Start()
        {
            base.Start();
            MyRole.Team.MyFlagPosition = spawnLocation.ToCell();
            MyRole.Team.EnemyFlagPosition = enemySpawnLocation.ToCell();

            MyRole.StartRole();
        }

        new void Update()
        {
            base.Update();
            MyRole.UpdateRole();
        }

        void LateUpdate()
        {
            MyRole.LateUpdateRole();
        }

        void OnDisable()
        {
            MyRole.OnDisableRole();
        }

        /// <summary>
        /// Damage was taken on this unit
        /// </summary>
        /// <param name="location">Location of the damage</param>
        public override void DamageCallback(Vector3 location)
        {
            MyRole.ReceiveMessage(new TeamMessage(TeamMessageTypes.UnderFire, new MessageContentUnderFire(location.ToCell())));
        }

        /// <summary>
        /// A Flag was grabbed or dropped
        /// </summary>
        /// <param name="flag"></param>
        public override void FlagStatusChangedCallback(IGrabable flag)
        {
            if (flag.GetTeam() != soldier.GetTeam())
            {   // Enemy flag
                if (flag.Grabbed())
                {                    
                    foreach (TeamRoleBase b in sneakerTeam1)
                        b.CreateRouteToHomeBase();
                    foreach (TeamRoleBase b in sneakerTeam2)
                        b.CreateRouteToHomeBase();
                }
                else
                {   // Lost
                    sneakerTeam1.EnemyFlagPosition = flag.GetLocation().ToCell();
                    sneakerTeam2.EnemyFlagPosition = flag.GetLocation().ToCell();
                    MyRole.CreateRouteToEnemyFlag(); // Everyone
                }
            }
            else
            {   // Own flag
                foreach (TeamRoleBase b in sniperTeam1)
                    b.CreateRouteToHomeBase();
                foreach (TeamRoleBase b in sniperTeam2)
                    b.CreateRouteToHomeBase();
            }
        }

        /// <summary>
        /// A Soldier has died
        /// </summary>
        /// <param name="soldier"></param>
        public override void SoldierDiedCallback(IAgent soldier)
        {
            if (this.soldier == soldier)
            {
                MyRole.ReceiveMessage(new TeamMessage(TeamMessageTypes.SoldierDied, null));
                MyRole.Team.RemoveMember(MyRole);
            }
        }
    }
}