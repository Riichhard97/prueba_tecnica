namespace Nexu.Shared
{
    public static class ServiceNames
    {
        public const string Rfid = "RFID";
        public const string Tracking = "TRACKING";
        public const string Lidar = "LiDAR";
        public const string Eye = "EYE";

        public static bool IsServiceName(string s)
        {
            return s == Rfid || s == Tracking || s == Lidar || s == Eye;
        }
    }
}
