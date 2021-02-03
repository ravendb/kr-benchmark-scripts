using System;

namespace RavenLibrary.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Karma Karma { get; set; }

        public DateTimeOffset Created { get; set; }
    }

    public class Karma
    {
        public int Comments { get; set; }

        public int Links { get; set; }
    }
}
