
using System;

namespace BlitzBit {

    public partial class BlitServer {

        public Action<int> onClientConnect;
        private void OnClientConnectEvent (int clientId) {

            if (onClientConnect != null) onClientConnect(clientId);
        }

        public Action<int> onClientDisconnect;
        private void OnClientDisconnectEvent (int clientId) {

            if (onClientDisconnect != null) onClientDisconnect(clientId);
        }
    }
}
