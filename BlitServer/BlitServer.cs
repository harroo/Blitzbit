
using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class BlitServer {

        private Thread coreThread;
        private List<Thread> threadCache = new List<Thread>();
        private Mutex mutex = new Mutex();

        public BlitServer (string address, int port) {

            Start(IPAddress.Parse(address), port);
        }
        public BlitServer (int port) {

            Start(IPAddress.Any, port);
        }
        public BlitServer () {}
    }
}
