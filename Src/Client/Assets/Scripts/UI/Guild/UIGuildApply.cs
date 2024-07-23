using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApply : UIWindow {

	public GameObject itemPrefab;
	public ListView listMain;
	public Transform listRoot;
	// Use this for initialization
	void Start () {
		GuildService.Instance.OnGuildUpdate += UpdateList;
		GuildService.Instance.SendGuildListRequest();
		this.UpdateList();
	}

	void OnDestroy()
	{
		GuildService.Instance.OnGuildUpdate -= UpdateList;
	}
	
	// Update is called once per frame
	void UpdateList () {
		ClearList();
		InitItems();
	}

	void ClearList()
	{
		this.listMain.RemoveAll();
	}

	void InitItems()
	{
		foreach(var item in GuildManager.Instance.guildInfo.Applies)
		{
			GameObject go = Instantiate(itemPrefab,this.listRoot);
			UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
			ui.SetItemInfo(item);
			this.listMain.AddItem(ui);
		}
	}
}
