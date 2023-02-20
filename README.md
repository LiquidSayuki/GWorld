# GWorld
An demo MMO for unity learning.


# 协议管理
Lib/proto/message.proto
当中管理着协议模板，服务器、客户端通讯所需的消息和消息字段来这里管理

双击 Tools/genproto.cmd 生成一套协议

生成的 protocal 位于 Lib/Protocol/message.cs

在Src\Lib\Protocol\bin\Debug 下，有四个protobuf生成的文件，需要拷贝到Src\Client\Assets\References中使用

此后，被修改的协议可以在SkillBridge.Message下的请求中得到

实例
SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
msg.Request.FirstTestRequest = new SkillBridge.Message.FirstTestRequest();
msg.Request.FirstTestRequest.Msg = "message";
msg.Request.FirstTestRequest.Pwd = "1234567";



# 角色、地图表格文件
Src\Data\Tables下有配置数据表
修改后，运行Excel2Json.cmd
将Sec\Data\Data下的Json格式文件复制到服务端的Src\Server\GameServer\Gameserver\bin\Debug\Data下，供服务端读取

如果要为表新增字段，需要在common下修改对应的实体的Define文件
重新生成解决方案后，将Src\lib\common\bin\debug下的common，protobuf-net.dll，protocol.dll，protocol.pdb
转移到Src\client\assets\references下以供同步