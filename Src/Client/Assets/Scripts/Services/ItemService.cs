using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Services
{
    class ItemService : Singleton<ItemService>,IDisposable
    {
        public ItemService() 
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show(string.Format("购买结果：{0} \n{1}", message.Result, message.Errormsg), "购买成功");
        }
    }
}
