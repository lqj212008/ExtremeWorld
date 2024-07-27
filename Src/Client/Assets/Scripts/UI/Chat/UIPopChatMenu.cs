using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopChatMenu : UIWindow,IDeselectHandler 
{
	public int targetId;
	public string targetName;

    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
		if(ed.hovered.Contains(this.gameObject))
			return;
		this.Close(WindowResult.None);
    }

	public void OnEnable()
	{
		this.GetComponent<Selectable>().Select();
		this.Root.transform.position = Input.mousePosition+new Vector3(0,2,0);
	}

	public void OnAddFriend()
	{
		this.Close(WindowResult.No);
	}

	public void OnChat()
	{
        this.Close(WindowResult.No);
    }

    public void OnInviteTeam()
	{
        this.Close(WindowResult.No);
    }
}
