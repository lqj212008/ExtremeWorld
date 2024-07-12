using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{

}
public class ListView : MonoBehaviour 
{
	public UnityAction<ListViewItem> OnItemSelected;
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnSelected(selected);
            }
        }
        public virtual void OnSelected(bool selected)
        {

        }
        public ListView owner;
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!this.selected)
            {
                this.Selected = true;
            }
            if(owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }

    }

    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get { return this.selectedItem; }
        private set
        {
            if (selectedItem != null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (OnItemSelected != null)
                OnItemSelected.Invoke((ListViewItem)value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach(var item in this.items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void ClearSelected()
    {
        foreach (var item in this.items)
        {
            item.Selected = false;
        }
    }
}
