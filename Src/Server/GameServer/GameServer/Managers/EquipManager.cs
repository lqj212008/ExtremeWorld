using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if(!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }
            if (DataManager.Instance.Items[itemId].LimitClass != character.Define.Class)
            {
                return Result.Failed;
            }
            UpDataEquip(character.Data.Equips, slot, itemId, isEquip);

            DBService.Instance.Save();
            return Result.Success;
        }

        unsafe void UpDataEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            fixed (byte* pt = equipData)
            {
                int* slotid = (int*)(pt + slot * sizeof(int));
                if(isEquip)
                {
                    *slotid = itemId;
                }
                else
                {
                    *slotid = 0;
                }
            }
        }
    }
}
