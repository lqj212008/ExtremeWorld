using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService() 
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse response)
        {
            Debug.LogFormat("OnQuestAccept:{0},ERR:{1}", response.Result, response.Errormsg);
            if(response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(response.Quest);
            }
            else
            {
                MessageBox.Show("任务接取失败","错误",MessageBoxType.Error);
            }
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse response)
        {
            Debug.LogFormat("OnQuestSubmit:{0},ERR:{1}",response.Result,response.Errormsg);
            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmited(response.Quest);
            }
            else
            {
                MessageBox.Show("任务接取失败", "错误", MessageBoxType.Error);
            }
        }

        

        
    }
}
