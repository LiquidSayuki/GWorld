using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    ///  Manage Monster Spawn
    /// </summary>
    internal class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;

        internal void Init(Map map)
        {
            this.Map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                // 读取自己所属地图的所有刷怪规则
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    // 将刷怪规则实例化出刷怪器
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }

        internal void Update()
        {
            if (Rules.Count == 0)
            {
                return;
            }
            for (int i = 0; i < this.Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
