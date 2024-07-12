using Managers;
using Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour,IPointerClickHandler
{
    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text limitCategory;

    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectBg : normalBg;
        }
    }
    public int index { get;set; }
    private UICharEquip owner;
    private Item item;
    bool isEquiped = false;

    public void SetEquipItem(int idx, Item item, UICharEquip owner, bool equiped)
    {
        this.owner = owner;
        this.index = idx;
        this.item = item;
        this.isEquiped = equiped;

        if (this.title != null) { this.title.text = this.item.Define.Name; }
        if (this.level != null) { this.level.text = this.item.Define.Level.ToString(); }
        if (this.limitClass != null) { this.limitClass.text = this.item.Define.LimitClass.ToString(); }
        if (this.limitCategory != null) { this.limitCategory.text = this.item.Define.Category; }
        if (this.icon != null) { this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon); }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
            {
                this.Selected = true;
            }
        }
    }

    void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗？", this.item.Define.Name), "确定", MessageBoxType.Confirm);
        msg.onYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换装备[{0}]吗？", oldEquip.Define.Name), "确定", MessageBoxType.Confirm);
                newmsg.onYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };
            }
            else
            {
                this.owner.DoEquip(this.item);
            }
        };
    }

    void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]吗？", this.item.Define.Name), "确定", MessageBoxType.Confirm);
        msg.onYes = () =>
        {
            this.owner.UnEquip(this.item);
        };
    }
}

