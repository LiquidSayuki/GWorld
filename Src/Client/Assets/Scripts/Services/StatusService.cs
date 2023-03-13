using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class StatusService: Singleton<StatusService>, IDisposable
    {
        // 使用一种eventMap的方式
        // 观察者
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();
        // hashset查询速度快
        HashSet<StatusNotifyHandler> handles = new HashSet<StatusNotifyHandler>();

        public void RegisterStatusNotify(StatusType function, StatusNotifyHandler action)
        {
            if (handles.Contains(action))
            {
                return;
            }
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }

            handles.Add(action);
        }
        public void Init()
        {

        }

        // Message Subscribe
        public StatusService() {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }


        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            foreach(NStatus status in notify.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            Debug.LogFormat("Satus Notify [Action:{0}], [Type:{1}]", status.Action.ToString(), status.Type.ToString());

            if (status.Action == StatusAction.Add)
            {
                User.Instance.AddGold(status.Value);
            }else if(status.Action == StatusAction.Delete){
                User.Instance.AddGold(-status.Value);
            }

            StatusNotifyHandler handler;
            if (eventMap.TryGetValue(status.Type, out handler))
            {
                handler(status);
            };
        }


    }
}
