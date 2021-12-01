
using System;
using System.Threading;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class UBlitHybrid {

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        public UBlitHybrid () {}
    }
}
