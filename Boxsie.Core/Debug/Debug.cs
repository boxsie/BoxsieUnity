using System;
using System.Diagnostics;

namespace Boxsie.Core.Debug
{
    public static class Debug
    {
        public static bool IsEnabled { get; private set; }

        private static readonly Stopwatch LogTimer;
        private static string _timerName;
        private static bool _stackTraceEnabled;
        private static Action<string> _onOutput;

        static Debug()
        {
            IsEnabled = false;
            LogTimer = new Stopwatch();
        }

        public static void Enable(Action<string> onOutput, bool enableStackTrace = true)
        {
            IsEnabled = true;
            _stackTraceEnabled = enableStackTrace;

            _onOutput = onOutput;
        }

        public static void StartTimer(string timerName, bool reset = true)
        {
            if (reset)
                LogTimer.Reset();

            _timerName = timerName;
            LogTimer.Start();
        }

        public static void StopTimer()
        {
            LogTimer.Stop();
            Log(string.Concat(_timerName, " completed in ", LogTimer.Elapsed.Seconds, ".", LogTimer.Elapsed.Milliseconds, "s"));
        }

        public static void Log(object obj, DebugLogType logType = DebugLogType.Info, int traceSkip = 1)
        {
            if (!IsEnabled)
                return;

            var header = "";

            if (_stackTraceEnabled)
            {
                var stackFrame = new StackFrame(traceSkip);
                var method = stackFrame.GetMethod();

                if (method != null)
                    header = string.Concat("[", method.DeclaringType?.Name ?? "", ":", method.Name, "]");
            }

            _onOutput.Invoke($"[{DateTime.Now:HH:mm:ss}]{header}[{logType}] - {obj}");
        }

        public static void LogDataMessage(bool isIncoming, object ip, object transId, object dataLen, object hub, object action, object msgType)
        {
            var logVars = new[]
            {
                isIncoming
                    ? "MSG-IN From:"
                    : "MSG-OUT To:",
                ip,
                transId,
                dataLen,
                hub,
                action,
                msgType
            };

            Log(string.Format("{0} '{1}' TId: '{2}' L: '{3}b' H: '{4}' A: '{5}' MT: '{6}'", logVars), DebugLogType.Info, 2);
        }

        public static void LogShortDataMessage(bool isIncoming, object ip, object dataLen)
        {
            var logVars = new[]
            {
                isIncoming
                    ? "MSG-IN From:"
                    : "MSG-OUT To:",
                ip,
                dataLen,
            };

            Log(string.Format("{0} '{1}' L: '{2}b'", logVars), DebugLogType.Info, 2);
        }
    }
}
