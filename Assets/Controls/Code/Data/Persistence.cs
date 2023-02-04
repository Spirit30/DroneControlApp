using UnityEngine;

namespace Controls.Data
{
    static class Persistence
    {
        const string IP_KEY = "733cf90e-08de-4a68-b987-823668ddc53d";
        const string PORT_SIGNAL_KEY = "3f930fb5-bda7-4802-8895-bdf127dae6f4";
        const string PORT_VIDEO_KEY = "7f972c0d-c42d-43d7-88d5-5b2ad958521d";

        public static string Ip
        {
            get => PlayerPrefs.GetString(IP_KEY);
            set => PlayerPrefs.SetString(IP_KEY, value);
        }

        public static int PortSignal
        {
            get => PlayerPrefs.GetInt(PORT_SIGNAL_KEY);
            set => PlayerPrefs.SetInt(PORT_SIGNAL_KEY, value);
        }

        public static int PortVideo
        {
            get => PlayerPrefs.GetInt(PORT_VIDEO_KEY);
            set => PlayerPrefs.SetInt(PORT_VIDEO_KEY, value);
        }
    }
}