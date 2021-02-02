using System;
using System.Collections.Generic;

namespace BungeeCore.Service.Base
{
    public abstract class BaseService : IService
    {
        public abstract Type PacketTypes { get; protected set; }
        public abstract object Parameter { protected get; set; }
        /// <summary>
        /// 由客户度发送
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<bool> Prerouting()
        {
            yield return true;
        }
        /// <summary>
        /// 由服务器发送
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<bool> Postrouting()
        {
            yield return true;
        }
    }
}