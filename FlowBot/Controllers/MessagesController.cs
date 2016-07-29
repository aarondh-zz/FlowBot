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

namespace FlowBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var startWorkflow = HttpRuntime.AppDomainAppPath + "Workflow\\Start.xaml";
                Dictionary<Type, object> extensions = new Dictionary<Type, object>();
                Dictionary<string, object> inputs = new Dictionary<string, object>();
                extensions[typeof(IDataService)] = new DataService();
                extensions[typeof(IConnectorService)] = new ConnectorService(activity);
                extensions[typeof(ILuisService)] = new LuisService("386327ee-db6e-4042-a3db-3804724d980c", "cb244805c4144637bfadde5d4da230ec");
                WorkflowServiceHost.Instance.RunNewWorkflow(startWorkflow, extensions, inputs);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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