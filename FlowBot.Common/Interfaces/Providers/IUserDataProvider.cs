using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IUserDataProvider : IDataProvider<IUser>
    {
        IUser ReadByName(string name);
        IUser ReadByExternalId(string id);
        IUser Read(string externalId, string name);
        IUser Create(string userGroupName, string externalId, string name);
        IUser ReadOrCreate(string userGroupName, string externalId, string name);
    }
}
