using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions;

namespace BungeeCore.Model.ServerBound
{
    [Packet(PakcetId = 1, Bound = Bound.Server)]
    public class EncryptionResponse
    {
        public VarInt SharedSecretLength;
        public byte[] SharedSecret { get; set; }
        public VarInt VerifyTokenLength;
        public byte[] VerifyToken { get; set; }
    }
}
