using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IUserGroup : IRecord
    {
        string Name { get; }
        ICollection<IUser> Users { get; }
    }
}
