using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Subscriptions;
using Raven.Client.Json;
using Raven.Client.ServerWide.Operations;

public class Program
{
    public class Book
    {
        public string Id;
        public List<string> creator { get; set; }
        public int downloads { get; set; }
        public string issued { get; set; }
        public List<string> language { get; set; }
        public string publisher { get; set; }
        public string rights { get; set; }
        public List<string> subject { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }
    public class Karma
    {
        public int Comments { get; set; }
        public int Links { get; set; }
    }

    public class User
    {
        public DateTimeOffset Created { get; set; }
        public Karma Karma { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Updated { get; set; }
        public List<string> Subjects = new List<string>();
    }

    public class UserBook
    {
        public string text;
        public string book, user;
        public int start;
        public DateTime at;
    }

    public class Quote
    {
        public string Text;
        public int Start;
    }

    private static Quote ToQuote(JObject j)
    {
        var txt = j.Value<string>("text");
        var firstPeriod = txt.IndexOf('.');
        var lastPeriod = txt.LastIndexOf('.');
        if (firstPeriod == -1 || firstPeriod == lastPeriod)
        {
            return new Quote
            {
                Text = txt,
                Start = j.Value<int>("start")
            };
        }
        return new Quote
        {
            Start = j.Value<int>("start") + firstPeriod,
            Text = txt.Substring(firstPeriod + 1, lastPeriod - firstPeriod - 1).Trim('.', ',', ' ', '\t')
        };
    }

    static async Task ImportUsersBooks(IDocumentStore store, DirectoryInfo directory, FileInfo snippets, int seed = 435)
    {
        var subjects = new[]
        {
            "Short stories",
            "Fiction",
            "Science fiction",
            "Adventure stories",
            "Historical fiction",
            "Poetry",
            "Love stories",
            "Detective and mystery stories",
            "Western stories",
            "Children's stories"
        };

        var baseDate = new DateTime(2015, 4, 10);
        var booksBySubject = subjects.Select(subject =>
        {
            using var s = store.OpenSession();
            var list = s.Query<Book>().Where(x => (object)x.subject == (subject)).OrderByDescending(x => x.downloads).ToList();
            return (subject, list);
        }).ToDictionary(x => x.subject, x => x.list);

        var stream = new GZipStream(File.OpenRead(snippets.FullName), CompressionMode.Decompress);
        var quotesById = JArray.Load(new JsonTextReader(new StreamReader(stream)))
            .GroupBy(x => x.Value<string>("id"))
            .ToDictionary(x => x.Key, x => x.Select(j => ToQuote((JObject)j)).ToList());
        var quotesIds = quotesById.Keys.ToArray();

        var random = new Random(seed);

        var name = await store.Subscriptions.CreateAsync<User>();
        var subscription = store.Subscriptions.GetSubscriptionWorker<User>(
            new SubscriptionWorkerOptions(name)
            {
                MaxDocsPerBatch = 4096,
                CloseWhenNoDocsLeft = true
            });

        using var bulk = store.BulkInsert();
        await subscription.Run(async batch =>
        {
           foreach (var user in batch.Items)
           {
               var topics = random.Next(0, 3) == 1 ? random.Next(1, 3) : 0;
               for (int i = 0; i < topics; i++)
               {
                   var subject = subjects[random.Next(subjects.Length)];

                   int numberOfBooks;
                   var luck = random.Next(100);
                   if (luck < 50)
                   {
                       numberOfBooks = random.Next(1, 3);
                   }
                   else if (luck < 90)
                   {
                       numberOfBooks = random.Next(3, 25);
                   }
                   else if (luck < 95)
                   {
                       numberOfBooks = random.Next(10, 200);
                   }
                   else if (luck < 97)
                   {
                       numberOfBooks = random.Next(100, 750);
                   }
                   else if (luck == 97)
                   {
                       numberOfBooks = random.Next(500, 1500);
                   }
                   else if (luck == 98)
                   {
                       numberOfBooks = random.Next(1000, 2500);
                   }
                   else
                   {
                       numberOfBooks = random.Next(1500, 5000);
                   }

                   var books = booksBySubject[subject];

                   for (int j = 0; j < numberOfBooks; j++)
                   {
                       int range;
                       luck = random.Next(100);
                       if (luck < 50)
                       {
                           range = Math.Min(books.Count, 25);
                       }
                       else if (luck < 90)
                       {
                           range = books.Count / 10;
                       }
                       else if (luck < 95)
                       {
                           range = books.Count / 4;
                       }
                       else if (luck < 99)
                       {
                           range = books.Count / 2;
                       }
                       else
                       {
                           range = books.Count;
                       }

                       var book = books[random.Next(range)];
                       luck = random.Next(100);
                       int numberOfQuotes;
                       if (luck < 50)
                       {
                           numberOfQuotes = random.Next(0, 3);
                       }
                       else if (luck < 80)
                       {
                           numberOfQuotes = random.Next(0, 15);
                       }
                       else if (luck < 90)
                       {
                           numberOfQuotes = random.Next(3, 20);
                       }
                       else if (luck < 95)
                       {
                           numberOfQuotes = random.Next(15, 100);
                       }
                       else if (luck < 99)
                       {
                           numberOfQuotes = random.Next(500);
                       }
                       else
                       {
                           numberOfQuotes = random.Next(250, 850);
                       }

                       for (int k = 0; k < numberOfQuotes; k++)
                       {
                           if (quotesById.TryGetValue(book.Id, out var quotes) == false)
                           {
                               quotes = quotesById[quotesIds[random.Next(quotesById.Count)]];
                           }
                           var q = quotes[random.Next(quotes.Count)];
                           bulk.Store(new UserBook
                           {
                               text = q.Text,
                               start = q.Start,
                               book = book.Id,
                               user = user.Id,
                               at = baseDate.AddMinutes(2_500_000)
                           }, "UsersBooks/" + user.Id + "-" + book.Id + "/");
                       }
                   }
               }
           }
       });
    }

