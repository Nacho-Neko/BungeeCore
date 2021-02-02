using BungeeCore.Common.Helper;
using BungeeCore.Common.Helper.Protocol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    public class AnalysisService
    {
        private readonly ILogger Logger;
        private readonly bool Compression = false;
        public AnalysisService(ILogger<AnalysisService> Logger)
        {
            this.Logger = Logger;
        }
        public object MapToEntities(Type type, byte[] PacketData)
        {
            Block block = new Block(PacketData);
            return EntityMapper.MapToEntities(type, block);
        }
        public List<ProtocolHeand> AnalysisHeand(byte[] Packet)
        {
            Block block = new Block(Packet);
            List<ProtocolHeand> protocolHeands = new List<ProtocolHeand>();
            ProtocolHeand protocolHeand;
            if (Compression)
            {
                do
                {
                    protocolHeand = new ZipProtocol();
                    protocolHeand.Analyze(block);
                    protocolHeands.Add(protocolHeand);
                } while (protocolHeand.block.step < Packet.Length);
            }
            else
            {
                do
                {
                    protocolHeand = new NormalProtocol();
                    protocolHeand.Analyze(block);
                    protocolHeands.Add(protocolHeand);
                } while (protocolHeand.block.step < Packet.Length);
            }
            return protocolHeands;
        }
    }
}