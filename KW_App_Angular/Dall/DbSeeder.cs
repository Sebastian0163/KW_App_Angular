using KW_App_Angular.Dall.Context;
using KW_App_Angular.Services.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Dall
{
    public static class DbSeeder
    {
        public static async Task Initialize(DataProtKeyContext dataProtKeyContext, ApplicationDbContext aplicationDbContext, IFunctionalService iFunctionalService)
        {
            // Check, if db DataProtectionKeysContext is created
            // Check, if db ApplicationDbContext is created
            await dataProtKeyContext.Database.EnsureCreatedAsync();
            await aplicationDbContext.Database.EnsureCreatedAsync();

            // Check, if db contains any users. If db is not empty, then db has been already seeded
            if (aplicationDbContext.ApplicationUsers.Any())
            {
                return;
            }

            // If empty create Admin User and App User
            await iFunctionalService.CreateDefaultAdminUser();
            await iFunctionalService.CreateDefaultUser();

        }
    }
}
