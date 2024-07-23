using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {

        public GuildService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate:: GuildName:{0} character:[{1}] {2}", request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会了";
                sender.SendResponse();
                return;
            }
            if(GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.SendResponse();
                return;
            }
            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList:: character:[{1}] {2}", character.Id, character.Name);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest:: GuildId:{0} character:[{1}] {2}", request.Apply.GuildId, request.Apply.characterId, request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;
            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.SendResponse();
            }
        }

        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse:: GuildId:{0} character:[{1}] {2}", response.Apply.GuildId, response.Apply.characterId, response.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (guild.JoinAppove(response.Apply))
            {
                if(requester != null)
                {
                    requester.Session.Character.Guild = guild;

                    requester.Session.Response.guildJoinRes = response;
                    requester.Session.Response.guildJoinRes.Result = Result.Success;
                    requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                    requester.SendResponse();
                }
            }
            else
            {

                if (requester != null)
                {
                    requester.Session.Response.guildJoinRes = response;               
                    requester.Session.Response.guildJoinRes.Result = Result.Failed;                        
                    requester.Session.Response.guildJoinRes.Errormsg = "加入公会请求被拒绝";                        
                    requester.SendResponse();
                }
            }
            
            
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave:: character:{0} GuildId:{1}", character.Id,character.Guild.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            if(character.Id == character.Guild.Data.LeaderID && character.Guild.Data.Members.Count != 1)
            {
                sender.Session.Response.guildLeave.Result = Result.Failed;
                sender.Session.Response.guildLeave.Errormsg = "请先将会长职务转让给其他成员后再退出公会！";
                sender.SendResponse();
            }
            else
            {
                character.Guild.Leave(character);
                sender.Session.Response.guildLeave.Result = Result.Success;
            }
            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin:: characterId:{0}, command:{1}, targetId:{2}", character.Id, request.Command, request.Target);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "角色没有公会";
                sender.SendResponse();
                return;
            }

            if (request.Command == GuildAdminCommand.Kickout && request.Target == character.Guild.Data.LeaderID)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "无法将会长踢出公会";
                sender.SendResponse();
                return;
            }
            character.Guild.ExecuteAdmin(request.Command, request.Target, character.Id);

            var target = SessionManager.Instance.GetSession(request.Target);
            if (target != null)
            {
                
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = request;
                target.SendResponse();
            }
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = request;
            sender.SendResponse();
        }

    }
}
