using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BungeeCore.Services
{
    /// <summary>
    /// 插件管理服务
    /// </summary>
    public class PluginService : IHostedService
    {
        private readonly ILogger Logger;
        public PluginService(ILogger<ListenService> Logger)
        {
            this.Logger = Logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("*********开始加载插件*********");
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
