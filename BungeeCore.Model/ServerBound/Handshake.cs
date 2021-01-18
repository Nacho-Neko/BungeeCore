using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions;

namespace BungeeCore.Model.ServerBound
{
    [Packet(PakcetId = 0, Bound = Bound.Server)]
    public class Handshake
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public VarInt ProtocolVersion { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort ServerPort { get; set; }
        /// <summary>
        /// 下一步状态
        /// </summary>
        public VarInt nextState { get; set; }
        public NextState NextState()
        {
            return (NextState)(int)nextState;
        }
        public Handshake()
        {

        }
    }
    public enum NextState
    {
        status = 1,
        login = 2
    }
}
