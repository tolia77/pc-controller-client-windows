using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

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
        public IPAddress IpAddress { get; set; }
        public Socket AutoSocket
        {
            get { return new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); }
            set { }
        }

        public SocketInfo(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
            MoveMouseEndpoint = new IPEndPoint(IpAddress, 50000);
            GetScreenshotEndpoint = new IPEndPoint(IpAddress, 50001);
            OpenLinkEndpoint = new IPEndPoint(IpAddress, 50002);
            CmdExecuteEndpoint = new IPEndPoint(IpAddress, 50003);
            ShutdownPcEndpoint = new IPEndPoint(IpAddress, 50004);
            RestartPcEndpoint = new IPEndPoint(IpAddress, 50005);

        }
    }
    public class SocketRequests
    {
        private readonly IPAddress ipAddress;
        private readonly SocketInfo socketInfo;
        public SocketRequests(string ip)
        {
            ipAddress = IPAddress.Parse(ip);
            socketInfo = new(ipAddress);
            IPEndPoint endpoint = new(ipAddress, 49999);
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
            Socket socketSender = socketInfo.AutoSocket;
            string data = ""; socketSender.Connect(socketInfo.MoveMouseEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes($"{repeats} {interval}");
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;

        }
        public string OpenLinkRequest(string link)
        {
            Socket socketSender = socketInfo.AutoSocket;
            string data = ""; socketSender.Connect(socketInfo.OpenLinkEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes(link);
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;
        }
        public string CmdExecuteRequest(string command)
        {
            Socket socketSender = socketInfo.AutoSocket;
            string data = ""; socketSender.Connect(socketInfo.CmdExecuteEndpoint);
            byte[] msg = Encoding.ASCII.GetBytes(command);
            socketSender.Send(msg);
            byte[] bytes = ReceiveAll(socketSender);
            data += Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return data;
        }
        public string ShutdownPcRequest()
        {
            socketInfo.AutoSocket.Connect(socketInfo.ShutdownPcEndpoint);
            return "SUCCESS";
        }
        public string RestartPcRequest()
        {
            socketInfo.AutoSocket.Connect(socketInfo.RestartPcEndpoint);
            return "SUCCESS";
        }
        public byte[] GetScreenshotRequest()
        {
            Socket socketSender = socketInfo.AutoSocket;
            socketSender.Connect(socketInfo.GetScreenshotEndpoint);
            byte[] bytes = ReceiveAll(socketSender);
            return bytes;
        }
    }
}
