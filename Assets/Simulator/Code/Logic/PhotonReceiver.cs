using Controls.Logic.Input;
using Photon.Pun;
using Photon.Realtime;
using Simulator.View;
using Simulator.View.UI;
using UnityEngine;

namespace Simulator.Logic
{
    public class PhotonReceiver : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        UIView uiView;
        public UIView UIview => uiView;

        [SerializeField]
        DroneView droneView;
        public DroneView DroneView => droneView;

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            Debug.Log("OnConnectedToMaster");

            PhotonNetwork.CreateRoom(PhotonSender.ROOM_NAME);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("OnJoinedRoom");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            Debug.Log("OnPlayerEnteredRoom");

            droneView.UpdateConnected(true);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            Debug.Log("OnPlayerLeftRoom");

            droneView.UpdateConnected(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            Debug.Log("OnDisconnected");
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}