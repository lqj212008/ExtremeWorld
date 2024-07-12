using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour {

	public TabButton[] tabButtons;
	public GameObject[] tabPages;
	public int index = -1;

    public Action<int> OnTabSelect { get; internal set; }


    // Use this for initialization
    IEnumerator Start () {
		for(int i = 0; i < tabButtons.Length; i++)
		{
			tabButtons[i].tabView = this;
			tabButtons[i].tabIndex = i;
		}
		yield return new WaitForEndOfFrame();
		SelectTab(0);
	}
	
	
	public void SelectTab(int index)
    {
        if(this.index != index)
		{
			for(int i = 0;i < tabButtons.Length;i++)
			{
				tabButtons[i].Select(i == index);
				if(i<tabPages.Length)
					tabPages[i].SetActive(i == index);
			}
			if(OnTabSelect != null)
				OnTabSelect(index);
			
		}
    }
}
