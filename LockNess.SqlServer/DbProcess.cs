using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;


namespace LockNess.WeiXin.Core.Db
{
    public class DbProcess
    {
        private string _oldconn;
        private string _newconn;
        public DbProcess(string oldconn,string newconn)
        {
            _oldconn = oldconn;
            _newconn = newconn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanResult"></param>
        /// <returns>dic Barcode,CheckCount, chandi,Name,Code</returns>
        public Dictionary<string,string> Process(string scanResult)
        {
            SqlServerStore serverOldStore = new SqlServerStore(_oldconn);
            SqlServerStore serverNewStore = new SqlServerStore(_newconn);
          
            string sql = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (scanResult.Substring(0, 4) == "2016" || scanResult.Substring(0, 4) == "2015")
            {
                sql = "select Barcode,Org,CheckCount from [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] where Barcode='" + scanResult + "'";

            }
            else
                sql = "select Barcode,Org,CheckCount from [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] where Md5Code='" + scanResult + "'";

            var tb = serverOldStore.GetDataReader(sql);
           
            if (tb.Rows.Count >= 1)
            {
                dic.Add("Barcode", tb.Rows[0]["Barcode"].ToString());
                dic.Add("CheckCount", tb.Rows[0]["CheckCount"].ToString());

                string chandi = string.Empty;
                string org = tb.Rows[0]["Org"].ToString();
                if (org == "1001902270236689")
                {
                    chandi = "晚安家纺";
                    dic.Add("chandi", chandi);
                
                    var sql1 = @"select  t.Name,t.Code from [GOODNIGHT].[dbo].WanAn_MO_BarcodeDetail x inner join [GOODNIGHT].[dbo].CBO_ItemMaster t on t.org=x.org and x.ItemCode=t.Code 
                                        where x.Barcode='" + tb.Rows[0]["Barcode"].ToString() + "'";

                    var dataTable = serverOldStore.GetDataReader(sql1);
                    if (dataTable.Rows.Count > 0)
                    {
                        dic.Add("Name", dataTable.Rows[0]["Name"].ToString());
                        dic.Add("Code", dataTable.Rows[0]["Code"].ToString());
                        sql = "update [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] set [CheckCount]=isnull([CheckCount],0)+1 where Md5Code='" + scanResult + "'";
                        serverNewStore.ExcuteSql(sql);
                    }
                }
            }
            else
            {
                if (scanResult.Substring(0, 4) == "2016" || scanResult.Substring(0, 4) == "2015")
                {
                    sql = "select Barcode,Org,CheckCount from [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] where Barcode='" + scanResult + "'";

                }
                else
                    sql = "select Barcode,Org,CheckCount from [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] where Md5Code='" + scanResult + "'";
                tb.Clear();
                tb =serverNewStore.GetDataReader(sql);

                if (tb.Rows.Count >= 1)
                {
                    dic.Add("Barcode", tb.Rows[0]["Barcode"].ToString());
                    dic.Add("CheckCount", tb.Rows[0]["CheckCount"].ToString());
                    string chandi = string.Empty;
                    string org = tb.Rows[0]["Org"].ToString();

                    if (org == "1001503170150373")
                    {
                        chandi = "晚安家纺";
                        dic.Add("chandi", chandi);
                        var sql1 = @"select  t.Name,t.Code from [GOODNIGHT].[dbo].WanAn_MO_BarcodeDetail x inner join [GOODNIGHT].[dbo].CBO_ItemMaster t on t.org=x.org and x.ItemCode=t.Code 
                                        where x.Barcode='" + tb.Rows[0]["Barcode"].ToString() + "'";
                        var dtnewJf = serverOldStore.GetDataReader(sql1);

                        while (dtnewJf.Rows.Count > 0)
                        {
                            dic.Add("Name", dtnewJf.Rows[0]["Name"].ToString());
                            dic.Add("Code", dtnewJf.Rows[0]["Code"].ToString());
                            sql = "update [GOODNIGHT].[dbo].[WanAn_MO_BarcodeDetail] set [CheckCount]=isnull([CheckCount],0)+1 where Md5Code='" + scanResult + "'";
                            serverNewStore.ExcuteSql(sql);
                            break;
                        }
                    }
                }
            
            }

            return dic;
        }
    }
}
