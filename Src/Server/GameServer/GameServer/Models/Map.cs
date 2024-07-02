using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> connection, Character character)
            {
                this.connection = connection;
                this.character = character;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int,MapCharacter> MapCharacters = new Dictionary<int,MapCharacter>();

        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {

        }

        ///<summary>
        ///角色进入地图
        ///</summary>
        ///<param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter:Map:{0} characterId:{1}",this.Define.ID,character.Info.Id);

            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);

            foreach(var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters[character.Info.Id] = new MapCharacter(conn,character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data,0,data.Length);
        }

        ///<summary>
        ///角色离开地图
        ///</summary>
        ///<param name="character"></param>
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}",this.Define.ID,cha.Id);
 
            
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection,cha);
            }

            this.MapCharacters.Remove(cha.Id);
        }

        void SendCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);
            connection.SendData(data, 0, data.Length);
        }

        void SendCharacterLeaveMap(NetConnection<NetSession> connection, Character character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            message.Response.mapCharacterLeave.characterId = character.Id;

            byte[] data = PackageHandler.PackMessage(message);
            connection.SendData(data, 0, data.Length);
        }

        internal void UpdateEntity(NEntitySync entity)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(kv.Value.character.entityId == entity.Id)
                {
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction = entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

    }
}
