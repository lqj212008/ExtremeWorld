using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem {

    public Text nickname;
    public Text @class;
    public Text level;

    public NGuildApplyInfo info;

    public void SetItemInfo(NGuildApplyInfo item)
    {
        this.info = item;
        if(this.nickname != null) this.nickname.text = this.info.Name;
        if(this.@class != null) this.@class.text = this.info.Class.ToString();
        if (this.level != null) this.level.text = this.info.Level.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("要通过【{0}】的公会申请吗？", this.info.Name), "入会审批", MessageBoxType.Confirm, "同意加入", "取消").onYes = () => 
        {
            GuildService.Instance.SendGuildJoinApply(true,this.info);
        };
    }

    public void OnDecline()
    {
        MessageBox.Show(string.Format("要拒绝【{0}】的公会申请吗？", this.info.Name), "入会审批", MessageBoxType.Confirm, "拒绝加入", "取消").onYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.info);
        };
    }
}
