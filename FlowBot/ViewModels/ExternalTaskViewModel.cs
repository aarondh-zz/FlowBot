﻿using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Models;
using FlowBot.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FlowBot.ViewModels
{
    public class ExternalTaskViewModel : IExternalTask
    {
        public string BookmarkName { get; set; }

        public DateTime? ClaimDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string ExternalId { get; set; }
        IExternalTaskType IExternalTask.ExternalTaskType
        {
            get
            {
                return null;
            }
        }
        public string ExternalTaskTypeName { get; set; }

        public Guid Id { get; set; }

        public ExternalTaskStates State { get; set; }

        [JsonConverter(typeof(PassthroughStringConverter))]
        public string InputData { get; set; }

        [JsonConverter(typeof(PassthroughStringConverter))]
        public string OutputData { get; set; }

        IUserGroup IExternalTask.UserGroup
        {
            get
            {
                return null;
            }
        }

        public string UserGroupName { get; set; }

        IUser IExternalTask.Worker
        {
            get
            {
                return null;
            }
        }
        public string WorkerId { get; set; }

        IWorkflowInstance IExternalTask.WorkflowInstance
        {
            get
            {
                return null;
            }
        }

        public Guid WorkflowInstanceId { get; set; }

        public ExternalTaskViewModel(IExternalTask externalTask)
        {
            this.BookmarkName = externalTask.BookmarkName;
            this.ClaimDate = externalTask.ClaimDate;
            this.CompletionDate = externalTask.CompletionDate;
            this.CreateDate = externalTask.CreateDate;
            this.ExternalId = externalTask.ExternalId;
            this.ExternalTaskTypeName = externalTask.ExternalTaskType.Name;
            this.State = externalTask.State;
            this.Id = externalTask.Id;
            this.InputData = externalTask.InputData;
            this.OutputData = externalTask.OutputData;
            this.UserGroupName = externalTask.UserGroup.Name;
            this.WorkerId = externalTask.Worker?.ExternalId;
            this.WorkflowInstanceId = externalTask.WorkflowInstance.InstanceId;
        }
        public ExternalTaskViewModel()
        {
        }
    }
}