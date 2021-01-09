using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Function
{
    public interface IFunctionalService
    {
        Task CreateDefaultAdminUser();
        Task CreateDefaultUser();
    }
}
