
using System;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class UBlitClient {

        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private bool QueueHasContents () { bool returnValue = false; mutex.WaitOne(); try {

            returnValue = sendQueue.Count != 0;

        } finally { mutex.ReleaseMutex(); } return returnValue; }

        private byte[] PopQueue () { byte[] returnData = new byte[0]; mutex.WaitOne(); try {

            returnData = sendQueue.Dequeue();

        } finally { mutex.ReleaseMutex(); } return returnData; }

        private void AddQueue (byte[] data) { mutex.WaitOne(); try {

            sendQueue.Enqueue(data);

        } finally { mutex.ReleaseMutex(); } }
    }
}
