using KW_App_Angular.Dall.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Activity
{
    public  interface IActivityService
    {
         Task AddUserActivity(ActivityEntities model);
        Task<List<ActivityEntities>> GetUserActivity(string userId);
    }
}
