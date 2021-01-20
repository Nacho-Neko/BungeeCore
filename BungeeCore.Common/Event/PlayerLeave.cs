namespace BungeeCore.Common.Event
{
    /// <summary>
    /// 当玩家断开连接
    /// </summary>
    /// <param name="playerToken">连接标识</param>
    public delegate void PlayerLeave(PlayerToken playerToken);
}
