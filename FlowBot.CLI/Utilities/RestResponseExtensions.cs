using FlowBot.CLI.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Utilities
{
    public static class RestResponseExtensions
    {
        public static void ReportComplete(this IRestResponse response, ILog log, object value)
        {
            if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                log.Write(Models.LogLevels.Info, value);
            }
            else if (response.ErrorMessage != null)
            {
                throw new InvalidOperationException(response.ErrorMessage);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(response.Content))
                {
                    throw new InvalidOperationException(response.StatusDescription);
                }
                else
                {
                    var message = response.StatusDescription;
                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(response.Content);
                        message = message += ": " + result.message;
                    }
                    catch
                    {
                        log.Write(Models.LogLevels.Verbose, response.Content);
                    }
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
