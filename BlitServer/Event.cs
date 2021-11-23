
using System;
using System.Net.Sockets;

namespace BlitzBit {

    public partial class BlitServer {

        public Action<TcpClient> onClientCatch;
        private bool OnClientCatchEvent (TcpClient client) {

            if (onClientCatch == null) return false;
            onClientCatch(client); return true;
        }

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
