﻿using System.Collections;
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

		public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }
	}
}
