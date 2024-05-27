using Services;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

/**
 * 注册面板逻辑，进行数据校验
 */
public class UIRegister : MonoBehaviour {
    
    public InputField email;
    public InputField password;
    public InputField passwordConfirm;
    
    public Button buttonRegister;

    void Start()
    {
        UserService.Instance.OnRegister = OnRegister;
    }

    void OnRegister(SkillBridge.Message.Result result, string msg)
    {
        MessageBox.Show(string.Format("结果: {0}  Msg: {1}",result,msg));
    }

    public void OnClickRegister()
    {
        if(string.IsNullOrEmpty(this.email.text)) 
        {
            MessageBox.Show("请输入邮箱！");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码！");
            return;
        }
        if (string.IsNullOrEmpty(this.passwordConfirm.text))
        {
            MessageBox.Show("请再次输入密码！");
            return;
        }
        if (this.password.text != this.passwordConfirm.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }
        UserService.Instance.SendRegister(this.email.text, this.password.text);
    }
}
