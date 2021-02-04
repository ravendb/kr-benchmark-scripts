using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using RavenLibrary.Models;
using RavenLibrary.Raven.Indexes;

namespace RavenLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnnotationController : ControllerBase
    {
        private readonly IAsyncDocumentSession _session;

        public AnnotationController(IAsyncDocumentSession session)
        {
            _session = session;
        }

    
        [HttpGet("/annotations/user/{skip}/{take}")]
        public AsyncQueryResult<Annotation> GetUserAnnotationsRange(string userId, int skip, int take)
        {
            var query = _session.Advanced.AsyncDocumentQuery<Annotation>(indexName: "Annotations/ByUser")
                .Skip(skip)
                .Take(take)
                .WhereEquals("UserId", userId);
            
            return new AsyncQueryResult<Annotation>(_session, query);
        }

        [HttpGet("/annotations/userbook/{skip}/{take}")]
        public AsyncQueryResult<Annotation> GetUserBookAnnotations(string userBookId, int skip, int take)
        {
            var query = _session.Advanced.AsyncDocumentQuery<Annotation>(collectionName: "Annotations")
                .Skip(skip)
                .Take(take)
                .WhereStartsWith("id()", $"Annotations/{userBookId}/");
            
            return new AsyncQueryResult<Annotation>(_session, query);
        }
    }
}
