using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEditor.VersionControl;
using UnityEngine.SocialPlatforms;
using Services;

public class UICharacterSelect : MonoBehaviour {

	public GameObject panelCreate;
	public GameObject panelSelect;
	public GameObject btnCreateCancel;

	public InputField charName;
	CharacterClass charClass;

	public Transform uiCharList;
	public GameObject uiCharInfo;

	public List<GameObject> uiChars = new List<GameObject>();

	public Image[] titles;  //标题

	public Text descs;  //描述

	public Text[] names;  

	private int selectCharacterIdx = -1;

	public UICharacterView characterView;

	// Use this for initialization
	void Start() {
		DataManager.Instance.Load();
		InitCharacterSelect(true);
        for (int i = 0; i < 3; i++)       
            names[i].text = DataManager.Instance.Characters[i + 1].Name;
		characterView.CurractCharacter = 2;
		UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }
   

	// Update is called once per frame
	void Update() {

	}

	public void InitCharacterSelect(bool init)
	{
		panelCreate.SetActive(false);
		panelSelect.SetActive(true);

		if (init)
		{
			foreach (var old in uiChars)
			{
				Destroy(old);
			}
			uiChars.Clear();

			for(int i = 0; i<User.Instance.info.Player.Characters.Count; i++)
			{
				GameObject go = Instantiate(uiCharInfo, this.uiCharList);
				UICharInfo charInfo = go.GetComponent<UICharInfo>();
				charInfo.info = User.Instance.info.Player.Characters[i];

				Button button = go.GetComponent<Button>();
				int idx = i;
				button.onClick.AddListener(() =>
				{
					OnSelectCharacter(idx);
				});

				uiChars.Add(go);
				go.SetActive(true);
			}
		}
	}

	public void InitChatacterCreate()
	{
		panelCreate.SetActive(true);
		panelSelect.SetActive(false);
	}

	public void OnClickCreate()
	{
		if (string.IsNullOrEmpty(this.charName.text))
		{
			MessageBox.Show("请输入角色名称！");
			InitCharacterSelect(true);
			return;
		}

		UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
		InitCharacterSelect(true);
	}

	public void OnSelectClass(int charClass)
	{
		this.charClass = (CharacterClass)charClass;
		characterView.CurractCharacter = charClass -1;

		for(int i = 0; i < 3; i++)
		{
			titles[i].gameObject.SetActive(i == charClass -1);
			names[i].text = DataManager.Instance.Characters[i+1].Name;
		}
		descs.text = DataManager.Instance.Characters[charClass].Description;
	}

	void OnCharacterCreate(Result result, string message)
	{
		if (result == Result.Success)
		{
			InitCharacterSelect(true);
		}
		else
		{
			MessageBox.Show(message, "错误", MessageBoxType.Error);
		}
	}

	public void OnSelectCharacter(int idx)
	{
		this.selectCharacterIdx = idx;
		var cha = User.Instance.info.Player.Characters[idx];
		Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
		User.Instance.CurrentCharacter = cha;
		characterView.CurractCharacter = idx;

		for (int i = 0; i < User.Instance.info.Player.Characters.Count; i++)
		{ 
			UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
			ci.Selected = (i == idx);
		}
           
		
	}

	public void OnClickPlay()
	{
		if(selectCharacterIdx >= 0)
		{
			UserService.Instance.SendGameEnter(selectCharacterIdx);
		}
	}

}
