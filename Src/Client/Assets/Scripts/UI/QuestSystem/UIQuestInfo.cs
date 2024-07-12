using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

	public Text title;
	public Text[] targets;
	public Text description;
	public List<Transform> rewardItems;
	public GameObject ItemIcon;
	public Text rewardMoney;
	public Text rewardExp;

	public void SetQuestInfo(Quest quest)
	{
		this.title.text = string.Format("[{0}]{1}",quest.Define.Type, quest.Define.Name);
		if(quest.Info == null)
		{
			this.description.text = quest.Define.Dialog;
		}
		else
		{
			if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
			{
				this.description.text = quest.Define.DialogFinish;
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

		foreach(var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
		}
	}

	public void OnClickAbandon()
	{

	}
}

