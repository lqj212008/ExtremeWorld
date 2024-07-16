using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    internal interface IPostResponse
    {
        void PostProcess(NetMessageResponse response);
    }
}
