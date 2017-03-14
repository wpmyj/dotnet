using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
namespace TongHuaGPRSserver.PublicC
{
    public class OtherClass
    {
        /// <summary>
        /// 监听IP地址
        /// </summary>
        /// <returns></returns>
        public string IPAddress()
        {
            return ConfigurationManager.AppSettings["lisAddress"].ToString();
        }


        /// <summary>
        /// 监听端口
        /// </summary>
        /// <returns></returns>
        public int lPort()
        {
            return int.Parse(ConfigurationManager.AppSettings["lisPort"].ToString());
        }



        #region 亿联
        public string getYLValue(byte[] resultAry,SqlConnection conn,string ipaddress)
        {
            try
            {
                StringBuilder str_result = new StringBuilder();
                StringBuilder str_update = new StringBuilder();
                str_update.Append("update YLData set ");
                DateTime theT = DateTime.Now;


                float thevalue = SaveModbusSS(resultAry, 3);//浊度
                str_result.Append(" 浊度:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("zhuodu=" + string.Format("{0:f4},", thevalue));
                // float Yulv = SaveModbusSS(resultAry, 7);//余氯
                thevalue = SaveModbusSS(resultAry, 7);//余氯
                str_result.Append(" 余氯:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("yulv=" + string.Format("{0:f4},", thevalue));
                // float Ddl = SaveModbusSS(resultAry, 11);//电导率
                thevalue = SaveModbusSS(resultAry, 11);//电导率
                str_result.Append(" 电导率:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("diandaolv=" + string.Format("{0:f4},", thevalue));

                // float Ph = SaveModbusSS(resultAry, 15);//PH
                thevalue = SaveModbusSS(resultAry, 15);//PH
                str_result.Append(" PH:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("ph=" + string.Format("{0:f4},", thevalue));
                // float Sd = SaveModbusSS(resultAry, 19);//色度
                thevalue = SaveModbusSS(resultAry, 19);//色度
                str_result.Append(" 色度:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("sedu=" + string.Format("{0:f4},", thevalue));
                // float Wd = SaveModbusSS(resultAry, 23);//温度
                thevalue = SaveModbusSS(resultAry, 23);//温度
                str_result.Append(" 温度:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("wendu=" + string.Format("{0:f4},", thevalue));
                // float Yl = SaveModbusSS(resultAry, 27);//压力
                thevalue = SaveModbusSS(resultAry, 27);//压力
                str_result.Append(" 压力:" + string.Format("{0:f2} ", thevalue));
                str_update.Append("yali=" + string.Format("{0:f4},", thevalue));


                str_update.Append("updateTime='" + theT.ToString() + "'");

                str_update.Append(" where IpAddress='" + ipaddress + "'");

                SqlCommand cmd = new SqlCommand(str_update.ToString(), conn);
                cmd.ExecuteNonQuery();
                return str_result.ToString();
            }
            catch (Exception ex )
            {
                Console.WriteLine("Error__getYLValue:"+ex.Message);
                return "";
            }
            
        }


        /// <summary>
        /// modbus解析
        /// </summary>
        /// <param name="inputSorce">接受报文BYTE[]</param>
        /// <param name="start">起始位</param>
        /// <returns></returns>
        public float SaveModbusSS(byte[] inputSorce, int start)
        {
            byte[] newAry = new byte[4];
            Array.Copy(inputSorce, start, newAry, 0, 4);
            Array.Reverse(newAry);
            float valueFloat = 0;
            valueFloat = BitConverter.ToSingle(newAry, 0);
            return valueFloat;
        }
        #endregion



        #region 封装LIST
        /// <summary>
        /// 封装LIST
        /// </summary>
        /// <param name="valuelist">读取的byte</param>
        /// <param name="dt">读取的table</param>
        /// <returns></returns>
        public List<TagValue> getValue(byte[] resultAry, DataTable dt)
        {
            List<TagValue> tagList = new List<TagValue>();

            byte[] valuelist = new byte[132];

            if (resultAry.Length != 137)
            {
                return tagList;
            }
            Array.Copy(resultAry, 3, valuelist, 0, resultAry.Length - 5);
            Array.Reverse(valuelist);
            try
            {

                TagValue objValue;
                //   int diviceId = BitConverter.ToInt32(valuelist, 0);
                int num0 = int.Parse(resultAry[0].ToString());
                if (num0 < 100 || num0 > 200)
                {
                    return tagList;
                }
                DataRow[] rows = dt.Select("PLCAddress='" + num0 + "'");
                Console.WriteLine("PLC" + num0 + "|" + resultAry.Length);
                if (rows.Length > 0)
                {
                    DataRow resultRow = dt.Select("PLCAddress='" + num0 + "'")[0];
                    int bitcount = int.Parse(resultRow["BitCount"].ToString());
                    float[] fvlist = new float[bitcount];
                    for (int i = 0; i < bitcount; i++)
                    {
                        float floatValue = BitConverter.ToSingle(valuelist, i * 4);
                        fvlist[i] = floatValue;
                    }
                    Array.Reverse(fvlist);
                    for (int i = 0; i < bitcount; i++)
                    {
                        string str_tagname = resultRow[5 + i].ToString();
                        if (str_tagname == "1" || str_tagname.Trim() == "")
                        {
                            continue;
                        }
                        objValue = new TagValue();
                        objValue.TagName = str_tagname;
                        objValue.DateTime = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
                        objValue.DeviceId = resultRow["HotName"].ToString();
                        if (fvlist[i] < 0)
                        {
                            objValue.Value = 0;
                        }
                        else
                        {
                            objValue.Value = Math.Round(fvlist[i] / 100, 2);
                        }
                        if (double.IsNaN(objValue.Value))
                        {
                            objValue.Value = -123456;
                        }
                        objValue.PLCadd = resultRow["PLCAddress"].ToString();
                        tagList.Add(objValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }
            return tagList;
        }
        #endregion
    }
}
