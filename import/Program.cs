using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents.Subscriptions;
using System.Threading.Tasks;
using Couchbase;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
                Certificate = new X509Certificate2(@"C:\Users\Oren\Downloads\RavenDB\cluster.server.certificate.huge.pfx")
            };
            store.Initialize();

            var subscription = store.Subscriptions.GetSubscriptionWorker<JObject>(new SubscriptionWorkerOptions("All"){
                MaxDocsPerBatch = 1024,
            });


            var cluster = await Cluster.ConnectAsync("172.31.38.235,172.31.47.223,172.31.47.54","Administrator", "Library");
            var library = await cluster.BucketAsync("Library");
            var collection = library.DefaultCollection();

            await subscription.Run(async batch => {
                foreach(var item in batch.Items){
                    await collection.UpsertAsync(item.Id, item.Result);
                }
            });
        }
    }
}
