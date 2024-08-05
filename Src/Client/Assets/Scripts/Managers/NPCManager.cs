using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {

	class NPCManager : Singleton<NPCManager> 
	{
		public delegate bool NpcActionHandler(NPCDefine npc);

		Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();
		Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();	

		public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
		{
			if (!eventMap.ContainsKey(function))
			{
				eventMap[function] = action;
			}
            else
            {
				eventMap[function] += action;
            }
        }
		public NPCDefine GetNPCDefine(int npcID)
		{
			NPCDefine npc = null;
			DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
			return npc;
		}

		public bool Interactive(int npcId)
		{
			if (DataManager.Instance.NPCs.ContainsKey(npcId))
			{
				var npc = DataManager.Instance.NPCs[npcId];
				return Interactive(npc);
			}
			return false;
		}

		public bool Interactive(NPCDefine npc)
		{
			if(DoTaskInteractive(npc))
			{
				return true;
			}
			else if(npc.Type == NpcType.Functional)
			{
				return DoFunctionInteractive(npc);
			}
			return false;
		}

		private bool DoTaskInteractive(NPCDefine npc)
		{
			var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
			if(status == NpcQuestStatus.None)
				return false;

			return QuestManager.Instance.OpenNpcQuest(npc.ID);
		} 

		private bool DoFunctionInteractive(NPCDefine npc)
		{
			if(npc.Type != NpcType.Functional)
				return false;
			if(!eventMap.ContainsKey(npc.Function))
				return false;

			return eventMap[npc.Function](npc);
		}

		public void UpdateNpcPosition(int npcId,Vector3 pos)
		{
			this.npcPositions[npcId] = pos;
		}

        public Vector3 GetNPCPosition(int npcId)
        {
            return this.npcPositions[npcId];
        }
    }
}
