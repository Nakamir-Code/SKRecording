using SuperSimpleTcp;
using System.Collections.Generic;

namespace SKRecording
{
    // TCP Server for receiving data. Supports multiple clients connecting at once.
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

        // When a new client connects, add their ip and port to the list of clientIPs
        protected override void OnConnected(object sender, ConnectionEventArgs e)
        {
            lock (clientIPPorts)
            {
                clientIPPorts.Add(e.IpPort);
            }
        }

        // When a client disconnects, remove their ip and port from the list of clientIPs
        protected override void OnDisconnected(object sender, ConnectionEventArgs e)
        {
            lock (clientIPPorts)
            {
                clientIPPorts.Remove(e.IpPort);
            }
        }

        // Start up the server
        public override void connect()
        {
            server.Start();
        }

        // Shut down the server
        public override void disconnect()
        {
            server.Stop();
        }

        // Resetting shuts down the server and resets the number of packets received
        public override void reset()
        {
            server.Stop();
            receivedCount = 0;
        }

        // Send a string to each client. raw bool indicated is we want to add a seperator for subsequent packets.
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

        // Returns the number of connected clients
        public int getClientCount()
        {
            lock (clientIPPorts)
            {
                return clientIPPorts.Count;
            }
        }
    }
}