    static void InsertRdfFiles(IDocumentStore store, DirectoryInfo directory)
    {       
        var bulk = new ThreadLocal<BulkInsertOperation>(() => store.BulkInsert(), true);        

        XNamespace marcel = "http://id.loc.gov/vocabulary/relators/";

        int i = 0;
        var files = Directory.GetFiles(directory.FullName, "*.rdf", SearchOption.AllDirectories);
        Parallel.ForEach(files, file =>
        {
            using var f = File.OpenRead(file);
            var x = XDocument.Load(f);

            var set = new HashSet<string>
            {
                "subject",
                "bookshelf",
                "creator",
                "language",
                "description",
                "alternative"
            };
            var ignore = new HashSet<string>
            {
                "hasFormat",
                "license"
            };

            string id = null;
            var obj = new JObject();
            foreach (XElement c in x.Root.Nodes())
            {
                switch (c.Name.LocalName)
                {
                    case "Description":
                        var s = c.FirstAttribute?.Value;
                        if (string.IsNullOrEmpty(s))
                            continue;
                        obj[((XElement)c.FirstNode).Value] = s;
                        break;
                    case "ebook":
                    {
                        obj["@metadata"] = new JObject
                        {
                            ["@collection"] = "Books",
                            ["@id"] = id = c.FirstAttribute?.Value
                        };
                        foreach (XElement item in c.Nodes())
                        {
                            if (item.Name.Namespace == marcel) continue;

                            if (ignore.Contains(item.Name.LocalName)) continue;
                            if (item.Name.LocalName.StartsWith("marc")) continue;

                            var exists = obj.ContainsKey(item.Name.LocalName);
                            if (set.Contains(item.Name.LocalName))
                            {
                                JArray a;
                                if (exists == false)
                                    obj[item.Name.LocalName] = a = new JArray();
                                else
                                    a = obj.Value<JArray>(item.Name.LocalName);
                                a.Add(GetValue(item));
                            }
                            else if (exists)
                            {
                                Console.WriteLine(file + " - " + item.Name.LocalName);
                            }
                            else
                            {
                                obj[item.Name.LocalName] = GetValue(item);
                            }
                        }

                        break;
                    }
                }
            }

            bulk.Value.Store(obj, id);
        });

        foreach (var value in bulk.Values)
            value.Dispose();
    }

    private static JToken GetValue(XElement x)
    {
        return x.DescendantNodes().OfType<XElement>().FirstOrDefault(c => c.Name.LocalName == "value")?.Value ?? x.Value;
    }

