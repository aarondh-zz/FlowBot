using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Data
{
    public partial class UserGroup : IUserGroup
    {
        ICollection<IUser> IUserGroup.Users
        {
            get
            {
                return this.Users.Select(u => (IUser)u).ToArray();
            }
        }
    }
}
