namespace BungeeCore.Common.Event
{
    /// <summary>
    /// 当收到服务器回复的数据包
    /// </summary>
    /// <param name="playerToken"></param>
    /// <param name="Packet"></param>
    public delegate void TunnelReceive(PlayerToken playerToken, byte[] Packet);
}
