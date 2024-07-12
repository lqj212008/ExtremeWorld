using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UINameBar : MonoBehaviour {

	public Text avaverName;
	public Camera cam;

	public Character character;

	// Use this for initialization
	void Start() {
		if (character != null)
		{

		}
	}

	// Update is called once per frame
	void Update()
	{
		this.UpdateInfo();
	}
	void UpdateInfo()
	{
        if (character != null)
        {
			string characterName = this.character.Name + " Lv."+ this.character.Info.Level;
			if(characterName != avaverName.text)
			{
				this.avaverName.text = characterName;
			}
        }


    }
}
