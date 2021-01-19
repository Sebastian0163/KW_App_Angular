using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Default
{
    public interface IDefaultService
    {
        Task CreateDefaultAdminUser();
        Task CreateDefaultUser();
    }
}
