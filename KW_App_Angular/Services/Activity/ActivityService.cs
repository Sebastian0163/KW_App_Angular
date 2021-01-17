using KW_App_Angular.Dall.Context;
using KW_App_Angular.Dall.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Activity
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _db;

        public ActivityService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddUserActivity(ActivityEntities model)
        {
            await using var dbContextTransaction = await _db.Database.BeginTransactionAsync();
            try
            {

                await _db.Activities.AddAsync(model);
                await _db.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }

            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

                await dbContextTransaction.RollbackAsync();
            }
        }
        public async Task<List<ActivityEntities>> GetUserActivity(string userId)
        {
            List<ActivityEntities> userActivities = new List<ActivityEntities>();

            try
            {
                await using var dbContextTransaction = await _db.Database.BeginTransactionAsync();
                userActivities = await _db.Activities.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return userActivities;
        }
    }
}
