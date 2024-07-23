using Managers;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow {

	public GameObject itemPrefab;
	public ListView listMain;
	public UIGuildInfo uiInfo;
	public UIGuildMemberItem selectedItem;

	public GameObject panelAdmin;
	public GameObject panelLeader;

	// Use this for initialization
	void Start () 
	{
		GuildService.Instance.OnGuildUpdate += UpdateUI;
		this.listMain.OnItemSelected += this.OnGuildMemberSelected;
		this.UpdateUI();
	}
	
	void OnDestroy()
	{
		GuildService.Instance.OnGuildUpdate -= UpdateUI;
	}
	void UpdateUI () 
	{
		this.uiInfo.Info = GuildManager.Instance.guildInfo;

		this.ClearList();
		this.InitItem();

		this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
		this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
	}

	public void OnGuildMemberSelected(ListView.ListViewItem item)
	{
		this.selectedItem = item as UIGuildMemberItem;
	}

	void ClearList()
	{
		listMain.RemoveAll();
	}

	void InitItem()
	{
		foreach(var item in GuildManager.Instance.guildInfo.Members)
		{
			GameObject go = Instantiate(itemPrefab, this.listMain.transform);
			UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
			ui.SetGuildMemberInfo(item);
			this.listMain.AddItem(ui);
		}
	}

	public void OnClickAppliesList()
	{
		UIManager.Instance.Show<UIGuildApply>();
	}

	public void OnClickLeave()
	{
        MessageBox.Show(string.Format("确定要退出【{0}】公会吗？",GuildManager.Instance.guildInfo.GuildName), "退出公会", MessageBoxType.Confirm, "确定", "取消").onYes = () =>
        {
			GuildService.Instance.SendGuildLeaveRequest();
        };
	}

	public void OnClickChar()
	{

	}

	public void OnClickKickout()
	{
		if(selectedItem == null)
		{
			MessageBox.Show("请选择要踢出的成员");
			return;
		}
		MessageBox.Show(string.Format("要将【{0}】踢出公会吗？", this.selectedItem.info.Info.Name), "踢出公会", MessageBoxType.Confirm, "确定", "取消").onYes = () =>
		{
			GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.info.characterId);
		};
	}

	public void OnClickPromote()
	{
		if(selectedItem == null)
		{
			MessageBox.Show("请选择要晋升的成员");
			return;
		}
		if(selectedItem.info.Title != GuildTitle.None)
		{
			MessageBox.Show("对方已经是副会长了！");
			return;
		}
        MessageBox.Show(string.Format("要将【{0}】晋升为副会长吗？", this.selectedItem.info.Info.Name), "晋升", MessageBoxType.Confirm, "确定", "取消").onYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.info.characterId);
        };
    }

	public void OnClickDepose()
	{
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方目前没有任何职务！");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.President)
        {
            MessageBox.Show("会长不能被罢免！");
            return;
        }
        MessageBox.Show(string.Format("要将【{0}】的职务罢免吗？", this.selectedItem.info.Info.Name), "罢免职务", MessageBoxType.Confirm, "确定", "取消").onYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.info.characterId);
        };
    }

	public void OnClickTransfer()
	{
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要吧会长转让给的成员");
            return;
        }
        MessageBox.Show(string.Format("确定要将会长转让给【{0}】吗？", this.selectedItem.info.Info.Name), "转让会长", MessageBoxType.Confirm, "确定", "取消").onYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.info.characterId);
        };
    }

	public void OnClickSetNotice()
	{

	}
}
