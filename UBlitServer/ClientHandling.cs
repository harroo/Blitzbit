
using System;
using System.Net;

namespace BlitzBit {

    public partial class UBlitServer {

        private void CoreLoop () {

            while (true) {

                byte[] recvData = serverClient.Receive(ref listenPoint);

                if (!clientPoints.Contains(listenPoint))
                    clientPoints.Add(listenPoint);

                clientPoints.ForEach(delegate(IPEndPoint point) {

                    if (point != listenPoint)

                    serverClient.Send(recvData, recvData.Length, point.Address.ToString(), point.Port);
                });
            }
        }
    }
}
