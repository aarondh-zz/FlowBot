using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using FlowBot.ViewModels;
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
        public IEnumerable<IExternalTask> Get(int skip = 0, int take = 50)
        {
            var query = _dataService.ExternalTasks.List(OrderBy.OldestToNewest).Skip(skip).Take(take);
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

        // POST: api/ExternalTask
        public void Post([FromBody]ExternalTaskViewModel value)
        {
            throw new NotSupportedException();
        }

        // PUT: api/ExternalTask/{guid}
        public void Put(Guid id, [FromBody]ExternalTaskViewModel value)
        {
            throw new NotSupportedException();
        }

        // DELETE: api/ExternalTask/{guid}
        public void Delete(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}
