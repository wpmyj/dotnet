using System;
using System.Collections;							// Access to the Array list
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
// Sleeping
using System.Net;									// Used to local machine info
using System.Net.Sockets;							// Socket namespace
using CRC;
using TongHuaGPRSserver.PublicC;
using FuGuangTCPServer_EG.PublicC;

namespace SocketListerGaoxin
{
    /// <summary>
    /// Main class from which all objects are created
    /// </summary>
    class AppMain
    {


        static SqlHelper objhelper;
        static SqlConnection connABB;
        static OtherClass oc;
        static WaterResources wr;
        // Attributes

        private ArrayList m_aryClients = new ArrayList();	// List of Client Connections
        /// <summary>
        /// Application starts here. Create an instance of this class and use it
        /// as the main object.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            AppMain app = new AppMain();
            oc = new OtherClass();
            wr = new WaterResources();
            // Welcome and Start listening
            Console.WriteLine("*** hash水质监测 Server Started {0} *** ", DateTime.Now.ToString("G"));
            //测试报文
            // byte[] test = strToToHexByte("683D68BA9970990E04C05F0600001841020000550800008300000010000000420000005200000000000000500000005000000034020000000030000000081600F816");



            objhelper = new SqlHelper();
            connABB = objhelper.getConn();
            if (connABB.State != ConnectionState.Open)
            {
                connABB.Open();
            }
            objhelper = new SqlHelper();
            connABB = objhelper.getConn();
            connABB.Open();

            // Method 2 
            //
            int nPortListen = oc.lPort();
            IPAddress ip = IPAddress.Parse(oc.IPAddress());
            // Determine the IPAddress of this machine

            String strHostName = "";
            try
            {
                // NOTE: DNS lookups are nice and all but quite time consuming.
                strHostName = Dns.GetHostName();
            }
            catch (Exception ex)
            {
                Console.WriteLine("试图获得本地地址错误 {0} ", ex.Message);
            }


            Console.WriteLine("开始监听数据: [{0}] {1}:{2}", strHostName, ip, nPortListen);

            // Create the listener socket in this machines IP address
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            listener.Bind(new IPEndPoint(ip, nPortListen));
            //listener.Bind( new IPEndPoint( IPAddress.Loopback, 399 ) );	// For use with localhost 127.0.0.1
            listener.Listen(30);

            // Setup a callback to be notified of connection requests
            listener.BeginAccept(new AsyncCallback(app.OnConnectRequest), listener);

            Console.WriteLine("按任意键退出");
            Console.ReadLine();
            Console.WriteLine("退出中...");

            // Clean up before we go home
            if (connABB.State != ConnectionState.Closed)
            {
                connABB.Close();
            }
            connABB.Close();
            listener.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }



        public DataTable getTagInfo()
        {
            DataTable dtTemp = new DataTable();

            objhelper = new SqlHelper();
            connABB = objhelper.getConn();
            if (connABB.State != ConnectionState.Open)
            {
                connABB.Open();
            }
            SqlDataAdapter sda = new SqlDataAdapter("select * from YLinfo", connABB);
            sda.Fill(dtTemp);
            dtTemp.TableName = "YLinfo";
            SqlCommandBuilder cbb = new SqlCommandBuilder(sda);
            //sda.InsertCommand = cbb.GetInsertCommand();

            //sda.UpdateCommand = cbb.GetUpdateCommand();

            //sda.DeleteCommand = cbb.GetDeleteCommand();

            string plcpath = ConfigurationManager.AppSettings["YLinfo"].ToString();
            dtTemp.WriteXml(plcpath, XmlWriteMode.WriteSchema);
            dtTemp.ReadXml(plcpath);
            return dtTemp;
        }


        static Dictionary<string, string> Dic_YL = new Dictionary<string, string>();

        /// <summary>
        /// 亿联使用
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getYldicInfo()
        {
            DataTable dtTemp = new DataTable();
            Dictionary<string, string> dic_temp = new Dictionary<string, string>();
            string plcpath = ConfigurationManager.AppSettings["YLinfo"].ToString();
            dtTemp.ReadXml(plcpath);
            foreach (DataRow item in dtTemp.Rows)
            {
                dic_temp.Add(item["IPAddress"].ToString().Trim(), item["HotName"].ToString().Trim());
            }
            return dic_temp;
        }


        /// <summary>
        /// 当客户端请求连接时使用回调. 
        /// 接受连接，将它添加到我们的列表和设置 
        /// accept more connections.
        /// </summary>
        /// <param name="ar"></param>
        public void OnConnectRequest(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            NewConnection(listener.EndAccept(ar));
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
        }

