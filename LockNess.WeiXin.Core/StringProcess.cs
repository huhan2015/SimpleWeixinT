using LockNess.WeiXin.Core.Db;
using LockNess.WeiXin.Core.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockNess.WeiXin.Core
{

    public class StringProcess
    {
        private readonly IOptions<ConConfig> _configs;
        public StringProcess(IOptions<ConConfig> configs)
        {
            _configs = configs;
        }


        public string Process(string scanResult)
        {
            DbProcess dbProcess = new DbProcess(_configs.Value.oldcon,_configs.Value.newCon);
            var dic = dbProcess.Process(scanResult);
            var res = "";
            if (dic.Count > 0 && !String.IsNullOrEmpty(dic["Name"]))
            {
                res= JFPrintTemplate.GetDisplayString(dic["Barcode"], dic["chandi"], Convert.ToInt32(dic["CheckCount"]), dic["Name"], dic["Code"]);
            }
            else
            {
                res = "尊敬的用户,您好! 该产品不属于晚安生产的产品,请谨防假冒.(注:防伪验证支持湖南床垫2015年9月1日(佛山床垫10月1日丶晚安软床10月1日)/晚安家纺(2016年8月15日)之后生产贴标的产品！)";
            }
        
          
            return res;
        }
    }
}
