using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Team
    {
        public int Id;
        public Character Leader;
        public List<Character> Members = new List<Character>();

        // 时间戳，记录队伍最后一次更新队伍信息的时间
        public int timestamp;

        public Team(Character leader)
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            Log.InfoFormat("Enter Team {0} Character ID {1}", this.Id, member.Id);
            if(this.Members.Count == 0)
            {
                this.Leader = member;
            }
            this.Members.Add(member);
            member.team = this;
            timestamp = Time.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Team {0} Character ID {1}", this.Id, member.Id);
            this.Members.Remove(member);
            
            if(member == this.Leader)
            {
                // 重选队长
/*                if (this.Members.Count > 0)
                {
                    this.Leader = this.Members[0];
                }
                else
                {
                    this.Leader = null;
                }
*/

                //遣散队伍
                foreach(var i in this.Members)
                {
                    i.team = null;
                }
                this.Members.Clear();
                this.Leader = null;
            }
            member.team = null;
            timestamp = Time.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                foreach(var member in this.Members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
