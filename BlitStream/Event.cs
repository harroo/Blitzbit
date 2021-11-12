
using System;

namespace BlitzBit {

    public partial class BlitStream {

        public Action onDisconnect;
        private void OnDisconnectEvent () {

            if (onDisconnect != null) onDisconnect();
        }
    }
}
