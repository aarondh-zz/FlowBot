using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using FlowBot.Common.Interfaces.Services;
using System.Diagnostics;
using Autofac;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Exceptions;

namespace FlowBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private const string DefaultPackageName = "FlowBot";
        private const string DefaultWorkflowName = "Start";
        public IWorkflowService _workflowService;
        public IDataService _dataService;
        public MessagesController(IWorkflowService workflowService, IDataService dataService)
        {
            _workflowService = workflowService;
            _dataService = dataService;
            _workflowService.SetWorkflowRootDirectory(HttpRuntime.AppDomainAppPath + "Workflow\\");
        }
        private bool ParsePath(string path, out string packageName, out string workflowName)
        {
            packageName = null;
            workflowName = DefaultWorkflowName;
            if ( string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            else
            {
                string[] components = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                switch( components.Length)
                {
                    case 0:
                        return false;
                    case 1:
                        packageName = components[0];
                        workflowName = DefaultWorkflowName;
                        return true;
                    case 2:
                        packageName = components[0];
                        workflowName = components[1];
                        return true;
                    default:
                        return false; 
                }
            }
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromUri]string packageName, [FromUri]string workflowName,[FromBody]Activity activity)
        {
            try
            {
                if (packageName == null)
                {
                    packageName = DefaultPackageName;
                }
                if (workflowName == null)
                {
                    workflowName = DefaultWorkflowName;
                }
                if (activity.Type == ActivityTypes.Message)
                {
                    var existingWorkflow = _workflowService.LookupWorkflow(activity.Conversation.Id, Common.Models.WorkflowInstanceStates.Idle,"message");
                    if (existingWorkflow == null)
                    {
                        Dictionary<string, object> inputs = new Dictionary<string, object>();
                        var newWorkflow = _workflowService.NewWorkflow(packageName, workflowName, activity.Conversation.Id, inputs);
                        newWorkflow.IOCService.Resolve<IConnectorService>().BindActivity(newWorkflow, activity);
                        newWorkflow.Run();
                    }
                    else
                    {
                        existingWorkflow.IOCService.Resolve<IConnectorService>().BindActivity(existingWorkflow, activity);
                        existingWorkflow.Resume<Activity>("message", activity);
                    }
                }
                else
                {
                    workflowName = "Handle-System-Message";
                    HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch(WorkflowNotFoundException e)
            {
                Debug.WriteLine(e);
                var apiResponse = new APIResponse(e.Message);
                var response = Request.CreateResponse<APIResponse>(HttpStatusCode.NotFound, apiResponse);
                return response;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                var apiResponse = new APIResponse($"Package {packageName} Workflow {workflowName} reported {e.Message}");
                var response = Request.CreateResponse<APIResponse>(HttpStatusCode.InternalServerError, apiResponse);
                return response;
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}