using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GameServer.Models
{
    internal class Guild
    {
        public int Id { get { return this.Data.Id; } }

        //private Character Leader;
        public string Name { get { return this.Data.Name; } }

        public List<Character> Members = new List<Character>();

        public double timestamp;
        public TGuild Data;
        public Guild(TGuild guild) 
        {
            this.Data= guild;
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            //防止同一个玩家重复申请
            var oldApply = this.Data.GuildApplies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if(oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.GuildApplies.Add(dbApply);
            DBService.Instance.Save();

            this.timestamp = Time.timestamp;
            return true;
        }

        internal bool JoinApprove(NGuildApplyInfo apply)
        {
            //防止出现不存在的申请
            var oldApply = this.Data.GuildApplies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply == null)
            {
                return false;
            }

            oldApply.Result = (int)apply.Result;
            if(apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level,GuildTitle.None);
            }

            DBService.Instance.Save();
            this.timestamp = Time.timestamp;
            return true;
        }

        /// <summary>
        ///  添加成员
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="name"></param>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <param name="none"></param>
        public void AddMember(int characterId, string name, int @class, int level, GuildTitle none)
        {
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)none,
                JoinTime = DateTime.Now,
                LastTime = DateTime.Now
            };
            this.Data.Members.Add(dbMember);

            var character = CharacterManager.Instance.GetCharacter(characterId);
            if(character != null)
            {//在线
                character.Data.GuildId = this.Id;
            }
            else
            {
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbChar.GuildId = this.Id;
            }
            this.timestamp = Time.timestamp;
        }

        /// <summary>
        /// 离开工会
        /// </summary>
        /// <param name="c"></param>
        internal void Leave(int characterId)
        {
            var character = CharacterManager.Instance.GetCharacter(characterId);
            //Log.InfoFormat("Leave Guild[{0}{1}]", c.Name, c.Id);

            //处理玩家方面的删除
            if (character != null)
            {
                character.Data.GuildId = 0;
                character.Guild = null;
            }
            else
            {
                //DBService.Instance.Entities.Database.ExecuteSqlCommand("UPDATE ExtermeWorld.dbo.Characters SET GuildId = @p0 WHERE ID = @p1", 0, characterId);
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbChar.GuildId = 0;
            }

            //处理工会DB删除

            //DBService.Instance.Entities.Database.ExecuteSqlCommand("DELETE FROM ExtremeWorld.dbo.GuildMembers WHERE CharacterId = @p0", characterId);
            TGuildMember dbMember = GetDBMember(characterId);
            DBService.Instance.Entities.GuildMembers.Remove(dbMember);

            DBService.Instance.Save();
            this.timestamp = Time.timestamp;
        }

        /// <summary>
        /// 执行管理员命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="targetId"></param>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal bool ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);

            switch(command)
            {
                case GuildAdminCommand.Promote:
                    if(target.Title == (int)GuildTitle.None)
                    {
                        target.Title = (int)GuildTitle.Senior;
                    }else if (target.Title == (int)GuildTitle.Senior)
                    {
                        target.Title = (int)GuildTitle.VicePresident;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case GuildAdminCommand.Depost:
                    if (target.Title == (int)GuildTitle.VicePresident)
                    {
                        target.Title = (int)GuildTitle.Senior;
                    }
                    else if (target.Title == (int)GuildTitle.Senior)
                    {
                        target.Title = (int)GuildTitle.None;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderId= targetId;
                    this.Data.LeaderName = target.Name;
                    break;

                case GuildAdminCommand.Kickout:
                    Leave(targetId);
                    break;
            }

            DBService.Instance.Save();
            timestamp = Time.timestamp;

            return true;
        }

        private TGuildMember GetDBMember(int targetId)
        {
            foreach(var member in this.Data.Members)
            {
                if(member.CharacterId == targetId)
                {
                    return member;
                }
            }
            return null;
        }

        internal NGuildInfo GuildInfo(Character character)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                guildName = this.Name,
                Notice = this.Data.Notice,
                leaderID = this.Data.LeaderId,
                leaderName = this.Data.LeaderName,
                createTime = (long)Time.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };

            if (character != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (character.Id == this.Data.LeaderId) //向工会会长发送加入申请
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }
            return info;
        }

        internal void PostProcess(Character character, NetMessageResponse message)
        {
            if(message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(character);
            }
        }



        /// <summary>
        /// 拉取工会申请信息
        /// </summary>
        /// <returns></returns>
        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach(var i in this.Data.GuildApplies)
            {
                if (i.Result != (int)ApplyResult.None) continue;

                applies.Add(new NGuildApplyInfo()
                {
                    characterId = i.CharacterId,
                    GuildId = (int)i.GuildId,
                    Class = i.Class,
                    Level = i.Level,
                    Name= i.Name,
                    Result = (ApplyResult)i.Result,
                });
            }
            return applies;
        }

        private IEnumerable<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();

            foreach(var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)Time.GetTimestamp(member.JoinTime),
                    lastTime = (long)Time.GetTimestamp(member.LastTime)
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if(character != null)//更新团员在线状态
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId, Name = member.Name,Class = (CharacterClass)member.Class,Level = member.Level,
            };
        }
    }
}
