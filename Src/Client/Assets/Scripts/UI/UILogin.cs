using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour {

	public InputField user;
	public InputField password;

	public Button buttonLogin;
	public Button buttonRegister;
	// Use this for initialization
	void Start () {
		UserService.Instance.OnLogin = OnLogin;
	}

    void OnLogin(SkillBridge.Message.Result result, string msg)
	{
		if(result == Result.Success)
		{
			MessageBox.Show("登录成功，准备角色选择" + msg, "提示", MessageBoxType.Information);
			SceneManager.Instance.LoadScene("CharSelect");
			SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
		}
		else
		{
			MessageBox.Show(msg,"错误", MessageBoxType.Error);
		}
	}

	public void OnClickLogin() 
	{
		if (string.IsNullOrEmpty(user.text))
		{
			MessageBox.Show("请输入用户名");
			return;
		}
		if (string.IsNullOrEmpty(password.text))
		{
			MessageBox.Show("请输入密码");
			return;
		}
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		UserService.Instance.SendLogin(this.user.text,this.password.text);
	}



}
