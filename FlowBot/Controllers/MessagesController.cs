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
using FlowBot.Services;
using System.Web;
using FlowBot.Common.Interfaces.Services;
using System.Diagnostics;
using Autofac;

namespace FlowBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public IWorkflowService _workflowService;
        public MessagesController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
            _workflowService.SetWorkflowRootDirectory(HttpRuntime.AppDomainAppPath + "Workflow\\");
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            string workflowPath = null;
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    var existingWorkflow = _workflowService.LookupWorkflow(activity.Conversation.Id);
                    if ( existingWorkflow == null)
                    {
                        Dictionary<string, object> inputs = new Dictionary<string, object>();
                        var newWorkflow = _workflowService.NewWorkflow("FlowBot","Start", activity.Conversation.Id, inputs);
                        newWorkflow.IOCService.Resolve<IConnectorService>().BindActivity(activity);
                        newWorkflow.Run();
                    }
                    else
                    {
                        existingWorkflow.IOCService.Resolve<IConnectorService>().BindActivity(activity);
                        //existingWorkflow.Run();
                        existingWorkflow.Resume<Activity>("message", activity);
                    }
                }
                else
                {
                    workflowPath = "Handle-System-Message";
                    HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                var apiResponse = new APIResponse($"Workflow {workflowPath} reported {e.Message}");
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