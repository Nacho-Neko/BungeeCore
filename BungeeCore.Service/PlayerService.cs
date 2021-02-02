using Microsoft.Extensions.Logging;
using System;

namespace BungeeCore.Service
{
    public class PlayerService
    {
        private readonly ChannelService channelService;
        private readonly ILogger Logger;

        public string PlayerName;                            // 玩家Name
        public DateTime ConnectDateTime;                     // 连接时间

        public PlayerService(ILogger<PlayerService> Logger, ChannelService channelService)
        {
            this.Logger = Logger;
            this.channelService = channelService;
        }
    }
}
