using System;
using UnityEngine;

namespace Game
{
    public static class Logger
    {
        public static bool IsLogsEnabled = true;

        public static void LogInfo(object message)
        {
            if (!IsLogsEnabled)
                return;

            Debug.Log(message);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void LogWarning(object message)
        {
            if (!IsLogsEnabled)
                return;

            Debug.LogWarning(message);
        }

        public static void LogError(string error)
        {
            Debug.LogError(error);
        }
    }
}