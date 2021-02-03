using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents.Subscriptions;
using System.Threading.Tasks;
using Couchbase;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace import
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var store = new DocumentStore
            {
                Urls = new[]{"https://i.huge.ravendb.run"},
                Database = "Library",
                Certificate = new X509Certificate2(@"/home/ubuntu/RavenDB/Server/cluster.server.certificate.huge.pfx")
            };
            store.Initialize();

            var subscription = store.Subscriptions.GetSubscriptionWorker<JObject>(new SubscriptionWorkerOptions("All"){
                MaxDocsPerBatch = 4096,
            });


            var cluster = await Cluster.ConnectAsync("172.31.38.235,172.31.47.223,172.31.47.54","Administrator", "Library");
            var library = await cluster.BucketAsync("Library");
            var collection = library.DefaultCollection();
            var sem = new SemaphoreSlim(32); // max concurrent requests
            await subscription.Run(async batch => {
                var sp = Stopwatch.StartNew();
                var tasks = new List<Task>();
                foreach(var item in batch.Items){
                    await sem.WaitAsync();
                    var t = collection.UpsertAsync(item.Id, item.Result)
                        .ContinueWith(_ => sem.Release());
                    tasks.Add(t);
                }
                await Task.WhenAll(tasks.ToArray());
                System.Console.WriteLine($"Wrote {batch.Items.Count} in {sp.Elapsed}");
            });
        }
    }
}
