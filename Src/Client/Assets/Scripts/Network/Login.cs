using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// 初始化网络
		Network.NetClient.Instance.Init("127.0.0.1", 8000);
		Network.NetClient.Instance.Connect();

		// 创建主消息
		//SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
		//msg.Request = new SkillBridge.Message.NetMessageRequest();
		// 创建自定义消息（请求，回复）
		//msg.Request.FirstTestRequest = new SkillBridge.Message.FirstTestRequest();
		// 修改消息内容
		//msg.Request.FirstTestRequest.Msg = "message";
		//msg.Request.FirstTestRequest.Pwd = "1234567";
		// 消息发送
		//Network.NetClient.Instance.SendMessage(msg);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
