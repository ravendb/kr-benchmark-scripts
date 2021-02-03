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

        [HttpGet("/annotations/user/{skip}/{take}")]
        public async Task<List<Annotation>> GetUserAnnotationsRange(string userId, int skip, int take)
        {
            string query = "SELECT RAW a FROM Library._default.Annotations a where a.`user` = ? offset ? limit ?";

            var res = await Startup.Cluster.QueryAsync<Annotation>(query, options=> options.AdHoc(false).Parameter(userId).Parameter(skip).Parameter(take));

            return await res.Rows.ToListAsync();
        }

        [HttpGet("/annotations/userbook/{skip}/{take}")]
        public async Task<List<Annotation>> GetUserBookAnnotations(string userId, string bookId, int skip, int take)
        {
            string query = "SELECT RAW a FROM Library._default.Annotations a where a.`user` = ? and a.book = ? offset ? limit ?";

            var res = await Startup.Cluster.QueryAsync<Annotation>(query, options => options.Parameter(userId).Parameter(bookId).Parameter(skip).Parameter(take).AdHoc(false));

            return await res.Rows.ToListAsync();
        }
    }
}
