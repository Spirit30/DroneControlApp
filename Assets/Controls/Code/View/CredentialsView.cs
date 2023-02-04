using Controls.Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Controls.View.UI
{
    class CredentialsView : MonoBehaviour
    {
        [SerializeField]
        InputField ip;

        [SerializeField]
        InputField portSignal;

        [SerializeField]
        InputField portVideo;

        [SerializeField]
        Button startButton;

        [SerializeField]
        RectTransform credentialsPanel;

        [SerializeField]
        Text loadingText;

        Action<string, int> onConnectSignal;
        Action<string, int> onConnectVideo;

        #region INTERFACE

        public void Open(Action<string, int> onConnectSignal, Action<string, int> onConnectVideo)
        {
            this.onConnectSignal = onConnectSignal;
            this.onConnectVideo = onConnectVideo;

            ip.SetTextWithoutNotify(Persistence.Ip);
            ip.onValueChanged.AddListener(OnIpChanged);
            ip.onEndEdit.AddListener(OnIpEndEdit);

            int portSignal = Persistence.PortSignal;
            if (portSignal > 0)
            {
                this.portSignal.SetTextWithoutNotify(portSignal.ToString());
            }
            this.portSignal.onValueChanged.AddListener(OnPortSignalChanged);
            this.portSignal.onEndEdit.AddListener(OnPortSignalEndEdit);

            int portVideo = Persistence.PortVideo;
            if (portVideo > 0)
            {
                this.portVideo.SetTextWithoutNotify(portVideo.ToString());
            }
            this.portVideo.onValueChanged.AddListener(OnPortVideoChanged);
            this.portVideo.onEndEdit.AddListener(OnPortVideoEndEdit);

            startButton.onClick.AddListener(OnStartButton);

            gameObject.SetActive(true);
        }

        #endregion

        #region UNITY EVENTS

        void OnDisable()
        {
            ip.onValueChanged.RemoveAllListeners();
            ip.onEndEdit.RemoveAllListeners();
            portSignal.onValueChanged.RemoveAllListeners();
            portSignal.onEndEdit.RemoveAllListeners();
        }

        #endregion

        #region UI EVENTS

        void OnIpChanged(string potentialIp)
        {
            UpdateStartButtonView();
        }

        void OnIpEndEdit(string potentialIp)
        {
            if (Utils.IsIp(potentialIp))
            {
                Persistence.Ip = potentialIp;
            }

            UpdateStartButtonView();
        }

        void OnPortSignalChanged(string potentialPort)
        {
            UpdateStartButtonView();
        }

        void OnPortSignalEndEdit(string potentialPort)
        {
            if (Utils.IsPort(potentialPort))
            {
                Persistence.PortSignal = int.Parse(potentialPort);
            }

            UpdateStartButtonView();
        }

        void OnPortVideoChanged(string potentialPort)
        {
            UpdateStartButtonView();
        }

        void OnPortVideoEndEdit(string potentialPort)
        {
            if (Utils.IsPort(potentialPort))
            {
                Persistence.PortVideo = int.Parse(potentialPort);
            }

            UpdateStartButtonView();
        }

        void OnStartButton()
        {
            StartCoroutine(Connect());
        }

        #endregion

        #region IMPLEMENTATION

        void UpdateStartButtonView()
        {
            startButton.interactable = 
                Utils.IsIp(ip.text) && 
                Utils.IsPort(portSignal.text) &&
                Utils.IsPort(portVideo.text);
        }

        IEnumerator Connect()
        {
            yield return new WaitForEndOfFrame();
            credentialsPanel.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(true);

            Debug.Log("Connecting Signal...");
            yield return new WaitForEndOfFrame();
            onConnectSignal(ip.text, int.Parse(portSignal.text));
            Debug.Log("Connected to Signal.");

            Debug.Log("Connecting Video...");
            yield return new WaitForEndOfFrame();
            onConnectVideo(ip.text, int.Parse(portVideo.text));
            Debug.Log("Connected to Video.");

            gameObject.SetActive(false);
        }

        #endregion
    }
}
