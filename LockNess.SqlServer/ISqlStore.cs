using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LockNess.WeiXin.Core.Db
{
    public interface ISqlStore
    {
        IDataReader GetDataReader(string sql);
    }

}
