using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Providers;
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
        private class WorkflowDataProvider : IWorkflowDataProvider
        {
            private DataService _dataService;
            public WorkflowDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IWorkflow Create(IWorkflow obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IWorkflow obj)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IWorkflow> List()
            {
                return from w in _dataService._container.Workflows
                       select w;
            }

            public IWorkflow Read(string name, string version = null)
            {
                var workflows = from w in _dataService._container.Workflows
                                where w.Name == name
                                && (version == null || w.Version == version)
                                orderby w.Version descending
                                select w;
                return workflows.FirstOrDefault();
            }

            public IWorkflow Read(string id)
            {
                throw new NotImplementedException();
            }

            public void Update(IWorkflow obj)
            {
                throw new NotImplementedException();
            }
        }

        private class UserDataProvider : IUserDataProvider
        {
            private DataService _dataService;
            public UserDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IUser Create(IUser obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IUser obj)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IUser> List()
            {
                throw new NotImplementedException();
            }

            public IUser Read(string id)
            {
                throw new NotImplementedException();
            }

            public void Update(IUser obj)
            {
                throw new NotImplementedException();
            }
        }
        private class ConversationDataProvider : IConversationDataProvider
        {
            private DataService _dataService;
            public ConversationDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IConversation Create(IConversation obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IConversation obj)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IConversation> List()
            {
                throw new NotImplementedException();
            }

            public IConversation Read(string id)
            {
                throw new NotImplementedException();
            }

            public void Update(IConversation obj)
            {
                throw new NotImplementedException();
            }
        }

        public DataService()
        {
            _container = new FlowBotModelContainer();
            this.Workflows = new WorkflowDataProvider(this);
            this.Users = new UserDataProvider(this);
            this.Conversations = new ConversationDataProvider(this);
        }

        public IWorkflowDataProvider Workflows { get; }
        public IUserDataProvider Users { get; }
        public IConversationDataProvider Conversations { get; }
    }
}
