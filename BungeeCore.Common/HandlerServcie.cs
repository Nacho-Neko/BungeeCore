using BungeeCore.Common.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BungeeCore.Service
{
    public class HandlerServcie
    {
        private readonly ILogger Logger;
        private static Dictionary<int, Type> ClientHandler = new Dictionary<int, Type>();
        // private static Dictionary<int, Type> ServerHandler = new Dictionary<int, Type>();

        public HandlerServcie(ILogger<HandlerServcie> Logger)
        {
            this.Logger = Logger;

            ushort HandlerCount = 0;

            Assembly assembly = Assembly.Load("BungeeCore.Service");
            Type[] types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(type, true);
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is PacketHandler packetHandler)
                    {
                        ClientHandler.Add(packetHandler.PakcetId, type);
                        HandlerCount++;
                        break;
                    }
                }
            };
            Logger.LogInformation($"Handler Count : {HandlerCount}");
        }

        public Type IHandler(int PacketId)
        {
            if (ClientHandler.TryGetValue(PacketId, out Type type))
            {
                return type;
            }
            return null;
        }

    }
}
