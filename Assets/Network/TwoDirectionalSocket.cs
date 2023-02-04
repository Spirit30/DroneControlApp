using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Drone.Network
{
    /// <summary>
    /// Realtime connection.
    /// </summary>
    public class TwoDirectionalSocket : Socket
    {
        #region REFERENCES

        Socket socket;

        #endregion

        #region DATA

        const int BUFFER_SIZE = 8196;

        #endregion

        #region VARIABLES

        byte[] buffer = new byte[BUFFER_SIZE];
        bool isAlive;

        #endregion

        #region GETTERS

        public bool CanSendBytes => socket != null;

        #endregion

        #region EVENTS

        public event Action<IPAddress> OnClientConnect = delegate { };
        public event Action OnClientDisconnect = delegate { };
        public event Action<byte> OnReceiveByte = delegate { };

        #endregion

        #region INTERFACE

        public TwoDirectionalSocket() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            isAlive = true;
        }

        #region CLIENT

        public void ConnectTo(string otherIp, int otherPort)
        {
            Console.WriteLine($"Connect to IP: {otherIp} PORT: {otherPort}");
            Connect(otherIp, otherPort);
            socket = this;
            Task.Run(ReceivingLoop);
        }

        public void SendBytes(byte[] bytes)
        {
            Console.WriteLine($"Send {bytes.Length} bytes.");
            socket.Send(bytes);
        }

        #endregion

        #region LISTENER

        public void ListenAt(int thisPort)
        {
            var endPoint = new IPEndPoint(IPAddress.Any, thisPort);
            Console.WriteLine($"Listen at PORT: {thisPort}");
            Bind(endPoint);
            Listen(10);
            Task.Run(AcceptingLoop);
        }

        #endregion

        public new void Dispose()
        {
            base.Dispose();
            isAlive = false;
        }

        #endregion

        #region IMPLEMENTATION

        void AcceptingLoop()
        {
            Console.WriteLine("Enter Accepting Loop.");

            while (isAlive)
            {
                try
                {
                    Console.WriteLine("Waiting for Client...");
                    socket = Accept();
                    Console.WriteLine("Client connected.");

                    var remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;
                    OnClientConnect(remoteEndPoint.Address);

                    ReceivingLoop();
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(exception);
                    DisconnectClient("exception");
                }
            }

            Console.WriteLine("Exit Accepting Loop.");
        }

        void ReceivingLoop()
        {
            Console.WriteLine("Enter Receiving Loop.");

            while (isAlive && socket != null)
            {
                Console.WriteLine("Waiting to receive...");
                int count = socket.Receive(buffer);
                Console.WriteLine($"Received {count} bytes.");

                if (count == 0)
                {
                    DisconnectClient("0 bytes");
                    break;
                }

                for (int i = 0; i < count; ++i)
                {
                    OnReceiveByte(buffer[i]);
                }
            }

            Console.WriteLine("Exit Receiving Loop.");
        }

        void DisconnectClient(string reason)
        {
            Console.WriteLine($"Client is disconnected (reason: {reason}).");
            socket = null;
            OnClientDisconnect();
        }

        #endregion
    }
}