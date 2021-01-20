using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions;

namespace BungeeCore.Model.ClientBound
{
    [Packet(PakcetId = 1, Bound = Bound.Client)]
    public class EncryptionRequest
    {
        public string ServerID { get; set; }
        public VarInt PublicKeyLength;
        public byte[] PublicKey { get; set; }
        public VarInt VerifyTokenLength;
        public byte[] VerifyToken { get; set; }
    }
}
