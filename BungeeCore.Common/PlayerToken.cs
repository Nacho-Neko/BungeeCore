using BungeeCore.Common.Sockets;
using System;

namespace BungeeCore.Common
{
    public class PlayerToken
    {
        public ServerCore ServerCore;
        public ClientCore ClientCore;

        public string PlayerName;                            // 玩家Name

        public DateTime ConnectDateTime;                     // 连接时间
        public DateTime EndTime;                             // 到期时间

        public void StartTunnel()
        {
            ClientCore = new ClientCore(ServerCore.ILogger, ServerCore.IConfiguration);
            ClientCore.Start(this);
        }

        /*
        public void Login(string ServerAddress, ushort ServerPort)
        {
            MemoryStream memoryStream1 = new MemoryStream();
            Handshake handshake = new Handshake();
            memoryStream1.WriteInt(0);
            memoryStream1.WriteInt(Handshake.PacketId);
            memoryStream1.WriteInt(ProtocolVersion);
            memoryStream1.WriteString(ServerAddress, true);
            memoryStream1.WriteUShort(ServerPort);
            memoryStream1.WriteInt((int)NextState.login);
            byte[] buffer = new byte[memoryStream1.Position];
            int size1 = (int)memoryStream1.Position - 1;
            memoryStream1.Position = 0;
            memoryStream1.WriteInt(size1);
            Array.Copy(memoryStream1.GetBuffer(), 0, buffer, 0, buffer.Length);

            ClientCore.SendPacket(buffer);


            MemoryStream memoryStream = new MemoryStream();
            Login login = new Login();
            login.Name = PlayerName;
            memoryStream.WriteInt(0);
            memoryStream.WriteInt(login.PacketId);
            memoryStream.WriteString(PlayerName, true);
            byte[] buffer2 = new byte[memoryStream.Position];
            int size = (int)memoryStream.Position - 1;
            memoryStream.Position = 0;
            memoryStream.WriteInt(size);
            Array.Copy(memoryStream.GetBuffer(), 0, buffer2, 0, buffer2.Length);

            ClientCore.SendPacket(buffer2);
        }
        */
    }
}
