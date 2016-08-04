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
using FlowBot.Common.Models;

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
                    State = obj.State,
                    WorkflowInstance = (WorkflowInstance)obj.WorkflowInstance
                };


                _dataService._container.Bookmarks.Add(newBookmark);
                _dataService._container.SaveChanges();

                return newBookmark;
            }

            public IBookmark Create(IWorkflowInstance workflowInstance, string bookmarkName, string ownerDisplayName, BookmarkStates state = BookmarkStates.Waiting, Nullable<DateTime> completionDate = null)
            {
                var newBookmark = new Bookmark()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Name = bookmarkName,
                    OwnerDisplayName = ownerDisplayName,
                    State = state,
                    CompletionDate = completionDate,
                    WorkflowInstance = (WorkflowInstance)workflowInstance
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

            public void SetState(IBookmark bookmark, BookmarkStates state)
            {
                var dbBookmark = (Bookmark)bookmark;
                dbBookmark.State = state;
                switch(state)
                {
                    case BookmarkStates.Undefined:
                    case BookmarkStates.Waiting:
                        dbBookmark.CompletionDate = null;
                        break;
                    case BookmarkStates.Completed:
                        dbBookmark.CompletionDate = DateTime.UtcNow;
                        break;
                }
                Update(dbBookmark);
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
                var newConversation = new Conversation()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    ExternalId = obj.ExternalId,
                    WorkflowInstance = (WorkflowInstance)obj.WorkflowInstance
                };
                _dataService._container.Conversations.Add(newConversation);
                _dataService._container.SaveChanges();
                return newConversation;
            }

            public IConversation Create(IWorkflowHandle workflowHandle, string externalId)
            {
                var workflowInstance = _dataService.WorkflowInstances.Read(workflowHandle);
                if ( workflowInstance == null)
                {
                    throw new ArgumentException($"The workflow handle {workflowHandle} does not have a corresponding workflow instance in the db", "workflowHandle");
                }
                var newConversation = new Conversation()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    ExternalId = externalId,
                    WorkflowInstance = (WorkflowInstance)workflowInstance
                };
                _dataService._container.Conversations.Add(newConversation);
                _dataService._container.SaveChanges();
                return newConversation;
            }

            public void Delete(IConversation obj)
            {
                _dataService._container.Conversations.Add((Conversation)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IConversation> List()
            {
                return _dataService._container.Conversations;
            }

            public IConversation Read(Guid id)
            {
                return _dataService._container.Conversations.Where(c => c.Id == id).FirstOrDefault();
            }

            public IConversation Read(IWorkflowHandle workflowHandle, string externalId)
            {
                return _dataService._container.Conversations.Where(c => c.WorkflowInstance.InstanceId == workflowHandle.InstanceId && c.ExternalId == externalId).FirstOrDefault();
            }
            public IConversation ReadOrCreate(IWorkflowHandle workflowHandle, string externalId)
            {
                var conversation = Read(workflowHandle, externalId);
                if ( conversation == null)
                {
                    conversation = Create(workflowHandle, externalId);
                }
                return conversation;
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
                var newExternalTask = new ExternalTask()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    ExternalTaskType = (ExternalTaskType)obj.ExternalTaskType,
                    ClaimDate = obj.ClaimDate,
                    CompletionDate = obj.CompletionDate,
                    InputData = obj.InputData,
                    OutputData = obj.OutputData,
                    UserGroup = (UserGroup)obj.UserGroup,
                    Worker = (User)obj.Worker

                };
                _dataService._container.ExternalTasks.Add(newExternalTask);
                _dataService._container.SaveChanges();
                return newExternalTask;
            }

            public IExternalTask Create(string externalTaskTypeName, string externalId, string userGroupName, object inputData)
            {
                var externalTaskType = _dataService.ExternalTaskTypes.Read(externalTaskTypeName);
                if (externalTaskType == null)
                {
                    throw new InvalidOperationException($"\"{externalTaskTypeName}\" is not a valid external task type");
                }
                var userGroup = _dataService.UserGroups.Read(userGroupName);
                if (userGroup == null)
                {
                    throw new InvalidOperationException($"\"{userGroupName}\" is not a valid user group name");
                }
                var newExternalTask = new ExternalTask()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    ExternalTaskType = (ExternalTaskType)externalTaskType,
                    InputData = null,
                    UserGroup = (UserGroup)userGroup
                };
                _dataService._container.ExternalTasks.Add(newExternalTask);
                _dataService._container.SaveChanges();
                return newExternalTask;
            }

            public void Delete(IExternalTask obj)
            {
                _dataService._container.ExternalTasks.Remove((ExternalTask)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IExternalTask> List()
            {
                return _dataService._container.ExternalTasks;
            }

            public IExternalTask Read(Guid id)
            {
                return _dataService._container.ExternalTasks.Where(et => et.Id == id).FirstOrDefault();
            }

            public void Update(IExternalTask obj)
            {
                _dataService._container.SaveChanges();
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
                _dataService._container.ExternalTaskTypes.Remove((ExternalTaskType)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IExternalTaskType> List()
            {
                return _dataService._container.ExternalTaskTypes;
            }

            public IExternalTaskType Read(Guid id)
            {
                return _dataService._container.ExternalTaskTypes.Where(ett => ett.Id == id).FirstOrDefault();
            }
            public IExternalTaskType Read( string externalTaskTypeName)
            {
                return _dataService._container.ExternalTaskTypes.Where(ett => ett.Name.Equals(externalTaskTypeName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }

            public void Update(IExternalTaskType obj)
            {
                _dataService._container.SaveChanges();
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
                    Topic = obj.Topic,
                    Body = obj.Body,
                    Locale = obj.Locale,
                };
                _dataService._container.Messages.Add(newMessage);
                _dataService._container.SaveChanges();
                return newMessage;
            }

            public IMessage Create( IConversation conversation, IUser from, IUser to, string topic, string body, string locale)
            {
                var newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Conversation = (Conversation)conversation,
                    From = (User)from,
                    To = (User)to,
                    Topic = topic,
                    Body = body,
                    Locale = locale
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
            private void SplitName(string name, out string firstName, out string lastName)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    firstName = "";
                    lastName = "";
                    return;
                }
                var components = name.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (components.Length == 1)
                {
                    firstName = "";
                    lastName = name;
                    return;
                }
                else
                {
                    firstName = components[0];
                    lastName = components[components.Length-1];
                }
            }

            public IUser Create(IUser obj)
            {
                var newUser = new User()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    ExternalId = obj.ExternalId,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                };
                _dataService._container.Users.Add(newUser);
                _dataService._container.SaveChanges();
                return newUser;
            }

            public IUser Create(string userGroupName, string externalId, string name)
            {
                string firstName;
                string lastName;
                SplitName(name, out firstName, out lastName);
                var userGroup = _dataService.UserGroups.Read(userGroupName);
                if (userGroup == null)
                {
                    throw new InvalidOperationException($"\"{userGroupName}\" is not a valid user group name");
                }
                var newUser = new User()
                {
                    Id = Guid.NewGuid(),
                    ExternalId = externalId,
                    CreateDate = DateTime.UtcNow,
                    FirstName = firstName,
                    LastName = lastName
                };
                newUser.UserGroups.Add((UserGroup)userGroup);
                _dataService._container.Users.Add(newUser);
                _dataService._container.SaveChanges();
                return newUser;
            }
            public IUser ReadOrCreate(string userGroupName, string externalId, string name)
            {
                var user = Read(externalId, name );
                if ( user == null)
                {
                    user = Create(userGroupName, externalId, name);
                }
                return user;
            }

            public void Delete(IUser obj)
            {
                _dataService._container.Users.Remove((User)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IUser> List()
            {
                return _dataService._container.Users;
            }

            public IUser Read(Guid id)
            {
                return _dataService._container.Users.Where(u=>u.Id==id).FirstOrDefault();
            }

            public void Update(IUser obj)
            {
                _dataService._container.SaveChanges();
            }

            public IUser ReadByName(string name)
            {
                string firstName;
                string lastName;
                SplitName(name, out firstName, out lastName);
                return _dataService._container.Users.Where(u => u.FirstName == firstName && u.LastName == lastName).FirstOrDefault();
            }

            public IUser ReadByExternalId(string externalId)
            {
                return _dataService._container.Users.Where(u => u.ExternalId == externalId).FirstOrDefault();
            }
            public IUser Read(string externalId, string name)
            {
                string firstName;
                string lastName;
                SplitName(name, out firstName, out lastName);
                return _dataService._container.Users.Where(u => u.ExternalId == externalId && u.FirstName == firstName && u.LastName == lastName).FirstOrDefault();
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
                var newUserGroup = new UserGroup()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Name = obj.Name
                };
                _dataService._container.UserGroups.Add(newUserGroup);
                _dataService._container.SaveChanges();
                return newUserGroup;
            }

            public void Delete(IUserGroup obj)
            {
                _dataService._container.UserGroups.Remove((UserGroup)obj);
                _dataService._container.SaveChanges();
            }

            public IQueryable<IUserGroup> List()
            {
                return _dataService._container.UserGroups;
            }

            public IUserGroup Read(Guid id)
            {
                return _dataService._container.UserGroups.Where(ug => ug.Id == id).FirstOrDefault();
            }
            public IUserGroup Read(string name)
            {
                return _dataService._container.UserGroups.Where(ug => ug.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }

            public void Update(IUserGroup obj)
            {
                _dataService._container.SaveChanges();
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
            public IWorkflow Read(IWorkflowIdentity workflowIdentity)
            {
                return Read(workflowIdentity.Package, workflowIdentity.Name, workflowIdentity.Major, workflowIdentity.Minor, workflowIdentity.Build, workflowIdentity.Revision);
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

            public IWorkflowInstance Create(IWorkflowHandle workflowHandle, WorkflowInstanceStates state = WorkflowInstanceStates.Runnable, Nullable<DateTime> completionDate = null)
            {
                var workflow = _dataService.Workflows.Read(workflowHandle.Identity);
                if ( workflow == null)
                {
                    throw new ArgumentException($"Workflow not found for identity {workflowHandle.Identity}", "workflowHandle");
                }
                var newWorkflowInstance = new WorkflowInstance()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    InstanceId = workflowHandle.InstanceId,
                    ExternalId = workflowHandle.ExternalId,
                    State = state,
                    CompletionDate = completionDate,
                    Workflow = (Workflow)workflow
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

            public IWorkflowInstance Read(IWorkflowHandle workflowHandle)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.InstanceId == workflowHandle.InstanceId);
            }

            public IWorkflowInstance Read(string externalId)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.ExternalId == externalId);
            }

            public void SetState(IWorkflowInstance workflowInstance, WorkflowInstanceStates state)
            {
                var dbWorkflowInstance = (WorkflowInstance)workflowInstance;
                dbWorkflowInstance.State = state;
                switch(state)
                {
                    case WorkflowInstanceStates.Undefined:
                    case WorkflowInstanceStates.Runnable:
                    case WorkflowInstanceStates.Idle:
                        dbWorkflowInstance.CompletionDate = null;
                        break;
                    case WorkflowInstanceStates.Aborted:
                    case WorkflowInstanceStates.Faulted:
                    case WorkflowInstanceStates.Completed:
                        dbWorkflowInstance.CompletionDate = DateTime.UtcNow;
                        break;
                }
                Update(dbWorkflowInstance);
            }
            public void SetState(IWorkflowHandle workflowHandle, WorkflowInstanceStates state)
            {
                var workflowInstance = Read(workflowHandle);
                if ( workflowInstance == null)
                {
                    throw new ArgumentException("Workflow {workflowHandle} not found in SetState", "workflowHandle");
                }
                SetState(workflowInstance, state);
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
