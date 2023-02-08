using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System;

namespace SocketApp
{
    public class SocketInfo
    {

        public IPEndPoint MoveMouseEndpoint { get; set; }
        public IPEndPoint GetScreenshotEndpoint { get; set; }
        public IPEndPoint OpenLinkEndpoint { get; set; }
        public IPEndPoint CmdExecuteEndpoint { get; set; }
        public IPEndPoint ShutdownPcEndpoint { get; set; }
        public IPEndPoint RestartPcEndpoint { get; set; }
        public IPEndPoint StreamScreenEndPoint { get; set; }
        public IPAddress IpAddress { get; set; }
        public Socket AutoSocket
        {
            get { return new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); }
            set { }
        }

        public SocketInfo(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
            MoveMouseEndpoint = new IPEndPoint(IpAddress, 40000);
            GetScreenshotEndpoint = new IPEndPoint(IpAddress, 40001);
            OpenLinkEndpoint = new IPEndPoint(IpAddress, 40002);
            CmdExecuteEndpoint = new IPEndPoint(IpAddress, 40003);
            ShutdownPcEndpoint = new IPEndPoint(IpAddress, 40004);
            RestartPcEndpoint = new IPEndPoint(IpAddress, 40005);
            StreamScreenEndPoint = new IPEndPoint(IpAddress, 40006);
        }
    }
    public class SocketRequests
    {
        private readonly IPAddress ipAddress;
        public SocketInfo SocketInfo;
        public SocketRequests(string ip)
        {
            ipAddress = IPAddress.Parse(ip);
            SocketInfo = new(ipAddress);
            IPEndPoint endpoint = new(ipAddress, 39999);
            Socket socketSender = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socketSender.Connect(endpoint);
        }
        public static byte[] ReceiveAll(Socket SocketSender)
        {
            List<byte> buffer = new();
            while (true)
            {
                byte[] currByte = new byte[1];
                int byteCounter = SocketSender.Receive(currByte, currByte.Length, SocketFlags.None);
                if (byteCounter == 0)
                {
                    return buffer.ToArray();
                }
                else
                {
                    buffer.Add(currByte[0]);
                }
            }
        }
        public string MoveMouseRequest(string repeats, string interval)
        {
            Socket socketSender = SocketInfo.AutoSocket;
            string data = ""; socketSender.Connect(SocketInfo.MoveMouseEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes($"{repeats} {interval}");
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;

        }
        public string OpenLinkRequest(string link)
        {
            Socket socketSender = SocketInfo.AutoSocket;
            string data = ""; socketSender.Connect(SocketInfo.OpenLinkEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes(link);
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;
        }
        public string CmdExecuteRequest(string command)
        {
            Socket socketSender = SocketInfo.AutoSocket;
            string data = ""; socketSender.Connect(SocketInfo.CmdExecuteEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes(command);
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;
        }
        public string ShutdownPcRequest()
        {
            SocketInfo.AutoSocket.Connect(SocketInfo.ShutdownPcEndpoint);
            return "SUCCESS";
        }
        public string RestartPcRequest()
        {
            SocketInfo.AutoSocket.Connect(SocketInfo.RestartPcEndpoint);
            return "SUCCESS";
        }
        public byte[] GetScreenshotRequest()
        {
            Socket socketSender = SocketInfo.AutoSocket;
            socketSender.Connect(SocketInfo.GetScreenshotEndpoint);
            byte[] bytes = ReceiveAll(socketSender);
            return bytes;
        }

        public static byte[] GetStreamScreen(Socket socketSender)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            recv = socketSender.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                recv = socketSender.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }
    }
}
