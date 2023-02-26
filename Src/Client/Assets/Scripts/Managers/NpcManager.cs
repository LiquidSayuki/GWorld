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
            if(npc.Type == NpcType.Task)
            {
                return DoTaskInteractive(npc);
            }
            else if(npc.Type == NpcType.Functional)
            {
                return DoFunctionalInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc) {
            MessageBox.Show("点击了NPC：" + npc.Name + "NPC对话");
            return true; 
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

