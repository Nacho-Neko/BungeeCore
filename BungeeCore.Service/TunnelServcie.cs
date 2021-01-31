using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    public class TunnelServcie
    {
        private readonly ILogger Logger;
        private readonly ClientCore ClientCore;
        public Type PacketTypes { get; private set; }

        private IEnumerator<bool> enumerator;
        public Stack<Action> Actions = new Stack<Action>();

        public TunnelServcie(ILogger<TunnelServcie> Logger, ClientCore ClientCore)
        {
            this.Logger = Logger;
            this.ClientCore = ClientCore;

            enumerator = Enumerator();
        }
        private IEnumerator<bool> Enumerator()
        {
            while (true)
            {
                Action action = Actions.Pop();
                action();
                yield return true;
            }
        }
        public void Next()
        {
            enumerator.MoveNext();
        }
    }
}