        /// <summary>
        /// 将给定的连接添加到我们的客户列表中
        /// Note we have a new friend
        /// Send a welcome to the new client
        /// Setup a callback to recieve data
        /// </summary>
        /// <param name="sockClient">Connection to keep</param>
        //public void NewConnection( TcpListener listener )
        public void NewConnection(Socket sockClient)
        {
            // Program blocks on Accept() until a client connects.
            //SocketChatClient client = new SocketChatClient( listener.AcceptSocket() );
            SocketChatClient client = new SocketChatClient(sockClient);
            m_aryClients.Add(client);
            Console.WriteLine("客户端 {0}, 加入连接", client.Sock.RemoteEndPoint);

            // Get current date and time.
            //DateTime now = DateTime.Now;
            //String strDateLine = "Welcome " + now.ToString("G") + "\n\r";

            //// Convert to byte array and send.
            //Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes( strDateLine.ToCharArray() );
            //client.Sock.Send( byteDateLine, byteDateLine.Length, 0 );

            client.SetupRecieveCallback(this);
        }

        /// <summary>
        /// 接收到数据，存入数据
        /// </summary>
        /// <param name="ar"></param>
        public void OnRecievedData(IAsyncResult ar)
        {
            try
            {
                SocketChatClient client = (SocketChatClient)ar.AsyncState;
                byte[] aryRet = client.GetRecievedData(ar);


                // If no data was recieved then the connection is probably dead
                if (aryRet.Length < 1)
                {
                    Console.WriteLine("客户端 {0}, 断开连接", client.Sock.RemoteEndPoint);
                    client.Sock.Close();
                    m_aryClients.Remove(client);
                    return;
                }
                if (wr.isOk(aryRet))
                {
                    if (wr.JudgeMethod(aryRet,connABB))
                    {
                        client.Sock.Send(wr.ConfirmMessage(aryRet));
                    }
                    
                }
                client.SetupRecieveCallback(this);
            }
            catch (Exception ex)
            {

                Console.WriteLine("OnRecievedData:" + ex.Message);
                WriteError("OnRecievedData:" + ex.Message);
                if (connABB.State != ConnectionState.Open)
                {
                    connABB.Open();
                }
            }

        }

        //校验
        private bool CRCisok(byte[] arylist)
        {

            byte[] byt = new byte[arylist.Length - 2];
            Array.Copy(arylist, byt, arylist.Length - 2);
            string crc1 = arylist[arylist.Length - 2].ToString("X") + arylist[arylist.Length - 1].ToString("X");
            //string crc2 = BitConverter.ToString(byt).Replace("-", "");
            CRC16 cc = new CRC16();
            string crc2 = cc.GetCrc16_string(BitConverter.ToString(byt).Replace("-", "")).ToUpper();
            return crc1.Equals(crc2);
        }

        //IEEE 754 Convertor


        #region 16进制


        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion


        #region 写入日志
        private void WriteError(string sText)
        {
            string sEventSource = "高信GPRS采集控制台";
            string sEventLog = "ABB错误日志";
            if (!EventLog.SourceExists(sEventSource))
                EventLog.CreateEventSource(sEventSource, sEventLog);
            EventLog.WriteEntry(sEventSource, sText, EventLogEntryType.Error);

        }

        private void WriteInfo(string sText)
        {
            string sEventSource = "高信GPRS采集控制台";
            string sEventLog = "ABB信息";
            if (!EventLog.SourceExists(sEventSource))
                EventLog.CreateEventSource(sEventSource, sEventLog);
            EventLog.WriteEntry(sEventSource, sText, EventLogEntryType.Information);
        }

        #endregion
    }



    /// <summary>
    /// Class holding information and buffers for the Client socket connection
    /// </summary>
    internal class SocketChatClient
    {
        private Socket m_sock;						// Connection to the client
        private byte[] m_byBuff = new byte[256];		// Receive data buffer
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sock">client socket conneciton this object represents</param>
        public SocketChatClient(Socket sock)
        {
            m_sock = sock;
        }

        // Readonly access
        public Socket Sock
        {
            get { return m_sock; }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="app"></param>
        public void SetupRecieveCallback(AppMain app)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(app.OnRecievedData);
                m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = m_sock.EndReceive(ar);
            }
            catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(m_byBuff, byReturn, nBytesRec);

            /*
            // Check for any remaining data and display it
            // This will improve performance for large packets 
            // but adds nothing to readability and is not essential
            int nToBeRead = m_sock.Available;
            if( nToBeRead > 0 )
            {
                byte [] byData = new byte[nToBeRead];
                m_sock.Receive( byData );
                // Append byData to byReturn here
            }
            */
            return byReturn;
        }
    }
}
