using SuperSimpleTcp;

namespace SKRecording
{
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

        public override void connect()
        {
            client.Connect();
        }

        public override void disconnect()
        {
            client.Disconnect();
        }

        public override void reset()
        {
            receivedCount = 0;
        }

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
