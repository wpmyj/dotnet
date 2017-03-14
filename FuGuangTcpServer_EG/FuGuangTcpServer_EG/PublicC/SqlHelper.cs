using FuGuangTCPServer_EG.PublicC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;


namespace TongHuaGPRSserver.PublicC
{
    public class SqlHelper
    {
        string str_conn = ConfigurationManager.ConnectionStrings["sqlconn"].ToString();

        public SqlConnection getConn()
        {
            SqlConnection conn = new SqlConnection(str_conn);
            return conn;
        }

        public int insterSQL2(SqlConnection conn, List<TagValue> tagvalueList)
        {
            //SqlCommand cmd=new SqlCommand();
            //cmd.Connection = conn;
            // string _strSql = string.Empty;
            try
            {
                StringBuilder _strBuilder = new StringBuilder();
                foreach (TagValue obj in tagvalueList)
                {
                    _strBuilder.AppendFormat("insert into HSData_Temp(SName,TagName,TagVal,GetDt,Flag) values ('{0}','{1}','{2}','{3}',{4});", obj.DeviceId, obj.TagName, obj.Value, obj.DateTime, 0);
                    // _strSql = string.Format("insert into HSData_Temp(SName,TagName,TagVal,GetDt,Flag) values ('{0}','{1}','{2}','{3}',{4});", obj.DeviceId, obj.TagName, obj.Value, obj.DateTime,0);
                }

                SqlCommand cmd = new SqlCommand(_strBuilder.ToString(), conn);
                IAsyncResult result = cmd.BeginExecuteNonQuery();
                while (!result.IsCompleted)
                {

                    System.Threading.Thread.Sleep(500);
                }

                return cmd.EndExecuteNonQuery(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("insterSQL2:" + ex.Message);
                return 0;
            }
        }

        public void insterSQL(SqlConnection conn,List<TagValue> tagvalueList)
        {
            //SqlCommand cmd=new SqlCommand();
            //cmd.Connection = conn;
           // string _strSql = string.Empty;
            StringBuilder _strBuilder = new StringBuilder();
            foreach (TagValue obj in tagvalueList)
            {
                _strBuilder.AppendFormat("insert into HSData_Temp(SName,TagName,TagVal,GetDt,Flag) values ('{0}','{1}','{2}','{3}',{4});",obj.DeviceId, obj.TagName, obj.Value, obj.DateTime, 0);
               // _strSql = string.Format("insert into HSData_Temp(SName,TagName,TagVal,GetDt,Flag) values ('{0}','{1}','{2}','{3}',{4});", obj.DeviceId, obj.TagName, obj.Value, obj.DateTime,0);
            }
            SqlCommand cmd = new SqlCommand(_strBuilder.ToString(), conn);
            cmd.ExecuteNonQuery();
        }

        public void insertFuGuang(SqlConnection conn, List<WaterQuality> wqs) 
        {
            StringBuilder _strBuilder = new StringBuilder();
            DateTime dt = DateTime.Now;
            foreach (WaterQuality obj in wqs)
            {
                _strBuilder.AppendFormat("INSERT INTO TPointValuesTable(TTagName,TDataValue,TDateTime) VALUES ('{0}',{1},'{2}');", obj.Name, obj.Value, dt.ToString("yyyy-MM-dd HH:mm:ss"));
                // _strSql = string.Format("insert into HSData_Temp(SName,TagName,TagVal,GetDt,Flag) values ('{0}','{1}','{2}','{3}',{4});", obj.DeviceId, obj.TagName, obj.Value, obj.DateTime,0);
            }
            SqlCommand cmd = new SqlCommand(_strBuilder.ToString(), conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine(DateTime.Now.ToString()+":成功插入"+wqs.Count+"条");
        }
    }
}
