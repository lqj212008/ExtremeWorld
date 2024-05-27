using UnityEngine;

public class MessageBox 
{
    static Object cacheObject = null;

    public static UIMessageBox Show( string message, string title = "", MessageBoxType type = MessageBoxType.Information, string btnOk = "", string btnCancel = "")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIMessageBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOk, btnCancel);
        return msgbox;
    }

    
}
public enum MessageBoxType
{

    Information = 1,

    Confirm = 2,

    Error = 3
}