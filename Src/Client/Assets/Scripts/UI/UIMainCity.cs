using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

	public Text avatarName;
	public Text avatarLevel;
	// Use this for initialization
	void Start () {
		this.UpdateAvatar();
	}

	void UpdateAvatar()
	{
		this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
		this.avatarName.text = string.Format("{0}", User.Instance.CurrentCharacter.Level);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBecameInvisible()
    {
		SceneManager.Instance.LoadScene("CharSelect");
		Services.UserService.Instance.SendGameLeave();
    }
}
