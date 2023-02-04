using Controls.Logic.Input;
using Drone.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controls.View.UI
{
    public class PlaneView : MonoBehaviour
    {
        [SerializeField]
        RectTransform uiView;

        [SerializeField]
        Text temperatureLable;

        [SerializeField]
        Image temperatureFillment;

        [SerializeField]
        Text gyroLable;

        [SerializeField]
        Text accelerometerLable;

        [SerializeField]
        PlaneView3D planeView3DPrefab;
        PlaneView3D planeView3D;
        PlaneView3D PlaneView3D
        {
            get
            {
                if(!planeView3D)
                {
                    planeView3D = Instantiate(planeView3DPrefab);
                }
                return planeView3D;
            }
        }

        TcpDroneSender sender;
        Queue<DataMpu> dataMpuResponses = new Queue<DataMpu>();

        bool Is3DEnabled
        {
            get => uiView.gameObject.activeSelf;
            set => uiView.gameObject.SetActive(value);
        }

        #region INTERFACE

        public void Init(TcpDroneSender sender)
        {
            this.sender = sender;
        }

        public void OnDataMpu(DataMpu data)
        {
            dataMpuResponses.Enqueue(data);
        }

        #endregion

        #region UI

        public void OnMpuButton()
        {
            Is3DEnabled = true;
        }

        public void OnBackButton()
        {
            Is3DEnabled = false;
        }

        #endregion

        IEnumerator Start()
        {
            var refreshDeltaTime = new WaitForSeconds(0.1f);

            while (enabled)
            {
                if (Is3DEnabled)
                {
                    sender.RequestDataMpu();
                }

                yield return refreshDeltaTime;
            }
        }

        void OnEnable()
        {
            PlaneView3D.Show(true);
            dataMpuResponses.Clear();
            ApplyDataMpu(new DataMpu());
        }

        void OnDisable()
        {
            PlaneView3D.Show(false);
        }

        void Update()
        {
            if (Is3DEnabled && dataMpuResponses.Count > 0)
            {
                var data = dataMpuResponses.Dequeue();
                ApplyDataMpu(data);
            }
        }

        void ApplyDataMpu(DataMpu data)
        {
            ApplyTemperature(data.temperature);
            ApplyGyro(data.gyro);
            ApplyAccelerometer(data.accelerometer);
        }

        void ApplyTemperature(float temperature)
        {
            temperatureLable.text = $"{temperature:0.00} °C";
            temperatureFillment.fillAmount = Mathf.Clamp01(temperature / 100.0f);
        }

        void ApplyGyro(DataVector3 gyro)
        {
            gyroLable.text = $"GYRO:\nX:{gyro.x:0.00}\nY:{gyro.y:0.00}\nZ:{gyro.z:0.00}";
            PlaneView3D.Rotate(gyro.x, gyro.y, gyro.z);
        }

        void ApplyAccelerometer(DataVector3 accelerometer)
        {
            accelerometerLable.text = $"ACCELEROMETER:\nX:{accelerometer.x:0.00}\nY:{accelerometer.y:0.00}\nZ:{accelerometer.z:0.00}";
            PlaneView3D.Move(accelerometer.x, accelerometer.y, accelerometer.z);
        }
    }
}