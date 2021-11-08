
using System;

namespace BlitzBit {

    public partial class BlitServer {

        public Action<string> onLog;
        private void Log (string message) {

            if (onLog != null) onLog(message);
        }

        public Action<string> onWarning;
        private void LogWarning (string message) {

            if (onError != null) onWarning(message);
        }

        public Action<string> onError;
        private void LogError (string message) {

            if (onError != null) onError(message);
        }
    }
}
