using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using GameServer.Managers;

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

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager SpawnManager = new SpawnManager();
        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
        }

        ///<summary>
        ///角色进入地图
        ///</summary>
        ///<param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter:Map:{0} characterId:{1}",this.Define.ID,character.Info.Id);

            character.Info.mapId = this.ID;
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            foreach(var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if(kv.Value.character != character)
                    this.AddCharacterEnterMap(kv.Value.connection,character.Info);
            }

            foreach(var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            conn.SendResponse();
        }

        ///<summary>
        ///角色离开地图
        ///</summary>
        ///<param name="Character"></param>
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}",this.Define.ID,cha.Id);
 
            
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection,cha);
            }

            this.MapCharacters.Remove(cha.Id);
        }

        void AddCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo character)
        {
            if(connection.Session.Response.mapCharacterEnter == null)
            {
                connection.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                connection.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }
            connection.Session.Response.mapCharacterEnter.Characters.Add(character);
            connection.SendResponse();
        }

        void SendCharacterLeaveMap(NetConnection<NetSession> connection, Character character)
        {
            connection.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            connection.Session.Response.mapCharacterLeave.characterId = character.Id;
            connection.SendResponse();
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

        ///<summary>
        ///怪物进入地图
        /// </summary>
        /// <param name="Monster"></param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0}  MonsterID:{1}",this.Define.ID,monster.Id);
            foreach (var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection,monster.Info);
            }
        }

    }
}
