using System;
using System.Collections.Generic;
using System.Text;

namespace LockNess.WeiXin.Core.MessageHandler
{
    public interface IMessageHandler
    {
        string HandleMessage(WeixinMessage weixinMessage);
    }

}
