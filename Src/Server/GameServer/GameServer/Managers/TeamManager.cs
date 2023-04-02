using Common;
using GameServer.Entities;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class TeamManager:Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();
        public void Init() { }

        public Team GetTeamByCharacter(int characterId)
        {
            Team team= null;
            this.CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }


        internal void AddTeamMember(Character leader, Character member)
        {
            if (leader.team == null)
            {
                leader.team = CreateTeam(leader);
            }

            leader.team.AddMember(member);
        }

        private Team CreateTeam(Character leader)
        {
            Team team = null;
            // 复用已经废弃的队伍
            for(int i=0; i<this.Teams.Count; i++)
            {
                team = this.Teams[i];
                if(team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }
            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;
        }
    }
}
