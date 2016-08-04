using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Data
{
    public partial class User : IUser
    {
        ICollection<IUserGroup> IUser.UserGroups
        {
            get
            {
                return this.UserGroups.Cast<IUserGroup>().ToArray();
            }
        }
    }
}
