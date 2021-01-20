namespace BungeeCore.Common.Event
{
    /// <summary>
    /// 当服务器接受到玩家数据包
    /// </summary>
    /// <param name="PlayerToken">连接标识</param>
    /// <param name="Packet">数据包</param>
    public delegate void ServerReceive(PlayerToken PlayerToken, byte[] Packet);
}
