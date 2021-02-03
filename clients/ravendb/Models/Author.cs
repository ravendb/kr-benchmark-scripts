using System;

namespace RavenLibrary.Models
{
    public class Author
    {
        public string Id { get; set; }

        public string name { get; set; }

        public DateTimeOffset created { get; set; }

        public int death_date { get; set; }

        public DateTimeOffset last_modified { get; set; }

        public int latest_revision { get; set; }

        public int birth_date { get; set; }

        public string personal_name { get; set; }

        public RemoteIds remote_ids { get; set; }
        
        public int revision { get; set; }
    }

    public class RemoteIds
    {
        public string viaf { get; set; }

        public string wikidata { get; set; }

        public string isni { get; set; }
    }


}
