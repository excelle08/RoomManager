using System;


namespace RoomManager
{
    public class Common
    {
        public static DateTime GetDatetime(double TimeStamp) {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            epoch = epoch.AddSeconds(TimeStamp).ToLocalTime();
            return epoch;
        }

        public static double  CurrentTimestamp() {
            return DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}