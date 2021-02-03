using System;
using System.ComponentModel.DataAnnotations;

namespace CouchLibrary.Models
{
    public class Annotation
    {
        [Key]
        public string Id { get; set; }

        public string user { get; set; }
        
        public string book { get; set; }

        public string text { get; set; }

        public long start { get; set; }

        public DateTimeOffset at { get; set; }
    }
}
