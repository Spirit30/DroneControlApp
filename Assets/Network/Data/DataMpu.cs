namespace Drone.Network.Data
{
    public struct DataMpu
    {
        internal const string CODE_TEMPERATURE = "t";
        internal const string CODE_GYRO = "g";
        internal const string CODE_ACCELEROMETER = "a";
        internal const string CODE_ALL = "tga";

        public float temperature;
        public DataVector3 gyro;
        public DataVector3 accelerometer;
    }
}