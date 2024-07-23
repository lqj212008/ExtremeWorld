using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return Data.Id; } }

        public string Name { get { return this.Data.Name; } }
        public double timestamp;
        public TGuild Data;
        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.TGuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null)
            {
                return false;
            }
            

            oldApply.Result = (int)apply.Result;
            if(apply.Result == ApplyResult.Accept) 
            { 
                this.AddMember(apply.characterId,apply.Name,apply.Class,apply.Level,GuildTitle.None);
                DBService.Instance.Entities.TGuildApplies.Remove(oldApply);
                DBService.Instance.Save();
                this.timestamp = TimeUtil.timestamp;
                
                return true;
            }
            else
            {
                DBService.Instance.Entities.TGuildApplies.Remove(oldApply);
                DBService.Instance.Save();
                this.timestamp = TimeUtil.timestamp;
                return false ;
            }    
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now,
            };
            this.Data.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if(character != null)
            {
                character.Data.GuildId = this.Id;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(c=>c.ID == characterId);  
                dbchar.GuildId = this.Id; 
            }
            this.timestamp = TimeUtil.timestamp;

        }

        public void Leave(Character character)
        {
            Log.InfoFormat("Leave Guild: {0}:{1}",character.Id,character.Name);
            var member = character.Guild.GetDBMember(character.Id);
            if (member != null)
            {
                DBService.Instance.Entities.TGuildMembers.Remove(member);
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(c=>c.ID == character.Id);
                dbchar.GuildId = 0;
                DBService.Instance.Save();
            }
            if (Data.Members.Count == 0)
            {
                DBService.Instance.Entities.Guilds.Remove(Data);
                DBService.Instance.Save();
            }
            character.Guild = null;
            timestamp = TimeUtil.timestamp;
        }

        internal NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };

            if(from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach(var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                { 
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if(character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo
            {
                Id = member.Id,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach(var apply in this.Data.Applies)
            {
                if(apply.Result != (int)ApplyResult.None) continue;
                applies.Add(new NGuildApplyInfo
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }

        TGuildMember GetDBMember(int characterId)
        {
            foreach(var member in this.Data.Members)
            {
                if(member.CharacterId == characterId)
                    return member;
            }
            return null;
        }

        public void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout:
                    if (source.Title != 0 && target.Title == 0)
                    {
                        Leave(CharacterManager.Instance.GetCharacter(targetId));
                        break;
                    }
                    else if (source.Title == (int)GuildTitle.President)
                    {
                        Leave(CharacterManager.Instance.GetCharacter(targetId));
                        break;
                    }
                    break;

            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(Character from, NetMessageResponse response)
        {
            if(response.Guild == null)
            {
                response.Guild = new GuildResponse();
                response.Guild.Result = Result.Success;
                response.Guild.Guilds = this.GuildInfo(from);
            }
        }

        
    }
}