    private static async Task ImportUsers(IDocumentStore store, FileInfo file, int maxUsers)
    {
        using var reader = new StreamReader(file.FullName);
        reader.ReadLine();  // Discard headers

        using var bulk = store.BulkInsert();

        string line = reader.ReadLine();
        int counter = 0;
        do
        {
            // We are not going to insert more than the maximum users requested.
            if (counter > maxUsers)
                break;

            var parts = line.Split(",");

            await bulk.StoreAsync(new JObject
            {
                ["Name"] = parts[1],
                ["Created"] = DateTimeOffset.FromUnixTimeSeconds(int.Parse(parts[2])),
                ["Updated"] = DateTimeOffset.FromUnixTimeSeconds(int.Parse(parts[3])),
                ["Karma"] = new JObject
                {
                    ["Comments"] = long.Parse(parts[4]),
                    ["Links"] = long.Parse(parts[5]),
                }
            }, "users/" + parts[0], new MetadataAsDictionary
            {
                ["@collection"] = "Users"
            });

            line = reader.ReadLine();
            counter++;
        }
        while (line != null);
    }

    static async Task Import(string directory, string databaseName, string databaseUrl, int? maxUsers)
    {
        using var store = new DocumentStore
        {
            Database = databaseName,           
            Urls = new[] { databaseUrl }
        }.Initialize();

        // Figure out if the directory exists.
        var directoryInfo = new DirectoryInfo(directory);
        if (!directoryInfo.Exists)
        {
            Console.WriteLine($"Directory '{directoryInfo.FullName}' does not exist.");
            Environment.Exit(-2);
        }
        
        // Figure out if the reddit accounts database is accessible.
        var accountsInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "69M_reddit_accounts.csv"));
        if (!accountsInfo.Exists)
        {
            Console.WriteLine($"Reddit accounts not found. Download the file from https://www.reddit.com/r/datasets/comments/9i8s5j/dataset_metadata_for_69_million_reddit_users_in/ and unzip it in the directory.");
            Environment.Exit(-3);
        }

        var projectGutembergInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, "rdf-files"));
        if (!projectGutembergInfo.Exists)
        {
            Console.WriteLine($"Project Gutemberg not found. Download the 'rdf-files.tar' from https://www.gutenberg.org/cache/epub/feeds/ and decompress it in 'rdf-files' directory.");
            Environment.Exit(-4);
        }

        var snippetsInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "snippets.json.gz"));
        if (!snippetsInfo.Exists)
        {
            Console.WriteLine($"The snippets file was not found. Download the file from https://www.gutenberg.org/cache/epub/feeds/ and store it in the directory.");
            Environment.Exit(-4);
        }

        // Ensure database has been created.
        var result = store.Maintenance.Server.Send(new GetDatabaseRecordOperation(databaseName));
        if (result == null)
        {
            Console.WriteLine($"Database with name '{databaseName}' does not exist on the server.");
            Environment.Exit(-4);
        }

        await ImportUsers(store, accountsInfo, maxUsers ?? int.MaxValue);
        InsertRdfFiles(store, projectGutembergInfo);
        await ImportUsersBooks(store, directoryInfo, snippetsInfo);        
    }

    static async Task<int> Main(params string[] args)
    {
        RootCommand rootCommand = new RootCommand(description: "Converts an image file from one format to another.")
        {
            TreatUnmatchedTokensAsErrors = true            
        };

        rootCommand.AddOption(
            new Option(new string[] { "--directory", "--dir" })
            {
                Argument = new Argument<string>(),
                Description = "The directory where all the data is found.",               
                IsRequired = true
            });

        rootCommand.AddOption(
            new Option(aliases: new string[] { "--databaseName", "--dbname" })
            {
                Argument = new Argument<string>(),
                Description = "The database name where the data will be inserted.",
                IsRequired = true
            });

        rootCommand.AddOption(
            new Option(aliases: new string[] { "--databaseUrl", "--url" })
            {
                Argument = new Argument<string>(),
                Description = "The database url where the data will be inserted.",
                IsRequired = true
            });

        rootCommand.AddOption(
            new Option(aliases: new string[] { "--maxUsers" })
            {
                Argument = new Argument<int?>(),
                Description = "The maximum number of users",
            });

        rootCommand.Handler = CommandHandler.Create<string, string, string, int?>(Import);

        return await rootCommand.InvokeAsync(args);
    }
}