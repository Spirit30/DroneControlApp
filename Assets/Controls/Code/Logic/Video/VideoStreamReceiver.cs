using Controls.View.UI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Controls.Logic.Video
{
    class VideoStreamReceiver : MonoBehaviour
    {
        [SerializeField]
        CameraView cameraView;

        UdpClient socket;
        int port;

        const byte JPG_PRE_LAST = 255;
        const byte JPG_LAST = 217;

        Queue<byte[]> segmentFrames = new Queue<byte[]>();
        Queue<byte[]> completeFrames = new Queue<byte[]>();

        bool IsAlive { get; set; } = true;

        public void Connect(string ip, int port)
        {
            this.port = port;

            socket = new UdpClient();
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Connect(endPoint);

            Task.Run(SendLoop);
            Task.Run(ReceiveLoop);
        }

        async Task SendLoop()
        {
            while (IsAlive)
            {
                try
                {
                    SendTick();
                    await Task.Delay(1000);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"VideoStreamReceiver.SendLoop ERROR: {exception}");
                }
            }
        }

        void SendTick()
        {
            var handShakeMessage = new byte[] { 255 };
            socket.Send(handShakeMessage, handShakeMessage.Length);
        }

        void ReceiveLoop()
        {
            while(IsAlive)
            {
                try
                {
                    ReceiveTick();
                }
                catch (Exception exception)
                {
                    Debug.LogError($"VideoStreamReceiver.ReceiveLoop ERROR: {exception}");
                }
            }
        }

        void ReceiveTick()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, port);
            var bytes = socket.Receive(ref endPoint);
            //Debug.Log($"Received {bytes.Length} Video bytes.");

            //Skip 0 symbols at the end
            int i = bytes.Length - 1;

            while(bytes[i] == 0)
            {
                --i;
            }

            //If end segment of the frame
            if(bytes[i] == JPG_LAST && bytes[i - 1] == JPG_PRE_LAST)
            {
                var completeFrame = new List<byte>();

                while(segmentFrames.Count > 0)
                {
                    var segment = segmentFrames.Dequeue();
                    completeFrame.AddRange(segment);
                }

                completeFrame.AddRange(bytes);

                completeFrames.Enqueue(completeFrame.ToArray());
            }
            else
            {
                segmentFrames.Enqueue(bytes);
            }
        }

        void Update()
        {
            if(completeFrames.Count > 0)
            {
                var frame = completeFrames.Dequeue();
                //Debug.Log($"Update frame from {frame.Length} bytes.");
                cameraView.UpdateFrameJPG(frame);
            }
        }

        void OnDestroy()
        {
            IsAlive = false;

            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
                socket = null;
            }
        }
    }
}