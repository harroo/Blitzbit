
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitStream {

        private Mutex mutex = new Mutex();

        private NetworkStream stream;

        public BlitStream (NetworkStream stream) {

            this.stream = stream;
        }

        public void Close () {

            try {

                stream.Close();

            } catch {}
        }
    }
}
