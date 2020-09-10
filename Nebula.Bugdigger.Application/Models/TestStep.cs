using System;
using System.Collections.Generic;

namespace Nebula.Bugdigger
{
    public class TestStep
    {
        public string Name { get; set; }
        public Operate Operate { get; set; }
        public int Delay { get; set; }
        public Param Param { get; set; } = new Param();
        public string Remark { get; set; }
        public List<Field> ICD { get; set; } = new List<Field>();

        public byte[] GetSendData()
        {
            List<byte> msg = new List<byte>();
            if (Operate == Operate.Send)
            {

                List<byte> header = GetMsgHead();
                List<byte> body = GetMsgBody();

                //msg leng 
                byte[] leng = HostToNetBytes(body.Count + header.Count);

                ListAddBytes(msg, leng, 4);

                foreach (byte headbyte in header)
                {
                    msg.Add(headbyte);
                }

                foreach (byte bodybyte in body)
                {
                    msg.Add(bodybyte);
                }
            }

            return msg.ToArray();
        }

        public List<byte> GetMsgHead()
        {
            List<byte> msgHeadList = new List<byte>();

            //MsgHeader bustype
            if (Param.BusType == BusType.P_1553B)
            {
                msgHeadList.Add((byte)0x0);
            }
            else if (Param.BusType == BusType.P_ARINC429)
            {
                msgHeadList.Add((byte)0x1);
            }
            else if (Param.BusType == BusType.P_FC)
            {
                msgHeadList.Add((byte)0x4);
            }
            else if (Param.BusType == BusType.P_ARINC422)
            {
                msgHeadList.Add((byte)0x3);
            }
            else if (Param.BusType == BusType.P_IO)
            {
                msgHeadList.Add((byte)0x5);
            }

            // Msgheader NetNo

            msgHeadList.Add((byte)Param.NetNo);

            //Msgheader  sc1 sc2 dc1 dc2
            byte[] sc1 = HostToNetBytes(Param.SrcChannel1);
            byte[] sc2 = HostToNetBytes(Param.SrcChannel2);
            byte[] dc1 = HostToNetBytes(Param.DesChannel1);
            byte[] dc2 = HostToNetBytes(Param.DesChannel2);

            //Msgheader nodeId, timeTag;
            byte[] node = HostToNetBytes((byte)0);
            byte[] time = HostToNetBytes((byte)0);

            ListAddBytes(msgHeadList, sc1, 4);
            ListAddBytes(msgHeadList, sc2, 4);
            ListAddBytes(msgHeadList, dc1, 4);
            ListAddBytes(msgHeadList, dc2, 4);
            ListAddBytes(msgHeadList, node, 4);
            ListAddBytes(msgHeadList, time, 4);

            //msgheader reserve[10]
            for (int i = 0; i < 10; i++)
            {
                msgHeadList.Add((byte)0);
            }

            return msgHeadList;
        }

        public void ListAddBytes(List<byte> list, byte[] byteArray, int len)
        {
            for (int i = 0; i < len; i++)
            {
                list.Add(byteArray[i]);
            }
        }

        //host to net
        public byte[] HostToNetBytes(int hostData)
        {
            int netData = System.Net.IPAddress.HostToNetworkOrder(hostData);

            return BitConverter.GetBytes((uint)netData);
        }

        public List<byte> GetMsgBody()
        {
            List<byte> msgBodyList = new List<byte>();
            msgBodyList = BinStringDataToByteList(GetData());
            return msgBodyList;

        }

        public List<byte> BinStringDataToByteList(string icdData)
        {
            List<byte> bl = new List<byte>();
            char[] ss = icdData.ToLower().ToCharArray();


            for (int i = 0; i < ss.Length / 2; i++)
            {
                int x = 0;
                if (ss[i * 2] >= 'a' && ss[i * 2] <= 'f' && ss[i * 2 + 1] >= 'a' && ss[i * 2 + 1] <= 'f')
                    x = 16 * (ss[i * 2] - 'a' + 10) + (ss[i * 2 + 1] - 'a' + 10);

                else if (ss[i * 2] >= '0' && ss[i * 2] <= '9' && ss[i * 2 + 1] >= 'a' && ss[i * 2 + 1] <= 'f')
                    x = 16 * (ss[i * 2] - '0') + (ss[i * 2 + 1] - 'a' + 10);

                else if (ss[i * 2] >= 'a' && ss[i * 2] <= 'f' && ss[i * 2 + 1] >= '0' && ss[i * 2 + 1] <= '9')
                    x = 16 * (ss[i * 2] - 'a' + 10) + (ss[i * 2 + 1] - '0');

                else x = 16 * (ss[i * 2] - '0') + (ss[i * 2 + 1] - '0');

                bl.Add((byte)(x & 0xff));

            }
            return bl;
        }

        public string GetData()
        {
            if (Param.Data.StartsWith("0x"))
            {
                var datas = Param.Data.Split("0x");
                return datas[1];
            }
            else
            {
                return Param.Data;
            }
        }
    }
}
