using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.WorkerPortal.Extensions
{
    public static class StringExtensions
    {
        public static string ToLabel(this string source, bool sentenceCase = false)
        {
            var result = "";
            if (source != null)
            {
                var isNewWord = true;
                var lastWasUpper = false;
                for (var i = 0; i < source.Length; i++)
                {
                    var c = source[i];
                    if (c == ' ' || c == '_' || c == '\n' || c == '\t' || c == '\r')
                    {
                        isNewWord = true;
                        lastWasUpper = false;
                        continue;
                    }
                    else if (c >= 'A' && c <= 'Z')
                    {
                        if (!lastWasUpper)
                        {
                            isNewWord = true;
                        }
                        lastWasUpper = true;
                    }
                    else
                    {
                        lastWasUpper = false;
                    }
                    if (isNewWord)
                    {
                        if (result.Length > 0 && sentenceCase)
                        {
                            c = char.ToLower(c);
                        }
                        else
                        {
                            c = char.ToUpper(c);
                        }
                        isNewWord = false;
                        if (result.Length > 0)
                        {
                            result += ' ';
                        }
                    }
                    result += c;
                }
            }
            return result;
        }
    }
}
