using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
     class QuestManager
    {
        Character Owner;

        public QuestManager(Character character)
        {
            this.Owner = character;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        private NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            // DB数据向N数据转换
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        internal Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                // 创建一个数据库对象，但只在内存中，不在数据库中
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                // 为这个T的任务赋值
                dbQuest.QuestID = quest.ID;
                if (quest.Target1 == QuestTarget.None)
                {
                    // 无目标直接完成
                    dbQuest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    dbQuest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbQuest);
                character.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        internal Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;

            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                //表中查询id对应任务
                var dbQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();

                if (dbQuest != null)
                {
                    if (dbQuest.Status != (int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    // 任务状态已结算，防止多次领奖
                    dbQuest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                    DBService.Instance.Save();

                    // 任务完成 处理奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp > 0)
                    {
                        // character.Exp += quest.RewardExp;
                    }

                    if(quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                else
                {
                    sender.Session.Response.questSubmit.Errormsg = "任务不存在玩家任务中";
                    return Result.Failed;
                }
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在任务列表中";
                return Result.Failed;
            }
        }
    }
}
