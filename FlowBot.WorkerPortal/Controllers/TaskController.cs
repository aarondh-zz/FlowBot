using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Interfaces.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using FlowBot.WorkerPortal.Models;
using FlowBot.WorkerPortal.Utils;
using Newtonsoft.Json;
using FlowBot.Common.Utils;
using FlowBot.Common.Models;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using FlowBot.Common.Interfaces.Api;

namespace FlowBot.WorkerPortal.Controllers
{
    public class TaskController : Controller
    {
        private string FlowBotApiRoot = ConfigurationManager.AppSettings["FlowBotApiRoot"];
        private IDataService _dataService;
        public TaskController(IDataService dataService)
        {
                _dataService = dataService;
        }
        // GET: Task
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(Guid id)
        {
            var task = _dataService.ExternalTasks.Read(id);
            if ( task == null)
            {
                return HttpNotFound($"Task {id} was not found");
            }
            _dataService.ExternalTasks.SetState(task, Guid.Empty, ExternalTaskStates.Claimed, null);
            return View( new ExternalTaskViewModel(task));
        }
        public class BookmarkParameters : IBookmarkParameters
        {
            public Guid WorkflowInstanceId { get; set; }
            public string BookmarkName { get; set; }
            public string Data { get; set; }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Done(Guid id)
        {
            var task = _dataService.ExternalTasks.Read(id);
            if (task == null)
            {
                return HttpNotFound($"Task {id} was not found");
            }
            var model = new ExternalTaskViewModel(task);
            string outputData = Request.GetBodyAsJson();
            try
            {
                using (var client = new HttpClient())
                {
                    var bookmarkParameters = new BookmarkParameters()
                    {
                        WorkflowInstanceId = task.WorkflowInstance.InstanceId,
                        BookmarkName = task.BookmarkName,
                        Data = outputData
                    };
                    var response = await client.PutAsJsonAsync<BookmarkParameters>(FlowBotApiRoot + "/Bookmark", bookmarkParameters);
                    response.EnsureSuccessStatusCode();
                }
                _dataService.ExternalTasks.SetState(task, Guid.Empty, ExternalTaskStates.Completed, outputData);
            }
            catch
            {
                _dataService.ExternalTasks.SetState(task, Guid.Empty, ExternalTaskStates.Error, outputData);
                throw;
            }
            return Redirect("/WorkQueue");
        }
    }
}