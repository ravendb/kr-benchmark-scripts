using System;
using System.ComponentModel.DataAnnotations;

namespace CouchLibrary.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public Karma Karma { get; set; }
    }

    public class Karma
    {
        public long Comments {get;set;}
        public long Links {get;set;}
    }
}
