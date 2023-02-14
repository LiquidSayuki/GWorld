using Common;
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
    class HelloWorldService : Singleton<HelloWorldService>
    {
        // Init,start,stop 为每一个service都需要的方法
        public void Init()
         {

         }

        public void Start()
        {
            // 差不多是一种固定句式
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.OnFirstTestRequest);
        }

        public void Stop()
        {

        }

        //接收到消息后，在这个函数中处理
        void OnFirstTestRequest(NetConnection<NetSession> sender, FirstTestRequest request)
        {
            Console.WriteLine("OnFirstTestRequest : Hello :{0} , Password:{1}", request.Msg, request.Pwd);
        }
    }
    
}
