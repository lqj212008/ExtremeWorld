using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Chat
    {
        Character Owner;

        public int localIdx;
        public int worldIdx;
        public int systemIdx;
        public int teamIdx;
        public int guildIdx;

        public Chat(Character owner)
        {
            this.Owner = owner;
        }

        public void PostProcess(NetMessageResponse response)
        {
            if(response.Chat == null)
            {
                response.Chat = new ChatResponse();
                response.Chat.Result = Result.Success;
            }
            this.localIdx = ChatManager.Instance.GetLocalMessages(this.Owner.Info.mapId, this.localIdx, response.Chat.localMessages);
            this.worldIdx = ChatManager.Instance.GetWorldMessages(this.worldIdx, response.Chat.worldMessages);
            this.systemIdx = ChatManager.Instance.GetSystemMessages(this.systemIdx, response.Chat.systemMessages);
            if(this.Owner.Team != null)
            {
                this.teamIdx = ChatManager.Instance.GetTeamMessages(this.Owner.Team.Id, this.teamIdx, response.Chat.teamMessages);
            }
            if(this.Owner.Guild != null)
            {
                this.guildIdx = ChatManager.Instance.GetGuildMessages(this.Owner.Guild.Id, this.guildIdx, response.Chat.guildMessages  );
            }
        }
        
    }
}
