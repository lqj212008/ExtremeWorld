﻿using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,
        Complete,
        Available,
        Incomplete,
    }
    class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();
        public UnityAction<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        void InitQuests()
        {
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAvailableQuests();
            
            foreach(var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        private void CheckAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;

                if (this.allQuests.ContainsKey(kv.Key))
                    continue;

                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if (preQuest.Info == null)
                            continue;
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;
                    }
                    else
                        continue;
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
            }

            if (quest.Info == null)
            {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }

        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcId,out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDialog(Quest quest)
        {
            if(quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            if(quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if(!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);     
            }
            return true;
        }

        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if(result == UIWindow.WindowResult.Yes)
            {
                if(dlg.quest.Info == null) 
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                else if(dlg.quest.Info.Status == QuestStatus.Complated)
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
            }
            else if(result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId))
            {
                this.allQuests[quest.QuestId].Info = quest;
                result = this.allQuests[quest.QuestId];
            }
            else
            {
                result = new Quest();
                this.allQuests[quest.QuestId] = result;
            }

            CheckAvailableQuests();

            foreach(var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC,kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC,kv.Value);
            }

            if(onQuestStatusChanged != null)
            {
                onQuestStatusChanged(result);
            }
            
            return result;
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
