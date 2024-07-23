using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{

    public NGuildInfo info;
    public Text guildId;
    public Text nickName;
    public Text leaderName;
    public Text memberCount;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void OnSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public void SetGuildInfo(NGuildInfo item)
    {
        this.info = item;
        if (this.guildId != null) { this.guildId.text = info.Id.ToString(); }
        if (this.nickName != null) { this.nickName.text = info.GuildName; }
        if (this.memberCount != null) { this.memberCount.text = string.Format("{0}/50", info.memberCount); }
        if (this.leaderName != null) { this.leaderName.text = info.leaderName; }
    }
}
