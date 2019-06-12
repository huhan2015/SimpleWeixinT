using LockNess.WeiXin.Core.DataProcess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockNess.WeiXin.Core
{
    public class MessageAnalysis
    {
        public static WeixinMessage Parse(string message)
        {
            var msg = new WeixinMessage();
            msg.Body = new DynamicXml(message);
            string msgType = msg.Body.MsgType.Value;
            switch (msgType)
            {
                case "text":
                    msg.Type = WeixinMessageType.Text;
                    break;
                case "image":
                    msg.Type = WeixinMessageType.Image;
                    break;
                case "voice":
                    msg.Type = WeixinMessageType.Voice;
                    break;
                case "video":
                    msg.Type = WeixinMessageType.Video;
                    break;
                case "location":
                    msg.Type = WeixinMessageType.Location;
                    break;
                case "link":
                    msg.Type = WeixinMessageType.Link;
                    break;
                case "event":
                    msg.Type = WeixinMessageType.Event;
                    break;
                default: throw new Exception("does not support this message type:" + msgType);
            }
            return msg;
        }
    }
}
