using FlowBot.Common.Exceptions;
using FlowBot.Common.Interfaces.Api;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using JsonPatch;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FlowBot.Controllers
{
    public class BookmarkController : ApiController

    {
        private IDataService _dataService;
        private IWorkflowService _workflowService;
        public BookmarkController(IDataService dataService, IWorkflowService workflowService)
        {
            _dataService = dataService;
            _workflowService = workflowService;
            _workflowService.SetWorkflowRootDirectory(HttpRuntime.AppDomainAppPath + "Workflow\\");
        }
        // GET: api/Bookmark
        public IEnumerable<IBookmark> Get(string bookmarkName = null, Guid? instanceId = null, string ownerDisplayName = null, BookmarkStates? state = null, OrderBy orderBy = OrderBy.OldestToNewest, int skip = 0, int take = 50)
        {
            return _dataService.Bookmarks.List(bookmarkName, instanceId, ownerDisplayName, state, orderBy).Skip(skip).Take(take);
        }
        // GET: api/Bookmark/{guid}
        public IBookmark Get(Guid id)
        {
            return _dataService.Bookmarks.Read(id);
        }
        public class BookMarkParameters : IBookmarkParameters
        {
            public string BookmarkName { get; set; }

            public string Data { get; set; }

            public Guid WorkflowInstanceId { get; set; }
        }
        public async Task<HttpResponseMessage> Put(BookMarkParameters parameters)
        {
            try
            { 
                var existingWorkflow = _workflowService.LookupWorkflow(parameters.WorkflowInstanceId);
                if (existingWorkflow == null)
                {
                    var message = $"Workflow instance {parameters.WorkflowInstanceId} was not found.";
                    Debug.WriteLine(message);
                    var apiResponse = new APIResponse(message);
                    return Request.CreateResponse<APIResponse>(HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    var bookmarkData = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters.Data);
                    existingWorkflow.Resume<Dictionary<string, object>>(parameters.BookmarkName, bookmarkData);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                var apiResponse = new APIResponse($"Bookmark \"{parameters.BookmarkName}\", Workflow instance {parameters.WorkflowInstanceId} reported {e.Message}");
                var response = Request.CreateResponse<APIResponse>(HttpStatusCode.InternalServerError, apiResponse);
                return response;
            }
        }
    }
}
