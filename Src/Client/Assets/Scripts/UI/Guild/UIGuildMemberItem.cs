using Common.Utils;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{

	public Text nickName;
	public Text @class;
	public Text level;
	public Text title;
	public Text joinTime;
	public Text status;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;

	public override void OnSelected(bool selected)
	{
		this.background.overrideSprite = selected ? selectedBg : normalBg;
	}

	public NGuildMemberInfo info;

	public void SetGuildMemberInfo(NGuildMemberInfo item)
	{
		this.info = item;
		if (this.nickName != null) { this.nickName.text = this.info.Info.Name; }
		if (this.@class != null) { this.@class.text = this.info.Info.Class.ToString(); }
		if (this.level != null) { this.level.text = this.info.Info.Level.ToString(); }
		if (this.title != null) { this.title.text = this.info.Title.ToString(); }
		if (this.joinTime != null) { this.joinTime.text = this.info.joinTime.ToString(); }
		if (this.status != null) { this.status.text = this.info.Status == 1 ? "在线" : TimeUtil.GetTime(this.info.lastTime).ToString(); }
	}
}
