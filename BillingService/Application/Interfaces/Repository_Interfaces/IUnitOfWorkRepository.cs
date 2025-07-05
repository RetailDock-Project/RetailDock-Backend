using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository_Interfaces
{
    public interface IUnitOfWorkRepository
    {
        Task _BiginTransaction();
        Task _CommitTransaction();
        Task _RolBackTransaction();
        Task _SaveChnages();
    }
}
