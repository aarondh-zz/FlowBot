﻿using FlowBot.Common.Interfaces;
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
using Newtonsoft.Json;

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
            public IOrderedQueryable<IBookmark> List(OrderBy orderBy = OrderBy.Unordered)
            {
                return _dataService._container.Bookmarks;
            }

            public IBookmark Read(Guid id)
            {
                return _dataService._container.Bookmarks.FirstOrDefault(bm => bm.Id == id);
            }

            public IBookmark Read(Guid workflowInstanceId, string bookmarkName, BookmarkStates state = BookmarkStates.Undefined)
            {
                return _dataService._container.Bookmarks.FirstOrDefault(bm => bm.WorkflowInstance.InstanceId == workflowInstanceId && bm.Name == bookmarkName && (state == BookmarkStates.Undefined || bm.State == state));
            }
            public int CancelAllWaiting(Guid workflowInstanceId)
            {
                int canceled = 0;
                var waitingBookmarks = _dataService._container.Bookmarks.Where(bm => bm.WorkflowInstance.InstanceId == workflowInstanceId && bm.State == BookmarkStates.Waiting).ToList();
                foreach( var waitingBookmark in waitingBookmarks )
                {
                    waitingBookmark.State = BookmarkStates.Canceled;
                    canceled++;
                }
                _dataService._container.SaveChanges();
                return canceled;
            }
            public IOrderedQueryable<IBookmark> List(string bookmarkName = null, Guid? instanceId = null, string ownerDisplayName = null, BookmarkStates? state = null, OrderBy orderBy = OrderBy.OldestToNewest)
            {
                IQueryable<Bookmark> query = _dataService._container.Bookmarks;
                if (bookmarkName != null)
                {
                    query = query.Where(bm => bm.Name == bookmarkName);
                }
                if (ownerDisplayName != null)
                {
                    query = query.Where(bm => bm.OwnerDisplayName == ownerDisplayName);
                }
                if (instanceId.HasValue)
                {
                    query = query.Where(bm => bm.WorkflowInstance.InstanceId == instanceId.Value);
                }
                if (state.HasValue)
                {
                    query = query.Where(bm => bm.State == state.Value);
                }
                switch (orderBy)
                {
                    case OrderBy.NewestToOldest:
                        return query.OrderBy(bm => bm.CreateDate);
                    case OrderBy.OldestToNewest:
                        return query.OrderByDescending(bm => bm.CreateDate);
                    case OrderBy.ByNameAssending:
                        return query.OrderBy(bm => bm.Name);
                    case OrderBy.ByNameDescending:
                        return query.OrderByDescending(bm => bm.Name);
                    case OrderBy.Unordered:
                    default:
                        return query.OrderBy(bm => bm.Id);
                }
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

            public IOrderedQueryable<IConversation> List(OrderBy orderBy = OrderBy.Unordered)
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

            public IExternalTask Create(IWorkflowInstance workflowInstance, string externalTaskTypeName, string externalId, string userGroupName, object inputData, string bookmarkName)
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
                    ExternalId = externalId,
                    ExternalTaskType = (ExternalTaskType)externalTaskType,
                    State = ExternalTaskStates.Queued,
                    InputData = null,
                    BookmarkName = bookmarkName,
                    UserGroup = (UserGroup)userGroup,
                    WorkflowInstance = (WorkflowInstance)workflowInstance
                };
                if (inputData != null)
                {
                    newExternalTask.InputData = JsonConvert.SerializeObject(inputData);
                }
                _dataService._container.ExternalTasks.Add(newExternalTask);
                _dataService._container.SaveChanges();
                return newExternalTask;
            }

            public void Delete(IExternalTask obj)
            {
                _dataService._container.ExternalTasks.Remove((ExternalTask)obj);
                _dataService._container.SaveChanges();
            }

            public IOrderedQueryable<IExternalTask> List(OrderBy orderBy = OrderBy.Unordered)
            {
                return List(orderBy: orderBy);
            }
            public IOrderedQueryable<IExternalTask> List(string groupName = null, Guid? workerId = null, ExternalTaskStates? state = null, OrderBy orderBy = OrderBy.Unordered)
            {
                IQueryable<IExternalTask> query = _dataService._container.ExternalTasks;
                if (groupName != null)
                {
                    query = query.Where(et => et.UserGroup.Name == groupName);
                }
                if (workerId.HasValue)
                {
                    query = query.Where(et => et.Worker != null && et.Worker.Id == workerId.Value);
                }
                if (state.HasValue)
                {
                    query = query.Where(et => et.State == state.Value);
                }
                switch (orderBy)
                {
                    case OrderBy.NewestToOldest:
                        return query.OrderBy(wi => wi.CreateDate);
                    case OrderBy.OldestToNewest:
                        return query.OrderByDescending(wi => wi.CreateDate);
                    case OrderBy.ByNameAssending:
                        return query.OrderBy(wi => wi.BookmarkName);
                    case OrderBy.ByNameDescending:
                        return query.OrderByDescending(wi => wi.BookmarkName);
                    case OrderBy.Unordered:
                    default:
                        return query.OrderBy(wi=> wi.Id);
                }
            }

            public IExternalTask Read(Guid id)
            {
                return _dataService._container.ExternalTasks.Where(et => et.Id == id).FirstOrDefault();
            }
            public void SetState(IExternalTask task, Guid workerId, ExternalTaskStates state, string outputData)
            {
                var dbTask = task as ExternalTask;
                if (workerId != Guid.Empty)
                {
                    User worker = _dataService.Users.Read(workerId) as User;
                    if (worker == null)
                    {
                        throw new ArgumentException($"worker {workerId} was not found.", "workerId");
                    }
                    dbTask.Worker = worker;
                }
                dbTask.State = state;
                switch(state)
                {
                    case ExternalTaskStates.Claimed:
                        dbTask.ClaimDate = DateTime.UtcNow;
                        break;
                    case ExternalTaskStates.Completed:
                    case ExternalTaskStates.Error:
                    case ExternalTaskStates.Failed:
                        dbTask.ClaimDate = DateTime.UtcNow;
                        break;
                }
                if ( outputData != null )
                {
                    dbTask.OutputData = outputData;
                }
                this.Update(dbTask);
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

            public IOrderedQueryable<IExternalTaskType> List(OrderBy orderBy = OrderBy.Unordered)
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

            public IOrderedQueryable<IMessage> List(OrderBy orderBy = OrderBy.Unordered)
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

            public IOrderedQueryable<IUser> List(OrderBy orderBy = OrderBy.Unordered)
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

            public IOrderedQueryable<IUserGroup> List(OrderBy orderBy = OrderBy.Unordered)
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
                Workflow workflow = new Workflow()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Package = obj.Package,
                    Name = obj.Name,
                    Major = obj.Major,
                    Minor = obj.Minor,
                    Build = obj.Build,
                    Revision = obj.Revision
                };

                _dataService._container.Workflows.Add(workflow);
                _dataService._container.SaveChanges();
                return workflow;
            }

            public void Delete(IWorkflow obj)
            {
                _dataService._container.Workflows.Remove((Workflow)obj);
                _dataService._container.SaveChanges();
            }

            public IOrderedQueryable<IWorkflow> List(string packageName = null, string workflowName = null, OrderBy orderBy = OrderBy.Unordered)
            {
                IQueryable<Workflow> query = _dataService._container.Workflows;
                if (packageName != null)
                {
                    query = query.Where(wf => wf.Package == packageName);
                }
                if (workflowName != null)
                {
                    query = query.Where(wf => wf.Name == workflowName);
                }
                switch (orderBy)
                {
                    case OrderBy.NewestToOldest:
                        return query.OrderBy(w => w.CreateDate);
                    case OrderBy.OldestToNewest:
                        return query.OrderByDescending(w => w.CreateDate);
                    case OrderBy.ByNameAssending:
                        return query.OrderBy(w => w.Package).ThenBy(w => w.Name).ThenBy(w => w.Major).ThenBy(w => w.Minor).ThenBy(w => w.Revision).ThenBy(w => w.Build);
                    case OrderBy.ByNameDescending:
                        return query.OrderBy(w => w.Package).ThenBy(w => w.Name).ThenBy(w => w.Major).ThenBy(w => w.Minor).ThenBy(w => w.Revision).ThenBy(w => w.Build);
                    case OrderBy.Unordered:
                    default:
                        return query.OrderBy(wf=>wf.Id);
                }
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

            IOrderedQueryable<IWorkflow> IDataProvider<IWorkflow>.List(OrderBy orderBy = OrderBy.Unordered)
            {
                return List(null, null, orderBy);
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

            public IOrderedQueryable<IWorkflowInstance> List(OrderBy orderBy = OrderBy.Unordered)
            {
                switch( orderBy )
                {
                    case OrderBy.NewestToOldest:
                        return _dataService._container.WorkflowInstances.OrderBy(wi => wi.CreateDate);
                    case OrderBy.OldestToNewest:
                        return _dataService._container.WorkflowInstances.OrderByDescending(wi => wi.CreateDate);
                    case OrderBy.ByNameAssending:
                    case OrderBy.ByNameDescending:
                    case OrderBy.Unordered:
                    default:
                        return _dataService._container.WorkflowInstances;
                }
            }

            public IWorkflowInstance Read(Guid id)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.Id == id);
            }
            public IWorkflowInstance ReadByInstanceId(Guid instanceId)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.InstanceId == instanceId);
            }
            public IWorkflowInstance Read(IWorkflowHandle workflowHandle)
            {
                return ReadByInstanceId(workflowHandle.InstanceId);
            }

            public IWorkflowInstance Read(string externalId, WorkflowInstanceStates state = WorkflowInstanceStates.Undefined, string bookMarkName = null)
            {
                return _dataService._container.WorkflowInstances.FirstOrDefault(wi => wi.ExternalId == externalId && (state == WorkflowInstanceStates.Undefined || wi.State == state) && (bookMarkName == null || wi.Bookmarks.Any(bm=>bm.Name == bookMarkName)));
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
                        _dataService.Bookmarks.CancelAllWaiting(workflowInstance.InstanceId);
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
