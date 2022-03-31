using SuperSimpleTcp;
using System.Collections.Generic;

namespace SKRecording
{
    class TCPServer : TCPPeer
    {

        protected SimpleTcpServer server;
        protected List<string> clientIPPorts;
       
        public TCPServer(string ip, int port, char startSymbol, char endSymbol, char seperatorSymbol) : base(startSymbol, endSymbol, seperatorSymbol)
        {
            clientIPPorts = new List<string>();
            server = new SimpleTcpServer(ip + ":" + port);
            server.Events.DataReceived += OnDataReceived;
            server.Events.ClientDisconnected += OnDisconnected;
            server.Events.ClientConnected += OnConnected;
        }

        protected override void OnConnected(object sender, ConnectionEventArgs e)
        {
            lock (clientIPPorts)
            {
                clientIPPorts.Add(e.IpPort);
            }
        }
        protected override void OnDisconnected(object sender, ConnectionEventArgs e)
        {
            lock (clientIPPorts)
            {
                clientIPPorts.Remove(e.IpPort);
            }
        }

        public override void connect()
        {
            server.Start();
        }

        public override void disconnect()
        {
            server.Stop();
        }

        public override void reset()
        {
            server.Stop();
            receivedCount = 0;
        }

        public override void send(string toSend, bool raw = false)
        {
            lock (clientIPPorts)
            {
                foreach (string ipPort in clientIPPorts)
                {
                    if (raw)
                    {
                        server.Send(toSend, ipPort);
                        continue;
                    }
                    server.Send(toSend + seperatorSymbol.ToString(), ipPort);
                }

            }
        }

        public int getClientCount()
        {
            lock (clientIPPorts)
            {
                return clientIPPorts.Count;
            }
        }
    }
}
