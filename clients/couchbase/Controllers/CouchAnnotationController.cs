using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Couchbase.Linq;
using CouchLibrary.Models;
using Couchbase.Query;
using Couchbase.Views;

namespace CouchLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CouchAnnotationController : ControllerBase
    {
        private readonly IBucket _bucket;

        public CouchAnnotationController(IBucket bucket)
        {
            _bucket = bucket;
        }

        [HttpGet("/user")]
        public async Task<object> GetUserAnnotationsRange(string userId)
        {
            try
            {
                var user = await _bucket.DefaultCollection().GetAsync(userId);
                return user.ContentAs<User>();
            }
            catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
            {
                return null;
            }
        }

        [HttpGet("/annotations/user/{skip}/{take}")]
        public async Task<object> GetUserAnnotationsRange(string userId, int skip, int take)
        {
            /*
            Prefix query with Couchbase is *slow* - got to 98% CPU and timed out on many queries

            string query = "SELECT RAW a FROM Library a WHERE meta().id LIKE ? OFFSET ? LIMIT ?";

            var res = await Startup.Cluster.QueryAsync<Annotation>(query, options=> 
                options.AdHoc(false).Parameter("Annotations/"+userId +"-%").Parameter(skip).Parameter(take));
            */

            string query = "SELECT RAW a FROM Library a WHERE a.`@metadata`.`@collection` = 'Annotations' AND a.`user` = ? OFFSET ? LIMIT ?";

            var res = await Startup.Cluster.QueryAsync<Annotation>(query, options => options.Parameter(userId).Parameter(skip).Parameter(take).AdHoc(false));


            return await res.Rows.ToListAsync();
        }

        [HttpGet("/annotations/userbook/{skip}/{take}")]
        public async Task<object> GetUserBookAnnotations(string userId, string bookId, int skip, int take)
        {
            string query = "SELECT RAW a FROM Library a WHERE a.`@metadata`.`@collection` = 'Annotations' AND a.`user` = ? AND a.book = ? OFFSET ? LIMIT ?";

            var res = await Startup.Cluster.QueryAsync<Annotation>(query, options => options.Parameter(userId).Parameter(bookId).Parameter(skip).Parameter(take).AdHoc(false));

            return await res.Rows.ToListAsync();
        }
    }
}
