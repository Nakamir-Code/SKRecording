using SuperSimpleTcp;

namespace SKRecording
{
    // A tcp client that receives data
    class TCPClient : TCPPeer
    {

        protected SimpleTcpClient client;

        public TCPClient(string ip, int port, char startSymbol, char endSymbol, char seperatorSymbol) : base(startSymbol, endSymbol, seperatorSymbol)
        {
            client = new SimpleTcpClient(ip + ":" + port);
            client.Events.DataReceived += OnDataReceived;
            client.Events.Disconnected += OnDisconnected;
            client.Events.Connected += OnConnected;
        }

        // Connect the socket
        public override void connect()
        {
            client.Connect();
        }

        // Disconnect the socket
        public override void disconnect()
        {
            client.Disconnect();
        }

        // Reseting the client just means resetting the receivedCount
        public override void reset()
        {
            receivedCount = 0;
        }

        // Send a string using the wrapped simpletcpclient. Raw param decides if we send the exact string or if we 
        // add a seperator for subsequent packets.
        public override void send(string toSend, bool raw = false)
        {
            if (raw)
            {
                client.Send(toSend);
                return;
            }
            client.Send(toSend + seperatorSymbol.ToString());
        }
    }
}
