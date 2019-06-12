using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LockNess.WeiXin.Core.Db
{
    public interface IPrintTemplate
    {
        string GetDisplayString(string barCode, string ProductAddress, int checkCount,DataTable dataTable);
    }

    /// <summary>
    ///PrintTemlate 的摘要说明
    /// </summary>
    public class CommonPrintTemplate : IPrintTemplate
    {

        public string GetDisplayString(string barCode, string ProductAddress, int checkCount, DataTable dataTable)
        {
            return "尊敬的用户,您好! 您扫描的条码【" + barCode + "】属于晚安生产的产品,产地【" + ProductAddress + "】,扫描次数【" + (checkCount + 1) + "】次,请谨防假冒.";
        }
    }

    public class JFPrintTemplate
    {
        public static string GetDisplayString(string barCode, string ProductAddress, int checkCount, string name, string code)
        {
            StringBuilder stb = new StringBuilder();
            stb.Append("经验证，本产品为晚安家纺正品,扫描次数【" + (checkCount + 1) + "】次,请谨防假冒.");
            stb.AppendFormat(" 商品名称：{0}", name);
            stb.AppendFormat(" 产品编码：{0}", code);
            stb.AppendFormat(" 制造产商：湖南省晚安家纺股份有限公司");
            stb.Append(" 公司热线：400-068-4988");
            stb.Append(" 请保留相应票据或产品二维码作为售后凭证");
            return stb.ToString();
        }

        public static string GerErrorString()
        {
            return "产品信息不存在,谨防假冒";
        }
    }
}
