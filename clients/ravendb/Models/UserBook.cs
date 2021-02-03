using System;

namespace RavenLibrary.Models
{
    public class UserBook
    {
        public string Id { get; set; }

        public string user { get; set; }

        public string book { get; set; }

        public int Stars { get; set; }

        public DateTimeOffset At { get; set; }
    }
}
