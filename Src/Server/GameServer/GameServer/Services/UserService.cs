using Common;
using GameServer.Entities;
using log4net;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        public UserService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCharacterCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserDeleteCharacterRequest>(this.OnDeleteCharacter);
        }


        public void Init()
        {
            Log.Info("UserService has been started");
        }

        void OnLogin(NetConnection<NetSession> sender,UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Password:{1}",request.User,request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null) 
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if(user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach(var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            Log.InfoFormat("UserLoginResponse: Result:{0}  msg:{1}", sender.Session.Response.userLogin.Result, sender.Session.Response.userLogin.Errormsg);
            sender.SendResponse();

        }

        void OnRegister(NetConnection<NetSession> sender,UserRegisterRequest request) 
        {
            Log.InfoFormat("UserRegisterRequest: User: {0}  Password: {1}",request.User,request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if(user != null )
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在。";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player});
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }
            Log.InfoFormat("UserRegisterResponse: Result:{0}  msg:{1}", message.Response.userRegister.Result, message.Response.userRegister.Errormsg);
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data,0,data.Length);
        }

        void OnCharacterCreate(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCharacterCreateRequest: Name: {0}  Class: {1}", request.Name, request.Class);

            sender.Session.Response.createChar = new UserCreateCharacterResponse();

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 100000,
                Equips = new byte[28]
            };
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1,
                ItemCount = 20,
            });
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,
            });


            try
            {
                character = DBService.Instance.Entities.Characters.Add(character);
                sender.Session.User.Player.Characters.Add(character);
                DBService.Instance.Entities.SaveChanges();
            }
            catch(SqlException e)
            {
                Log.Error(e.Message);
                sender.Session.Response.createChar.Result = Result.Failed;
                sender.Session.Response.createChar.Errormsg = "数据库创建失败";
            }

            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";
            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.Tid = c.ID;
                sender.Session.Response.createChar.Characters.Add(info);
            }
            sender.SendResponse();
        }

        private void OnDeleteCharacter(NetConnection<NetSession> sender, UserDeleteCharacterRequest request)
        {

            sender.Session.Response.deleteChar = new UserDeleteCharacterResponse();

            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.CharacterId);
            
            if(dbchar == null)
            {
                sender.Session.Response.deleteChar.Result = Result.Failed;
                sender.Session.Response.deleteChar.Errormsg = "角色不存在或已删除";
            }
            else
            {
                Log.InfoFormat("UserDeleteCharacterRequest: characterID:{0} Name:{1}",dbchar.ID,dbchar.Name);
                DBService.Instance.Entities.Characters.Remove(dbchar);
                sender.Session.User.Player.Characters.Remove(dbchar);
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.deleteChar.Result= Result.Success;
                sender.Session.Response.deleteChar.Errormsg= "None";
                foreach (var c in sender.Session.User.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = 0;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;
                    sender.Session.Response.deleteChar.Characters.Add(info);

                }
            }

            Log.InfoFormat("UserDeleteCharacterResponse: Result:{0}  msg:{1}", sender.Session.Response.deleteChar.Result, sender.Session.Response.deleteChar.Errormsg);
            sender.SendResponse();
        }

        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0} :{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";
            sender.Session.Response.gameEnter.Character = character.Info;

#if DEBUG
            ////道具系统测试
            //int itemID = 1;
            //bool hasItem = character.ItemManager.HasItem(itemID);
            //Log.InfoFormat("HasItem:[{0}]{1}", itemID, hasItem);
            //if (hasItem)
            //{
            //    //character.ItemManager.RemoveItem(itemID, 1);
            //    character.ItemManager.AddItem(1, 200);
            //    character.ItemManager.AddItem(2, 100);
            //    character.ItemManager.AddItem(3, 30);
            //    character.ItemManager.AddItem(4, 120);
            //}
            //else
            //{
            //    character.ItemManager.AddItem(itemID, 2);
            //}
            //Models.Item item = character.ItemManager.GetItem(itemID);
            //Log.InfoFormat("Item:[{0}][{1}]", itemID, item);
            //DBService.Instance.Save();
#endif
            sender.SendResponse();
            sender.Session.Character = character;
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender,character);
        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: CharacterID: {0}: {1} Map: {2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);
        
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
        }

         public void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
  
}
