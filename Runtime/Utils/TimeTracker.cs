using System;
using System.Collections.Generic;

namespace Utils
{
    public static class TimeTracker
    {
        private static readonly Dictionary<string, long> _durationsMap;
        private static readonly Dictionary<string, long> _timesMap;

        public static Dictionary<string, long> DurationsMap => _durationsMap;

        static TimeTracker()
        {
            _timesMap = new Dictionary<string, long>();
            _durationsMap = new Dictionary<string, long>();
        }

        public static void Start(string key)
        {
            _timesMap[key] = Environment.TickCount;
        }

        public static void Stop(string key)
        {
            if (!_durationsMap.ContainsKey(key))
            {
                _durationsMap[key] = 0;
            }

            _durationsMap[key] = _durationsMap[key] + (Environment.TickCount - _timesMap[key]);
        }
    }
}