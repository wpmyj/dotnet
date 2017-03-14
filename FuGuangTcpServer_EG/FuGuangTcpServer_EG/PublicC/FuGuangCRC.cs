/*
 * ====================================================
 *
 * 说      明: 
 *
 * 创建时间：2015/9/15 9:50:06
 *
 * 作      者：张军龙
 * 修改时间：2015/9/15 9:50:06
 * 修 改 人：
 *
 * ====================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuGuangTCPServer_EG.PublicC
{
    class FuGuangCRC
    {
        public byte GetCRCByte(byte[] data)
        {
            int crc = 0x0; //（初始值为 0） 
            foreach (byte bt in data)
            {
                crc = crc ^ bt;
                for (int j = 1; j <= 8; j++)
                {
                    if ((crc & 0x80) == 0x80)
                        crc = (crc << 1) ^ 0xE5;
                    else
                        crc = crc << 1;
                }
            }
            return (byte)crc;
        }
    }
}
