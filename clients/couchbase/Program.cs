using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;

namespace CouchLibrary
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Startup.Cluster = await Cluster.ConnectAsync("172.31.37.146,172.31.44.133,172.31.41.235", "Administrator", "Library");
            Startup.TheBucket = await Startup.Cluster.BucketAsync("Library");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
