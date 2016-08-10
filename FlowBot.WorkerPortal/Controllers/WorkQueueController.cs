using FlowBot.Common.Interfaces.Services;
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
            var tasks = _dataService.ExternalTasks.List(group).ToList();
            return View(tasks);
        }
    }
}