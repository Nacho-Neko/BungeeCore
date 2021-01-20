using BungeeCore.Common.Attributes;
using BungeeCore.Common.Helper;
using BungeeCore.Common.Helper.Protocol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BungeeCore.Service
{
    public class AnalysisService
    {
        private readonly ILogger Logger;

        private static Dictionary<int, Type> ClientBound = new Dictionary<int, Type>();
        private static Dictionary<int, Type> ServerBound = new Dictionary<int, Type>();
        public AnalysisService(ILogger<AnalysisService> Logger)
        {

            this.Logger = Logger;

            Assembly assembly = Assembly.Load("BungeeCore.Model");
            Type[] types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(type, true);
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is PacketAttribute packetAttribute)
                    {
                        if (packetAttribute.Bound == Bound.Client)
                            ClientBound.Add(packetAttribute.PakcetId, type);
                        else if (packetAttribute.Bound == Bound.Server)
                            ServerBound.Add(packetAttribute.PakcetId, type);
                        break;
                    }
                }
            };
        }
        public object MapToEntities(Type type, byte[] PacketData)
        {
            Block block = new Block(PacketData);
            return EntityMapper.MapToEntities(type, block);
        }
        public List<ProtocolHeand> AnalysisHeand(bool Compression, byte[] Packet)
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