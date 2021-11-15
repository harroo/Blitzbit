
using System;

namespace BlitzBit {

    public partial class BlitClient {

        public Action onDisconnect;
        private void OnDisconnectEvent () {

            if (onDisconnect != null) onDisconnect();
        }
    }
}
