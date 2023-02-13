# GWorld
An demo MMO for unity learning.


# 协议管理
Lib/proto/message.proto
当中管理着协议模板，服务器、客户端通讯所需的消息和消息字段来这里管理

双击 Tools/genproto.cmd 生成一套协议

生成的 protocal 位于 Lib/Protocol/message.cs

此后，被修改的协议可以在SkillBridge.Message下的请求中得到