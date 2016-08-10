using JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.Extensions
{
    public static class JsonPatchDocumentExtensions
    {
        public static bool HasValidOperations<T>(this JsonPatchDocument<T> patchDocument, params string[] validPaths) where T : class, new()
        {
            HashSet<string> validPathSet = new HashSet<string>(validPaths, StringComparer.InvariantCultureIgnoreCase);
            foreach( var operation in patchDocument.Operations)
            {
                if (!validPathSet.Contains(operation.Path))
                {
                    return false;
                }
            }
            return true;
        }
    }
}