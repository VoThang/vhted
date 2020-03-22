using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace VHTED.Api
{
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:8001");
    }
}
