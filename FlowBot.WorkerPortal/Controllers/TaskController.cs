using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Interfaces.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using FlowBot.WorkerPortal.Models;

namespace FlowBot.WorkerPortal.Controllers
{
    public class TaskController : Controller
    {
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
            return View( new ExternalTaskViewModel(task));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Done(Guid id, string action)
        {
            var task = _dataService.ExternalTasks.Read(id);
            if (task == null)
            {
                return HttpNotFound($"Task {id} was not found");
            }
            return Redirect("/WorkQueue");
        }
    }
}