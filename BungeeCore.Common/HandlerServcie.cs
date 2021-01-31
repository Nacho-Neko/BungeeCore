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
        private static readonly Dictionary<int, Type> AnonymouseHandler = new Dictionary<int, Type>();
        private static readonly Dictionary<int, Type> PlayerHandler = new Dictionary<int, Type>();

        public HandlerServcie(ILogger<HandlerServcie> Logger)
        {
            this.Logger = Logger;

            ushort HandlerCount = 0;

            Assembly assembly = Assembly.Load("BungeeCore.Service");
            Type[] types = assembly.GetExportedTypes();
            foreach (Type type in types)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(type, true);
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is PacketHandler packetHandler)
                    {
                        switch (packetHandler.Rose)
                        {
                            case Rose.Anonymouse: AnonymouseHandler.Add(packetHandler.PakcetId, type); break;
                            case Rose.Player: PlayerHandler.Add(packetHandler.PakcetId, type); break;
                        }
                        HandlerCount++;
                        break;
                    }
                }
            };
            Logger.LogInformation($"Handler Count : {HandlerCount}");
        }

        public Type IHandler(int PacketId, Rose rose)
        {
            if (rose == Rose.Anonymouse)
            {
                if (AnonymouseHandler.TryGetValue(PacketId, out Type type))
                {
                    return type;
                }
            }

            if (rose == Rose.Player)
            {
                if (PlayerHandler.TryGetValue(PacketId, out Type type))
                {
                    return type;
                }
            }

            return null;
        }
    }
}
