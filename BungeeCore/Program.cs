using Autofac;
using Autofac.Extensions.DependencyInjection;
using BungeeCore.Common;
using BungeeCore.Common.Sockets;
using BungeeCore.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftTunnel.Core;
using System.IO;
using System.Reflection;
using System.Text;

namespace BungeeCore
{
    public class Program
    {
        private static IHost host;
        public static IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddHostedService<ServerListen>();
                 
                 })
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.yaml");
                     configApp.AddYamlFile(path, optional: true);
                     configApp.AddCommandLine(args);
                 })
                 .ConfigureLogging((hostContext, configLogging) =>
                 {
                     configLogging.AddConsole();
                     configLogging.AddDebug();
                 })
                 .UseConsoleLifetime()
                 .UseServiceProviderFactory(new AutofacServiceProviderFactory(ApplicationContainer));

        private static void ApplicationContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<HandlerServcie>().SingleInstance();
            containerBuilder.RegisterType<AnalysisService>().SingleInstance();
        }
    }
}
