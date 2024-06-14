﻿using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>,IDisposable
    {
        public Dictionary<int,Character> Characters = new Dictionary<int,Character>();

        public UnityAction<Character> OnCharacterEnter;

        public CharacterManager() 
        { 

        }

        public void Dispose()
        {
            
        }

        public void Init()
        {

        }

        public void Cleat()
        {
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter: {0}: {1} Map: {2} Entity: {3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;

            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }

        }

        public void RemoveCharacter(int chaId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", chaId);
            this.Characters.Remove(chaId);
        }
    }
}