using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IUser : IRecord
    {
        string FirstName { get; }
        string LastName { get; }
        string ExternalId { get; }
        ICollection<IUserGroup> UserGroups { get; }
    }
}
