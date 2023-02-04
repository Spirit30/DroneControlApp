using Drone.Network.Data;
using System;
using System.Collections.Generic;

namespace Drone.Network.Message
{
    public struct MessageToSend
    {
        public byte[] Bytes => bytes.ToArray();
        Queue<byte> bytes;

        #region CONSTRUCT

        public MessageToSend(MessageType type)
        {
            bytes = new Queue<byte>();
            AddByte((byte)type);
        }

        public MessageToSend(MessageType type, DataMpu data) : this(type)
        {
            Serialize(data);
        }

        public MessageToSend(MessageType type, DataInput data) : this(type)
        {
            Serialize(data);
        }

        public MessageToSend(MessageType type, DataCameraZoom data) : this(type)
        {
            Serialize(data);
        }

        #endregion

        #region SERIALIZE

        void Serialize(DataInput data)
        {
            AddFloat(data.leftX);
            AddFloat(data.leftY);
            AddFloat(data.rightX);
            AddFloat(data.rightY);
            AddFloat(data.cameraX);
            AddFloat(data.cameraY);
        }

        void Serialize(DataCameraZoom data)
        {
            AddInt(data.zoomLevel);
        }

        void Serialize(DataMpu data)
        {
            AddFloat(data.temperature);
            AddFloat(data.gyro.x);
            AddFloat(data.gyro.y);
            AddFloat(data.gyro.z);
            AddFloat(data.accelerometer.x);
            AddFloat(data.accelerometer.y);
            AddFloat(data.accelerometer.z);
        }

        #endregion

        #region ADD

        void AddInt(int value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        void AddFloat(float value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        void AddBool(bool value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        void AddByte(byte @byte)
        {
            bytes.Enqueue(@byte);
        }

        void AddBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                AddByte(bytes[i]);
            }
        }

        #endregion
    }
}