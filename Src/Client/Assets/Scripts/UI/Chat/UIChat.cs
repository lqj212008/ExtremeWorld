using Candlelight.UI;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

	public HyperText teatArea;

	public TabView cannelTab;

	public InputField chatText;
	public Text chatTarget;
	public Dropdown channelSelect;

	// Use this for initialization
	void Start () {
		this.cannelTab.OnTabSelect += OnDisplayChannelSelected;
		ChatManager.Instance.OnChat += RefreshUI;

	}

	void OnDestroy()
	{
		ChatManager.Instance.OnChat -= RefreshUI;
	}
	
	// Update is called once per frame
	void Update () {
		InputManager.Instance.IsInputMode = chatText.isFocused;
	}

	void OnDisplayChannelSelected(int idx) 
	{
		ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
		RefreshUI();
	}

	public void RefreshUI()
	{
		this.teatArea.text = ChatManager.Instance.GetCurrentMessages();
		this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
		if(ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
		{
			this.chatTarget.gameObject.SetActive(true);
			if(ChatManager.Instance.PrivateID != 0)
			{
				this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
			}else
			{
				this.chatTarget.text = "<无>:";
			}
		}
		else
		{
			this.chatTarget.gameObject.SetActive(false);
		}
	}

	public void OnClickChatLink(HyperText text,HyperText.LinkInfo link)
	{
		if (string.IsNullOrEmpty(link.Name))
			return;
        //定义：角色的超链接为<a name = "c:角色ID:角色名字" class = "player">玩家姓名</a>
        //定义：道具的超链接为<a name = "i:道具ID" class = "item">道具名称</a>
        if (link.Name.StartsWith("c:"))
		{
			string[] strs = link.Name.Split(":".ToCharArray());
			UIPopChatMenu menu = UIManager.Instance.Show<UIPopChatMenu>();
			menu.targetId = int.Parse(strs[1]);
			menu.targetName = strs[2];
		}
	}

	public void OnClickSend()
	{
		OnEndInput(this.chatText.text);
	}

	public void OnEndInput(string text)
	{
		if(!string.IsNullOrEmpty(text.Trim()))
			this.SendChat(text);

		this.chatText.text = "";
	}

	void SendChat(string content)
	{
		ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
	}

	public void OnSendChannelChanged(int idx)
	{
		if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
			return;

		if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
		{
			this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
		}
		else
		{
			this.RefreshUI();
		}
	}
}
