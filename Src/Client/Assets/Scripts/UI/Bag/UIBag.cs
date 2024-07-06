using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text momey;
    public Transform[] pages;
    public GameObject bagItem;
    

    List<Image> slots;

    void Start()
    {
        if(slots == null)
        {
            slots = new List<Image>();
            for(int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(this.InitBags());
    }

    IEnumerator InitBags()
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIconltem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon,item.Count.ToString());
            }
            this.momey.text = User.Instance.CurrentCharacter.Gold.ToString();
        }

        for(int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        
        yield return null;
    }

    void Clear()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void OnClickReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
        momey.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
   
}


