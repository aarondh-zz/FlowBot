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
        private class BookmarkDataProvider : IBookmarkDataProvider
        {
            private DataService _dataService;
            public BookmarkDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IBookmark Create(IBookmark obj)
            {
                var newBookmark = new Bookmark()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Name = obj.Name,
                    OwnerDisplayName = obj.OwnerDisplayName,
                    WorkflowInstance = (WorkflowInstance)obj.WorkflowInstance
                };


                _dataService._container.Bookmarks.Add(newBookmark);
                _dataService._container.SaveChanges();

                return newBookmark;
            }

            public void Delete(IBookmark obj)
            {
                _dataService._container.Bookmarks.Remove((Bookmark)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IBookmark> List()
            {
                return _dataService._container.Bookmarks;
            }

            public IBookmark Read(Guid id)
            {
                return _dataService._container.Bookmarks.FirstOrDefault(bm => bm.Id == id);
            }

            public IBookmark Read(string externalId, string bookmarkName)
            {
                return _dataService._container.Bookmarks.FirstOrDefault(bm => bm.WorkflowInstance.ExternalId == externalId && bm.Name == bookmarkName);
            }

            public void Update(IBookmark obj)
            {
                _dataService._container.SaveChanges();
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

            public IQueryable<IConversation> List()
            {
                return _dataService._container.Conversations;
            }

            public IConversation Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IConversation obj)
            {
                _dataService._container.SaveChanges();
            }
        }
        private class ExternalTaskDataProvider : IExternalTaskDataProvider
        {
            private DataService _dataService;
            public ExternalTaskDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }
            public IExternalTask Create(IExternalTask obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IExternalTask obj)
            {
                throw new NotImplementedException();
            }

            public IQueryable<IExternalTask> List()
            {
                throw new NotImplementedException();
            }

            public IExternalTask Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IExternalTask obj)
            {
                throw new NotImplementedException();
            }
        }
        private class ExternalTaskTypeDataProvider : IExternalTaskTypeDataProvider
        {
            private DataService _dataService;
            public ExternalTaskTypeDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }
            public IExternalTaskType Create(IExternalTaskType obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IExternalTaskType obj)
            {
                throw new NotImplementedException();
            }

            public IQueryable<IExternalTaskType> List()
            {
                throw new NotImplementedException();
            }

            public IExternalTaskType Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IExternalTaskType obj)
            {
                throw new NotImplementedException();
            }
        }
        private class MessageDataProvider : IMessageDataProvider
        {
            private DataService _dataService;
            public MessageDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IMessage Create(IMessage obj)
            {
                var newMessage = new Message() {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    From = (User)obj.From,
                    To = (User)obj.To,
                    Body = obj.Body,
                };
                _dataService._container.Messages.Add(newMessage);
                _dataService._container.SaveChanges();
                return newMessage;
            }

            public void Delete(IMessage obj)
            {
                _dataService._container.Messages.Remove((Message)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IMessage> List()
            {
                return _dataService._container.Messages;
            }

            public IMessage Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IMessage obj)
            {
                _dataService._container.SaveChanges();
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

            public IQueryable<IUser> List()
            {
                return _dataService._container.Users;
            }

            public IUser Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IUser obj)
            {
                _dataService._container.SaveChanges();
            }
        }
        private class UserGroupDataProvider : IUserGroupDataProvider
        {
            private DataService _dataService;
            public UserGroupDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }
            public IUserGroup Create(IUserGroup obj)
            {
                throw new NotImplementedException();
            }

            public void Delete(IUserGroup obj)
            {
                throw new NotImplementedException();
            }

            public IQueryable<IUserGroup> List()
            {
                throw new NotImplementedException();
            }

            public IUserGroup Read(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Update(IUserGroup obj)
            {
                throw new NotImplementedException();
            }
        }
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

            public IQueryable<IWorkflow> List()
            {
                return _dataService._container.Workflows;
            }

            public IWorkflow Read(string package, string name, Nullable<int> major = null, Nullable<int> minor = null, Nullable<int> build = null, Nullable<int> revision = null)
            {
                var workflows = from w in _dataService._container.Workflows
                                where w.Package == package
                                && w.Name == name
                                && (major == null || w.Major == major)
                                && (minor == null || w.Minor == minor)
                                && (build == null || w.Build == build)
                                && (revision == null || w.Revision == revision)
                                orderby w.Major, w.Minor, w.Build, w.Revision descending
                                select w;
                return workflows.FirstOrDefault();
            }

            public IWorkflow Read(Guid id)
            {
                return _dataService._container.Workflows.FirstOrDefault(wf => wf.Id == id);
            }

            public void Update(IWorkflow obj)
            {
                _dataService._container.SaveChanges();
            }
        }
        private class WorkflowInstanceDataProvider : IWorkflowInstanceDataProvider
        {
            private DataService _dataService;
            public WorkflowInstanceDataProvider(DataService dataService)
            {
                _dataService = dataService;
            }

            public IWorkflowInstance Create(IWorkflowInstance obj)
            {
                var newWorkflowInstance = new WorkflowInstance()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    InstanceId = obj.InstanceId,
                    ExternalId = obj.ExternalId,
                    Workflow = (Workflow)obj.Workflow
                };
                _dataService._container.WorkflowInstances.Add(newWorkflowInstance);
                _dataService._container.SaveChanges();
                return newWorkflowInstance;
            }

            public void Delete(IWorkflowInstance obj)
            {
                _dataService._container.WorkflowInstances.Remove((WorkflowInstance)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IWorkflowInstance> List()
            {
                return _dataService._container.WorkflowInstances;
            }

            public IWorkflowInstance Read(Guid id)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.Id == id);
            }

            public IWorkflowInstance Read(string externalId)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.ExternalId == externalId);
            }

            public void Update(IWorkflowInstance obj)
            {
                _dataService._container.SaveChanges();
            }
        }

       public DataService()
        {
            _container = new FlowBotModelContainer();
            this.Bookmarks = new BookmarkDataProvider(this);
            this.Conversations = new ConversationDataProvider(this);
            this.ExternalTasks = new ExternalTaskDataProvider(this);
            this.ExternalTaskTypes = new ExternalTaskTypeDataProvider(this);
            this.Messages = new MessageDataProvider(this);
            this.Users = new UserDataProvider(this);
            this.UserGroups = new UserGroupDataProvider(this);
            this.Workflows = new WorkflowDataProvider(this);
            this.WorkflowInstances = new WorkflowInstanceDataProvider(this);
        }

        public IBookmarkDataProvider Bookmarks { get; }
        public IConversationDataProvider Conversations { get; }
        public IExternalTaskDataProvider ExternalTasks { get; }
        public IExternalTaskTypeDataProvider ExternalTaskTypes { get; }
        public IMessageDataProvider Messages { get; }
        public IUserDataProvider Users { get; }
        public IUserGroupDataProvider UserGroups { get; }
        public IWorkflowDataProvider Workflows { get; }
        public IWorkflowInstanceDataProvider WorkflowInstances { get; }
    }
}
