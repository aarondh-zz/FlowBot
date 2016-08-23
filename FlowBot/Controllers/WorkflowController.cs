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
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FlowBot.Controllers
{
    public class WorkflowController : ApiController
    {
        public IDataService _dataService;
        public WorkflowController(IWorkflowService workflowService, IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET: api/Workflow
        public IEnumerable<IWorkflow> Get(string package = null, string name = null, OrderBy orderBy = OrderBy.OldestToNewest, int skip = 0, int take = 50)
        {
            var query = _dataService.Workflows.List(package, name, orderBy).Skip(skip).Take(take);
            return query.ToArray().Select(wf => new WorkflowViewModel(wf)).ToArray();
        }

        // GET: api/Workflow/{guid}
        public IWorkflow Get(Guid id)
        {
            return _dataService.Workflows.Read(id);
        }
        public IHttpActionResult Post(WorkflowViewModel workflow)
        {
            try
            {
                var originalWorkflow = _dataService.Workflows.Create(workflow);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        // PATCH: api/Workflow
        [System.Web.Http.Route("api/workflow/{id}")]
        [System.Web.Http.HttpPatch]
        public IHttpActionResult Patch(Guid id, [FromBody]JsonPatchDocument<WorkflowViewModel> deltaWorkflow)
        {
            var originalWorkflow = _dataService.Workflows.Read(id);
            if (originalWorkflow == null)
            {
                return NotFound();
            }
            var originaWorkflowViewModel = new WorkflowViewModel(originalWorkflow);
            if (deltaWorkflow.HasValidOperations<WorkflowViewModel>("/body"))
            {
                deltaWorkflow.ApplyUpdatesTo(originaWorkflowViewModel);
                originaWorkflowViewModel.CopyTo(originalWorkflow);
                _dataService.Workflows.Update(originalWorkflow);
            }
            else
            {
                return BadRequest("Operation not allowed");
            }
            return Ok();

        }
        // DELETE: api/Workflow/{guid}
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var workflow = _dataService.Workflows.Read(id);
                if ( workflow == null)
                {
                    return NotFound();
                }
                _dataService.Workflows.Delete(workflow);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
