//WARNING: DON'T EDIT THIS FILE!!!
using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userRegister); } //1
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); } //2

            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); } //3

            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); } //4
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); } // 5

            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); } //6
            if (message.mapCharacterLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterLeave); } // 7

            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }   //8

            if (message.itemBuy != null){ MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuy); } //10
            if (message.itemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemEquip); } //11

            if (message.questList!= null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questList); } //12
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); } //13
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); } //14

            if (message.statusNotify != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.statusNotify); } //100

        }

        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender,message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }

            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }

            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }

            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.mapTeleport != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapTeleport); }

            if (message.itemBuy != null){ MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuy); }
            if (message.itemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemEquip); }

            if (message.questList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questList); }
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); }
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); }  //14

            // 每新增消息类型，需要来此处添加分发
        }
    }
}