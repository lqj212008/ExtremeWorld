using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour 
{
	public Text guildName;
	public Text guildID;
	public Text leader;
	public Text notice;
	public Text memberNumber;

	private NGuildInfo info;

	public NGuildInfo Info
	{
		get { return info; }
		set { this.info = value; this.UpdateUI(); }
	}

	void UpdateUI()
	{
		if (this.info == null)
		{
			this.guildName.text = "无";
			this.guildID.text = "ID：0";
			this.leader.text = "会长：无";
			this.notice.text = "";
			this.memberNumber.text = string.Format("成员数量：0/{0}", GameDefine.GuildMaxMemberCount);
		}
		else
		{
			this.guildName.text = this.info.GuildName;
			this.guildID.text = string.Format("ID：{0}", this.info.Id);
			this.leader.text = string.Format("会长：{0}",this.info.leaderName);
            this.notice.text = this.info.Notice;
            this.memberNumber.text = string.Format("成员数量：{0}/{1}",this.info.memberCount, GameDefine.GuildMaxMemberCount);
        }
	}
}
