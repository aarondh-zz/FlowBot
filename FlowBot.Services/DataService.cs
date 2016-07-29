using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services
{
    public class DataService : IDataService
    {
        private FlowBotModelContainer _container;
        public DataService()
        {
            _container = new FlowBotModelContainer();
        }
        public IWorkflow GetWorkflow(string name, string version=null)
        {
            var workflows = from w in _container.Workflows
                            where w.Name == name 
                            && (version==null || w.Version == version)
                            orderby w.Version descending
                            select w;
            return workflows.FirstOrDefault();
        }
    }
}
