using System.Collections.Generic;

namespace RavenLibrary.Models
{
    public class Book
    {
        public string Id { get; set; }

        public List<string> subject { get; set; }

        public List<string> language { get; set; }

        public List<string> bookshelf { get; set; }

        public string tableOfContents { get; set; }

        public int downloads { get; set; }

        public List<string> creator { get; set; }

        public string publisher { get; set; }

        public string issued { get; set; }

        public string title { get; set; }

        public string rights { get; set; }

        public string type { get; set; }
    }
}
