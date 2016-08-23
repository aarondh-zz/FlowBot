using FlowBot.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Interfaces
{
    public interface IFunction
    {
        void Create(IFunctionContext context);
        void Read(IFunctionContext context);
        void Update(IFunctionContext context);
        void Delete(IFunctionContext context);
        void List(IFunctionContext context);
    }
}
