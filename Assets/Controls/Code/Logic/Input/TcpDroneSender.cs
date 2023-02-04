using Controls.View.UI;
using Drone.Network;
using Drone.Network.Data;
using Drone.Network.Message;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Controls.Logic.Input
{
    public class TcpDroneSender : MonoBehaviour, ISender, IDisposable
    {
        public bool IsConnected { get; private set; }

        public event Action OnConnect = delegate { };
        public event Action<string> OnConnectFail = delegate { };
        public event Action OnPing = delegate { };
        public event Action OnPingLost = delegate { };
        public event Action<string> OnDisconnect = delegate { };

        /// <summary>
        /// In seconds.
        /// </summary>
        public static WaitForSeconds Interval { get; } = new WaitForSeconds(SEND_INTERVAL_SEC);

        /// <summary>
        /// In milliseconds.
        /// </summary>
        public static Task Delay => Task.Delay(SEND_INTERVAL_MS);

        const int TICK_PER_SECOND = 20;
        const float SEND_INTERVAL_SEC = 1.0f / TICK_PER_SECOND;
        const int SEND_INTERVAL_MS = 1000 / TICK_PER_SECOND;

        static readonly TimeSpan disconnectPingInterval = TimeSpan.FromSeconds(5);

        [SerializeField]
        CredentialsView credentialsViewPrefab;
        CredentialsView credentialsView;
        CredentialsView CredentialsView
        {
            get
            {
                if (!credentialsView)
                {
                    credentialsView = Instantiate(credentialsViewPrefab);
                }

                return credentialsView;
            }
        }

        [SerializeField]
        CameraView cameraViewPrefab;
        CameraView cameraView;
        CameraView CameraView
        {
            get
            {
                if (!cameraView)
                {
                    cameraView = Instantiate(cameraViewPrefab);
                }

                return cameraView;
            }
        }

        [SerializeField]
        PlaneView planeViewPrefab;
        PlaneView planeView;
        PlaneView PlaneView
        {
            get
            {
                if(!planeView)
                {
                    planeView = Instantiate(planeViewPrefab);
                }
                return planeView;
            }
        }

        MessageListener messageListener;
        TwoDirectionalSocket twoDirectionalSocket;

        //Input variables
        Vector2 leftJoystickDirection;
        Vector2 rightJoystickDirection;
        Vector2 cameraJoystickDirection;

        //Zoom variables
        int cameraZoomLevel;

        //Ping variables
        DateTime pingTimeStampUTC;

        #region SENDER

        public void Connect()
        {
            CredentialsView.Open(Connect, CameraView.VideoStreamReceiver.Connect);
        }

        public void Connect(string ip, int port)
        {
            ResetAll();

            messageListener = new MessageListener();
            twoDirectionalSocket = new TwoDirectionalSocket();
            Subscribe(true);

            twoDirectionalSocket.ConnectTo(ip, port);
            IsConnected = true;

            StartCoroutine(CheckPingInterval());

            OnConnect();

            //Disable indicator before first ping message
            OnPingLost();
        }

        void Subscribe(bool flag)
        {
            if(flag)
            {
                messageListener.OnMessage += OnMessage;
                twoDirectionalSocket.OnReceiveByte += messageListener.Add;
            }
            else
            {
                messageListener.OnMessage -= OnMessage;
                twoDirectionalSocket.OnReceiveByte -= messageListener.Add;
            }
        }

        public void ReConnect()
        {
            Dispose();
            Connect();
        }

        public void Send(Vector2 leftJoystickDirection, Vector2 rightJoystickDirection)
        {
            if (IsConnected)
            {
                //If input changed
                if (this.leftJoystickDirection != leftJoystickDirection
                || this.rightJoystickDirection != rightJoystickDirection
                || cameraJoystickDirection != CameraView.JoystickDirection)
                {
                    this.leftJoystickDirection = leftJoystickDirection;
                    this.rightJoystickDirection = rightJoystickDirection;
                    cameraJoystickDirection = CameraView.JoystickDirection;

                    var dataInput = new DataInput
                    {
                        leftX = leftJoystickDirection.x,
                        leftY = leftJoystickDirection.y,
                        rightX = rightJoystickDirection.x,
                        rightY = rightJoystickDirection.y,
                        cameraX = cameraJoystickDirection.x,
                        cameraY = cameraJoystickDirection.y
                    };

                    var message = new MessageToSend(MessageType.Input, dataInput);
                    SendMessage(message);
                }

                //If zoom changed
                if(cameraZoomLevel != CameraView.ZoomLevel)
                {
                    cameraZoomLevel = CameraView.ZoomLevel;

                    var dataCameraZoom = new DataCameraZoom
                    {
                        zoomLevel = cameraZoomLevel
                    };

                    var message = new MessageToSend(MessageType.Zoom, dataCameraZoom);
                    SendMessage(message);
                }
            }
        }

        #endregion

        #region DISPOSABLE

        public void Dispose()
        {
            if (twoDirectionalSocket != null)
            {
                Subscribe(false);

                twoDirectionalSocket.Disconnect(false);
                twoDirectionalSocket.Dispose();
                twoDirectionalSocket = null;
            }
        }

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            PlaneView.Init(this);
        }

        void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region IMPLEMENTATION

        void ResetAll()
        {
            StopAllCoroutines();
            pingTimeStampUTC = DateTime.MinValue;
        }

        void SendMessage(MessageToSend message)
        {
            twoDirectionalSocket.SendBytes(message.Bytes);
        }

        /// <summary>
        /// WARNING: Not Thread Safe.
        /// </summary>
        /// <param name="message">From Drone.</param>
        void OnMessage(MessageReceived message)
        {
            Debug.Log($"{message.Type} Message.");

            switch (message.Type)
            {
                case MessageType.Ping:
                    OnPingMessage();
                    break;

                case MessageType.MpuResponse:
                    PlaneView.OnDataMpu(message.ReceiveDataMpu());
                    break;

                default:
                    Debug.LogError($"Unknown Message Type: {message.Type}");
                    break;
            }
        }

        #region PING

        void OnPingMessage()
        {
            pingTimeStampUTC = DateTime.UtcNow;
        }

        IEnumerator CheckPingInterval()
        {
            WaitForSeconds interval = new WaitForSeconds(1.0f);

            while (IsConnected)
            {
                var lastPingInterval = DateTime.UtcNow - pingTimeStampUTC;

                if (lastPingInterval > disconnectPingInterval)
                {
                    OnPingLost();
                    CameraView.ShowChecker(true);
                }
                else
                {
                    OnPing();
                }

                yield return interval;
            }
        }

        #endregion

        #region MPU

        public void RequestDataMpu()
        {
            var message = new MessageToSend(MessageType.MpuRequest);
            SendMessage(message);
        }

        #endregion

        void Disconnect(string error)
        {
            Dispose();
            IsConnected = false;
            OnDisconnect(error);
        }

        #endregion
    }
}