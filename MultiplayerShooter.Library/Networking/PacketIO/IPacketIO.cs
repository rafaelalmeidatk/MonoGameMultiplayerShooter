using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace MultiplayerShooter.Library.Networking.PacketIO
{
    internal interface IPacketIO<TRequest, TResponse>
    {
        void WriteRequest(NetOutgoingMessage msg, TRequest packetData);

        TRequest ReadRequest(NetIncomingMessage msg);

        void WriteResponse(NetOutgoingMessage msg, TResponse packetData);

        TResponse ReadResponse(NetIncomingMessage msg);
    }
}
