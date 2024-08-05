using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

	public Text title;
	public Text[] targets;
	public Text description;
	public Text overview;
	public List<Transform> rewardItems;
	public GameObject ItemIcon;
	public Text rewardMoney;
	public Text rewardExp;
    public Button navButton;
    private int npc = 0;

	public void SetQuestInfo(Quest quest)
	{
		this.title.text = string.Format("[{0}]{1}",quest.Define.Type, quest.Define.Name);
	
		if(this.overview != null) this.overview.text = quest.Define.Overview;
        if (this.description != null)
        {
            if (quest.Info == null)
            {
                this.description.text = quest.Define.Dialog;
            }
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                {
                    this.description.text = quest.Define.DialogFinish;
                }
            }
        }

		if(quest.Define.RewardItem1 != 0)
		{
			var item = DataManager.Instance.Items[quest.Define.RewardItem1];
			GameObject go = Instantiate(ItemIcon, rewardItems[0]);
            UIconltem ui = go.GetComponent<UIconltem>();
			ui.SetMainIcon(item.Icon, quest.Define.RewardItem1Count.ToString());
        }
        if (quest.Define.RewardItem2 != 0)
        {
            var item = DataManager.Instance.Items[quest.Define.RewardItem2];
            GameObject go = Instantiate(ItemIcon, rewardItems[1]);
            UIconltem ui = go.GetComponent<UIconltem>();
            ui.SetMainIcon(item.Icon, quest.Define.RewardItem2Count.ToString());
        }
        if (quest.Define.RewardItem3 != 0)
        {
            var item = DataManager.Instance.Items[quest.Define.RewardItem3];
            GameObject go = Instantiate(ItemIcon, rewardItems[2]);
            UIconltem ui = go.GetComponent<UIconltem>();
            ui.SetMainIcon(item.Icon, quest.Define.RewardItem3Count.ToString());
        }

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
		this.rewardExp.text = quest.Define.RewardExp.ToString();

        if(quest.Info == null)
        {
            this.npc = quest.Define.AcceptNPC;
        }
        else if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
        {
            this.npc = quest.Define.SubmitNPC;
        }
        if(navButton)
        this.navButton.gameObject.SetActive(this.npc > 0);

		foreach(var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
		}
	}

	public void OnClickAbandon()
	{

	}

    public void OnClickNav()
    {
        Vector3 pos = NPCManager.Instance.GetNPCPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }
}

