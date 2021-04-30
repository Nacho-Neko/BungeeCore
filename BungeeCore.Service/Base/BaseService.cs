using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public virtual IEnumerable<Task<bool>> Prerouting()
        {
            yield return Task.FromResult(true);
        }
        /// <summary>
        /// 由服务器发送
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Task<bool>> Postrouting()
        {
            yield return Task.FromResult(true);
        }
    }
}