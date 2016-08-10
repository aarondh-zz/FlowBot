using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using System.Linq;
using System.Web.Mvc;

namespace FlowBot.WorkerPortal.Controllers
{
    public class WorkQueueController : Controller
    {
        private IDataService _dataService;
        public WorkQueueController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET: WorkQueue
        public ActionResult Index(string group = "Worker")
        {
            var tasks = _dataService.ExternalTasks.List(groupName:group, state: ExternalTaskStates.Queued, orderBy: OrderBy.OldestToNewest).ToList();
            return View(tasks);
        }
    }
}