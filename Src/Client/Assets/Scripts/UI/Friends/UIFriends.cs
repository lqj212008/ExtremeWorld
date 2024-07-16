using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow {

	public GameObject itemPrefab;
	public ListView listMain;
	public Transform listRoot;
	public UIFriendItem selectedItem;
	// Use this for initialization
	void Start () {
		FriendService.Instance.OnFriendUpdate = RefreshUI;
		this.listMain.OnItemSelected += this.OnFriendSelected;
		RefreshUI();
	}

	public void OnFriendSelected(ListView.ListViewItem item)
	{
		this.selectedItem = item as UIFriendItem;
	}

	public void OnClickFriendAdd()
	{
		InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
	}
	
	private bool OnFriendAddSubmit(string input, out string tips)
	{
		tips = "";
		int friendId = 0;
		string friendName = "";
		if (!int.TryParse(input, out friendId)) friendName = input;
		if(friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
		{
			tips = "开玩笑嘛？不能添加自己哦";
			return false;
		}
		FriendService.Instance.SendFriendAddRequest(friendId, friendName);
		return true;
	}

	public void OnClickFriendChat()
	{
		MessageBox.Show("暂未开放");
	}

	public void OnClickFriendRemove()
	{
		if (selectedItem == null) 
		{
			MessageBox.Show("请选择要删除的好友");
			return;
		}
		MessageBox.Show(string.Format("确定要删除好友【{0}】吗？", selectedItem.info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").onYes = () =>{
			FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.info.Id, this.selectedItem.info.friendInfo.Id);
		};
	}

	void RefreshUI()
	{
		ClearFriendList();
		InitFriendItems();
	}

	void InitFriendItems()
	{
		foreach (var item in FriendManager.Instance.allFriends)
		{
			GameObject go = Instantiate(itemPrefab,this.listMain.transform);
			UIFriendItem ui = go.GetComponent<UIFriendItem>();
			ui.SetFriendInfo(item);
			this.listMain.AddItem(ui);
		}
	}

	void ClearFriendList()
	{
		this.listMain.RemoveAll();
	}
}
