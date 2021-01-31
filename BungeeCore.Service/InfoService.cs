using BungeeCore.Common.Attributes;
using System;

namespace BungeeCore.Service
{
    public class InfoService
    {
        public bool Encryption;
        public string PlayerName;                            // 玩家Name
        public Rose Rose = Rose.Anonymouse;
        public DateTime ConnectDateTime;                     // 连接时间
        public DateTime EndTime;                             // 到期时间
        public InfoService()
        {

        }
        public void Info(string PlayerName)
        {
            this.PlayerName = PlayerName;
            Rose = Rose.Player;
            ConnectDateTime = DateTime.Now;
        }
    }
}
