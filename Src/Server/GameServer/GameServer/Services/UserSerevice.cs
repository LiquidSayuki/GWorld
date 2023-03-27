using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }


        public void Init()
        {

        }

        #region 登录 login
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                // 将当前的user绑定进入本session的User中
                // 在之后需要调用时，就可以使用了
                // 如果不将此处user存入Session中，无法进行类似于查找属于用户的角色之类的操作
                sender.Session.User = user;

                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.ConfigId = c.ID;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    info.Type = CharacterType.Player;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
        #endregion

        #region 注册 register
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
        #endregion

        #region 创建角色 create character
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 100000,
                Equips = new byte[28],
            };
            // 初始化新角色的背包
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            TCharacterItem it = new TCharacterItem();
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);

            // 赋予初始道具
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemId = 1,
                ItemCount = 10,
            });

            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemId = 2,
                ItemCount = 10,
            });

            // 操作DB，将新角色保存至数据库
            character = DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            //回发消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";

            //所有已有角色都回发给用户
            foreach(var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = c.ID;
                info.ConfigId = c.TID;
                info.Name = c.Name;
                info.Class = (CharacterClass)c.Class;
                info.Type = CharacterType.Player;
                message.Response.createChar.Characters.Add(info);
            }

            // 服务器使用session（sender）管理回发给哪一个用户
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
        #endregion

        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            //在角色管理器中添加角色
            Character character = CharacterManager.Instance.AddCharacter(dbchar);
            //记录此角色的Session
            SessionManager.Instance.AddSession(character.Id, sender);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";
            // 进入游戏成功，发送初始角色信息
            sender.Session.Response.gameEnter.Character = character.Info;

            sender.SendResponse();
            sender.Session.Character = character;
            sender.Session.PostResponser = character;
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserService -- GameLeave:[{0},CharacterID{1}:{2},Map{3}]", sender.Session.User.Username, character.Id, character.Info.Name, character.Info.mapId);
            //移除此角色的Session
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result= Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";
            sender.SendResponse();

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.gameLeave = new UserGameLeaveResponse();
            //message.Response.gameLeave.Result = Result.Success;
            //message.Response.gameLeave.Errormsg = "None";
            //byte[] data = PackageHandler.PackMessage(message);
            //sender.SendData(data, 0, data.Length);
        }

        public void CharacterLeave(Character character)
        {
            // 调用角色管理器，移除角色
            CharacterManager.Instance.RemoveCharacter(character.Id);
            //让地图移除角色
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
            //角色对象自己清理自己
            character.Clear();
        }
    }
}
