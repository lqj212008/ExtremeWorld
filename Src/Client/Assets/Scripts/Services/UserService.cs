﻿using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
	class UserService : Singleton<UserService>,IDisposable
	{
		public UnityEngine.Events.UnityAction<Result, String> OnLogin;
		public UnityEngine.Events.UnityAction<Result, String> OnRegister;
		public UnityEngine.Events.UnityAction<Result, String> OnCharacterCreate;

		NetMessage pendingMessage = null;

		bool connected = false;

		public UserService() 
		{
			NetClient.Instance.OnConnect += OnGameServerConnect;
			NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
			MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
			MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);  //监听服务器的注册返回结果
			MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
		}

        

        public void Dispose()
		{
			MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
			MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
			MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
			NetClient.Instance.OnConnect -= OnGameServerConnect;
			NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
		}

		public void Init()
		{

		}

		//客户端连接到服务器
		public void ConnectToServer() 
		{
			Debug.Log("ConnectToServer() Start");
			NetClient.Instance.Init("127.0.0.1", 8000);
			NetClient.Instance.Connect();
		}

		//游戏服务连接服务器，并表示占用
		public void OnGameServerConnect(int result, string reason) 
		{
			Log.InfoFormat("LoadingMessager::OnGameServerConnect: {0} reason: {1}", result, reason);
			if (NetClient.Instance.Connected)
			{
				this.connected = true;
				if(this.pendingMessage != null)
				{
					NetClient.Instance.SendMessage(this.pendingMessage);
					this.pendingMessage = null;
				}
				else
				{
					if (!this.DisconnectNotify(result, reason))
					{
						MessageBox.Show(string.Format("网络错误，无法连接到服务器！ \n RESULT: {0} ERROR: {1}",result,reason),"错误",MessageBoxType.Error);
					}
				}
			}
		}

		public void OnGameServerDisconnect(int result, string reason)
		{
			this.DisconnectNotify(result, reason);
			return;
		}

		bool DisconnectNotify(int result, string reason)
		{
			if(this.pendingMessage != null)
			{
				if (this.pendingMessage.Response.userRegister != null)
				{
					if (this.OnRegister != null)
					{
						this.OnRegister(Result.Failed, string.Format("服务器断开！ \n RESULT: {0} ERROR: {1}", result, reason));
					}
				}
				return true;
			}
			return false;
		}

		public void SendLogin(string user, string password)
		{
            Debug.LogFormat("UserLoginRequest::user: {0} psw: {1}", user, password);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = password;

            if (this.connected && NetClient.Instance.Connected)  //判断连接是否连上
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();        //重连操作
            }
        }

		private void OnUserLogin(object sender, UserLoginResponse response)
		{
			Debug.LogFormat("OnUserLogin: {0}  [{1}]", response.Result, response.Errormsg);

			if (response.Result == Result.Success)
			{
				Models.User.Instance.SetupUserInfo(response.Userinfo);
			};
			if(this.OnLogin != null)
			{
				this.OnLogin(response.Result, response.Errormsg);
			}
		}

		public void SendRegister(string user, string password)
		{
			Debug.LogFormat("UserRegisterRequest::user: {0} psw: {1}",user,password);
			NetMessage message = new NetMessage();
			message.Request = new NetMessageRequest();
			message.Request.userRegister = new UserRegisterRequest();
			message.Request.userRegister.User = user;
			message.Request.userRegister.Passward = password;

			if(this.connected && NetClient.Instance.Connected)  //判断连接是否连上
			{
				this.pendingMessage = null;
				NetClient.Instance.SendMessage(message); 
			}
			else 
			{ 
				this.pendingMessage = message;
				this.ConnectToServer();        //重连操作
			}
		}


		//注册结束，服务器返回的响应
        private void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister: {0}  [{1}]",response.Result,response.Errormsg);

			if(this.OnRegister != null) 
			{
				this.OnRegister(response.Result,response.Errormsg);
			}
        }

		//角色创建
		public void SendCharacterCreate(string charName, CharacterClass cls)
		{
			Debug.LogFormat("SendCharacterCreate::charName:{0} Class:{1}", charName,cls);
			NetMessage message = new NetMessage();
			message.Request = new NetMessageRequest();
			message.Request.createChar = new UserCreateCharacterRequest();
			message.Request.createChar.Name = charName;
			message.Request.createChar.Class = cls;

			if(this.connected && NetClient.Instance.Connected)
			{
				this.pendingMessage= null;
				NetClient.Instance.SendMessage(message);
			}
			else
			{
				this.pendingMessage = message;
				this.ConnectToServer();
			}
		}

		void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
		{
			Debug.LogFormat("OnUserCreateCharacter:{0} {1}", response.Result, response.Errormsg);

			if(response.Result == Result.Success)
			{
				Models.User.Instance.info.Player.Characters.Clear();
				Models.User.Instance.info.Player.Characters.AddRange(response.Characters);
			}

			if(this.OnCharacterCreate != null)
			{
				this.OnCharacterCreate(response.Result, response.Errormsg);
			}
		}

		
    }
}
