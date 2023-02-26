using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService: Singleton<BagService>
    {
        public BagService() {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest: Character:[{0}], Unlocked{1}", character.Data.ID, request.BagInfo.Unlocked);

            if(request.BagInfo != null)
            {
                character.Data.Bag.Items = request.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
