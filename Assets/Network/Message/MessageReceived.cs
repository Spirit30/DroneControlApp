using Drone.Network.Data;
using System;
using System.Collections.Generic;

namespace Drone.Network.Message
{
    public struct MessageReceived
    {
        #region DATA

        public MessageType Type { get; }
        Queue<byte> bytes;

        const int BYTES_IN_INT = 4;
        const int BYTES_IN_FLOAT = 4;
        const int BYTES_IN_BOOL = 1;

        #endregion

        #region CONSTRUCT

        public MessageReceived(byte @byte)
        {
            Type = (MessageType)@byte;
            bytes = new Queue<byte>();
        }

        #endregion

        #region ADD

        public void AddByte(byte @byte)
        {
            bytes.Enqueue(@byte);
        }

        #endregion

        #region GETTERS

        public bool IsEmpty()
        {
            return Type == MessageType.Empty;
        }

        public bool IsComplete()
        {
            switch (Type)
            {
                case MessageType.Empty:
                    return false;
                case MessageType.Ping:
                    return true;
                case MessageType.Input:
                    return CanReceiveDataInput();
                case MessageType.Zoom:
                    return CanReceiveDataCameraZoom();
                case MessageType.MpuRequest:
                    return true;
                case MessageType.MpuResponse:
                    return CanReceiveDataMpu();
                default:
                    throw new Exception($"Not handled Message Type: {Type}");
            }
        }

        #endregion

        #region RECEIVE

        bool CanReceiveDataInput()
        {
            return CanReceive(0, 6, 0);
        }

        public DataInput ReceiveDataInput()
        {
            return new DataInput
            {
                leftX = ReceiveFloat(),
                leftY = ReceiveFloat(),
                rightX = ReceiveFloat(),
                rightY = ReceiveFloat(),
                cameraX = ReceiveFloat(),
                cameraY = ReceiveFloat(),
            };
        }

        bool CanReceiveDataCameraZoom()
        {
            return CanReceive(1, 0, 0);
        }

        public DataCameraZoom ReceiveCameraZoom()
        {
            return new DataCameraZoom
            {
                zoomLevel = ReceiveInt()
            };
        }

        bool CanReceiveDataMpu()
        {
            return CanReceive(0, 7, 0);
        }

        public DataMpu ReceiveDataMpu()
        {
            return new DataMpu
            {
                temperature = ReceiveFloat(),
                gyro = new DataVector3
                {
                    x = ReceiveFloat(),
                    y = ReceiveFloat(),
                    z = ReceiveFloat(),
                },
                accelerometer = new DataVector3
                {
                    x = ReceiveFloat(),
                    y = ReceiveFloat(),
                    z = ReceiveFloat(),
                }
            };
        }

        bool CanReceive(int intsCount, int floatsCount, int boolsCount)
        {
            int intBytesCount = intsCount * BYTES_IN_INT;
            int floatBytesCount = floatsCount * BYTES_IN_FLOAT;
            int boolBytesCount = boolsCount * BYTES_IN_BOOL;
            int totalBytesCount = intBytesCount + floatBytesCount + boolBytesCount;
            return CanReceive(totalBytesCount);
        }

        bool CanReceive(int bytesCount)
        {
            return bytes.Count >= bytesCount;
        }

        int ReceiveInt()
        {
            return BitConverter.ToInt32(ReceiveBytes(BYTES_IN_INT), 0);
        }

        float ReceiveFloat()
        {
            return BitConverter.ToSingle(ReceiveBytes(BYTES_IN_FLOAT), 0);
        }

        bool ReceiveBool()
        {
            return BitConverter.ToBoolean(ReceiveBytes(BYTES_IN_BOOL), 0);
        }

        byte[] ReceiveBytes(int count)
        {
            var result = new byte[count];

            for (int i = 0; i < count; ++i)
            {
                result[i] = bytes.Dequeue();
            }

            return result;
        }

        #endregion
    }
}