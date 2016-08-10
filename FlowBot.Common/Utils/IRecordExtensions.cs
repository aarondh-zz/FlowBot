using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Utils
{
    public static class IRecordExtensions
    {
        public static void CopyTo(this IRecord from, IRecord to)
        {
            var fromType = from.GetType();
            var toType = to.GetType();
            var properties = fromType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var fromProperty in properties)
            {
                var toProperty = toType.GetProperty(fromProperty.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.SetProperty);
                if ( toProperty != null && fromProperty.PropertyType == toProperty.PropertyType)
                {
                    object value = fromProperty.GetValue(from);
                    toProperty.SetValue(to, value);
                }
            }
        }
    }
}
