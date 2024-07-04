using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
	public class User : Singleton<User> {

		SkillBridge.Message.NUserInfo userInfo;

		public SkillBridge.Message.NUserInfo info
		{
			get { return userInfo; }
		}

		public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
		{
			this.userInfo = info;
		}

        public void AddGold(int gold)
        {
			this.CurrentCharacter.Gold += gold;
        }

        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }
		public MapDefine CurrentMapData { get; set; }
		public GameObject CurrentCharacterObject { get; set; }

	}
}

