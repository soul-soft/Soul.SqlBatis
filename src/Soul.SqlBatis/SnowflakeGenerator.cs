using System;
using System.Threading;

namespace Soul.SqlBatis
{
    public static class SnowflakeGenerator
    {
        private static int _counter = 0;

        private static long _timestamp = 0;

        private readonly static object _locker = new object();

        private static long GenerateTimestamp(int year)
        {
            lock (_locker)
            {
                var timestamp = (long)(DateTime.Now - new DateTime(year, 1, 1)).TotalMilliseconds;
                if (_timestamp > timestamp)
                {
                    throw new InvalidOperationException("Abnormal system clock");
                }
                if (timestamp >= 2199023255552L)
                {
                    throw new InvalidOperationException("Timestamp overflow");
                }
                return timestamp;
            }
        }

        private static long GenerateNumber(long timestamp)
        {
            lock (_locker)
            {
                if (_timestamp == timestamp)
                {
                    _counter++;
                }
                else
                {
                    _counter = 0;
                }
                _timestamp = timestamp;
                return _counter;
            }
        }

        public static long Generate(long mac, int year = 1970)
        {
            if (mac >= 1024)
            {
                throw new InvalidOperationException("Mac overflow");
            }
            do
            {
                var timestamp = GenerateTimestamp(year);
                var number = GenerateNumber(timestamp);
                if (number >= 4096)
                {
                    Thread.Sleep(1);
                    continue;
                }
                var p1 = timestamp << 22;
                var p2 = mac << 12;
                var p3 = number;
                return p1 | p2 | p3;
            } while (true);

        }

        public static long Generate(long mac)
        {
            return Generate(mac, 1970);
        }
    }
}
