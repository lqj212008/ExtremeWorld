using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using log4net;

public class UnityLogger : MonoBehaviour 
{

    public static void Init() 
    {
        Application.logMessageReceived += onLogMessageReceived;
    }

    private static ILog log = LogManager.GetLogger("Unity");

    private static void onLogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
    {
        switch (type) 
        {
            case LogType.Error:
                log.ErrorFormat("{1}\r\n{0}",condition, stackTrace.Replace("\n","\r\n")); 
                break;
            case LogType.Assert:
                log.DebugFormat("{1}\r\n{0}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{1}\r\n{0}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{1}\r\n{0}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
    
   
}
