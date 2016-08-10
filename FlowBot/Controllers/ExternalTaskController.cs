using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using FlowBot.Common.Utils;
using FlowBot.Extensions;
using FlowBot.ViewModels;
using JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlowBot.Controllers
{
    public class ExternalTaskController : ApiController
    {
        private IDataService _dataService;
        public ExternalTaskController( IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET: api/ExternalTask
        public IEnumerable<IExternalTask> Get(string userGroupName = null, Guid? workerId = null, ExternalTaskStates? state = null, OrderBy orderBy = OrderBy.OldestToNewest, int skip = 0, int take = 50)
        {
            var query = _dataService.ExternalTasks.List(userGroupName, workerId, state, orderBy).Skip(skip).Take(take);
            List<IExternalTask> results = new List<IExternalTask>();
            foreach( var externalTask in query)
            {
                results.Add(new ExternalTaskViewModel(externalTask));
            }
            return results;
        }

        // GET: api/ExternalTask/{guid}
        public IExternalTask Get(Guid id)
        {
            return _dataService.ExternalTasks.Read(id);
        }
        // PATCH: api/ExternalTask
        public IHttpActionResult Patch(Guid id, JsonPatchDocument<ExternalTaskViewModel> deltaExternalTask)
        {
            var originalExternalTask = _dataService.ExternalTasks.Read(id);
            if (originalExternalTask == null)
            {
                return NotFound();
            }
            var originalExternalTaskViewModel = new ExternalTaskViewModel(originalExternalTask);
            if ( deltaExternalTask.HasValidOperations<ExternalTaskViewModel>("/state","/outputdata") )
            {
                deltaExternalTask.ApplyUpdatesTo(originalExternalTaskViewModel);
                originalExternalTaskViewModel.CopyTo(originalExternalTask);
                _dataService.ExternalTasks.Update(originalExternalTask);
            }
            else
            {
                return BadRequest("Operation not allowed");
            }
            return Ok();
            
        }
    }
}
