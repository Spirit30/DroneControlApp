using Controls.Logic.Video;
using UnityEngine;
using UnityEngine.UI;

namespace Controls.View.UI
{
    class CameraView : MonoBehaviour
    {
        #region REFERENCES

        [SerializeField]
        RawImage streamView;

        [SerializeField]
        AspectRatioFitter streamAspectRatio;

        [SerializeField]
        Image checkerView;

        [SerializeField]
        Text cameraOutput;

        [SerializeField]
        Joystick joystickCamera;

        [SerializeField]
        Dropdown dropdownCameraZoom;

        [SerializeField]
        VideoStreamReceiver videoStreamReceiver;

        #endregion

        #region GETTERS

        public Vector2 JoystickDirection => joystickCamera.Direction;
        public int ZoomLevel => dropdownCameraZoom.value + 1;
        public VideoStreamReceiver VideoStreamReceiver => videoStreamReceiver;

        #endregion

        #region VARIABLES

        Texture2D buffer;

        #endregion

        #region INTERFACE

        public void UpdateFrameJPG(byte[] bytes)
        {
            bool success = buffer.LoadImage(bytes);

            if (success)
            {
                streamAspectRatio.aspectRatio = (float)buffer.width / buffer.height;
                ShowChecker(false);
            }
            else
            {
                Debug.LogError("Fail parse JPG.");
                ShowChecker(true);
            }
        }

        public void ShowChecker(bool flag)
        {
            checkerView.gameObject.SetActive(flag);
        }

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            buffer = new Texture2D(2, 2);
            streamView.texture = buffer;
        }

        void Update()
        {
            var value = JoystickDirection;
            cameraOutput.text = $"CX: {value.x:0.0}\nCY: {value.y:0.0}";
        }

        #endregion

        #region EDITOR

#if UNITY_EDITOR
        [ContextMenu("Test Image JPG")]
        void TestImageJPG()
        {
            string path = UnityEditor.EditorUtility.OpenFilePanel("Pick Image JPG", string.Empty, "jpg,jpeg");

            if(string.IsNullOrWhiteSpace(path))
            {
                Debug.Log("Canceled.");
                return;
            }

            var bytes = System.IO.File.ReadAllBytes(path);
            UpdateFrameJPG(bytes);
        }
#endif

        #endregion
    }
}