using System;
using System.Collections.Generic;
using System.Text;

namespace LockNess.WeiXin.Core
{
    public interface IWeixinExecutor
    {
        /// <summary>
        /// 接受消息后返回XML
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string Execute(WeixinMessage message);

    }
}
