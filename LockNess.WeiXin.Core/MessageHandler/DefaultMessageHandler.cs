using LockNess.WeiXin.Core.Db;
using LockNess.WeiXin.Core.Utility;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockNess.WeiXin.Core.MessageHandler
{
    public class DefaultMessageHandler : IMessageHandler
    {
        private ILog _log;
        private StringProcess _dbProcess;
        public DefaultMessageHandler(StringProcess dbProcess)
        {
            this._log = LogManager.GetLogger(this.GetType());
            _dbProcess = dbProcess;
        }

        public string HandleMessage(WeixinMessage message)
        {
            var result = "";
            var openId = message.Body.FromUserName.Value;
            var myUserName = message.Body.ToUserName.Value;
            var domain = "";
            //这里需要调用TokenHelper获取Token的，省略了。
            switch (message.Type)
            {
                case WeixinMessageType.Text://文字消息
                    string userMessage = message.Body.Content.Value;
                    result = ReplayMessageApi.RepayText(openId, myUserName, " 晚安家纺欢迎您 ");
                    break;
                case WeixinMessageType.Image://图片消息
                    string imageUrl = message.Body.PicUrl.Value;//图片地址
                    string mediaId = message.Body.MediaId.Value;//mediaId
                    result = ReplayMessageApi.ReplayImage(openId, myUserName, mediaId);
                    break;

                case WeixinMessageType.Video://视频消息
                    #region 视频消息
                    {
                        var media_id = message.Body.MediaId.Value.ToString();
                        var thumb_media_id = message.Body.ThumbMediaId.Value.ToString();
                        var msgId = message.Body.MsgId.Value.ToString();
                        //TODO
                        result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("视频消息:openid:{0},media_id:{1},thumb_media_id:{2},msgId:{3}", openId, media_id, thumb_media_id, msgId));
                    }
                    #endregion
                    break;
                case WeixinMessageType.Voice://语音消息
                    #region 语音消息
                    {
                        var media_id = message.Body.MediaId.Value.ToString();
                        var format = message.Body.Format.Value.ToString();
                        var msgId = message.Body.MsgId.Value.ToString();
                        //TODO
                        result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("语音消息:openid:{0},media_id:{1},format:{2},msgId:{3}", openId, media_id, format, msgId));
                    }
                    #endregion
                    break;
              
                case WeixinMessageType.Link://链接消息
                    #region 链接消息
                    {
                        var title = message.Body.Title.Value.ToString();
                        var description = message.Body.Description.Value.ToString();
                        var url = message.Body.Url.Value.ToString();
                        var msgId = message.Body.MsgId.Value.ToString();
                        //TODO
                        result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("openid:{0},title:{1},description:{2},url:{3},msgId:{4}", openId, title, description, url, msgId));
                    }
                    #endregion
                    break;
                case WeixinMessageType.Event:
                    string eventType = message.Body.Event.Value.ToLower();
                    string eventKey = string.Empty;
                    try
                    {
                        eventKey = message.Body.EventKey.Value;
                    }
                    catch { }
                    switch (eventType)
                    {
                        case "subscribe"://用户未关注时，进行关注后的事件推送
                          
                                result = ReplayMessageApi.RepayText(openId, myUserName,
                                "欢迎订阅 晚安家纺公众号");

                            break;
                        case "unsubscribe"://取消关注
                            #region 取消关注
                            result = ReplayMessageApi.RepayText(openId, myUserName, "欢迎再来");
                            #endregion
                            break;
                        case "scan":// 用户已关注时的事件推送
                            #region 已关注扫码事件
                            if (!string.IsNullOrEmpty(eventKey))
                            {
                                var qrscene = eventKey.Replace("qrscene_", "");//此为场景二维码的场景值
                                result = ReplayMessageApi.RepayNews(openId, myUserName,
                                    new WeixinNews
                                    {
                                        title = "欢迎使用，场景值：" + qrscene,
                                        description = "欢迎使用，场景值：" + qrscene,
                                        picurl = string.Format("{0}/ad.jpg", domain),
                                        url = domain
                                    });
                            }
                            else
                            {
                                result = ReplayMessageApi.RepayNews(openId, myUserName,
                                 new WeixinNews
                                 {
                                     title = "欢迎使用",
                                     description = "欢迎订阅，点击此消息查看在线demo",
                                     picurl = string.Format("{0}/ad.jpg", domain),
                                     url = domain
                                 });
                            }
                            #endregion
                            break;
                        case "masssendjobfinish"://事件推送群发结果,
                            #region 事件推送群发结果
                            {
                                var msgId = message.Body.MsgID.Value;
                                var msgStatus = message.Body.Status.Value;//“send success”或“send fail”或“err(num)” 
                                //send success时，也有可能因用户拒收公众号的消息、系统错误等原因造成少量用户接收失败。
                                //err(num)是审核失败的具体原因，可能的情况如下：err(10001)涉嫌广告, err(20001)涉嫌政治, err(20004)涉嫌社会, err(20002)涉嫌色情, err(20006)涉嫌违法犯罪,
                                //err(20008)涉嫌欺诈, err(20013)涉嫌版权, err(22000)涉嫌互推(互相宣传), err(21000)涉嫌其他
                                var totalCount = message.Body.TotalCount.Value;//group_id下粉丝数；或者openid_list中的粉丝数
                                var filterCount = message.Body.FilterCount.Value;//过滤（过滤是指特定地区、性别的过滤、用户设置拒收的过滤，用户接收已超4条的过滤）后，准备发送的粉丝数，原则上，FilterCount = SentCount + ErrorCount
                                var sentCount = message.Body.SentCount.Value;//发送成功的粉丝数
                                var errorCount = message.Body.FilterCount.Value;//发送失败的粉丝数
                                                                                //TODO:开发者自己的处理逻辑,这里用log4net记录日志
                                _log.Info(string.Format("mass send job finishe,msgId:{0},msgStatus:{1},totalCount:{2},filterCount:{3},sentCount:{4},errorCount:{5}", msgId, msgStatus, totalCount, filterCount, sentCount, errorCount));
                            }
                            #endregion
                            break;
                        case "templatesendjobfinish"://模版消息结果,
                            #region 模版消息结果
                            {
                                var msgId = message.Body.MsgID.Value;
                                var msgStatus = message.Body.Status.Value;//发送状态为成功: success; 用户拒绝接收:failed:user block; 发送状态为发送失败（非用户拒绝）:failed: system failed
                                                                          //TODO:开发者自己的处理逻辑,这里用log4net记录日志
                                _log.Info(string.Format("template send job finish,msgId:{0},msgStatus:{1}", msgId, msgStatus));
                            }
                            #endregion
                            break;
                        case "location"://上报地理位置事件
                            #region 上报地理位置事件
                            var lat = message.Body.Latitude.Value.ToString();
                            var lng = message.Body.Longitude.Value.ToString();
                            var pcn = message.Body.Precision.Value.ToString();
                            //TODO:在此处将经纬度记录在数据库,这里用log4net记录日志
                            _log.Info(string.Format("openid:{0} ,location,lat:{1},lng:{2},pcn:{3}", openId, lat, lng, pcn));
                            #endregion
                            break;
                        case "image"://图片消息
                            #region 图片消息
                            var userImage = message.Body.PicUrl.Value;//用户语音消息文字
                            result = ReplayMessageApi.RepayNews(openId, myUserName, new WeixinNews
                            {
                                title = "您刚才发送了图片消息",
                                picurl = string.Format("{0}/Images/ad.jpg", domain),
                                description = "点击查看图片",
                                url = userImage
                            });
                            #endregion
                            break;
                        case "click"://自定义菜单事件
                            #region 自定义菜单事件
                            {
                                switch (eventKey)
                                {
                                    case "myaccount"://CLICK类型事件举例
                                        #region 我的账户
                                        result = ReplayMessageApi.RepayNews(openId, myUserName, new List<WeixinNews>()
                                    {
                                        new WeixinNews{
                                            title="我的帐户",
                                            url=string.Format("{0}/user?openId={1}",domain,openId),
                                            description="点击查看帐户详情",
                                            picurl=string.Format("{0}/Images/ad.jpg",domain)
                                        },
                                    });
                                        #endregion
                                        break;
                                    case "www.weixinsdk.net"://VIEW类型事件举例，注意：点击菜单弹出子菜单，不会产生上报。
                                        //TODO:后台处理逻辑
                                        break;
                                    default:
                                        result = ReplayMessageApi.RepayText(openId, myUserName, "没有响应菜单事件");
                                        break;
                                }
                            }
                            #endregion
                            break;
                        case "view"://点击菜单跳转链接时的事件推送
                            #region 点击菜单跳转链接时的事件推送
                            result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("您将跳转至：{0}", eventKey));
                            #endregion
                            break;
                        case "scancode_push"://扫码推事件的事件推送
                            {
                                var scanType = message.Body.ScanCodeInfo.ScanType.Value;//扫描类型，一般是qrcode
                                var scanResult = message.Body.ScanCodeInfo.ScanResult.Value;//扫描结果，即二维码对应的字符串信息
                                result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("您扫描了二维码,scanType：{0},scanResult:{1},EventKey:{2}", scanType, scanResult, eventKey));
                            }
                            break;
                        case "scancode_waitmsg"://扫码推事件且弹出“消息接收中”提示框的事件推送
                            {

                                if (!string.IsNullOrEmpty(eventKey) && eventKey == "rselfmenu_0_1")
                                {
                                    try
                                    {
                                        var ms = _dbProcess.Process(message.Body.ScanCodeInfo.ScanResult.Value);
                                        result = ReplayMessageApi.RepayText(openId, myUserName, ms);
                                    }
                                    catch (Exception ex)
                                    {
                                        result = ReplayMessageApi.RepayText(openId, myUserName, ex.Message);
                                    }
                              
                                }
                                //else
                                //{
                                //    var scanType = message.Body.ScanCodeInfo.ScanType.Value;//扫描类型，一般是qrcode
                                //    var scanResult = message.Body.ScanCodeInfo.ScanResult.Value;//扫描结果，即二维码对应的字符串信息
                                //    result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("您扫描了二维码,scanType：{0},scanResult:{1},EventKey:{2}", scanType, scanResult, eventKey));
                                //}
                            }
                            break;
                        case "pic_sysphoto"://弹出系统拍照发图的事件推送
                            {
                                var count = message.Body.SendPicsInfo.Count;//发送的图片数量
                                var picList = message.Body.PicList;//发送的图片信息
                                result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("弹出系统拍照发图,count：{0},EventKey:{1}", count, eventKey));
                            }
                            break;
                        case "pic_photo_or_album"://弹出拍照或者相册发图的事件推送
                            {
                                var count = message.Body.SendPicsInfo.Count.Value;//发送的图片数量
                                var picList = message.Body.PicList.Value;//发送的图片信息
                                result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("弹出拍照或者相册发图,count：{0},EventKey:{1}", count, eventKey));
                            }
                            break;
                    }
                    break;
                default:
                    result = ReplayMessageApi.RepayText(openId, myUserName, string.Format("未处理消息类型:{0}", message.Type));
                    break;
            }
            return result;
        }
    }
}
