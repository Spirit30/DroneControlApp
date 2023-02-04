using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace Controls.Logic.Input
{
    public class PhotonSender : MonoBehaviourPunCallbacks, ISender
    {
        [SerializeField]
        PhotonView photonViewPrefab;

        public const string ROOM_NAME = "SIMULATOR";

        public event Action OnConnect = delegate { };
        public event Action<string> OnConnectFail = delegate { };
        public event Action OnPing = delegate { };
        public event Action OnPingLost = delegate { };
        public event Action<string> OnDisconnect = delegate { };
        public bool IsConnected { get; private set; }

        public float LX { get; private set; }
        public float LY { get; private set; }
        public float RX { get; private set; }
        public float RY { get; private set; }

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void ReConnect()
        {
            PhotonNetwork.JoinRoom(ROOM_NAME);
        }

        public void Send(Vector2 left, Vector2 right)
        {
            LX = left.x;
            LY = left.y;
            RX = right.x;
            RY = right.y;
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            Debug.Log("OnConnectedToMaster");

            PhotonNetwork.JoinRoom(ROOM_NAME);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("OnJoinedRoom");

            PhotonNetwork.Instantiate(photonViewPrefab.name, Vector3.zero, Quaternion.identity);

            IsConnected = true;
            OnConnect();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            Debug.LogError("OnJoinRoomFailed");

            OnConnectFail($"returnCode: {returnCode}, message: {returnCode}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            Debug.Log("OnPlayerLeftRoom");

            PhotonNetwork.LeaveRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            Debug.Log("OnDisconnected");

            IsConnected = false;
            OnDisconnect($"cause: {cause}");
        }
    }
}