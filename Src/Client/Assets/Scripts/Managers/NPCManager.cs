using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {

	class NPCManager : Singleton<NPCManager> 
	{
		public delegate bool NpcActionHandler(NPCDefine npc);

		Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

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
			if(npc.Type == NpcType.Task)
			{
				return DoTaskInteractive(npc);
			}
			else if(npc.Type == NpcType.Functional)
			{
				return DoFunctionInteractive(npc);
			}
			return false;
		}

		private bool DoTaskInteractive(NPCDefine npc)
		{
			MessageBox.Show("点击了NPC：" + npc.Name, "NPC对话");
			return true;
		}

		private bool DoFunctionInteractive(NPCDefine npc)
		{
			if(npc.Type != NpcType.Functional)
				return false;
			if(!eventMap.ContainsKey(npc.Function))
				return false;

			return eventMap[npc.Function](npc);
		}

    }
}
