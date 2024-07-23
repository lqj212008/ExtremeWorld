using Common;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;


namespace Services
{
    class TeamService : Singleton<TeamService>,IDisposable
    {
        public void Init()
        {

        }

        public TeamService() 
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        /// <summary>
        /// 发送组队请求
        /// </summary>
        /// <param name="friendId">邀请组队的好友id</param>
        /// <param name="friendName">邀请组队的好友姓名</param>
        public void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.Log("SendTeamInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id; ;
            message.Request.teamInviteReq.fromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到添加组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("【{0}】邀请你加入队伍", request.fromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.onYes = () =>
            {
                this.SendTeamInviteResponse(true, request);
            };
            confirm.onNo = () =>
            {
                this.SendTeamInviteResponse(false, request);
            };
        }

        /// <summary>
        /// 发送是否同意组队邀请
        /// </summary>
        /// <param name="accept">是否同意组队，同意为true,拒绝为false</param>
        /// <param name="request">收到的组队请求</param>
        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRes = new TeamInviteResponse();
            message.Response.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.teamInviteRes.Errormsg = accept ? "组队成功" : "对方拒绝了组队请求";
            message.Response.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到组队回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteResponse(object sender, TeamInviteResponse response)
        {
            if (response.Result == Result.Success)
                MessageBox.Show(response.Request.ToName + " 加入你的队伍", "邀请组队成功");
            else
                MessageBox.Show(response.Errormsg, "邀请组队失败");
        }

        private void OnTeamInfo(object sender, TeamInfoResponse response)
        {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(response.Team);
        }

        public void SendTeamLeaveRequest(int id)
        {
            Debug.Log("SendTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }

        private void OnTeamLeave(object sender, TeamLeaveResponse response)
        {
            if(response.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show(response.Errormsg, "退出队伍");
            }
            else
            {
                MessageBox.Show("退出失败","退出队伍",MessageBoxType.Error);
            }
        }
    }
}
