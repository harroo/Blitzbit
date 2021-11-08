
using System;
using System.Threading;

namespace BlitzBit {

    public partial class BlitClient {

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        public BlitClient (string address, int port) {

            Connect(address, port);
        }
        public BlitClient () {}

        public void Close () {

            Disconnect();
        }
    }
}
