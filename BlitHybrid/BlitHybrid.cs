
using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class BlitHybrid {

        private Thread coreThread, listenThread;
        private Mutex mutex = new Mutex();

        public BlitHybrid () {}
    }
}
