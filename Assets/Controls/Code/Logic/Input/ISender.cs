using System;
using UnityEngine;

namespace Controls.Logic.Input
{
    public interface ISender
    {
        public event Action OnConnect;
        public event Action<string> OnConnectFail;
        public event Action OnPingLost;
        public event Action OnPing;
        public event Action<string> OnDisconnect;
        public bool IsConnected { get; }
        void Connect();
        void ReConnect();
        void Send(Vector2 left, Vector2 right);
    }
}