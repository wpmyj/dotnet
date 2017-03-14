/*
 * ====================================================
 *
 * 说      明: 
 *
 * 创建时间：2015/9/15 13:30:30
 *
 * 作      者：张军龙
 * 修改时间：2015/9/15 13:30:30
 * 修 改 人：
 *
 * ====================================================
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TongHuaGPRSserver.PublicC;

namespace FuGuangTCPServer_EG.PublicC
{
    public class WaterResources
    {
        private Dictionary<int, WaterQuality> wqDic = WaterQualityDic();

        public bool isOk(byte[] res)
        {
            //验证包头包尾
            if (res[0] != 0x68 || res[2] != 0x68 || res[res.Length - 1] != 0x16)
            {
                return false;
            }
            int len = int.Parse(res[1].ToString());
            byte[] datas = new byte[len];
            Array.Copy(res, 3, datas, 0, len);
            FuGuangCRC crc = new FuGuangCRC();
            //crc校验
            if (res[res.Length - 2] != crc.GetCRCByte(datas))
            {
                return false;
            }
            return true;
        }

        public string FunctionCode(byte c)
        {
            string str_c = Convert.ToString(c, 2).PadLeft(8, '0');
            int code = Convert.ToInt32(str_c.Substring(4), 2);
            return setUPDic()[code];
        }

        private Dictionary<int, string> setUPDic()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(3, "流量（水量）参数");//流量（水量）参数
            dic.Add(10, "analyzeWaterQuality");//水质参数
            dic.Add(2, "水位参数");//水位参数
            return dic;
        }
        private Dictionary<string, string> dicAddress = AddressDic();
        private static Dictionary<int, WaterQuality> WaterQualityDic()
        {
            Dictionary<int, WaterQuality> dic = new Dictionary<int, WaterQuality>();
            WaterQuality wq = new WaterQuality();
            wq.Count = 3;
            wq.Name = "水温";
            wq.Rate = 10;
            dic.Add(0, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "pH 值";
            wq.Rate = 100;
            dic.Add(1, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "溶解氧";
            wq.Rate = 10;
            dic.Add(2, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "高锰酸盐指数";
            wq.Rate = 10;
            dic.Add(3, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "电导率";
            wq.Rate = 1;
            dic.Add(4, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "氧化还原电位";
            wq.Rate = 10;
            dic.Add(5, wq);

            wq = new WaterQuality();
            wq.Count = 3;
            wq.Name = "浊度";
            wq.Rate = 1;
            dic.Add(6, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "化学需氧量";
            wq.Rate = 10;
            dic.Add(7, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "五日生化需氧量";
            wq.Rate = 10;
            dic.Add(8, wq);

            wq = new WaterQuality();
            wq.Count = 6;
            wq.Name = "氨氮";
            wq.Rate = 100;
            dic.Add(9, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "总氮";
            wq.Rate = 100;
            dic.Add(10, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "铜";
            wq.Rate = 10000;
            dic.Add(11, wq);

            wq = new WaterQuality();
            wq.Count = 6;
            wq.Name = "锌";
            wq.Rate = 10000;
            dic.Add(12, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "氟化物";
            wq.Rate = 100;
            dic.Add(13, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "硒";
            wq.Rate = 100000;
            dic.Add(14, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "砷";
            wq.Rate = 100000;
            dic.Add(15, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "汞";
            wq.Rate = 100000;
            dic.Add(16, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "镉";
            wq.Rate = 100000;
            dic.Add(17, wq);


            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "六价铬";
            wq.Rate = 1000;
            dic.Add(18, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "铅";
            wq.Rate = 100000;
            dic.Add(19, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "氰化物";
            wq.Rate = 1000;
            dic.Add(20, wq);


            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "挥发酚";
            wq.Rate = 1000;
            dic.Add(21, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "苯酚";
            wq.Rate = 100;
            dic.Add(22, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "硫化物";
            wq.Rate = 1000;
            dic.Add(23, wq);

            wq = new WaterQuality();
            wq.Count = 10;
            wq.Name = "粪大肠菌群";
            wq.Rate = 1;
            dic.Add(24, wq);

            wq = new WaterQuality();
            wq.Count = 6;
            wq.Name = "硫酸盐";
            wq.Rate = 100;
            dic.Add(25, wq);

            wq = new WaterQuality();
            wq.Count = 8;
            wq.Name = "氯化物";
            wq.Rate = 100;
            dic.Add(26, wq);

            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "硝酸盐氮";
            wq.Rate = 100;
            dic.Add(27, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "铁";
            wq.Rate = 100;
            dic.Add(28, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "锰";
            wq.Rate = 100;
            dic.Add(29, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "石油类";
            wq.Rate = 100;
            dic.Add(30, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "阴离子表面活性剂";
            wq.Rate = 100;
            dic.Add(31, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "六六六";
            wq.Rate = 1000000;
            dic.Add(32, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "滴滴涕";
            wq.Rate = 1000000;
            dic.Add(33, wq);

            wq = new WaterQuality();
            wq.Count = 8;
            wq.Name = "蓝绿藻";
            wq.Rate = 10;
            dic.Add(34, wq);

            wq = new WaterQuality();
            wq.Count = 12;
            wq.Name = "总磷";
            wq.Rate = 10000000;
            dic.Add(35, wq);

            wq = new WaterQuality();
            wq.Count = 4;
            wq.Name = "叶绿素";
            wq.Rate = 100;
            dic.Add(36, wq);


            wq = new WaterQuality();
            wq.Count = 5;
            wq.Name = "总有机碳";
            wq.Rate = 100;
            dic.Add(37, wq);

            wq = new WaterQuality();
            wq.Count = 7;
            wq.Name = "余氯";
            wq.Rate = 1000000;
            dic.Add(38, wq);
            return dic;
        }

        /// <summary>
        /// 判断是何种报文
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public bool JudgeMethod(byte[] res, SqlConnection conn)
        {
            try
            {
                if (res[9] != 0xc0)
                {
                    string str_address = analyzeAddress(res);
                    if (dicAddress.ContainsKey(str_address))
                        str_address = dicAddress[str_address];
                    Console.WriteLine("本条是心跳包来自：" + str_address);
                    return false;
                }
                WaterResources wr = new WaterResources();
                Type type = typeof(WaterResources);
                System.Reflection.MethodInfo mi = type.GetMethod(FunctionCode(res[3]));
                if (mi == null)
                {
                    return false;
                }
                Object[] parameters = new Object[2];
                parameters[0] = res;
                parameters[1] = conn;
                mi.Invoke(wr, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("方法JudgeMethod中:" + ex.Message);
                return false;
            }

        }

        public byte[] ConfirmMessage(byte[] res)
        {
            byte[] back = { 0x68, 0x08, 0x68, res[3], res[4], res[5], res[6], res[7], res[8], res[9], 0x00, 0x00, 0x16 };
            byte[] datas = { res[3], res[4], res[5], res[6], res[7], res[8], res[9], 0x00 };
            FuGuangCRC crc = new FuGuangCRC();
            back[11] = crc.GetCRCByte(datas);
            return back;
        }

        /// <summary>
        /// 水质参数报文解析
        /// </summary>
        public void analyzeWaterQuality(byte[] res, SqlConnection conn)
        {
            SqlHelper sh = new SqlHelper();
            byte[] byte_class = new byte[5];
            Array.Copy(res, 10, byte_class, 0, 5);
            string code = ByteArrayToBinaryString(byte_class);
            char[] arr_code = code.ToArray();
            //Console.WriteLine(code);
            //Dictionary<int, WaterQuality> dic = WaterQualityDic();
            
            List<WaterQuality> listw = new List<WaterQuality>();
            byte[] byte_data = new byte[4];
            int index = 0;
            int indexfive = 0;
            string str_address = analyzeAddress(res);
            if (dicAddress.ContainsKey(str_address))
                str_address = dicAddress[str_address];
            StringBuilder sbout = new StringBuilder();
            Console.WriteLine("[" + str_address + "]接收水质参数报文：" + BitConverter.ToString(res));
            for (int i = 0; i < 40; i++)
            {
                if (arr_code[i] == '1')
                {
                    Array.Clear(byte_data, 0, 4);
                    if (i == 24)
                    {
                        indexfive = 1;
                    }
                    else
                    {
                        indexfive = 0;
                    }
                    Array.Copy(res, indexfive + 15 + index * 4, byte_data, 0, 4);
                    
                    Array.Reverse(byte_data);
                    String bcdstring = converToBCD(byte_data);
                    //Console.WriteLine(bcdstring);
                    int value =int.Parse(bcdstring);
                    WaterQuality wq = null;
                    if (!wqDic.TryGetValue(i, out wq))
                    {
                        index++;
                        continue;
                    }
    
                    wq.Value = decimal.Parse(value.ToString());
                    if (str_address + "." + wq.Name == "管网.浊度")
                    {
                        wq.Value = wq.Value / 1000;
                    }
                    String finallValue = QFormat(wq);
                    sbout.Append(String.Format("{0}:{1},D<{2}>|", wq.Name, finallValue, i));
                    wq.Name = str_address + "." + wq.Name;
                    //wq.Value = decimal.Parse(finallValue);
                    wq.Value = ChangeDataToD(finallValue);

                    listw.Add(wq);
                    index++;
                }
            }
            if (listw.Count > 0)
            {
                Console.WriteLine("[" + str_address + "]接收" + listw.Count + "个水质参数：" + sbout.ToString());
                sh.insertFuGuang(conn, listw);
            }
            else
            {
                Console.WriteLine("[" + str_address + "]虽然收到报文但是没有采集任何参数");
            }

            //Convert.ToString(btye_class, 2).PadLeft(39, '0');
        }


        private Decimal ChangeDataToD(string strData)
        {
            Decimal dData = 0.0M;
            if (strData.Contains("E"))
            {
                dData = Convert.ToDecimal(Decimal.Parse(strData.ToString(), System.Globalization.NumberStyles.Float));
            }
            else
            {
                dData = Decimal.Parse(strData);
            }
            return dData;
        }

        private string converToBCD(byte[] bdata)
        {
            StringBuilder sb = new StringBuilder(bdata.Length * 2);
            foreach (byte b in bdata)
            {
                sb.Append(b >> 4);
                sb.Append(b & 0x0f);
                
            }
            return sb.ToString();
        }

        private string analyzeAddress(byte[] res)
        {
            string hi = res[7].ToString().PadLeft(2, '0');
            string low = res[8].ToString().PadLeft(2, '0'); ;
            return low + hi;
        }

        private string QFormat(WaterQuality wq)
        {
            //decimal pow = (decimal) Math.Pow(10, (8 - wq.Count));
            return float.Parse((wq.Value / wq.Rate).ToString("G" + wq.Count)).ToString();
            //return wq.Value.ToString();

        }

        /// <summary>
        /// 字节数组节转换成二进制字符串
        /// </summary>
        /// <param name="byteArray">要转换的字节数组</param>
        /// <returns></returns>
        private static string ByteArrayToBinaryString(byte[] byteArray)
        {
            int capacity = byteArray.Length * 8;
            StringBuilder sb = new StringBuilder(capacity);
            for (int i = 0; i < byteArray.Length; i++)
            {
                sb.Append(byte2BinString(byteArray[i]));
            }
            return sb.ToString();
        }

        private static string byte2BinString(byte b)
        {
            //return  Convert.ToString(b & 0x0f, 2).PadLeft(4, '0')+Convert.ToString(b >> 4, 2).PadLeft(4, '0');
            char[] arrchar = Convert.ToString(b, 2).PadLeft(8, '0').ToArray();
            Array.Reverse(arrchar);
            return new string(arrchar);
        }

        /// <summary>
        /// 地址字典
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> AddressDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("0001", "崂山水库");
            dic.Add("0002", "洪江河管理站");
            dic.Add("0003", "渠首管理站");
            dic.Add("0004", "管网");
            return dic;
        }
    }

    public class WaterQuality
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        int rate;

        public int Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        decimal value;

        public decimal Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
