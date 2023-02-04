using UnityEngine;

namespace Controls.Logic
{
    using Input;
    using System;
    using System.Collections;
    using View.UI;

    public class Controller : MonoBehaviour
    {
        [SerializeField]
        Joystick joystickLeft;

        [SerializeField]
        Joystick joystickRight;

        [SerializeField]
        ControlsView view;

        [SerializeField]
        GameObject senderPrefab;

        ISender sender;

        void Start()
        {
            sender = Instantiate(senderPrefab).GetComponent<ISender>();

            if(sender == null)
            {
                throw new Exception("Invalid ISender.");
            }

            sender.Connect();
            sender.OnConnect += OnConnect;
            sender.OnConnectFail += OnConnectionFail;
            sender.OnPing += OnPing;
            sender.OnPingLost += OnPingLost;
            sender.OnDisconnect += OnDisconnect;
        }

        void Update()
        {
            var left = joystickLeft.Direction;
            var right = joystickRight.Direction;

            view.UpdateInput(left, right);

            if (sender.IsConnected)
            {
                sender.Send(left, right);
            }
        }

        void OnConnect()
        {
            view.SetConnectionIndicator(true);
        }

        void OnConnectionFail(string error)
        {
            Debug.LogError(error);
            view.SetConnectionIndicator(false);
            StartCoroutine(TryReconnect());
        }

        void OnPing()
        {
            view.SetConnectionIndicator(true);
        }

        void OnPingLost()
        {
            view.SetConnectionIndicator(false);
        }

        void OnDisconnect(string message)
        {
            Debug.Log(message);
            view.SetConnectionIndicator(false);
            StartCoroutine(TryReconnect());
        }

        IEnumerator TryReconnect()
        {
            yield return new WaitForSeconds(2.0f);
            sender.ReConnect();
        }
    }
}