using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LockNess.WeiXin.Core;
using LockNess.WeiXin.Core.MessageHandler;
using LockNess.WeiXin.Core.Utility;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LockNess.WeiXin.Web.Controllers
{
    public class WeiXinController : Controller
    {
        private readonly WeixinConfig _weixinConfig;
        private ILog _log;
        private IMessageHandler _messageHandler;
        public WeiXinController(IOptions<WeixinConfig> options, IMessageHandler messageHandler)
        {
            _weixinConfig = options.Value;
            this._log = LogManager.GetLogger(Startup.repository.Name, this.GetType());
            _messageHandler = messageHandler;
        }
        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            var token = _weixinConfig.Token;//微信公众平台后台设置的Token
            if (string.IsNullOrEmpty(token)) return Content("请先设置Token！");
        
            if (!Weixinverification.Check(signature, timestamp, nonce, token))
            {
           
                    return Content("failed:" + signature + "," + Weixinverification.GetSignature(timestamp, nonce, _weixinConfig.Token) + "。" +
                        "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                
            }
            return Content(echostr); //返回随机字符串则表示验证通过
        }
        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(string signature, string timestamp, string nonce, string echostr)
        {
            var streamReader = new StreamReader(Request.Body);
            var msg = streamReader.ReadToEnd();
            var message = MessageAnalysis.Parse(msg);
            var response = _messageHandler.HandleMessage(message);
            return new ContentResult
            {
                Content = response,
                ContentType = "text/xml"
            };
        }
    }
}