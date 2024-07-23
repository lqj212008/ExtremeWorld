using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Services 
{
    public class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        public UnityAction<bool> OnGuildCreateResult;

        public void Init()
        {

        }
        public GuildService() 
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        

        /// <summary>
        /// 发送创建公会请求
        /// </summary>
        /// <param name="guildName">公会名</param>
        /// <param name="notice">公会宣言</param>
        public void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到公会创建响应
        /// </summary>
        /// <param name="sender">连接对象</param>
        /// <param name="response">服务器返回的响应信息</param>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse: {0}",response.Result);
            if(OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会创建失败", response.guildInfo.GuildName), "公会");
            }

        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildId">要加入的公会ID</param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到加入公会请求的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm,"同意","拒绝");
            confirm.onYes = () =>
            {
                this.SendGuildJoinResponse(true, request.Apply);
            };

            confirm.onNo = () =>
            {
                this.SendGuildJoinResponse(false, request.Apply);
            };
        }

        /// <summary>
        /// 发送回应加入公会结果的请求
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="response"></param>
        public void SendGuildJoinResponse(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.guildJoinRes = new GuildJoinResponse();
            message.Response.guildJoinRes.Result = Result.Success;
            message.Response.guildJoinRes.Apply = apply;
            message.Response.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到回应加入公会结果的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse: {0}", response.Result);
            if(response.Result == Result.Success)
            {
                MessageBox.Show("加入公会请求通过", "公 会");
            }
            else
            {
                MessageBox.Show(response.Errormsg, "公 会");
            }
        }

        /// <summary>
        /// 收到公会信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuild(object sender, GuildResponse response)
        {
            Debug.LogFormat("OnGuild:{0} {1}:{2}", response.Result, response.Guilds.Id, response.Guilds.GuildName);
            GuildManager.Instance.Init(response.Guilds);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();
        }

        /// <summary>
        /// 发送退出公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到退出公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公 会");
            }
            else
            {
                MessageBox.Show("离开公会失败", "公 会",MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 请求公会列表
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到公会列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if(OnGuildListResult != null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }

        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.guildJoinRes = new GuildJoinResponse();
            message.Response.guildJoinRes.Result = Result.Success;
            message.Response.guildJoinRes.Apply = apply;
            message.Response.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        public void SendAdminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.Log("SendAdminCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse response)
        {
            Debug.LogFormat("GuildAdmin: {0} {1}", response.Command.Command, response.Result);
            MessageBox.Show(string.Format("执行操作：{0} 结果：{1}", response.Command.Command, response.Result ),"公会操作");
        }
    }
}


