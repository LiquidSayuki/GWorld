using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class NpcManager : Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);
        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        // 在其他类中将一个方法绑定在NPC事件中（监听npc事件）
        // npc被唤醒时，会通过delegate唤醒其它类的对应方法
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
        }

        public NpcDefine GetNpcDefine(int ID)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(ID, out npc);
            return npc;
        }

        public bool Interactive(int npcID)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcID))
            {
                var npc = DataManager.Instance.Npcs[npcID];
                return Interactive(npc.TID);
            }
            return false;
        }

        public bool Interactive(NpcDefine npc)
        {
            // 优先检查npc任务
            if(DoTaskInteractive(npc))
            {
                return true;
            }

            else if(npc.Type == NpcType.Functional)
            {
                return DoFunctionalInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc) {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.TID);
            if (status == NpcQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenNpcQuest(npc.TID);
        }

        private bool DoFunctionalInteractive(NpcDefine npc) { 
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }
            if(!eventMap.ContainsKey(npc.Function))
            {
                return false;
            }
            
            return eventMap[npc.Function](npc);
        }
    }

}

