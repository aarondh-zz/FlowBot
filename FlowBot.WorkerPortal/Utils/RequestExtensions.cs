using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FlowBot.WorkerPortal.Utils
{
    public static class RequestExtensions
    {
        public static string GetRawBody(this HttpRequestBase request)
        {
            if (request.InputStream.CanSeek)
            {
                // Reset the position to zero to read from the beginning.
                request.InputStream.Position = 0;
            }

            return new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
        }
        public static string GetBodyAsJson(this HttpRequestBase request)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            foreach( string parameterName in request.Form)
            {
                var value = request.Form[parameterName];
                if ( value != null)
                {
                    double doubleValue;
                    bool boolValue;
                    DateTime dateTimeValue;
                    if (double.TryParse(value, out doubleValue) )
                    {
                        body[parameterName] = doubleValue;
                    }
                    else if (bool.TryParse(value, out boolValue) )
                    {
                        body[parameterName] = boolValue;
                    }
                    else if (DateTime.TryParse(value, out dateTimeValue))
                    {
                        body[parameterName] = dateTimeValue;
                    }
                    else
                    {
                        body[parameterName] = value;
                    }
                }
            }
            return JsonConvert.SerializeObject(body);
        }
   }
}